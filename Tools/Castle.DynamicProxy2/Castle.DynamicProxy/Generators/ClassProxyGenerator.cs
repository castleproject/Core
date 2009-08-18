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
#if !SILVERLIGHT
	using System.Xml.Serialization;
#endif
	using Castle.Core.Interceptor;
	using Castle.Core.Internal;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Contributors;

	/// <summary>
	/// 
	/// </summary>
	public class ClassProxyGenerator : BaseProxyGenerator
	{
#if !SILVERLIGHT
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
			Type proxyType;

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

				proxyType = GenerateType(newName, interfaces);

				AddToCache(cacheKey, proxyType);
			}

			return proxyType;
		}

		protected override void AddInterfaceHierarchyMapping(Type @interface, ITypeContributor implementer, IDictionary<Type, ITypeContributor> mapping)
		{
			Debug.Assert(@interface.IsInterface, "@interface.IsInterface");

			AddInterfaceMapping(@interface, implementer, mapping);

			foreach (var baseInterface in @interface.GetInterfaces())
			{
				AddInterfaceHierarchyMapping(baseInterface, implementer, mapping);
			}
		}

		private void AddInterfaceMapping(Type @interface, ITypeContributor implementer, IDictionary<Type, ITypeContributor> mapping)
		{
			Debug.Assert(@interface.IsInterface, "@interface.IsInterface");
			if (!mapping.ContainsKey(@interface))
			{
				mapping.Add(@interface, implementer);
			}
		}

		private Type GenerateType(string newName, Type[] interfaces)
		{
			// TODO: this anemic dictionary should be made into a real object
			ClassProxyInstanceContributor proxyContributor;
			ClassProxyTargetContributor targetContributor;
			IDictionary<Type, ITypeContributor> typeImplementerMapping = GetTypeImplementerMapping(interfaces,out proxyContributor,out targetContributor);

			ClassEmitter emitter = BuildClassEmitter(newName, targetType, typeImplementerMapping.Keys);
			proxyContributor.ProxyGenerationOptions = CreateOptionsField(emitter);
			emitter.AddCustomAttributes(ProxyGenerationOptions);

#if !SILVERLIGHT
			emitter.DefineCustomAttribute(new XmlIncludeAttribute(targetType), ProxyGenerationOptions.AttributeDisassembler);
#endif
			// Custom attributes

			ReplicateNonInheritableAttributes(targetType, emitter);

			// Fields generations

			FieldReference interceptorsField = emitter.CreateField("__interceptors", typeof (IInterceptor[]));

#if !SILVERLIGHT
			emitter.DefineCustomAttributeFor(interceptorsField, new XmlIgnoreAttribute(),
			                                 ProxyGenerationOptions.AttributeDisassembler);
#endif
			// Collect methods
			IList<ProxyElementContributor> targets = new List<ProxyElementContributor>();

			// 1.first for the class we're proxying
			targets.Add(CollectElementsToProxy(new KeyValuePair<Type, ITypeContributor>(targetType, targetContributor), EmptyInterfaceMapping));

			// 2. then for interfaces
			foreach (var mapping in typeImplementerMapping)
			{
				// NOTE: make sure this is what it should be
				if (mapping.Value is ProxyInstanceContributor) continue;
				if (mapping.Value is ClassProxyTargetContributor)
				{
					var map = targetType.GetInterfaceMap(mapping.Key);
					targets.Add(CollectElementsToProxy(mapping, map));
				}
				else
				{
					targets.Add(CollectElementsToProxy(mapping, EmptyInterfaceMapping));
				}
			}

			ProxyGenerationOptions.Hook.MethodsInspected();

			// Constructor

			ConstructorEmitter typeInitializer = GenerateStaticConstructor(emitter);

			FieldReference[] mixinFields = AddMixinFields(emitter);

			// constructor arguments
			List<FieldReference> constructorArguments = new List<FieldReference>(mixinFields);
			constructorArguments.Add(interceptorsField);

			CreateInitializeCacheMethodBody(targetType, GetMethods(targets), emitter, typeInitializer);
			GenerateConstructors(emitter, targetType, constructorArguments.ToArray());
			GenerateParameterlessConstructor(emitter, targetType, interceptorsField);

			// Create callback methods and invocation types
			foreach (var target in targets)
			{
				foreach (var method in target.Methods)
				{
					var methodOnTarget = GetMethodOnTarget(method);
					var methodInfo = method.Method;
					var callback = default(MethodInfo);
					if (methodOnTarget != null)
					{
						callback = CreateCallbackMethod(emitter, methodInfo, methodOnTarget);
					}

					var targetForInvocation = methodInfo.DeclaringType;
					if (callback != null)
					{
						targetForInvocation = callback.DeclaringType;
					}

					method2Invocation[methodInfo] = BuildInvocationNestedType(emitter,
					                                                          targetForInvocation,
					                                                          method,
					                                                          callback);
					AddFieldToCacheMethodTokenAndStatementsToInitialize(methodInfo, typeInitializer, emitter);
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

				foreach (var property in target.Properties)
				{
					ImplementProperty(emitter, interceptorsField, mixinFields, property);
				}

				foreach (EventToGenerate @event in target.Events)
				{
					ImplementEvent(emitter, interceptorsField, mixinFields, @event);
				}
			}
			// TODO: not use 'interfaces', but 'typeImplementerMapping.Keys'
			proxyContributor.Generate(emitter, interceptorsField, mixinFields, interfaces);

			// Complete type initializer code body

			CompleteInitCacheMethod(typeInitializer.CodeBuilder);

			// Crosses fingers and build type

			Type proxyType = emitter.BuildType();
			InitializeStaticFields(proxyType);
			return proxyType;
		}

		protected override MethodInfo GetMethodOnTarget(IProxyMethod proxyMethod)
		{
			if(!proxyMethod.HasTarget)
			{
				return null;
			}

			var interfaceMethod = proxyMethod.Method;
			if (interfaceMethod.DeclaringType.IsClass)
			{
				return interfaceMethod;
			}

			if (proxyMethod.Target is MixinContributor)
			{
				return null;
			}

			if(!interfaceMethod.DeclaringType.IsAssignableFrom(targetType))
			{
				return null;
			}
			var method = TypeUtil.FindImplementingMethod(interfaceMethod, targetType);
			if (method == null)
			{
				throw new ProxyGenerationException("Could not find interfaceMethod which implements '" + interfaceMethod +
				                                   "' on target type.");
			}
			if (IsExplicitInterfaceMethodImplementation(method))
			{
				// it seems there's no way to invoke base explicit implementation. we can't call (base as IFoo).Bar();
				// we may be able to do this using technique described here: http://kennethxu.blogspot.com/2009/07/intercept-explicit-interface.html
				// for now however, we throw.
				throw new ProxyGenerationException(
					string.Format(
						"Type {0} implement explicitly method {1} from interface {2}. " +
						"Due to how CLR handles explicit interface implementation, we're not able to proxy this method.",
						targetType.FullName,
						interfaceMethod.ToString(),
						interfaceMethod.DeclaringType.FullName)
					);
			}
			return method;

			
		}

		private bool IsExplicitInterfaceMethodImplementation(MethodBase method)
		{
			// NOTE: we assume that method is interface implementation, we only check if it's implemented explicitly
			return method.IsFinal && method.IsPrivate;
		}

		protected override ConstructorVersion ConstructorVersion
		{
			get { return ConstructorVersion.WithoutTargetMethod; }
		}

		protected override Reference GetProxyTargetReference()
		{
			return SelfReference.Self;
		}

		protected override bool CanOnlyProxyVirtual()
		{
			return true;
		}

		protected override Reference GetMethodTargetReference(MethodInfo method)
	    {
	    	return new AsTypeReference(SelfReference.Self, method.DeclaringType);
	    }

		private IDictionary<Type, ITypeContributor> GetTypeImplementerMapping(Type[] interfaces, out ClassProxyInstanceContributor proxyInstance, out ClassProxyTargetContributor proxyTarget)
		{
			proxyTarget = new ClassProxyTargetContributor(targetType);
			proxyInstance = new ClassProxyInstanceContributor(targetType, methodsToSkip);
			IDictionary<Type, ITypeContributor> typeImplementerMapping = new Dictionary<Type, ITypeContributor>();

			// Order of interface precedence:
			// 1. first target
			// target is not an interface so we do nothing

			var targetInterfaces = TypeUtil.GetAllInterfaces(targetType);
			var additionalInterfaces = TypeUtil.GetAllInterfaces(interfaces);
			// 2. then mixins
			if (ProxyGenerationOptions.HasMixins)
			{
				foreach (var mixinInterface in ProxyGenerationOptions.MixinData.MixinInterfaces)
				{
					object mixinInstance = ProxyGenerationOptions.MixinData.GetMixinInstance(mixinInterface);
					AddMixinInterfaceMapping(mixinInterface, mixinInstance.GetType(),
					                         targetInterfaces, additionalInterfaces, typeImplementerMapping, proxyTarget);
				}
			}

			// 3. then additional interfaces

			foreach (var @interface in additionalInterfaces)
			{
				if (targetInterfaces.Contains(@interface))
				{
					// we intercept the interface, and forward calls to the target type
					AddInterfaceMapping(@interface, proxyTarget, typeImplementerMapping);
				}
				else
				{
					AddInterfaceMapping(@interface, null /*because there is no target*/, typeImplementerMapping);
				}
			}
#if !SILVERLIGHT
			// 4. plus special interfaces
			if (targetType.IsSerializable)
			{
				AddMappingForISerializable(typeImplementerMapping, proxyInstance);
			}
#endif
			AddInterfaceHierarchyMapping(typeof(IProxyTargetAccessor), proxyInstance, typeImplementerMapping);
			return typeImplementerMapping;
		}

		private void AddMixinInterfaceMapping(Type mixinInterface, Type typeUnderMixin, ICollection<Type> targetInterfaces, ICollection<Type> additionalInterfaces, IDictionary<Type, ITypeContributor> typeImplementerMapping, ITypeContributor proxyTarget)
		{
			if (targetInterfaces.Contains(mixinInterface))
			{
				// OK, so the target implements this interface. We now do one of two things:
				if(additionalInterfaces.Contains(mixinInterface))
				{
					// we intercept the interface, and forward calls to the target type
					AddInterfaceHierarchyMapping(mixinInterface, proxyTarget, typeImplementerMapping);
				}
				// we do not intercept the interface
			}
			else
			{
				if (!typeImplementerMapping.ContainsKey(mixinInterface))
				{
					typeImplementerMapping.Add(mixinInterface, new MixinContributor(typeUnderMixin, mixinInterface));
				}

			}
		}

		protected override bool IsInterfaceMethodForExplicitImplementation(IProxyMethod method)
		{
			var baseSays = base.IsInterfaceMethodForExplicitImplementation(method);
			return baseSays && (method.Target is ClassProxyTargetContributor) && !IsInterfaceMethodImplementedVirtually(method);
		}

		private bool IsInterfaceMethodImplementedVirtually(IProxyMethod method)
		{
			// TODO: this is not very optimal, that we obtain InterfaceMap anew each time. This should be cached somewhere probably.
			var @interface = method.Method.DeclaringType;
			var mapping = targetType.GetInterfaceMap(@interface);
			var index = Array.IndexOf(mapping.InterfaceMethods,method.Method);
			Debug.Assert(index >= 0);
			var classMethod = mapping.TargetMethods[index];
			return classMethod.IsFinal == false;
		}
	}
}
