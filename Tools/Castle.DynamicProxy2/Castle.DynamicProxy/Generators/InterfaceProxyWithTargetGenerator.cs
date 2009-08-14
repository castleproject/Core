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
#if !SILVERLIGHT
	using System.Xml.Serialization;
#endif
	using Castle.Core.Interceptor;
	using Castle.Core.Internal;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Tokens;
	using Contributors;

	/// <summary>
	/// 
	/// </summary>
	public class InterfaceProxyWithTargetGenerator : BaseProxyGenerator
	{
		private FieldReference targetField;
		protected readonly Dictionary<MethodInfo, MethodInfo> method2methodOnTarget = new Dictionary<MethodInfo, MethodInfo>();


		public InterfaceProxyWithTargetGenerator(ModuleScope scope, Type @interface)
			: base(scope, @interface)
		{
			CheckNotGenericTypeDefinition(@interface, "@interface");
		}

		public Type GenerateCode(Type proxyTargetType, Type[] interfaces, ProxyGenerationOptions options)
		{
			// make sure ProxyGenerationOptions is initialized
			options.Initialize();

			CheckNotGenericTypeDefinition(proxyTargetType, "proxyTargetType");
			CheckNotGenericTypeDefinitions(interfaces, "interfaces");
			Type proxyType;

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
				proxyType = GenerateType(newName, proxyTargetType, interfaces);

				AddToCache(cacheKey, proxyType);
			}

			return proxyType;
		}

		private Type GenerateType(string typeName, Type proxyTargetType, Type[] interfaces)
		{
			// TODO: this anemic dictionary should be made into a real object
			InterfaceProxyInstanceContributor proxyContributor;
			IDictionary<Type, ITypeContributor> typeImplementerMapping = GetTypeImplementerMapping(interfaces,out proxyContributor);

			// This is flawed. We allow any type to be a base type but we don't realy handle it properly.
			// What if the type implements interfaces? What if it implements target interface?
			// What if it implement mixin interface? What if it implements any additional interface?
			// What if it has no default constructor?
			// We handle none of these cases.
			Type baseType = ProxyGenerationOptions.BaseTypeForInterfaceProxy;

			ClassEmitter emitter = BuildClassEmitter(typeName, baseType, typeImplementerMapping.Keys);
			proxyContributor.ProxyGenerationOptions = CreateOptionsField(emitter);
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
			proxyContributor.TargetField = targetField;

#if SILVERLIGHT
#warning XmlIncludeAttribute is in silverlight, do we want to explore this?
#else
			emitter.DefineCustomAttributeFor(interceptorsField, new XmlIgnoreAttribute());
			emitter.DefineCustomAttributeFor(targetField, new XmlIgnoreAttribute());
#endif

			// Collect methods
			IList<ProxyElementContributor> targets = new List<ProxyElementContributor>();
			foreach (var mapping in typeImplementerMapping)
			{
				// NOTE: make sure this is what it should be
				if (mapping.Value is ProxyInstanceContributor) continue;

				targets.Add(CollectElementsToProxy(mapping, EmptyInterfaceMapping));
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
			List<FieldReference> constructorArguments = new List<FieldReference>(mixinFields);
			constructorArguments.Add(interceptorsField);
			constructorArguments.Add(targetField);
			GenerateConstructors(emitter, baseType, constructorArguments.ToArray());

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

			proxyContributor.Generate(emitter, interceptorsField, mixinFields, interfaces);

			// Complete type initializer code body

			CompleteInitCacheMethod(typeInitializer.CodeBuilder);

			// Crosses fingers and build type

			Type generatedType = emitter.BuildType();
			InitializeStaticFields(generatedType);
			return generatedType;
		}

		protected override ConstructorVersion ConstructorVersion
		{
			get { return ConstructorVersion.WithTargetMethod; }
		}

		protected virtual void CreateInvocationForMethod(ClassEmitter emitter, MethodToGenerate method, Type proxyTargetType)
		{
			var methodInfo = method.Method;
			MethodInfo methodOnTarget = methodInfo;
			// TODO: this is a temporary workaround
			if (method.Target is InterfaceProxyTargetContributor)
			{
				if(!proxyTargetType.IsInterface)
				{
					var foundCandidate = TypeUtil.FindImplementingMethod(methodInfo, proxyTargetType);
					if (foundCandidate != null)
					{
						methodOnTarget = foundCandidate;
					}
				}
			}
			else if (method.HasTarget)
			{
				var mixin = method.Target as MixinContributor;
				if (mixin != null)
				{
					var foundCandidate = TypeUtil.FindImplementingMethod(methodInfo, mixin.ClassUnderMixinInterface);
					if (foundCandidate != null)
					{
						methodOnTarget = foundCandidate;
					}
				}
			}

			method2methodOnTarget[methodInfo] = methodOnTarget;

			method2Invocation[methodInfo] = BuildInvocationNestedType(emitter,
			                                                          methodInfo.DeclaringType,
			                                                          method,
			                                                          methodInfo,
			                                                          AllowChangeTarget);
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

				Type paramType = TypeUtil.GetClosedParameterType(nested, param.ParameterType);
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
				Type returnType = TypeUtil.GetClosedParameterType(nested, callbackMethod1.ReturnType);
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

		protected override Reference GetProxyTargetReference()
		{
			return targetField;
		}

		protected override bool CanOnlyProxyVirtual()
		{
			return false;
		}

		protected override Reference GetMethodTargetReference(MethodInfo method)
	    {
	        return new AsTypeReference(targetField, method.DeclaringType);
	    }

		protected override MethodInfo GetMethodOnTarget(IProxyMethod proxyMethod)
		{
			return method2methodOnTarget[proxyMethod.Method];
		}

		protected virtual InterfaceGeneratorType GeneratorType
		{
			get { return InterfaceGeneratorType.WithTarget; }
		}

		protected virtual bool AllowChangeTarget
		{
			get { return false; }
		}

		protected IDictionary<Type, ITypeContributor> GetTypeImplementerMapping(Type[] interfaces, out InterfaceProxyInstanceContributor instance)
		{
			IDictionary<Type, ITypeContributor> typeImplementerMapping = new Dictionary<Type, ITypeContributor>();

			// Order of interface precedence:
			// 1. first target
			AddMappingForTargetType(typeImplementerMapping);

			// 2. then mixins
			if (ProxyGenerationOptions.HasMixins)
			{
				foreach (var mixinInterface in ProxyGenerationOptions.MixinData.MixinInterfaces)
				{
					object mixinInstance = ProxyGenerationOptions.MixinData.GetMixinInstance(mixinInterface);
					AddInterfaceHierarchyMapping(mixinInterface, new MixinContributor(mixinInstance.GetType(), mixinInterface),
					                             typeImplementerMapping);
				}
			}

			// 3. then additional interfaces
			if (interfaces != null)
			{
				foreach (var @interface in interfaces)
				{
					AddInterfaceHierarchyMapping(@interface, ProxyContributor.Empty, typeImplementerMapping);
				}
			}

			// 4. plus special interfaces
			instance = new InterfaceProxyInstanceContributor(targetType, GeneratorType);
			AddMappingForISerializable(typeImplementerMapping, instance);
			AddInterfaceHierarchyMapping(typeof(IProxyTargetAccessor), instance, typeImplementerMapping);
			return typeImplementerMapping;
		}

		protected virtual void AddMappingForTargetType(IDictionary<Type, ITypeContributor> typeImplementerMapping)
		{
			AddInterfaceHierarchyMapping(targetType, new InterfaceProxyTargetContributor(targetType), typeImplementerMapping);
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
