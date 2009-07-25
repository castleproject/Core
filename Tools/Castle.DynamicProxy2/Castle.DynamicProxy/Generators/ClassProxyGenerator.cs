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
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Runtime.Serialization;
	using System.Threading;
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
	public class ClassProxyGenerator : BaseProxyGenerator
	{
#if !SILVERLIGHT
		private bool delegateToBaseGetObjectData;
#endif

		public ClassProxyGenerator(ModuleScope scope, Type targetType) : base(scope, targetType)
		{
			CheckNotGenericTypeDefinition(targetType, "targetType");
		}

		public Type GenerateCode(Type[] interfaces, ProxyGenerationOptions options)
		{
			// make sure ProxyGenerationOptions is initialized
			options.Initialize();

			CheckNotGenericTypeDefinitions(interfaces, "interfaces");
			Type type;

			CacheKey cacheKey = new CacheKey(targetType, interfaces, options);

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

				// Add Interfaces that the proxy implements 

				List<Type> interfaceList = new List<Type>();

				if (interfaces != null)
				{
					interfaceList.AddRange(interfaces);
				}

				ValidateMixinInterfaces(targetType.GetInterfaces(), "target type " + targetType.Name);
				if (interfaces != null)
				{
					ValidateMixinInterfaces(interfaces, "additional interfaces");
				}

				AddMixinInterfaces(interfaceList);

				AddDefaultInterfaces(interfaceList);

#if !SILVERLIGHT
				if (targetType.IsSerializable)
				{
					delegateToBaseGetObjectData = VerifyIfBaseImplementsGetObjectData(targetType);

					if (!interfaceList.Contains(typeof(ISerializable)))
					{
						interfaceList.Add(typeof(ISerializable));
					}
				}
#endif
				ClassEmitter emitter = BuildClassEmitter(newName, targetType, interfaceList);
				CreateOptionsField(emitter);
				emitter.AddCustomAttributes(options);

#if !SILVERLIGHT
				emitter.DefineCustomAttribute(new XmlIncludeAttribute(targetType));
#endif
				// Custom attributes

				ReplicateNonInheritableAttributes(targetType, emitter);

				// Fields generations

				FieldReference interceptorsField =
					emitter.CreateField("__interceptors", typeof (IInterceptor[]));

				// Implement builtin Interfaces
				ImplementProxyTargetAccessor(targetType, emitter, interceptorsField);

#if !SILVERLIGHT
				emitter.DefineCustomAttributeFor(interceptorsField, new XmlIgnoreAttribute());
#endif
				// Collect methods

				PropertyToGenerate[] propsToGenerate;
				EventToGenerate[] eventsToGenerate;
				MethodInfo[] methods = CollectMethodsAndProperties(targetType, out propsToGenerate, out eventsToGenerate, true);
				if (interfaces != null && interfaces.Length != 0)
				{
					List<Type> tmpInterfaces = new List<Type>(interfaces);
					List<PropertyToGenerate> actualProperties = new List<PropertyToGenerate>(propsToGenerate);
					List<EventToGenerate> actualEvents = new List<EventToGenerate>(eventsToGenerate);
					List<MethodInfo> actualMethods = new List<MethodInfo>(methods);

					foreach (Type inter in interfaces)
					{
						if (inter.IsAssignableFrom(targetType))
						{
							PropertyToGenerate[] tempPropsToGenerate;
							EventToGenerate[] tempEventToGenerates;
							MethodInfo[] methodsTemp =
								CollectMethodsAndProperties(inter, out tempPropsToGenerate, out tempEventToGenerates, true);

							AddIfNew(actualMethods, methodsTemp);
							AddIfNew(actualProperties, tempPropsToGenerate);
							AddIfNew(actualEvents, tempEventToGenerates);

							tmpInterfaces.Remove(inter);
						}
					}

					interfaces = tmpInterfaces.ToArray();
					propsToGenerate = actualProperties.ToArray();
					eventsToGenerate = actualEvents.ToArray();
					methods = actualMethods.ToArray();
				}


				RegisterMixinMethodsAndProperties(ref methods, ref propsToGenerate, ref eventsToGenerate);

				options.Hook.MethodsInspected();

				// Constructor

				ConstructorEmitter typeInitializer = GenerateStaticConstructor(emitter);

				FieldReference[] mixinFields = AddMixinFields(emitter);

				// constructor arguments
				List<FieldReference> constructorArguments = new List<FieldReference>(mixinFields);
				constructorArguments.Add(interceptorsField);

				CreateInitializeCacheMethodBody(targetType, methods, emitter, typeInitializer);
				GenerateConstructors(emitter, targetType, constructorArguments.ToArray());
				GenerateParameterlessConstructor(emitter, targetType, interceptorsField);

#if !SILVERLIGHT
				if (delegateToBaseGetObjectData)
				{
					GenerateSerializationConstructor(emitter, interceptorsField, mixinFields);
				}
#endif
				if (interfaces != null && interfaces.Length != 0)
				{
					ImplementBlankInterfaces(interfaces, emitter, interceptorsField, typeInitializer, false, methods,propsToGenerate,eventsToGenerate);
				}

				// Create callback methods

				Dictionary<MethodInfo, MethodBuilder> method2Callback = new Dictionary<MethodInfo, MethodBuilder>();

				foreach (MethodInfo method in methods)
				{
					var methodOnTarget = GetMethodOnTarget(method);

					if(methodOnTarget== null) continue;

					method2Callback[method] = CreateCallbackMethod(emitter, method, methodOnTarget);
				}

				// Create invocation types

				Dictionary<MethodInfo, NestedClassEmitter> method2Invocation = new Dictionary<MethodInfo, NestedClassEmitter>();

				foreach (MethodInfo method in methods)
				{
					MethodBuilder callbackMethod;
					method2Callback.TryGetValue(method, out callbackMethod);

					method2Invocation[method] = BuildInvocationNestedType(emitter,
					                                                      IsMixinMethod(method)
					                                                      	? method.DeclaringType
					                                                      	: emitter.TypeBuilder,
					                                                      method, callbackMethod,
					                                                      ConstructorVersion.WithoutTargetMethod);
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

					NestedClassEmitter nestedClass = method2Invocation[method];

					//TODO: this is very temporary, until I refactor class generator similarily to interface generator
					var HACK = new MethodToGenerate(method, false, false, new object());

					Reference targetRef = GetTargetReference(HACK, mixinFields, SelfReference.Self);
					MethodEmitter newProxiedMethod = CreateProxiedMethod(method, emitter, nestedClass, interceptorsField, targetRef,
						ConstructorVersion.WithoutTargetMethod, null);

					ReplicateNonInheritableAttributes(method, newProxiedMethod);

					method2Emitter[method] = newProxiedMethod;
				}

				foreach (PropertyToGenerate propToGen in propsToGenerate)
				{
					propToGen.BuildPropertyEmitter(emitter);
					if (propToGen.CanRead)
					{
						NestedClassEmitter nestedClass = method2Invocation[propToGen.GetMethod];

						string name;
						MethodAttributes atts = ObtainMethodAttributes(propToGen.GetMethod, out name);

						MethodEmitter getEmitter = propToGen.Emitter.CreateGetMethod(name, atts);
						var HACK = new MethodToGenerate(propToGen.GetMethod, false, false,new object());
						Reference targetRef = GetTargetReference(HACK, mixinFields, SelfReference.Self);

						MethodEmitter proxiedMethod = ImplementProxiedMethod(getEmitter,
						                                              propToGen.GetMethod, emitter,
						                                              nestedClass, interceptorsField, targetRef,
						                                              ConstructorVersion.WithoutTargetMethod, null);
						if (propToGen.GetMethod.DeclaringType.IsInterface)
						{
							emitter.TypeBuilder.DefineMethodOverride(proxiedMethod.MethodBuilder, propToGen.GetMethod);
						}
						ReplicateNonInheritableAttributes(propToGen.GetMethod, getEmitter);
					}

					if (propToGen.CanWrite)
					{
						NestedClassEmitter nestedClass = method2Invocation[propToGen.SetMethod];

						string name;
						MethodAttributes atts = ObtainMethodAttributes(propToGen.SetMethod,out name);

						MethodEmitter setEmitter = propToGen.Emitter.CreateSetMethod(name, atts);

						var HACK = new MethodToGenerate(propToGen.SetMethod, false, false, new object());
						Reference targetRef = GetTargetReference(HACK, mixinFields, SelfReference.Self);

						MethodEmitter proxiedMethod = ImplementProxiedMethod(setEmitter,
						                                              propToGen.SetMethod, emitter,
						                                              nestedClass, interceptorsField, targetRef,
						                                              ConstructorVersion.WithoutTargetMethod, null);
						if (propToGen.SetMethod.DeclaringType.IsInterface)
						{
							emitter.TypeBuilder.DefineMethodOverride(proxiedMethod.MethodBuilder, propToGen.SetMethod);
						}
						ReplicateNonInheritableAttributes(propToGen.SetMethod, setEmitter);
					}
				}

				foreach (EventToGenerate eventToGenerate in eventsToGenerate)
				{
					eventToGenerate.BuildEventEmitter(emitter);
					NestedClassEmitter add_nestedClass = method2Invocation[eventToGenerate.AddMethod];

					string name;
					MethodAttributes add_atts = ObtainMethodAttributes(eventToGenerate.AddMethod,out name);

					MethodEmitter addEmitter = eventToGenerate.Emitter.CreateAddMethod(name, add_atts);

					var HACK = new MethodToGenerate(eventToGenerate.AddMethod, false, false, new object());
					MethodEmitter proxiedMethod = ImplementProxiedMethod(addEmitter,
					                                              eventToGenerate.AddMethod, emitter,
					                                              add_nestedClass, interceptorsField,
					                                              GetTargetReference(HACK, mixinFields, SelfReference.Self),
					                                              ConstructorVersion.WithoutTargetMethod, null);
					if (eventToGenerate.AddMethod.DeclaringType.IsInterface)
					{
						emitter.TypeBuilder.DefineMethodOverride(proxiedMethod.MethodBuilder, eventToGenerate.AddMethod);
					}
					ReplicateNonInheritableAttributes(eventToGenerate.AddMethod, addEmitter);

					NestedClassEmitter remove_nestedClass = method2Invocation[eventToGenerate.RemoveMethod];

					MethodAttributes remove_atts = ObtainMethodAttributes(eventToGenerate.RemoveMethod,out name);

					MethodEmitter removeEmitter = eventToGenerate.Emitter.CreateRemoveMethod(name, remove_atts);

					HACK = new MethodToGenerate(eventToGenerate.RemoveMethod, false, false, new object());
					MethodEmitter method = ImplementProxiedMethod(removeEmitter,
					                                              eventToGenerate.RemoveMethod, emitter,
					                                              remove_nestedClass, interceptorsField,
					                                              GetTargetReference(HACK, mixinFields, SelfReference.Self),
					                                              ConstructorVersion.WithoutTargetMethod, null);
					if (eventToGenerate.RemoveMethod.DeclaringType.IsInterface)
					{
						emitter.TypeBuilder.DefineMethodOverride(method.MethodBuilder, eventToGenerate.RemoveMethod);
					}
					ReplicateNonInheritableAttributes(eventToGenerate.RemoveMethod, removeEmitter);
				}

#if !SILVERLIGHT
				ImplementGetObjectData(emitter, interceptorsField, mixinFields, interfaces);
#endif

				// Complete type initializer code body

				CompleteInitCacheMethod(typeInitializer.CodeBuilder);

				// Build type

				type = emitter.BuildType();
				InitializeStaticFields(type);

				AddToCache(cacheKey, type);
			}

			return type;
		}

		private MethodInfo GetMethodOnTarget(MethodInfo interfaceMethod)
		{
			if (interfaceMethod.DeclaringType.IsClass)
			{
				return interfaceMethod;
			}
			if(!interfaceMethod.DeclaringType.IsAssignableFrom(targetType))
			{
				return null;
			}

			var map = targetType.GetInterfaceMap(interfaceMethod.DeclaringType);
			var index = Array.IndexOf(map.InterfaceMethods, interfaceMethod);
			if (index == -1)
			{
				throw new ProxyGenerationException("Could not find interfaceMethod which implements '" + interfaceMethod + "' on target type.");
			}
			var method = map.TargetMethods[index];
			if (!IsExplicitInterfaceMethodImplementation(method))
			{
				return method;
			}

			//it seems there's no way to invoke base explicit implementation. we can't call (base as IFoo).Bar();
			return null;
		}

		private bool IsExplicitInterfaceMethodImplementation(MethodInfo method)
		{
			return method.IsFinal && method.IsPrivate;
		}

		protected override Reference GetProxyTargetReference()
		{
			return SelfReference.Self;
		}

		protected override bool CanOnlyProxyVirtual()
		{
			return true;
		}

		protected override MethodInfo GetCallbackForMethod(MethodInfo method)
		{
			if (method.DeclaringType.IsAssignableFrom(targetType))
			{
				return method;
			}

			return null;
		}

		protected override Reference GetMethodTargetReference(MethodInfo method)
	    {
	    	return new AsTypeReference(SelfReference.Self, method.DeclaringType);
	    }

