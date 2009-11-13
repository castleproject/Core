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
	using Contributors;

	/// <summary>
	/// 
	/// </summary>
	public class ClassProxyGenerator : BaseProxyGenerator
	{
		public ClassProxyGenerator(ModuleScope scope, Type targetType) : base(scope, targetType)
		{
			CheckNotGenericTypeDefinition(targetType, "targetType");
			EnsureDoesNotImplementIProxyTargetAccessor(targetType, "targetType");
		}

		private void EnsureDoesNotImplementIProxyTargetAccessor(Type type, string name)
		{
			if (!typeof (IProxyTargetAccessor).IsAssignableFrom(type))
			{
				return;
			}
			var message = "Target type for the proxy implements IProxyTargetAccessor " +
			              "which is a DynamicProxy infrastructure interface and you should never implement it yourself. " +
			              "Are you trying to proxy an existing proxy?";
			throw new ArgumentException(message, name);
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

				ProxyGenerationOptions = options;

				var name = Scope.NamingScope.GetUniqueName("Castle.Proxies." + targetType.Name + "Proxy");
				proxyType = GenerateType(name, interfaces,Scope.NamingScope.SafeSubScope());

				AddToCache(cacheKey, proxyType);
			}

			return proxyType;
		}

		private Type GenerateType(string newName, Type[] interfaces, INamingScope namingScope)
		{
			IEnumerable<ITypeContributor> contributors;
			var implementedInterfaces = GetTypeImplementerMapping(interfaces, out contributors,namingScope);

			// Collect methods
			foreach (var contributor in contributors)
			{
				contributor.CollectElementsToProxy(ProxyGenerationOptions.Hook);
			}
			ProxyGenerationOptions.Hook.MethodsInspected();

			var emitter = BuildClassEmitter(newName, targetType, implementedInterfaces);
			CreateOptionsField(emitter);
			CreateSelectorField(emitter);
			emitter.AddCustomAttributes(ProxyGenerationOptions);

#if !SILVERLIGHT
			emitter.DefineCustomAttribute<XmlIncludeAttribute>(new object[] {targetType});
#endif
			// Fields generations
			FieldReference interceptorsField = CreateInterceptorsField(emitter);



			// Constructor
			var cctor = GenerateStaticConstructor(emitter);

			var constructorArguments = new List<FieldReference>();
			foreach (var contributor in contributors)
			{
				contributor.Generate(emitter, ProxyGenerationOptions);

				// TODO: redo it
				if (contributor is MixinContributorBase)
				{
					constructorArguments.Add((contributor as MixinContributorBase).BackingField);
				}
			}

			// constructor arguments
			constructorArguments.Add(interceptorsField);
			var selector = emitter.GetField("__selector");
			if (selector != null)
			{
				constructorArguments.Add(selector);
			}

			GenerateConstructors(emitter, targetType, constructorArguments.ToArray());
			GenerateParameterlessConstructor(emitter, targetType, interceptorsField);


			// Complete type initializer code body
			CompleteInitCacheMethod(cctor.CodeBuilder);

			// Crosses fingers and build type

			Type proxyType = emitter.BuildType();
			InitializeStaticFields(proxyType);
			return proxyType;
		}

		private IEnumerable<Type> GetTypeImplementerMapping(Type[] interfaces, out IEnumerable<ITypeContributor> contributors, INamingScope namingScope)
		{
			var methodsToSkip = new List<MethodInfo>();
			var proxyInstance = new ClassProxyInstanceContributor(targetType, methodsToSkip, interfaces);
			// TODO: the trick with methodsToSkip is not very nice...
			var proxyTarget = new ClassProxyTargetContributor(targetType, methodsToSkip, namingScope);
			IDictionary<Type, ITypeContributor> typeImplementerMapping = new Dictionary<Type, ITypeContributor>();

			// Order of interface precedence:
			// 1. first target
			// target is not an interface so we do nothing

			var targetInterfaces = TypeUtil.GetAllInterfaces(targetType);
			var additionalInterfaces = TypeUtil.GetAllInterfaces(interfaces);
			// 2. then mixins
			var mixins = new List<MixinContributorBase>();
			if (ProxyGenerationOptions.HasMixins)
			{
				foreach (var mixinInterface in ProxyGenerationOptions.MixinData.MixinInterfaces)
				{
					object mixinInstance = ProxyGenerationOptions.MixinData.GetMixinInstance(mixinInterface);
					if (targetInterfaces.Contains(mixinInterface))
					{
						// OK, so the target implements this interface. We now do one of two things:
						if (additionalInterfaces.Contains(mixinInterface) && typeImplementerMapping.ContainsKey(mixinInterface) == false)
						{
							SafeAddMapping(mixinInterface, proxyTarget, typeImplementerMapping);
							proxyTarget.AddInterfaceMapping(mixinInterface);
						}
						// we do not intercept the interface
						mixins.Add(new EmptyMixinContributor(mixinInterface));
					}
					else
					{
						if (!typeImplementerMapping.ContainsKey(mixinInterface))
						{
							var mixin = new MixinContributor(mixinInterface, namingScope);
							mixins.Add(mixin);
							SafeAddMapping(mixinInterface, mixin, typeImplementerMapping);
						}
					}
				}
			}
			var additionalInterfacesContributor = new InterfaceProxyWithoutTargetContributor(namingScope, (c, m) => NullExpression.Instance);
			// 3. then additional interfaces
			foreach (var @interface in additionalInterfaces)
			{
				
				if (targetInterfaces.Contains(@interface))
				{
					
					if (typeImplementerMapping.ContainsKey(@interface)) continue;

					// we intercept the interface, and forward calls to the target type
					SafeAddMapping(@interface, proxyTarget, typeImplementerMapping);
					proxyTarget.AddInterfaceMapping(@interface);
				}
				else if (ProxyGenerationOptions.MixinData.ContainsMixin(@interface) == false)
				{
					additionalInterfacesContributor.AddInterfaceMapping(@interface);
					AddMapping(@interface, additionalInterfacesContributor, typeImplementerMapping);
				}
			}
#if !SILVERLIGHT
			// 4. plus special interfaces
			if (targetType.IsSerializable)
			{
				AddMappingForISerializable(typeImplementerMapping, proxyInstance);
			}
#endif
			try
			{
				SafeAddMapping(typeof(IProxyTargetAccessor), proxyInstance, typeImplementerMapping);
			}
			catch (ArgumentException )
			{
				HandleExplicitlyPassedProxyTargetAccessor(targetInterfaces, additionalInterfaces);
			}
			var contributorsList = new List<ITypeContributor>();
			contributorsList.Add(proxyTarget);
			foreach (var mixin in mixins)
			{
				contributorsList.Add(mixin);
			}
			contributorsList.Add(additionalInterfacesContributor);
			contributorsList.Add(proxyInstance);
			contributors = contributorsList;
			return typeImplementerMapping.Keys;
		}
	}
}
