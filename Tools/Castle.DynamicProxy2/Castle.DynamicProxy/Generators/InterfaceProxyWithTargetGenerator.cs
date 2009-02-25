// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Threading;
#if !SILVERLIGHT
	using System.Xml.Serialization;
#endif
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.Core.Internal;

	/// <summary>
	/// 
	/// </summary>
	public class InterfaceProxyWithTargetGenerator : BaseProxyGenerator
	{
		private FieldReference targetField;
		protected Dictionary<MethodInfo, NestedClassEmitter> method2Invocation = new Dictionary<MethodInfo, NestedClassEmitter>();
		protected Dictionary<MethodInfo, MethodInfo> method2methodOnTarget = new Dictionary<MethodInfo, MethodInfo>();

		public InterfaceProxyWithTargetGenerator(ModuleScope scope, Type theInterface)
			: base(scope, theInterface)
		{
			CheckNotGenericTypeDefinition(theInterface, "theInterface");
		}

		public Type GenerateCode(Type proxyTargetType, Type[] interfaces, ProxyGenerationOptions options)
		{
			// make sure ProxyGenerationOptions is initialized
			options.Initialize();

			CheckNotGenericTypeDefinition(proxyTargetType, "proxyTargetType");
			CheckNotGenericTypeDefinitions(interfaces, "interfaces");
			Type generatedType;

			CacheKey cacheKey = new CacheKey(proxyTargetType, targetType, interfaces, options);

			using (UpgradableLock locker = new UpgradableLock(Scope.RWLock))
			{
				Type cacheType = GetFromCache(cacheKey);

				if (cacheType != null)
				{
					return cacheType;
				}

				locker.Upgrade();

				cacheType = GetFromCache(cacheKey);

				if (cacheType != null)
				{
					return cacheType;
				}

				SetGenerationOptions(options);

				String newName = targetType.Name + "Proxy" + Guid.NewGuid().ToString("N");

				// Add Interfaces that the proxy implements 

				List<Type> interfaceList = new List<Type>();

				interfaceList.Add(targetType);

				if (interfaces != null)
				{
					interfaceList.AddRange(interfaces);
				}
#if SILVERLIGHT
#warning What to do?
#else
				if (!interfaceList.Contains(typeof(ISerializable)))
					interfaceList.Add(typeof(ISerializable));
#endif

				AddMixinInterfaces(interfaceList);
				AddDefaultInterfaces(interfaceList);

				Type baseType = options.BaseTypeForInterfaceProxy;

				ClassEmitter emitter = BuildClassEmitter(newName, baseType, interfaceList);
				CreateOptionsField(emitter);
                emitter.AddCustomAttributes(options);
#if SILVERLIGHT
#warning XmlIncludeAttribute is in silverlight, do we want to explore this?
#else
				emitter.DefineCustomAttribute(new XmlIncludeAttribute(targetType));
				emitter.DefineCustomAttribute(new SerializableAttribute());
#endif

				// Custom attributes
				ReplicateNonInheritableAttributes(targetType, emitter);

				// Fields generations

				FieldReference interceptorsField = emitter.CreateField("__interceptors", typeof(IInterceptor[]));
				targetField = emitter.CreateField("__target", proxyTargetType);

#if SILVERLIGHT
#warning XmlIncludeAttribute is in silverlight, do we want to explore this?
#else
				emitter.DefineCustomAttributeFor(interceptorsField, new XmlIgnoreAttribute());
				emitter.DefineCustomAttributeFor(targetField, new XmlIgnoreAttribute());
#endif
				// Implement builtin Interfaces
				ImplementProxyTargetAccessor(targetType, emitter, interceptorsField);

				// Collect methods

				PropertyToGenerate[] propsToGenerate;
				EventToGenerate[] eventToGenerates;
				MethodInfo[] methods = CollectMethodsAndProperties(emitter, targetType, out propsToGenerate, out eventToGenerates);

				if (interfaces != null && interfaces.Length != 0)
				{
					List<Type> tmpInterfaces = new List<Type>(interfaces);

					foreach (Type inter in interfaces)
					{
						if (inter.IsAssignableFrom(proxyTargetType))
						{
							PropertyToGenerate[] tempPropsToGenerate;
							EventToGenerate[] tempEventToGenerates;
							MethodInfo[] methodsTemp =
								CollectMethodsAndProperties(emitter, inter, out tempPropsToGenerate, out tempEventToGenerates);

							PropertyToGenerate[] newPropsToGenerate =
								new PropertyToGenerate[tempPropsToGenerate.Length + propsToGenerate.Length];
							MethodInfo[] newMethods = new MethodInfo[methodsTemp.Length + methods.Length];
							EventToGenerate[] newEvents = new EventToGenerate[eventToGenerates.Length + tempEventToGenerates.Length];

							Array.Copy(methods, newMethods, methods.Length);
							Array.Copy(methodsTemp, 0, newMethods, methods.Length, methodsTemp.Length);

							Array.Copy(propsToGenerate, newPropsToGenerate, propsToGenerate.Length);
							Array.Copy(tempPropsToGenerate, 0, newPropsToGenerate, propsToGenerate.Length, tempPropsToGenerate.Length);

							Array.Copy(eventToGenerates, newEvents, eventToGenerates.Length);
							Array.Copy(tempEventToGenerates, 0, newEvents, eventToGenerates.Length, tempEventToGenerates.Length);

							methods = newMethods;
							propsToGenerate = newPropsToGenerate;
							eventToGenerates = newEvents;

							tmpInterfaces.Remove(inter);
						}
					}

					interfaces = tmpInterfaces.ToArray();
				}

				RegisterMixinMethodsAndProperties(emitter, ref methods, ref propsToGenerate, ref eventToGenerates);

				options.Hook.MethodsInspected();

				// Constructor

				ConstructorEmitter typeInitializer = GenerateStaticConstructor(emitter);

				if (!proxyTargetType.IsInterface)
				{
					CacheMethodTokens(emitter, MethodFinder.GetAllInstanceMethods(proxyTargetType,
																				  BindingFlags.Public | BindingFlags.Instance),
									  typeInitializer);
				}

				CreateInitializeCacheMethodBody(proxyTargetType, methods, emitter, typeInitializer);
				FieldReference[] mixinFields = AddMixinFields(emitter);
				List<FieldReference> fields = new List<FieldReference>(mixinFields);
				fields.Add(interceptorsField);
				fields.Add(targetField);
				GenerateConstructors(emitter, baseType, fields.ToArray());
				// GenerateParameterlessConstructor(emitter, interceptorsField, baseType);

				// Implement interfaces

				if (interfaces != null && interfaces.Length != 0)
				{
					foreach (Type inter in interfaces)
					{
						ImplementBlankInterface(targetType, inter, emitter, interceptorsField, typeInitializer, AllowChangeTarget);
					}
				}

				// Create invocation types

				foreach (MethodInfo method in methods)
				{
					CreateInvocationForMethod(emitter, method, proxyTargetType);
				}

				// Create methods overrides

				Dictionary<MethodInfo, MethodEmitter> method2Emitter = new Dictionary<MethodInfo, MethodEmitter>();

				foreach (MethodInfo method in methods)
				{
					if (method.IsSpecialName &&
						(method.Name.StartsWith("get_") || method.Name.StartsWith("set_") ||
						 method.Name.StartsWith("add_") || method.Name.StartsWith("remove_")) ||
						methodsToSkip.Contains(method))
					{
						continue;
					}

					NestedClassEmitter nestedClass = (NestedClassEmitter)method2Invocation[method];

					MethodEmitter newProxiedMethod = CreateProxiedMethod(
						targetType, method, emitter, nestedClass, interceptorsField, GetTargetRef(method, mixinFields, targetField),
						ConstructorVersion.WithTargetMethod, (MethodInfo)method2methodOnTarget[method]);

					ReplicateNonInheritableAttributes(method, newProxiedMethod);

					method2Emitter[method] = newProxiedMethod;
				}

				foreach (PropertyToGenerate propToGen in propsToGenerate)
				{
					if (propToGen.CanRead)
					{
						NestedClassEmitter nestedClass = (NestedClassEmitter)method2Invocation[propToGen.GetMethod];

						MethodAttributes atts = ObtainMethodAttributes(propToGen.GetMethod);

						MethodEmitter getEmitter = propToGen.Emitter.CreateGetMethod(atts);

						ImplementProxiedMethod(targetType, getEmitter,
											   propToGen.GetMethod, emitter,
											   nestedClass, interceptorsField, GetTargetRef(propToGen.GetMethod, mixinFields, targetField),
											   ConstructorVersion.WithTargetMethod,
											   (MethodInfo)method2methodOnTarget[propToGen.GetMethod]);

						ReplicateNonInheritableAttributes(propToGen.GetMethod, getEmitter);

						// emitter.TypeBuilder.DefineMethodOverride(getEmitter.MethodBuilder, propToGen.GetMethod);
					}

					if (propToGen.CanWrite)
					{
						NestedClassEmitter nestedClass = (NestedClassEmitter)method2Invocation[propToGen.SetMethod];

						MethodAttributes atts = ObtainMethodAttributes(propToGen.SetMethod);

						MethodEmitter setEmitter = propToGen.Emitter.CreateSetMethod(atts);

						ImplementProxiedMethod(targetType, setEmitter,
											   propToGen.SetMethod, emitter,
											   nestedClass, interceptorsField, GetTargetRef(propToGen.SetMethod, mixinFields, targetField),
											   ConstructorVersion.WithTargetMethod,
											   (MethodInfo)method2methodOnTarget[propToGen.SetMethod]);

						ReplicateNonInheritableAttributes(propToGen.SetMethod, setEmitter);

						// emitter.TypeBuilder.DefineMethodOverride(setEmitter.MethodBuilder, propToGen.SetMethod);
					}
				}


				foreach (EventToGenerate eventToGenerate in eventToGenerates)
				{
					NestedClassEmitter add_nestedClass = (NestedClassEmitter)method2Invocation[eventToGenerate.AddMethod];

					MethodAttributes add_atts = ObtainMethodAttributes(eventToGenerate.AddMethod);

					MethodEmitter addEmitter = eventToGenerate.Emitter.CreateAddMethod(add_atts);

					ImplementProxiedMethod(targetType, addEmitter,
										   eventToGenerate.AddMethod, emitter,
										   add_nestedClass, interceptorsField,
										   GetTargetRef(eventToGenerate.AddMethod, mixinFields, targetField),
										   ConstructorVersion.WithTargetMethod,
										   (MethodInfo)method2methodOnTarget[eventToGenerate.AddMethod]);

					ReplicateNonInheritableAttributes(eventToGenerate.AddMethod, addEmitter);


					NestedClassEmitter remove_nestedClass = (NestedClassEmitter)method2Invocation[eventToGenerate.RemoveMethod];

					MethodAttributes remove_atts = ObtainMethodAttributes(eventToGenerate.RemoveMethod);

					MethodEmitter removeEmitter = eventToGenerate.Emitter.CreateRemoveMethod(remove_atts);

					ImplementProxiedMethod(targetType, removeEmitter,
										   eventToGenerate.RemoveMethod, emitter,
										   remove_nestedClass, interceptorsField,
										   GetTargetRef(eventToGenerate.RemoveMethod, mixinFields, targetField),
										   ConstructorVersion.WithTargetMethod,
										   (MethodInfo)method2methodOnTarget[eventToGenerate.RemoveMethod]);

					ReplicateNonInheritableAttributes(eventToGenerate.RemoveMethod, removeEmitter);
				}

#if SILVERLIGHT
#warning What to do?
#else
				ImplementGetObjectData(emitter, interceptorsField, mixinFields, interfaces);
#endif

				// Complete Initialize 

				CompleteInitCacheMethod(typeInitializer.CodeBuilder);

				// Crosses fingers and build type

				generatedType = emitter.BuildType();
				InitializeStaticFields(generatedType);

				/*foreach (MethodInfo m in TypeFinder.GetMethods(generatedType, BindingFlags.Instance | BindingFlags.Public))
				{
					ParameterInfo[] parameters = m.GetParameters();

					// Console.WriteLine(m.Name);

					for (int i = 0; i < parameters.Length; i++)
					{
						ParameterInfo paramInfo = parameters[i];

						// Console.WriteLine("{0} {1} {2} {3}", paramInfo.Name, paramInfo.ParameterType, paramInfo.Attributes, paramInfo.Position);
						// Console.WriteLine("{0} {1} {2} {3}", paramInfo2.Name, paramInfo2.ParameterType, paramInfo2.Attributes, paramInfo2.Position);
					}
				}
				*/

				AddToCache(cacheKey, generatedType);
			}

			return generatedType;
		}

		protected virtual bool AllowChangeTarget
		{
			get { return false; }
		}

		protected virtual void CreateInvocationForMethod(ClassEmitter emitter, MethodInfo method, Type proxyTargetType)
		{
			MethodInfo methodOnTarget = FindMethodOnTargetType(method, proxyTargetType, true);

			method2methodOnTarget[method] = methodOnTarget;

			method2Invocation[method] = BuildInvocationNestedType(emitter, proxyTargetType,
																  IsMixinMethod(method) ? method.DeclaringType : proxyTargetType,
																  method, methodOnTarget,
																  ConstructorVersion.WithTargetMethod,
																  AllowChangeTarget);
		}

		/// <summary>
		/// Finds the type of the method on target.
		/// </summary>
		/// <param name="methodOnInterface">The method on interface.</param>
		/// <param name="proxyTargetType">Type of the proxy target.</param>
		/// /// <param name="checkMixins">if set to <c>true</c> will check implementation on mixins.</param>
		/// <returns></returns>
		protected MethodInfo FindMethodOnTargetType(MethodInfo methodOnInterface, Type proxyTargetType, bool checkMixins)
		{
			// The code below assumes that the target
			// class uses the same generic arguments
			// as the interface generic arguments

			MemberInfo[] members = proxyTargetType.FindMembers(MemberTypes.Method,
															   BindingFlags.Public | BindingFlags.Instance,
															   delegate(MemberInfo mi, object criteria)
															   {
																   if (mi.Name != criteria.ToString()) return false;

																   MethodInfo methodInfo = (MethodInfo)mi;

																   return IsMethodEquivalent(methodInfo, methodOnInterface);
															   }, methodOnInterface.Name);


			if (members.Length == 0)
			{
				// Before throwing an exception, we look for an explicit
				// interface method implementation

				MethodInfo[] privateMethods =
					MethodFinder.GetAllInstanceMethods(proxyTargetType, BindingFlags.NonPublic | BindingFlags.Instance);

				foreach (MethodInfo methodInfo in privateMethods)
				{
					// We make sure it is a method used for explicit implementation

					if (!methodInfo.IsFinal || !methodInfo.IsVirtual || !methodInfo.IsHideBySig)
					{
						continue;
					}

					if (IsMethodEquivalent(methodInfo, methodOnInterface))
					{
						throw new GeneratorException(String.Format("DynamicProxy cannot create an interface (with target) " +
																   "proxy for '{0}' as the target '{1}' has an explicit implementation of one of the methods exposed by the interface. " +
																   "The runtime prevents use from invoking the private method on the target. Method {2}",
																   methodOnInterface.DeclaringType.Name, methodInfo.DeclaringType.Name,
																   methodInfo.Name));
					}
				}
			}

			if (members.Length > 1)
			{
				throw new GeneratorException("Found more than one method on target " + proxyTargetType.FullName + " matching " +
											 methodOnInterface.Name);
			}
			else if (members.Length == 0)
			{
				if (checkMixins && IsMixinMethod(methodOnInterface))
				{
					return FindMethodOnTargetType(methodOnInterface, method2MixinType[methodOnInterface], false);
				}
				throw new GeneratorException("Could not find a matching method on " + proxyTargetType.FullName + ". Method " +
											 methodOnInterface.Name);
			}

			return (MethodInfo)members[0];
		}

		/// <summary>
		/// Checks whether the given types are the same. This is 
		/// more complicated than it looks.
		/// </summary>
		/// <param name="sourceType"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		public static bool IsTypeEquivalent(Type sourceType, Type targetType)
		{
			if (sourceType.IsGenericParameter)
			{
				if (sourceType.Name != targetType.Name)
				{
					return false;
				}
			}
			else
			{
				if (sourceType.IsGenericType != targetType.IsGenericType)
				{
					return false;
				}
				else if (sourceType.IsArray != targetType.IsArray)
				{
					return false;
				}

				if (sourceType.IsGenericType)
				{
					if (sourceType.GetGenericTypeDefinition() != targetType.GetGenericTypeDefinition())
					{
						return false;
					}

					// Compare generic arguments

					Type[] sourceGenArgs = sourceType.GetGenericArguments();
					Type[] targetGenArgs = targetType.GetGenericArguments();

					for (int i = 0; i < sourceGenArgs.Length; i++)
					{
						if (!IsTypeEquivalent(sourceGenArgs[i], targetGenArgs[i]))
						{
							return false;
						}
					}
				}
				else if (sourceType.IsArray)
				{
					Type sourceArrayType = sourceType.GetElementType();
					Type targetArrayType = targetType.GetElementType();

					if (!IsTypeEquivalent(sourceArrayType, targetArrayType))
					{
						return false;
					}

					int sourceRank = sourceType.GetArrayRank();
					int targetRank = targetType.GetArrayRank();

					if (sourceRank != targetRank)
					{
						return false;
					}
				}
				else if (sourceType != targetType)
				{
					return false;
				}
			}

			return true;
		}

		protected override Reference GetProxyTargetReference()
		{
			return targetField;
		}

		protected override bool CanOnlyProxyVirtual()
		{
			return false;
		}

		/// <summary>
		/// Checks whether the given methods are the same.
		/// </summary>
		/// <param name="methodInfo"></param>
		/// <param name="methodOnInterface"></param>
		/// <returns>True if the methods are the same.</returns>
		private static bool IsMethodEquivalent(MethodInfo methodInfo, MethodInfo methodOnInterface)
		{
			// Check return type equivalence
			if (!IsTypeEquivalent(methodInfo.ReturnType, methodOnInterface.ReturnType))
			{
				return false;
			}

			// Check parameters equivalence
			ParameterInfo[] sourceParams = methodOnInterface.GetParameters();
			ParameterInfo[] targetParams = methodInfo.GetParameters();
			if (sourceParams.Length != targetParams.Length)
			{
				return false;
			}
			for (int i = 0; i < sourceParams.Length; i++)
			{
				Type sourceParamType = sourceParams[i].ParameterType;
				Type targetParamType = targetParams[i].ParameterType;

				if (!IsTypeEquivalent(sourceParamType, targetParamType))
				{
					return false;
				}
			}

			// Check generic arguments equivalence
			Type[] sourceGenericArguments = methodOnInterface.GetGenericArguments();
			Type[] targetGenericArguments = methodInfo.GetGenericArguments();
			if (sourceGenericArguments.Length != targetGenericArguments.Length)
			{
				return false;
			}
			for (int i = 0; i < sourceGenericArguments.Length; i++)
			{
				Type sourceGenericArgument = sourceGenericArguments[i];
				Type targetGenericArgument = targetGenericArguments[i];
				if (!IsTypeEquivalent(sourceGenericArgument, targetGenericArgument))
				{
					return false;
				}
			}

			// They are equivalent
			return true;
		}

#if SILVERLIGHT
#warning What to do?
#else
		protected override void CustomizeGetObjectData(AbstractCodeBuilder codebuilder, ArgumentReference arg1,
													   ArgumentReference arg2)
		{
			Type[] key_and_object = new Type[] { typeof(String), typeof(Object) };
			Type[] key_and_int = new Type[] { typeof(String), typeof(int) };
			Type[] key_and_string = new Type[] { typeof(String), typeof(string) };
			MethodInfo addValueMethod = typeof(SerializationInfo).GetMethod("AddValue", key_and_object);
			MethodInfo addIntMethod = typeof(SerializationInfo).GetMethod("AddValue", key_and_int);
			MethodInfo addStringMethod = typeof(SerializationInfo).GetMethod("AddValue", key_and_string);

			codebuilder.AddStatement(new ExpressionStatement(
										new MethodInvocationExpression(arg1, addValueMethod,
																	   new ConstReference("__target").ToExpression(),
																	   targetField.ToExpression())));

			codebuilder.AddStatement(new ExpressionStatement(
										new MethodInvocationExpression(arg1, addValueMethod,
																	   new ConstReference("__targetFieldType").ToExpression(),
																	   new ConstReference(
																		targetField.Reference.FieldType.AssemblyQualifiedName).
																		ToExpression())));

			codebuilder.AddStatement(new ExpressionStatement(
										new MethodInvocationExpression(arg1, addIntMethod,
																	   new ConstReference("__interface_generator_type").
																		ToExpression(),
																	   new ConstReference((int)GeneratorType).ToExpression())));

			codebuilder.AddStatement(new ExpressionStatement(
										new MethodInvocationExpression(arg1, addStringMethod,
																	   new ConstReference("__theInterface").ToExpression(),
																	   new ConstReference(targetType.AssemblyQualifiedName).
																		ToExpression())));
		}
#endif

		protected virtual InterfaceGeneratorType GeneratorType
		{
			get { return InterfaceGeneratorType.WithTarget; }
		}
	}

	/// <summary>
	/// This is used by the ProxyObjectReference class durin de-serialiation, to know
	/// which generator it should use
	/// </summary>
	public enum InterfaceGeneratorType
	{
		WithTarget = 1,
		WithoutTarget = 2,
		WithTargetInterface = 3
	}
}
