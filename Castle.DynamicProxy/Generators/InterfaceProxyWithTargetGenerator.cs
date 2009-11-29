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
#if !SILVERLIGHT
	using System.Reflection;
	using System.Xml.Serialization;
#endif
	using Castle.Core.Interceptor;
	using Castle.Core.Internal;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Serialization;

	using Contributors;

	/// <summary>
	/// 
	/// </summary>
	public class InterfaceProxyWithTargetGenerator : BaseProxyGenerator
	{
		protected FieldReference targetField;


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
			EnsureValidBaseType(options.BaseTypeForInterfaceProxy);
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

				ProxyGenerationOptions = options;

				var name = Scope.NamingScope.GetUniqueName("Castle.Proxies." + targetType.Name + "Proxy");
				proxyType = GenerateType(name, proxyTargetType, interfaces, Scope.NamingScope.SafeSubScope());

				AddToCache(cacheKey, proxyType);
			}

			return proxyType;
		}

		private void EnsureValidBaseType(Type type)
		{
			if (type == null)
			{
				throw new ArgumentException(
					"Base type for proxy is null reference. Please set it to System.Object or some other valid type.");
			}

			if (!type.IsClass)
			{
				ThrowInvalidBaseType(type, "it is not a class type");
			}

			if(type.IsSealed)
			{
				ThrowInvalidBaseType(type, "it is sealed");
			}
#if !SILVERLIGHT
			var constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
			                                      null, Type.EmptyTypes, null);

			if (constructor == null || constructor.IsPrivate)
			{
				ThrowInvalidBaseType(type, "it does not have accessible parameterless constructor");
			}
