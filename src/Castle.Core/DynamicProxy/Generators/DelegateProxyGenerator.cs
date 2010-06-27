// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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
	using System.Xml.Serialization;

	using Castle.DynamicProxy.Contributors;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Serialization;

	public class DelegateProxyGenerator : BaseProxyGenerator
	{
		public DelegateProxyGenerator(ModuleScope scope, Type delegateType) : base(scope, delegateType)
		{
			ProxyGenerationOptions = new ProxyGenerationOptions(new DelegateProxyGenerationHook());
			ProxyGenerationOptions.Initialize();
		}

		public Type GetProxyType(IProxyBuilder builder)
		{
			var scope = builder.ModuleScope;
			var logger = builder.Logger;
			var cacheKey = new CacheKey(targetType, null, null);
			using (var locker = scope.Lock.ForReadingUpgradeable())
			{
				var cacheType = scope.GetFromCache(cacheKey);
				if (cacheType != null)
				{
					logger.Debug("Found cached proxy type {0} for target type {1}.", cacheType.FullName, targetType.FullName);
					return cacheType;
				}

				// Upgrade the lock to a write lock, then read again. This is to avoid generating duplicate types
				// under heavy multithreaded load.
				locker.Upgrade();

				cacheType = scope.GetFromCache(cacheKey);
				if (cacheType != null)
				{
					logger.Debug("Found cached proxy type {0} for target type {1}.", cacheType.FullName, targetType.FullName);
					return cacheType;
				}

				// Log details about the cache miss
				logger.Debug("No cached proxy type was found for target type {0}.", targetType.FullName);

				var name = scope.NamingScope.GetUniqueName("Castle.Proxies." + targetType.Name + "Proxy");
				var proxyType = GenerateType(name, scope.NamingScope.SafeSubScope());

				scope.RegisterInCache(cacheKey, proxyType);
				return proxyType;
			}
		}


		private Type GenerateType(string name, INamingScope namingScope)
		{
			IEnumerable<ITypeContributor> contributors;
			var implementedInterfaces = GetTypeImplementerMapping(out contributors, namingScope);

			var model = new MetaType(name, typeof (object), implementedInterfaces);
			// Collect methods
			foreach (var contributor in contributors)
			{
				contributor.CollectElementsToProxy(ProxyGenerationOptions.Hook, model);
			}
			ProxyGenerationOptions.Hook.MethodsInspected();

			var emitter = BuildClassEmitter(name, typeof (object), implementedInterfaces);

			CreateFields(emitter);
			CreateTypeAttributes(emitter);

			// Constructor
			var cctor = GenerateStaticConstructor(emitter);

			var targetField = CreateTargetField(emitter);
			var constructorArguments = new List<FieldReference> {targetField};

			foreach (var contributor in contributors)
			{
				contributor.Generate(emitter, ProxyGenerationOptions);
			}

			// constructor arguments
			var interceptorsField = emitter.GetField("__interceptors");
			constructorArguments.Add(interceptorsField);
			var selector = emitter.GetField("__selector");
			if (selector != null)
			{
				constructorArguments.Add(selector);
			}

			GenerateConstructor(emitter, null, constructorArguments.ToArray());
			GenerateParameterlessConstructor(emitter, targetType, interceptorsField);

			// Complete type initializer code body
			CompleteInitCacheMethod(cctor.CodeBuilder);

			// Crosses fingers and build type

			var proxyType = emitter.BuildType();
			InitializeStaticFields(proxyType);
			return proxyType;
		}

		protected virtual IEnumerable<Type> GetTypeImplementerMapping(out IEnumerable<ITypeContributor> contributors,
		                                                              INamingScope namingScope)
		{
			var methodsToSkip = new List<MethodInfo>();
			var proxyInstance = new ClassProxyInstanceContributor(targetType, methodsToSkip, Type.EmptyTypes,
			                                                      ProxyTypeConstants.ClassWithTarget);
			var proxyTarget = new DelegateProxyTargetContributor(targetType, namingScope) {Logger = Logger};
			IDictionary<Type, ITypeContributor> typeImplementerMapping = new Dictionary<Type, ITypeContributor>();

			// Order of interface precedence:
			// 1. first target, target is not an interface so we do nothing
			// 2. then mixins - we support none so we do nothing
			// 3. then additional interfaces - we support none so we do nothing
#if !SILVERLIGHT
			// 4. plus special interfaces
			if (targetType.IsSerializable)
			{
				AddMappingForISerializable(typeImplementerMapping, proxyInstance);
			}
#endif
			AddMappingNoCheck(typeof (IProxyTargetAccessor), proxyInstance, typeImplementerMapping);

			contributors = new List<ITypeContributor>
			               	{
			               		proxyTarget,
			               		proxyInstance
			               	};
			return typeImplementerMapping.Keys;
		}


		private FieldReference CreateTargetField(ClassEmitter emitter)
		{
			var targetField = emitter.CreateField("__target", targetType);
			emitter.DefineCustomAttributeFor<XmlIgnoreAttribute>(targetField);
			return targetField;
		}
	}
}