// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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
#if !SILVERLIGHT
	using System.Xml.Serialization;
#endif

	using Castle.DynamicProxy.Contributors;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Serialization;

	public class ClassProxyWithTargetGenerator : BaseProxyGenerator
	{
		private readonly Type[] additionalInterfacesToProxy;

		public ClassProxyWithTargetGenerator(ModuleScope scope, Type classToProxy, Type[] additionalInterfacesToProxy,
		                                     ProxyGenerationOptions options)
			: base(scope, classToProxy)
		{
			CheckNotGenericTypeDefinition(targetType, "targetType");
			EnsureDoesNotImplementIProxyTargetAccessor(targetType, "targetType");
			CheckNotGenericTypeDefinitions(additionalInterfacesToProxy, "additionalInterfacesToProxy");

			options.Initialize();
			ProxyGenerationOptions = options;
			this.additionalInterfacesToProxy = TypeUtil.GetAllInterfaces(additionalInterfacesToProxy).ToArray();
		}

		public Type GetGeneratedType()
		{
			var cacheKey = new CacheKey(targetType, targetType, additionalInterfacesToProxy, ProxyGenerationOptions);
			return ObtainProxyType(cacheKey, GenerateType);
		}

		protected virtual IEnumerable<Type> GetTypeImplementerMapping(out IEnumerable<ITypeContributor> contributors,
		                                                              INamingScope namingScope)
		{
			var methodsToSkip = new List<MethodInfo>();
			var proxyInstance = new ClassProxyInstanceContributor(targetType, methodsToSkip, additionalInterfacesToProxy,
			                                                      ProxyTypeConstants.ClassWithTarget);
			// TODO: the trick with methodsToSkip is not very nice...
			var proxyTarget = new ClassProxyWithTargetTargetContributor(targetType, methodsToSkip, namingScope)
			{ Logger = Logger };
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
						if (additionalInterfacesToProxy.Contains(mixinInterface) &&
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
			foreach (var @interface in additionalInterfacesToProxy)
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
#if !SILVERLIGHT
			// 4. plus special interfaces
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
				HandleExplicitlyPassedProxyTargetAccessor(targetInterfaces, additionalInterfacesToProxy);
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

		private FieldReference CreateTargetField(ClassEmitter emitter)
		{
			var targetField = emitter.CreateField("__target", targetType);
#if !SILVERLIGHT
			emitter.DefineCustomAttributeFor<XmlIgnoreAttribute>(targetField);
#endif
			return targetField;
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

		private Type GenerateType(string name, INamingScope namingScope)
		{
			IEnumerable<ITypeContributor> contributors;
			var implementedInterfaces = GetTypeImplementerMapping(out contributors, namingScope);

			var model = new MetaType();
			// Collect methods
			foreach (var contributor in contributors)
			{
				contributor.CollectElementsToProxy(ProxyGenerationOptions.Hook, model);
			}
			ProxyGenerationOptions.Hook.MethodsInspected();

			var emitter = BuildClassEmitter(name, targetType, implementedInterfaces);

			CreateFields(emitter);
			CreateTypeAttributes(emitter);

			// Constructor
			var cctor = GenerateStaticConstructor(emitter);

			var targetField = CreateTargetField(emitter);
			var constructorArguments = new List<FieldReference> { targetField };

			foreach (var contributor in contributors)
			{
				contributor.Generate(emitter, ProxyGenerationOptions);

				// TODO: redo it
				if (contributor is MixinContributor)
				{
					constructorArguments.AddRange((contributor as MixinContributor).Fields);
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
	}
}