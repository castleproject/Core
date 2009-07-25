// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using System.Runtime.Serialization;
#if !SILVERLIGHT
	using System.Xml.Serialization;
#endif
	using Castle.Core.Interceptor;
	using Castle.Core.Internal;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Tokens;

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

				String newName = "Castle.Proxies." + targetType.Name + "Proxy" + Guid.NewGuid().ToString("N");
				generatedType = GenerateType(newName, proxyTargetType, interfaces);

				AddToCache(cacheKey, generatedType);
			}

			return generatedType;
		}

		private Type GenerateType(string typeName, Type proxyTargetType, Type[] interfaces)
		{
			// TODO: this anemic dictionary should be made into a real object
			IDictionary<Type, object> interfaceTypeImplementerMapping = new Dictionary<Type, object>();

			// Order of interface precedence:
			// 1. first target
			AddTargetInterfaceMapping(interfaceTypeImplementerMapping);

			// 2. then mixins
			if(ProxyGenerationOptions.HasMixins)
			{
				foreach (var mixinInterface in ProxyGenerationOptions.MixinData.MixinInterfaces)
				{
					object mixinInstance = ProxyGenerationOptions.MixinData.GetMixinInstance(mixinInterface);
					AddInterfaceMapping(mixinInterface, mixinInstance, interfaceTypeImplementerMapping);
				}
			}

			// 3. then additional interfaces
			if (interfaces != null)
			{
				foreach (var @interface in interfaces)
				{
					AddInterfaceMapping(@interface, null /*because there is no target*/, interfaceTypeImplementerMapping);
				}
			}

			// 4. plus special interfaces
#if SILVERLIGHT
#warning What to do?
#else
			AddInterfaceMapping(typeof (ISerializable), Proxy.Instance, interfaceTypeImplementerMapping);
#endif
			AddInterfaceMapping(typeof (IProxyTargetAccessor), Proxy.Instance, interfaceTypeImplementerMapping);

			// This is flawed. We allow any type to be a base type but we don't realy handle it properly.
			// What if the type implements interfaces? What if it implements target interface?
			// What if it implement mixin interface? What if it implements any additional interface?
			// What if it has no default constructor?
			// We handle none of these cases.
			Type baseType = ProxyGenerationOptions.BaseTypeForInterfaceProxy;

			ClassEmitter emitter = BuildClassEmitter(typeName, baseType, interfaceTypeImplementerMapping.Keys);
			CreateOptionsField(emitter);
			emitter.AddCustomAttributes(ProxyGenerationOptions);
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
			IList<ProxyElementTarget> targets = new List<ProxyElementTarget>();
			foreach (var mapping in interfaceTypeImplementerMapping)
			{
				// NOTE: make sure this is what it should be
				if (ReferenceEquals(mapping.Value, Proxy.Instance)) continue;

				targets.Add(CollectElementsToProxy(mapping));

			}

			ProxyGenerationOptions.Hook.MethodsInspected();

			// Constructor

			ConstructorEmitter typeInitializer = GenerateStaticConstructor(emitter);

			// TODO: this affects caching. is it required?
			if (!proxyTargetType.IsInterface)
			{
				var methods = MethodFinder.GetAllInstanceMethods(proxyTargetType, BindingFlags.Public | BindingFlags.Instance);
				CacheMethodTokens(emitter, methods, typeInitializer);
			}

			CreateInitializeCacheMethodBody(proxyTargetType, GetMethods(targets), emitter, typeInitializer);

			FieldReference[] mixinFields = AddMixinFields(emitter);
			List<FieldReference> fields = new List<FieldReference>(mixinFields);
			fields.Add(interceptorsField);
			fields.Add(targetField);
			GenerateConstructors(emitter, baseType, fields.ToArray());
			// GenerateParameterlessConstructor(emitter, interceptorsField, baseType);




			// Create invocation types
			// NOTE: does this have to happen separately from the generation of below implementation methods?
			foreach (var target in targets)
			{
				foreach (var method in target.Methods)
				{
					CreateInvocationForMethod(emitter, method, proxyTargetType);
					AddFieldToCacheMethodTokenAndStatementsToInitialize(method.Method, typeInitializer, emitter);
					MethodInfo methodOnTarget;
					if(method2methodOnTarget.TryGetValue(method.Method,out methodOnTarget))
					{
						AddFieldToCacheMethodTokenAndStatementsToInitialize(methodOnTarget, typeInitializer, emitter);
					}
				}
			}

			// Create methods overrides
			var method2Emitter = new Dictionary<MethodInfo, MethodEmitter>();
			foreach (var target in targets)
			{
				foreach (var method in target.Methods)
				{
					ImplementMethod(emitter, interceptorsField, mixinFields, method, method2Emitter);
				}

				foreach (PropertyToGenerate property in target.Properties)
				{
					ImplementProperty(emitter, interceptorsField, mixinFields, property);
				}

				foreach (EventToGenerate @event in target.Events)
				{
					ImplementEvent(emitter, interceptorsField, mixinFields, @event);
				}
			}

#if SILVERLIGHT
#warning What to do?
#else
			ImplementGetObjectData(emitter, interceptorsField, mixinFields, interfaces);
#endif

			// Complete Initialize 

			CompleteInitCacheMethod(typeInitializer.CodeBuilder);

			// Crosses fingers and build type

			Type generatedType = emitter.BuildType();
			InitializeStaticFields(generatedType);
			return generatedType;
		}

		protected virtual void AddTargetInterfaceMapping(IDictionary<Type, object> interfaceTypeImplementerMapping)
		{
			AddInterfaceMapping(targetType, Proxy.Target, interfaceTypeImplementerMapping);
		}

		private void ImplementEvent(ClassEmitter emitter, FieldReference interceptorsField, FieldReference[] mixinFields, EventToGenerate @event)
		{
			@event.BuildEventEmitter(emitter);
			NestedClassEmitter add_nestedClass = method2Invocation[@event.AddMethod];
			string name;
			MethodAttributes add_attributes = ObtainMethodAttributes(@event.AddMethod, out name);

			MethodEmitter addEmitter = @event.Emitter.CreateAddMethod(name, add_attributes);

			MethodEmitter method = ImplementProxiedMethod(addEmitter,
			                                              @event.AddMethod, emitter,
			                                              add_nestedClass, interceptorsField,
			                                              GetTargetReference(@event.Adder, mixinFields, targetField),
			                                              ConstructorVersion.WithTargetMethod,
			                                              method2methodOnTarget[@event.AddMethod]);
			if (@event.AddMethod.DeclaringType.IsInterface)
			{
				emitter.TypeBuilder.DefineMethodOverride(method.MethodBuilder, @event.AddMethod);
			}
			ReplicateNonInheritableAttributes(@event.AddMethod, addEmitter);


			NestedClassEmitter remove_nestedClass = method2Invocation[@event.RemoveMethod];

			MethodAttributes remove_attributes = ObtainMethodAttributes(@event.RemoveMethod, out name);

			MethodEmitter removeEmitter = @event.Emitter.CreateRemoveMethod(name, remove_attributes);

			MethodEmitter methodEmitter = ImplementProxiedMethod(removeEmitter,
			                                                     @event.RemoveMethod, emitter,
			                                                     remove_nestedClass, interceptorsField,
			                                                     GetTargetReference(@event.Remover, mixinFields, targetField),
			                                                     ConstructorVersion.WithTargetMethod,
			                                                     method2methodOnTarget[@event.RemoveMethod]);
			if (@event.RemoveMethod.DeclaringType.IsInterface)
			{
				emitter.TypeBuilder.DefineMethodOverride(methodEmitter.MethodBuilder, @event.RemoveMethod);
			}
			ReplicateNonInheritableAttributes(@event.RemoveMethod, removeEmitter);
		}

		private void ImplementProperty(ClassEmitter emitter, FieldReference interceptorsField, FieldReference[] mixinFields, PropertyToGenerate property)
		{
			property.BuildPropertyEmitter(emitter);
			if (property.CanRead)
			{
				NestedClassEmitter nestedClass = method2Invocation[property.GetMethod];

				string name;
				MethodAttributes attributes = ObtainMethodAttributes(property.GetMethod, out name);
				MethodEmitter getEmitter = property.Emitter.CreateGetMethod(name, attributes);
				Reference targetReference = GetTargetReference(property.Getter, mixinFields, targetField);

				MethodEmitter method = ImplementProxiedMethod(getEmitter,
				                                              property.GetMethod,
				                                              emitter,
				                                              nestedClass,
				                                              interceptorsField,
				                                              targetReference,
				                                              ConstructorVersion.WithTargetMethod,
				                                              method2methodOnTarget[property.GetMethod]);
				if (property.GetMethod.DeclaringType.IsInterface)
				{
					emitter.TypeBuilder.DefineMethodOverride(method.MethodBuilder, property.GetMethod);
				}
				ReplicateNonInheritableAttributes(property.GetMethod, getEmitter);

				// emitter.TypeBuilder.DefineMethodOverride(getEmitter.MethodBuilder, propToGen.GetMethod);
			}

			if (property.CanWrite)
			{
				NestedClassEmitter nestedClass = method2Invocation[property.SetMethod];

				string name;
				MethodAttributes attributes = ObtainMethodAttributes(property.SetMethod, out name);

				MethodEmitter setEmitter = property.Emitter.CreateSetMethod(name, attributes);

				MethodEmitter method = ImplementProxiedMethod(setEmitter,
				                                              property.SetMethod, emitter,
				                                              nestedClass, interceptorsField, GetTargetReference(property.Setter, mixinFields, targetField),
				                                              ConstructorVersion.WithTargetMethod,
				                                              method2methodOnTarget[property.SetMethod]);
				if (property.SetMethod.DeclaringType.IsInterface)
				{
					emitter.TypeBuilder.DefineMethodOverride(method.MethodBuilder, property.SetMethod);
				}
				ReplicateNonInheritableAttributes(property.SetMethod, setEmitter);

				// emitter.TypeBuilder.DefineMethodOverride(setEmitter.MethodBuilder, propToGen.SetMethod);
			}
		}

		private void ImplementMethod(ClassEmitter emitter, FieldReference interceptorsField, FieldReference[] mixinFields, MethodToGenerate method, Dictionary<MethodInfo, MethodEmitter> method2Emitter)
		{
			var methodInfo = method.Method;
			if (!method.Standalone || methodsToSkip.Contains(methodInfo))
			{
				return;
			}

			NestedClassEmitter nestedClass = method2Invocation[methodInfo];

			MethodEmitter newProxiedMethod = CreateProxiedMethod(methodInfo,
			                                                     emitter,
			                                                     nestedClass,
			                                                     interceptorsField,
			                                                     GetTargetReference(method, mixinFields, targetField),
			                                                     ConstructorVersion.WithTargetMethod,
			                                                     method2methodOnTarget[methodInfo]);

			ReplicateNonInheritableAttributes(methodInfo, newProxiedMethod);

			method2Emitter[methodInfo] = newProxiedMethod;
		}

		private IEnumerable<MethodInfo> GetMethods(IEnumerable<ProxyElementTarget> targets)
		{
			foreach (var target in targets)
			{
				foreach (var method in target.MethodInfos)
				{
					yield return method;
				}
			}
		}

		private ProxyElementTarget CollectElementsToProxy(KeyValuePair<Type, object> mapping)
		{
			var elementsToProxy = new ProxyElementTarget(mapping, CanOnlyProxyVirtual());
			elementsToProxy.Collect(ProxyGenerationOptions.Hook);

			// HACK: This is a temporary hack until it gets implemented properly in the class proxy as well
			if (mapping.Value != null && mapping.Value != Proxy.Target && mapping.Value != Proxy.Instance) // mixin, yes I know it's ugly as hell
			{
				foreach (var method in elementsToProxy.MethodInfos)
				{
					method2MixinType[method] = mapping.Key;
				}
			}

			return elementsToProxy;
		}

		protected void AddInterfaceMapping(Type @interface, object implementer, IDictionary<Type, object> mapping)
		{
			Debug.Assert(@interface.IsInterface, "@interface.IsInterface");
		
			if (!mapping.ContainsKey(@interface))
			{
				mapping.Add(@interface, implementer);
			}

			foreach (var baseInterface in @interface.GetInterfaces())
			{
				AddInterfaceMapping(baseInterface, implementer, mapping);
			}
		}

		protected virtual bool AllowChangeTarget
		{
			get { return false; }
		}

		protected virtual void CreateInvocationForMethod(ClassEmitter emitter, MethodToGenerate method, Type proxyTargetType)
		{
			var methodInfo = method.Method;
			object target = method.Target;
			MethodInfo methodOnTarget = methodInfo;
			// TODO: this is a temporary workaround
			if (target == Proxy.Target)
			{
				if(!proxyTargetType.IsInterface)
				{
					var foundCandidate = FindImplementingMethod(methodInfo, proxyTargetType);
					if (foundCandidate != null)
					{
						methodOnTarget = foundCandidate;
					}
				}
			}
			else if (method.HasTarget) // then it must be a mixin
			{
				var foundCandidate = FindImplementingMethod(methodInfo, method.Target.GetType());
				if (foundCandidate != null)
				{
					methodOnTarget = foundCandidate;
				}
			}

			method2methodOnTarget[methodInfo] = methodOnTarget;

			method2Invocation[methodInfo] = BuildInvocationNestedType(emitter,
			                                                          methodInfo.DeclaringType,
			                                                          methodInfo,
			                                                          GetCallbackForMethod(methodInfo),
			                                                          GetConstructorVersion(method),
			                                                          AllowChangeTarget);
		}

		private MethodInfo FindImplementingMethod(MethodInfo interfaceMethod, Type implementingType)
		{
			Type interfaceType = interfaceMethod.DeclaringType;
			Debug.Assert(interfaceType.IsAssignableFrom(implementingType),
			             "interfaceMethod.DeclaringType.IsAssignableFrom(implementingType)");
			Debug.Assert(interfaceType.IsInterface, "interfaceType.IsInterface");
			InterfaceMapping map = implementingType.GetInterfaceMap(interfaceType);
			int index = Array.IndexOf(map.InterfaceMethods,interfaceMethod);
			if (index == -1)
			{
				// can this ever happen?
				return null;
			}
			return map.TargetMethods[index];
		}

		private ConstructorVersion GetConstructorVersion(MethodToGenerate method)
		{
			return method.HasTarget ? ConstructorVersion.WithTargetMethod : ConstructorVersion.WithoutTargetMethod;
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

			List<MethodInfo> members = GetMembers(methodOnInterface, proxyTargetType);

			if (members.Count == 0)
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

			if (members.Count > 1)
			{
				throw new GeneratorException("Found more than one method on target " + proxyTargetType.FullName + " matching " +
											 methodOnInterface.Name);
			}
			if (members.Count == 0)
			{
				if (checkMixins && IsMixinMethod(methodOnInterface))
				{
					return FindMethodOnTargetType(methodOnInterface, method2MixinType[methodOnInterface], false);
				}
				throw new GeneratorException("Could not find a matching method on " + proxyTargetType.FullName + ". Method " +
				                             methodOnInterface.Name);
			}

			return members[0];
		}

		private static List<MethodInfo> GetMembers(MethodInfo methodOnInterface, Type proxyTargetType)
		{
			List<MethodInfo> members = new List<MethodInfo>();
			foreach (MethodInfo method in proxyTargetType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
			{
				if (method.Name != methodOnInterface.Name) continue;

				if (IsMethodEquivalent(method, methodOnInterface))
				{
					members.Add(method);
				}
			}
			if (members.Count > 0 || proxyTargetType.IsClass)
			{
				return members;
			}

			// Didn't find it here, and we are looking at an interface, so we want to try the
			// base interfaces as well
			foreach (Type type in proxyTargetType.GetInterfaces())
			{
				members.AddRange(GetMembers(methodOnInterface, type));
				if (members.Count > 0)
				{
					return members;
				}
			}

			return members;
		}

		protected override void ImplementInvokeMethodOnTarget(NestedClassEmitter nested, ParameterInfo[] parameters, MethodEmitter method, MethodInfo callbackMethod, Reference targetField)
		{
			MethodInfo callbackMethod1 = callbackMethod;
			method.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(SelfReference.Self, InvocationMethods.EnsureValidTarget)));
			Expression[] args = new Expression[parameters.Length];

			// Idea: instead of grab parameters one by one
			// we should grab an array
			Dictionary<int, LocalReference> byRefArguments = new Dictionary<int, LocalReference>();

			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo param = parameters[i];

				Type paramType = GetParamType(nested, param.ParameterType);
				if (paramType.IsByRef)
				{
					LocalReference localReference = method.CodeBuilder.DeclareLocal(paramType.GetElementType());
					method.CodeBuilder.AddStatement(
						new AssignStatement(localReference,
						                    new ConvertExpression(paramType.GetElementType(),
						                                          new MethodInvocationExpression(SelfReference.Self,
						                                                                         InvocationMethods.GetArgumentValue,
						                                                                         new LiteralIntExpression(i)))));
					ByRefReference byRefReference = new ByRefReference(localReference);
					args[i] = new ReferenceExpression(byRefReference);
					byRefArguments[i] = localReference;
				}
				else
				{
					args[i] =
						new ConvertExpression(paramType,
						                      new MethodInvocationExpression(SelfReference.Self,
						                                                     InvocationMethods.GetArgumentValue,
						                                                     new LiteralIntExpression(i)));
				}
			}

			if (callbackMethod1.IsGenericMethod)
			{
				callbackMethod1 = callbackMethod1.MakeGenericMethod(nested.GetGenericArgumentsFor(callbackMethod1));
			}

			MethodInvocationExpression baseMethodInvExp = new MethodInvocationExpression(targetField, callbackMethod1, args);
			baseMethodInvExp.VirtualCall = true;

			LocalReference returnValue = null;
			if (callbackMethod1.ReturnType != typeof(void))
			{
				Type returnType = this.GetParamType(nested, callbackMethod1.ReturnType);
				returnValue = method.CodeBuilder.DeclareLocal(returnType);
				method.CodeBuilder.AddStatement(new AssignStatement(returnValue, baseMethodInvExp));
			}
			else
			{
				method.CodeBuilder.AddStatement(new ExpressionStatement(baseMethodInvExp));
			}

			foreach (KeyValuePair<int, LocalReference> byRefArgument in byRefArguments)
			{
				int index = byRefArgument.Key;
				LocalReference localReference = byRefArgument.Value;
				method.CodeBuilder.AddStatement(
					new ExpressionStatement(
						new MethodInvocationExpression(SelfReference.Self,
						                               InvocationMethods.SetArgumentValue,
						                               new LiteralIntExpression(index),
						                               new ConvertExpression(typeof(object), localReference.Type,
						                                                     new ReferenceExpression(localReference)))
						));
			}

			if (callbackMethod1.ReturnType != typeof(void))
			{
				MethodInvocationExpression setRetVal =
					new MethodInvocationExpression(SelfReference.Self,
					                               InvocationMethods.SetReturnValue,
					                               new ConvertExpression(typeof(object), returnValue.Type, returnValue.ToExpression()));

				method.CodeBuilder.AddStatement(new ExpressionStatement(setRetVal));
			}

			method.CodeBuilder.AddStatement(new ReturnStatement());
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

		protected override MethodInfo GetCallbackForMethod(MethodInfo method)
		{
			return method;
		}

		protected override Reference GetMethodTargetReference(MethodInfo method)
	    {
	        return new AsTypeReference(targetField, method.DeclaringType);
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
			codebuilder.AddStatement(new ExpressionStatement(
										new MethodInvocationExpression(arg1, SerializationInfoMethods.AddValue_Object,
																	   new ConstReference("__target").ToExpression(),
																	   targetField.ToExpression())));

			codebuilder.AddStatement(new ExpressionStatement(
										new MethodInvocationExpression(arg1, SerializationInfoMethods.AddValue_Object,
																	   new ConstReference("__targetFieldType").ToExpression(),
																	   new ConstReference(
																		targetField.Reference.FieldType.AssemblyQualifiedName).
																		ToExpression())));

			codebuilder.AddStatement(new ExpressionStatement(
										new MethodInvocationExpression(arg1, SerializationInfoMethods.AddValue_Int32,
																	   new ConstReference("__interface_generator_type").
																		ToExpression(),
																	   new ConstReference((int)GeneratorType).ToExpression())));

			codebuilder.AddStatement(new ExpressionStatement(
										new MethodInvocationExpression(arg1, SerializationInfoMethods.AddValue_Object,
																	   new ConstReference("__theInterface").ToExpression(),
																	   new ConstReference(targetType.AssemblyQualifiedName).
																		ToExpression())));
		}
#endif

		protected virtual InterfaceGeneratorType GeneratorType
		{
			get { return InterfaceGeneratorType.WithTarget; }
		}

		private static class Proxy
		{
			/// <summary>
			/// Used to mark interface target as proxy instance. Used for special interfaces <see cref="ISerializable"/> and <see cref="IProxyTargetAccessor"/>.
			/// </summary>
			public static readonly object Instance = new object();

			/// <summary>
			/// Used to mark interface target as proxy target.
			/// </summary>
			public static readonly object Target = new object();
		}
	}

	/// <summary>
	/// This is used by the ProxyObjectReference class during de-serialiation, to know
	/// which generator it should use
	/// </summary>
	public enum InterfaceGeneratorType
	{
		WithTarget = 1,
		WithoutTarget = 2,
		WithTargetInterface = 3
	}
}
