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
	using System.Reflection.Emit;
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
	public class ClassProxyGenerator : BaseProxyGenerator
	{
#if !SILVERLIGHT
		private bool delegateToBaseGetObjectData = false;
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

				String newName = targetType.Name + "Proxy" + Guid.NewGuid().ToString("N");

				// Add Interfaces that the proxy implements 

				List<Type> interfaceList = new List<Type>();

				if (interfaces != null)
				{
					interfaceList.AddRange(interfaces);
				}

				ValidateMixinInterfaces (targetType.GetInterfaces (), "target type " + targetType.Name);
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
				EventToGenerate[] eventToGenerates;
				MethodInfo[] methods = CollectMethodsAndProperties(emitter, targetType, out propsToGenerate, out eventToGenerates);

				RegisterMixinMethodsAndProperties(emitter, ref methods, ref propsToGenerate, ref eventToGenerates);

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
				// Implement interfaces

				if (interfaces != null && interfaces.Length != 0)
				{
					foreach (Type inter in interfaces)
					{
						ImplementBlankInterface(targetType, inter, emitter, interceptorsField, typeInitializer);
					}
				}

				// Create callback methods

				Dictionary<MethodInfo, MethodBuilder> method2Callback = new Dictionary<MethodInfo, MethodBuilder>();

				foreach (MethodInfo method in methods)
				{
					method2Callback[method] = CreateCallbackMethod(emitter, method, method);
				}

				// Create invocation types

				Dictionary<MethodInfo, NestedClassEmitter> method2Invocation = new Dictionary<MethodInfo, NestedClassEmitter>();

				foreach (MethodInfo method in methods)
				{
					MethodBuilder callbackMethod = method2Callback[method];

					method2Invocation[method] = BuildInvocationNestedType(emitter, targetType,
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

					Reference targetRef = GetTargetRef(method, mixinFields, SelfReference.Self);
					MethodEmitter newProxiedMethod = CreateProxiedMethod(
						targetType, method, emitter, nestedClass, interceptorsField, targetRef,
						ConstructorVersion.WithoutTargetMethod, null);

					ReplicateNonInheritableAttributes(method, newProxiedMethod);

					method2Emitter[method] = newProxiedMethod;
				}

				foreach (PropertyToGenerate propToGen in propsToGenerate)
				{
					if (propToGen.CanRead)
					{
						NestedClassEmitter nestedClass = method2Invocation[propToGen.GetMethod];

						MethodAttributes atts = ObtainMethodAttributes(propToGen.GetMethod);

						MethodEmitter getEmitter = propToGen.Emitter.CreateGetMethod(atts);

						Reference targetRef = GetTargetRef(propToGen.GetMethod, mixinFields, SelfReference.Self);

						ImplementProxiedMethod(targetType, getEmitter,
						                       propToGen.GetMethod, emitter,
						                       nestedClass, interceptorsField, targetRef,
						                       ConstructorVersion.WithoutTargetMethod, null);

						ReplicateNonInheritableAttributes(propToGen.GetMethod, getEmitter);
					}

					if (propToGen.CanWrite)
					{
						NestedClassEmitter nestedClass = method2Invocation[propToGen.SetMethod];

						MethodAttributes atts = ObtainMethodAttributes(propToGen.SetMethod);

						MethodEmitter setEmitter = propToGen.Emitter.CreateSetMethod(atts);

						Reference targetRef = GetTargetRef(propToGen.SetMethod, mixinFields, SelfReference.Self);

						ImplementProxiedMethod(targetType, setEmitter,
						                       propToGen.SetMethod, emitter,
						                       nestedClass, interceptorsField, targetRef,
						                       ConstructorVersion.WithoutTargetMethod, null);

						ReplicateNonInheritableAttributes(propToGen.SetMethod, setEmitter);
					}
				}

				foreach (EventToGenerate eventToGenerate in eventToGenerates)
				{
					NestedClassEmitter add_nestedClass = method2Invocation[eventToGenerate.AddMethod];

					MethodAttributes add_atts = ObtainMethodAttributes(eventToGenerate.AddMethod);

					MethodEmitter addEmitter = eventToGenerate.Emitter.CreateAddMethod(add_atts);

					ImplementProxiedMethod(targetType, addEmitter,
					                       eventToGenerate.AddMethod, emitter,
					                       add_nestedClass, interceptorsField,
					                       GetTargetRef(eventToGenerate.AddMethod, mixinFields, SelfReference.Self),
					                       ConstructorVersion.WithoutTargetMethod, null);

					ReplicateNonInheritableAttributes(eventToGenerate.AddMethod, addEmitter);

					NestedClassEmitter remove_nestedClass = method2Invocation[eventToGenerate.RemoveMethod];

					MethodAttributes remove_atts = ObtainMethodAttributes(eventToGenerate.RemoveMethod);

					MethodEmitter removeEmitter = eventToGenerate.Emitter.CreateRemoveMethod(remove_atts);

					ImplementProxiedMethod(targetType, removeEmitter,
					                       eventToGenerate.RemoveMethod, emitter,
					                       remove_nestedClass, interceptorsField,
					                       GetTargetRef(eventToGenerate.RemoveMethod, mixinFields, SelfReference.Self),
					                       ConstructorVersion.WithoutTargetMethod, null);

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

		protected override Reference GetProxyTargetReference()
		{
			return SelfReference.Self;
		}

		protected override bool CanOnlyProxyVirtual()
		{
			return true;
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

			Type[] object_arg = new Type[] {typeof (String), typeof (Type)};
			MethodInfo getValueMethod = typeof (SerializationInfo).GetMethod("GetValue", object_arg);

			MethodInvocationExpression getInterceptorInvocation =
				new MethodInvocationExpression(arg1, getValueMethod,
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
					new MethodInvocationExpression(arg1, getValueMethod,
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
			Type[] key_and_object = new Type[] {typeof (String), typeof (Object)};
			Type[] key_and_bool = new Type[] {typeof (String), typeof (bool)};
			MethodInfo addValueMethod = typeof (SerializationInfo).GetMethod("AddValue", key_and_object);
			MethodInfo addValueBoolMethod = typeof (SerializationInfo).GetMethod("AddValue", key_and_bool);

			codebuilder.AddStatement(new ExpressionStatement(
			                         	new MethodInvocationExpression(arg1, addValueBoolMethod,
			                         	                               new ConstReference("__delegateToBase").ToExpression(),
			                         	                               new ConstReference(delegateToBaseGetObjectData ? 1 : 0).
			                         	                               	ToExpression())));

			if (delegateToBaseGetObjectData)
			{
				MethodInfo baseGetObjectData = targetType.GetMethod("GetObjectData",
				                                                    new Type[]
				                                                    	{typeof (SerializationInfo), typeof (StreamingContext)});

				codebuilder.AddStatement(new ExpressionStatement(
				                         	new MethodInvocationExpression(baseGetObjectData,
				                         	                               arg1.ToExpression(), arg2.ToExpression())));
			}
			else
			{
				LocalReference members_ref = codebuilder.DeclareLocal(typeof (MemberInfo[]));
				LocalReference data_ref = codebuilder.DeclareLocal(typeof (object[]));

				MethodInfo getSerMembers = typeof (FormatterServices).GetMethod("GetSerializableMembers",
				                                                                new Type[] {typeof (Type)});
				MethodInfo getObjData = typeof (FormatterServices).GetMethod("GetObjectData",
				                                                             new Type[] {typeof (object), typeof (MemberInfo[])});

				codebuilder.AddStatement(new AssignStatement(members_ref,
				                                             new MethodInvocationExpression(null, getSerMembers,
				                                                                            new TypeTokenExpression(targetType))));

				codebuilder.AddStatement(new AssignStatement(data_ref,
				                                             new MethodInvocationExpression(null, getObjData,
				                                                                            SelfReference.Self.ToExpression(),
				                                                                            members_ref.ToExpression())));

				codebuilder.AddStatement(new ExpressionStatement(
				                         	new MethodInvocationExpression(arg1, addValueMethod,
				                         	                               new ConstReference("__data").ToExpression(),
				                         	                               data_ref.ToExpression())));
			}
		}
#endif
	}
}