#if !SILVERLIGHT
		protected void GenerateSerializationConstructor(ClassEmitter emitter, FieldReference interceptorField,
		                                                FieldReference[] mixinFields)
		{
			ArgumentReference arg1 = new ArgumentReference(typeof (SerializationInfo));
			ArgumentReference arg2 = new ArgumentReference(typeof (StreamingContext));

			ConstructorEmitter constr = emitter.CreateConstructor(arg1, arg2);

			constr.CodeBuilder.AddStatement(
				new ConstructorInvocationStatement(serializationConstructor,
				                                   arg1.ToExpression(), arg2.ToExpression()));

			MethodInvocationExpression getInterceptorInvocation =
				new MethodInvocationExpression(arg1, SerializationInfoMethods.GetValue,
				                               new ConstReference("__interceptors").ToExpression(),
				                               new TypeTokenExpression(typeof (IInterceptor[])));

			constr.CodeBuilder.AddStatement(new AssignStatement(
			                                	interceptorField,
			                                	new ConvertExpression(typeof (IInterceptor[]), typeof (object),
			                                	                      getInterceptorInvocation)));

			// mixins
			foreach (FieldReference mixinFieldReference in mixinFields)
			{
				MethodInvocationExpression getMixinInvocation =
					new MethodInvocationExpression(arg1, SerializationInfoMethods.GetValue,
					                               new ConstReference(mixinFieldReference.Reference.Name).ToExpression(),
					                               new TypeTokenExpression(mixinFieldReference.Reference.FieldType));

				constr.CodeBuilder.AddStatement(new AssignStatement(
				                                	mixinFieldReference,
				                                	new ConvertExpression(mixinFieldReference.Reference.FieldType, typeof (object),
				                                	                      getMixinInvocation)));
			}

			constr.CodeBuilder.AddStatement(new ReturnStatement());
		}

		protected override void CustomizeGetObjectData(AbstractCodeBuilder codebuilder,
		                                               ArgumentReference arg1, ArgumentReference arg2)
		{
			codebuilder.AddStatement(new ExpressionStatement(
			                         	new MethodInvocationExpression(arg1, SerializationInfoMethods.AddValue_Bool,
			                         	                               new ConstReference("__delegateToBase").ToExpression(),
			                         	                               new ConstReference(delegateToBaseGetObjectData ? 1 : 0).
			                         	                               	ToExpression())));

			if (delegateToBaseGetObjectData)
			{
				MethodInfo baseGetObjectData = targetType.GetMethod("GetObjectData",
					new Type[] { typeof(SerializationInfo), typeof(StreamingContext) });

				codebuilder.AddStatement(new ExpressionStatement(
				                         	new MethodInvocationExpression(baseGetObjectData,
				                         	                               arg1.ToExpression(), arg2.ToExpression())));
			}
			else
			{
				LocalReference members_ref = codebuilder.DeclareLocal(typeof (MemberInfo[]));
				LocalReference data_ref = codebuilder.DeclareLocal(typeof (object[]));

				codebuilder.AddStatement(new AssignStatement(members_ref,
				                                             new MethodInvocationExpression(null, FormatterServicesMethods.GetSerializableMembers,
				                                                                            new TypeTokenExpression(targetType))));

				codebuilder.AddStatement(new AssignStatement(data_ref,
				                                             new MethodInvocationExpression(null, FormatterServicesMethods.GetObjectData,
				                                                                            SelfReference.Self.ToExpression(),
				                                                                            members_ref.ToExpression())));

				codebuilder.AddStatement(new ExpressionStatement(
				                         	new MethodInvocationExpression(arg1, SerializationInfoMethods.AddValue_Object,
				                         	                               new ConstReference("__data").ToExpression(),
				                         	                               data_ref.ToExpression())));
			}
		}
#endif
	}
}