#else
#warning this constructor exists in SL 3, so we can remove the if when we move to SL 3
#endif
		}

		private void ThrowInvalidBaseType(Type type, string doesNotHaveAccessibleParameterlessConstructor)
		{
			var format = "Type {0} is not valid base type for interface proxy, because {1}. Only a non-sealed class with non-private default constructor can be used as base type for interface proxy. Please use some other valid type.";
			throw new ArgumentException(string.Format(format, type, doesNotHaveAccessibleParameterlessConstructor));
		}

		protected virtual Type GenerateType(string typeName, Type proxyTargetType, Type[] interfaces, INamingScope namingScope)
		{
			// TODO: this anemic dictionary should be made into a real object
			IEnumerable<ITypeContributor> contributors;
			var typeImplementerMapping = GetTypeImplementerMapping(interfaces, proxyTargetType, out contributors,namingScope);

			ClassEmitter emitter;
			FieldReference interceptorsField;
			Type baseType = Init(typeName, typeImplementerMapping, out emitter, proxyTargetType, out interceptorsField);


			// Collect methods
			foreach (var contributor in contributors)
			{
				contributor.CollectElementsToProxy(ProxyGenerationOptions.Hook);
			}

			ProxyGenerationOptions.Hook.MethodsInspected();

			// Constructor

			var cctor = GenerateStaticConstructor(emitter);
			var ctorArguments = new List<FieldReference>();

			foreach (var contributor in contributors)
			{
				contributor.Generate(emitter, ProxyGenerationOptions);

				// TODO: redo it
				if (contributor is MixinContributor)
				{
					ctorArguments.AddRange((contributor as MixinContributor).Fields);
					
				}
			}

			ctorArguments.Add(interceptorsField);
			ctorArguments.Add(targetField);
			var selector = emitter.GetField("__selector");
			if (selector != null)
			{
				ctorArguments.Add(selector);
			}

			GenerateConstructors(emitter, baseType, ctorArguments.ToArray());

			// Complete type initializer code body
			CompleteInitCacheMethod(cctor.CodeBuilder);

			// Crosses fingers and build type
			Type generatedType = emitter.BuildType();

			InitializeStaticFields(generatedType);
			return generatedType;
		}

		protected Type Init(string typeName, IDictionary<Type, ITypeContributor> typeImplementerMapping, out ClassEmitter emitter, Type proxyTargetType, out FieldReference interceptorsField)
		{
			Type baseType = ProxyGenerationOptions.BaseTypeForInterfaceProxy;

			emitter = BuildClassEmitter(typeName, baseType, typeImplementerMapping.Keys);
			CreateOptionsField(emitter);
			emitter.AddCustomAttributes(ProxyGenerationOptions);
#if SILVERLIGHT
#warning XmlIncludeAttribute is in silverlight, do we want to explore this?
#else
			emitter.DefineCustomAttribute<XmlIncludeAttribute>(new object[] {targetType});
			emitter.DefineCustomAttribute<SerializableAttribute>();
#endif

			// Fields generations
			interceptorsField = CreateInterceptorsField(emitter);

			CreateTargetField(emitter, proxyTargetType);
			CreateSelectorField(emitter);

			return baseType;
		}

		private void CreateTargetField(ClassEmitter emitter, Type proxyTargetType)
		{
			targetField = emitter.CreateField("__target", proxyTargetType);

#if SILVERLIGHT
#warning XmlIncludeAttribute is in silverlight, do we want to explore this?
#else
			emitter.DefineCustomAttributeFor<XmlIgnoreAttribute>(targetField);
#endif
		}

		protected virtual string GeneratorType
		{
			get { return ProxyTypeConstants.InterfaceWithTarget; }
		}

		protected virtual bool AllowChangeTarget
		{
			get { return false; }
		}

		protected IDictionary<Type, ITypeContributor> GetTypeImplementerMapping(Type[] interfaces, Type proxyTargetType, out IEnumerable<ITypeContributor> contributors, INamingScope namingScope)
		{
			IDictionary<Type, ITypeContributor> typeImplementerMapping = new Dictionary<Type, ITypeContributor>();
			var mixins = new MixinContributor(namingScope,AllowChangeTarget);
			// Order of interface precedence:
			// 1. first target
			var targetInterfaces = TypeUtil.GetAllInterfaces(proxyTargetType);
			var additionalInterfaces = TypeUtil.GetAllInterfaces(interfaces);
			var target = AddMappingForTargetType(typeImplementerMapping, proxyTargetType, targetInterfaces, additionalInterfaces,namingScope);


			// 2. then mixins
			if (ProxyGenerationOptions.HasMixins)
			{
				foreach (var mixinInterface in ProxyGenerationOptions.MixinData.MixinInterfaces)
				{
					if (targetInterfaces.Contains(mixinInterface))
					{
						// OK, so the target implements this interface. We now do one of two things:
						if(additionalInterfaces.Contains(mixinInterface))
						{
							// we intercept the interface, and forward calls to the target type
							AddMapping(mixinInterface, target, typeImplementerMapping);
						}
						// we do not intercept the interface
						mixins.AddEmptyInterface(mixinInterface);
					}
					else
					{
						if (!typeImplementerMapping.ContainsKey(mixinInterface))
						{
							mixins.AddInterfaceToProxy(mixinInterface);
							typeImplementerMapping.Add(mixinInterface, mixins);
						}
					}
				}
			}

			var additionalInterfacesContributor = GetContributorForAdditionalInterfaces(namingScope);
			// 3. then additional interfaces
			foreach (var @interface in additionalInterfaces)
			{
				if(typeImplementerMapping.ContainsKey(@interface)) continue;
				if(ProxyGenerationOptions.MixinData.ContainsMixin(@interface)) continue;

				additionalInterfacesContributor.AddInterfaceMapping(@interface);
				SafeAddMapping(@interface, additionalInterfacesContributor, typeImplementerMapping);
			}

			// 4. plus special interfaces
			var instance = new InterfaceProxyInstanceContributor(targetType, GeneratorType, interfaces);
			AddMappingForISerializable(typeImplementerMapping, instance);
			try
			{
				SafeAddMapping(typeof(IProxyTargetAccessor), instance, typeImplementerMapping);
			}
			catch (ArgumentException)
			{
				HandleExplicitlyPassedProxyTargetAccessor(targetInterfaces, additionalInterfaces);
			}

			var list = new List<ITypeContributor>();
			list.Add(target);
			list.Add(additionalInterfacesContributor);
			//foreach (var mixin in mixins)
			//{
				list.Add(mixins);
			//}
			list.Add(instance);

			contributors = list;
			return typeImplementerMapping;
		}

		protected virtual InterfaceProxyWithoutTargetContributor GetContributorForAdditionalInterfaces(INamingScope namingScope)
		{
			return new InterfaceProxyWithoutTargetContributor(namingScope, (c, m) => NullExpression.Instance);
		}

		protected override void SafeAddMapping(Type @interface, ITypeContributor implementer, IDictionary<Type, ITypeContributor> mapping)
		{
			base.SafeAddMapping(@interface, implementer, mapping);
			if(implementer is InterfaceProxyTargetContributor)
			{
				// TODO: REMOVE IT!
				(implementer as InterfaceProxyTargetContributor).AddInterfaceToProxy(@interface);
			}
		}

		protected virtual ITypeContributor AddMappingForTargetType(IDictionary<Type, ITypeContributor> typeImplementerMapping, Type proxyTargetType, ICollection<Type> targetInterfaces, ICollection<Type> additionalInterfaces,INamingScope namingScope)
		{
			var contributor = new InterfaceProxyTargetContributor(proxyTargetType, AllowChangeTarget, namingScope);
			var proxiedInterfaces = TypeUtil.GetAllInterfaces(targetType);
			foreach (var @interface in proxiedInterfaces)
			{
				SafeAddMapping(@interface, contributor, typeImplementerMapping);
			}

			foreach (var @interface in additionalInterfaces)
			{
				if (!ImplementedByTarget(targetInterfaces, @interface)) continue;
				AddMapping(@interface, contributor, typeImplementerMapping);
			}
			return contributor;
		}

		private bool ImplementedByTarget(ICollection<Type> targetInterfaces, Type @interface)
		{
			return targetInterfaces.Contains(@interface);
		}
	}
}
