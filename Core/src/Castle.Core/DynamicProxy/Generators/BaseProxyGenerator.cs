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
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
#if !SILVERLIGHT
	using System.Runtime.Serialization;
	using System.Xml.Serialization;
#endif

	using Castle.Core.Internal;
	using Castle.Core.Logging;
	using Castle.DynamicProxy.Contributors;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Internal;

#if SILVERLIGHT
	using Castle.DynamicProxy.SilverlightExtensions;
#endif

	/// <summary>
	///   Base class that exposes the common functionalities
	///   to proxy generation.
	/// </summary>
	public abstract class BaseProxyGenerator
	{
		protected readonly Type targetType;
		private readonly ModuleScope scope;
		private ILogger logger = NullLogger.Instance;
		private ProxyGenerationOptions proxyGenerationOptions;

		protected BaseProxyGenerator(ModuleScope scope, Type targetType)
		{
			this.scope = scope;
			this.targetType = targetType;
		}

		public ILogger Logger
		{
			get { return logger; }
			set { logger = value; }
		}

		protected ProxyGenerationOptions ProxyGenerationOptions
		{
			get
			{
				if (proxyGenerationOptions == null)
				{
					throw new InvalidOperationException("ProxyGenerationOptions must be set before being retrieved.");
				}
				return proxyGenerationOptions;
			}
			set
			{
				if (proxyGenerationOptions != null)
				{
					throw new InvalidOperationException("ProxyGenerationOptions can only be set once.");
				}
				proxyGenerationOptions = value;
			}
		}

		protected ModuleScope Scope
		{
			get { return scope; }
		}

		protected void AddMapping(Type @interface, ITypeContributor implementer, IDictionary<Type, ITypeContributor> mapping)
		{
			Debug.Assert(implementer != null, "implementer != null");
			Debug.Assert(@interface != null, "@interface != null");
			Debug.Assert(@interface.IsInterface, "@interface.IsInterface");

			if (!mapping.ContainsKey(@interface))
			{
				AddMappingNoCheck(@interface, implementer, mapping);
			}
		}

		protected void AddMappingForISerializable(IDictionary<Type, ITypeContributor> typeImplementerMapping,
		                                          ITypeContributor instance)
		{
#if !SILVERLIGHT
			AddMapping(typeof(ISerializable), instance, typeImplementerMapping);
#endif
		}

		/// <summary>
		///   It is safe to add mapping (no mapping for the interface exists)
		/// </summary>
		/// <param name = "implementer"></param>
		/// <param name = "interface"></param>
		/// <param name = "mapping"></param>
		protected void AddMappingNoCheck(Type @interface, ITypeContributor implementer,
		                                 IDictionary<Type, ITypeContributor> mapping)
		{
			mapping.Add(@interface, implementer);
		}

		protected void AddToCache(CacheKey key, Type type)
		{
			scope.RegisterInCache(key, type);
		}

		protected virtual ClassEmitter BuildClassEmitter(string typeName, Type parentType, IEnumerable<Type> interfaces)
		{
			CheckNotGenericTypeDefinition(parentType, "parentType");
			CheckNotGenericTypeDefinitions(interfaces, "interfaces");

			return new ClassEmitter(Scope, typeName, parentType, interfaces);
		}

		protected void CheckNotGenericTypeDefinition(Type type, string argumentName)
		{
			if (type != null && type.IsGenericTypeDefinition)
			{
				throw new ArgumentException("Type cannot be a generic type definition. Type: " + type.FullName, argumentName);
			}
		}

		protected void CheckNotGenericTypeDefinitions(IEnumerable<Type> types, string argumentName)
		{
			if (types == null)
			{
				return;
			}
			foreach (var t in types)
			{
				CheckNotGenericTypeDefinition(t, argumentName);
			}
		}

		protected void CompleteInitCacheMethod(ConstructorCodeBuilder constCodeBuilder)
		{
			constCodeBuilder.AddStatement(new ReturnStatement());
		}

		protected virtual void CreateFields(ClassEmitter emitter)
		{
			CreateOptionsField(emitter);
			CreateSelectorField(emitter);
			CreateInterceptorsField(emitter);
		}

		protected void CreateInterceptorsField(ClassEmitter emitter)
		{
			var interceptorsField = emitter.CreateField("__interceptors", typeof(IInterceptor[]));

#if !SILVERLIGHT
			emitter.DefineCustomAttributeFor<XmlIgnoreAttribute>(interceptorsField);
#endif
		}

		protected FieldReference CreateOptionsField(ClassEmitter emitter)
		{
			return emitter.CreateStaticField("proxyGenerationOptions", typeof(ProxyGenerationOptions));
		}

		protected void CreateSelectorField(ClassEmitter emitter)
		{
			if (ProxyGenerationOptions.Selector == null)
			{
				return;
			}

			emitter.CreateField("__selector", typeof(IInterceptorSelector));
			return;
		}

		protected virtual void CreateTypeAttributes(ClassEmitter emitter)
		{
			emitter.AddCustomAttributes(ProxyGenerationOptions);
#if !SILVERLIGHT
			emitter.DefineCustomAttribute<XmlIncludeAttribute>(new object[] { targetType });
#endif
		}

		protected void EnsureOptionsOverrideEqualsAndGetHashCode(ProxyGenerationOptions options)
		{
			if (Logger.IsWarnEnabled)
			{
				// Check the proxy generation hook
				if (!OverridesEqualsAndGetHashCode(options.Hook.GetType()))
				{
					Logger.WarnFormat("The IProxyGenerationHook type {0} does not override both Equals and GetHashCode. " +
					                  "If these are not correctly overridden caching will fail to work causing performance problems.",
					                  options.Hook.GetType().FullName);
				}

				// Interceptor selectors no longer need to override Equals and GetHashCode
			}
		}

		protected void GenerateConstructor(ClassEmitter emitter, ConstructorInfo baseConstructor,
		                                   params FieldReference[] fields)
		{
			ArgumentReference[] args;
			ParameterInfo[] baseConstructorParams = null;

			if (baseConstructor != null)
			{
				baseConstructorParams = baseConstructor.GetParameters();
			}

			if (baseConstructorParams != null && baseConstructorParams.Length != 0)
			{
				args = new ArgumentReference[fields.Length + baseConstructorParams.Length];

				var offset = fields.Length;
				for (var i = offset; i < offset + baseConstructorParams.Length; i++)
				{
					var paramInfo = baseConstructorParams[i - offset];
					args[i] = new ArgumentReference(paramInfo.ParameterType);
				}
			}
			else
			{
				args = new ArgumentReference[fields.Length];
			}

			for (var i = 0; i < fields.Length; i++)
			{
				args[i] = new ArgumentReference(fields[i].Reference.FieldType);
			}

			var constructor = emitter.CreateConstructor(args);
			if (baseConstructorParams != null && baseConstructorParams.Length != 0)
			{
				var last = baseConstructorParams.Last();
				if (last.ParameterType.IsArray && last.HasAttribute<ParamArrayAttribute>())
				{
					var parameter = constructor.ConstructorBuilder.DefineParameter(args.Length, ParameterAttributes.None, last.Name);
					var builder = AttributeUtil.CreateBuilder<ParamArrayAttribute>();
					parameter.SetCustomAttribute(builder);
				}
			}

			for (var i = 0; i < fields.Length; i++)
			{
				constructor.CodeBuilder.AddStatement(new AssignStatement(fields[i], args[i].ToExpression()));
			}

			// Invoke base constructor

			if (baseConstructor != null)
			{
				Debug.Assert(baseConstructorParams != null);

				var slice = new ArgumentReference[baseConstructorParams.Length];
				Array.Copy(args, fields.Length, slice, 0, baseConstructorParams.Length);

				constructor.CodeBuilder.InvokeBaseConstructor(baseConstructor, slice);
			}
			else
			{
				constructor.CodeBuilder.InvokeBaseConstructor();
			}

			constructor.CodeBuilder.AddStatement(new ReturnStatement());
		}

		protected void GenerateConstructors(ClassEmitter emitter, Type baseType, params FieldReference[] fields)
		{
			var constructors =
				baseType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			foreach (var constructor in constructors)
			{
				if (!IsConstructorVisible(constructor))
				{
					continue;
				}

				GenerateConstructor(emitter, constructor, fields);
			}
		}

		/// <summary>
		///   Generates a parameters constructor that initializes the proxy
		///   state with <see cref = "StandardInterceptor" /> just to make it non-null.
		///   <para>
		///     This constructor is important to allow proxies to be XML serializable
		///   </para>
		/// </summary>
		protected void GenerateParameterlessConstructor(ClassEmitter emitter, Type baseClass, FieldReference interceptorField)
		{
			// Check if the type actually has a default constructor
			var defaultConstructor = baseClass.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes,
			                                                  null);

			if (defaultConstructor == null)
			{
				defaultConstructor = baseClass.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes,
				                                              null);

				if (defaultConstructor == null || defaultConstructor.IsPrivate)
				{
					return;
				}
			}

			var constructor = emitter.CreateConstructor();

			// initialize fields with an empty interceptor

			constructor.CodeBuilder.AddStatement(new AssignStatement(interceptorField,
			                                                         new NewArrayExpression(1, typeof(IInterceptor))));
			constructor.CodeBuilder.AddStatement(
				new AssignArrayStatement(interceptorField, 0, new NewInstanceExpression(typeof(StandardInterceptor), new Type[0])));

			// Invoke base constructor

			constructor.CodeBuilder.InvokeBaseConstructor(defaultConstructor);

			constructor.CodeBuilder.AddStatement(new ReturnStatement());
		}

		protected ConstructorEmitter GenerateStaticConstructor(ClassEmitter emitter)
		{
			return emitter.CreateTypeConstructor();
		}

		protected Type GetFromCache(CacheKey key)
		{
			return scope.GetFromCache(key);
		}

		protected void HandleExplicitlyPassedProxyTargetAccessor(ICollection<Type> targetInterfaces,
		                                                         ICollection<Type> additionalInterfaces)
		{
			var interfaceName = typeof(IProxyTargetAccessor).ToString();
			//ok, let's determine who tried to sneak the IProxyTargetAccessor in...
			string message;
			if (targetInterfaces.Contains(typeof(IProxyTargetAccessor)))
			{
				message =
					string.Format(
						"Target type for the proxy implements {0} which is a DynamicProxy infrastructure interface and you should never implement it yourself. Are you trying to proxy an existing proxy?",
						interfaceName);
			}
			else if (ProxyGenerationOptions.MixinData.ContainsMixin(typeof(IProxyTargetAccessor)))
			{
				var mixinType = ProxyGenerationOptions.MixinData.GetMixinInstance(typeof(IProxyTargetAccessor)).GetType();
				message =
					string.Format(
						"Mixin type {0} implements {1} which is a DynamicProxy infrastructure interface and you should never implement it yourself. Are you trying to mix in an existing proxy?",
						mixinType.Name, interfaceName);
			}
			else if (additionalInterfaces.Contains(typeof(IProxyTargetAccessor)))
			{
				message =
					string.Format(
						"You passed {0} as one of additional interfaces to proxy which is a DynamicProxy infrastructure interface and is implemented by every proxy anyway. Please remove it from the list of additional interfaces to proxy.",
						interfaceName);
			}
			else
			{
				// this can technicaly never happen
				message = string.Format("It looks like we have a bug with regards to how we handle {0}. Please report it.",
				                        interfaceName);
			}
			throw new ProxyGenerationException("This is a DynamicProxy2 error: " + message);
		}

		protected void InitializeStaticFields(Type builtType)
		{
			builtType.SetStaticField("proxyGenerationOptions", BindingFlags.Public, ProxyGenerationOptions);
		}

		protected Type ObtainProxyType(CacheKey cacheKey, Func<string, INamingScope, Type> factory)
		{
			using (var locker = Scope.Lock.ForReadingUpgradeable())
			{
				var cacheType = GetFromCache(cacheKey);
				if (cacheType != null)
				{
					Logger.DebugFormat("Found cached proxy type {0} for target type {1}.", cacheType.FullName, targetType.FullName);
					return cacheType;
				}

				// Upgrade the lock to a write lock, then read again. This is to avoid generating duplicate types
				// under heavy multithreaded load.
				locker.Upgrade();

				cacheType = GetFromCache(cacheKey);
				if (cacheType != null)
				{
					Logger.DebugFormat("Found cached proxy type {0} for target type {1}.", cacheType.FullName, targetType.FullName);
					return cacheType;
				}

				// Log details about the cache miss
				Logger.DebugFormat("No cached proxy type was found for target type {0}.", targetType.FullName);
				EnsureOptionsOverrideEqualsAndGetHashCode(ProxyGenerationOptions);

				var name = Scope.NamingScope.GetUniqueName("Castle.Proxies." + targetType.Name + "Proxy");
				var proxyType = factory.Invoke(name, Scope.NamingScope.SafeSubScope());

				AddToCache(cacheKey, proxyType);
				return proxyType;
			}
		}

		private bool IsConstructorVisible(ConstructorInfo constructor)
		{
			return constructor.IsPublic
			       || constructor.IsFamily
			       || constructor.IsFamilyOrAssembly
#if !Silverlight
			       || (constructor.IsAssembly && InternalsUtil.IsInternalToDynamicProxy(constructor.DeclaringType.Assembly));
#else
            ;
#endif
		}

		private bool OverridesEqualsAndGetHashCode(Type type)
		{
			var equalsMethod = type.GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance);
			if (equalsMethod == null || equalsMethod.DeclaringType == typeof(object) || equalsMethod.IsAbstract)
			{
				return false;
			}

			var getHashCodeMethod = type.GetMethod("GetHashCode", BindingFlags.Public | BindingFlags.Instance);
			if (getHashCodeMethod == null || getHashCodeMethod.DeclaringType == typeof(object) || getHashCodeMethod.IsAbstract)
			{
				return false;
			}

			return true;
		}
	}
}