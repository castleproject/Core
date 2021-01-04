// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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
	using System.Linq;
	using System.Reflection;

	using Castle.DynamicProxy.Contributors;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Internal;

	internal abstract class BaseClassProxyGenerator : BaseProxyGenerator
	{
		protected BaseClassProxyGenerator(ModuleScope scope, Type targetType, Type[] interfaces, ProxyGenerationOptions options)
			: base(scope, targetType, interfaces, options)
		{
			EnsureDoesNotImplementIProxyTargetAccessor(targetType, nameof(targetType));
		}

		protected abstract FieldReference TargetField { get; }

		protected abstract ProxyInstanceContributor GetProxyInstanceContributor(List<MethodInfo> methodsToSkip);

		protected abstract CompositeTypeContributor GetProxyTargetContributor(List<MethodInfo> methodsToSkip, INamingScope namingScope);

		protected sealed override Type GenerateType(string name, INamingScope namingScope)
		{
			IEnumerable<ITypeContributor> contributors;
			var allInterfaces = GetTypeImplementerMapping(out contributors, namingScope);

			var model = new MetaType();
			// Collect methods
			foreach (var contributor in contributors)
			{
				contributor.CollectElementsToProxy(ProxyGenerationOptions.Hook, model);
			}
			ProxyGenerationOptions.Hook.MethodsInspected();

			var emitter = BuildClassEmitter(name, targetType, allInterfaces);

			CreateFields(emitter);
			CreateTypeAttributes(emitter);

			// Constructor
			var cctor = GenerateStaticConstructor(emitter);

			var constructorArguments = new List<FieldReference>();

			if (TargetField is { } targetField)
			{
				constructorArguments.Add(targetField);
			}

			foreach (var contributor in contributors)
			{
				contributor.Generate(emitter);

				// TODO: redo it
				if (contributor is MixinContributor mixinContributor)
				{
					constructorArguments.AddRange(mixinContributor.Fields);
				}
			}

			// constructor arguments
			var interceptorsField = emitter.GetField("__interceptors");
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

			var proxyType = emitter.BuildType();
			InitializeStaticFields(proxyType);
			return proxyType;
		}

		private IEnumerable<Type> GetTypeImplementerMapping(out IEnumerable<ITypeContributor> contributors, INamingScope namingScope)
		{
			var methodsToSkip = new List<MethodInfo>();
			var proxyInstance = GetProxyInstanceContributor(methodsToSkip);
			// TODO: the trick with methodsToSkip is not very nice...
			var proxyTarget = GetProxyTargetContributor(methodsToSkip, namingScope);
			IDictionary<Type, ITypeContributor> typeImplementerMapping = new Dictionary<Type, ITypeContributor>();

			// Order of interface precedence:
			// 1. first target
			// target is not an interface so we do nothing

			var targetInterfaces = targetType.GetAllInterfaces();
			// 2. then mixins
			var mixins = new MixinContributor(namingScope, false) { Logger = Logger };
			if (ProxyGenerationOptions.HasMixins)
			{
				foreach (var mixinInterface in ProxyGenerationOptions.MixinData.MixinInterfaces)
				{
					if (targetInterfaces.Contains(mixinInterface))
					{
						// OK, so the target implements this interface. We now do one of two things:
						if (interfaces.Contains(mixinInterface) &&
							typeImplementerMapping.ContainsKey(mixinInterface) == false)
						{
							AddMappingNoCheck(mixinInterface, proxyTarget, typeImplementerMapping);
							proxyTarget.AddInterfaceToProxy(mixinInterface);
						}
						// we do not intercept the interface
						mixins.AddEmptyInterface(mixinInterface);
					}
					else
					{
						if (!typeImplementerMapping.ContainsKey(mixinInterface))
						{
							mixins.AddInterfaceToProxy(mixinInterface);
							AddMappingNoCheck(mixinInterface, mixins, typeImplementerMapping);
						}
					}
				}
			}
			var additionalInterfacesContributor = new InterfaceProxyWithoutTargetContributor(namingScope,
			                                                                                 (c, m) => NullExpression.Instance)
			{ Logger = Logger };
			// 3. then additional interfaces
			foreach (var @interface in interfaces)
			{
				if (targetInterfaces.Contains(@interface))
				{
					if (typeImplementerMapping.ContainsKey(@interface))
					{
						continue;
					}

					// we intercept the interface, and forward calls to the target type
					AddMappingNoCheck(@interface, proxyTarget, typeImplementerMapping);
					proxyTarget.AddInterfaceToProxy(@interface);
				}
				else if (ProxyGenerationOptions.MixinData.ContainsMixin(@interface) == false)
				{
					additionalInterfacesContributor.AddInterfaceToProxy(@interface);
					AddMapping(@interface, additionalInterfacesContributor, typeImplementerMapping);
				}
			}
			// 4. plus special interfaces
#if FEATURE_SERIALIZATION
			if (targetType.IsSerializable)
			{
				AddMappingForISerializable(typeImplementerMapping, proxyInstance);
			}
#endif
			try
			{
				AddMappingNoCheck(typeof(IProxyTargetAccessor), proxyInstance, typeImplementerMapping);
			}
			catch (ArgumentException)
			{
				HandleExplicitlyPassedProxyTargetAccessor(targetInterfaces);
			}

			contributors = new List<ITypeContributor>
			{
				proxyTarget,
				mixins,
				additionalInterfacesContributor,
				proxyInstance
			};
			return typeImplementerMapping.Keys;
		}

		private void EnsureDoesNotImplementIProxyTargetAccessor(Type type, string name)
		{
			if (!typeof(IProxyTargetAccessor).IsAssignableFrom(type))
			{
				return;
			}
			var message =
				string.Format(
					"Target type for the proxy implements {0} which is a DynamicProxy infrastructure interface and you should never implement it yourself. Are you trying to proxy an existing proxy?",
					typeof(IProxyTargetAccessor));
			throw new ArgumentException(message, name);
		}
	}
}
