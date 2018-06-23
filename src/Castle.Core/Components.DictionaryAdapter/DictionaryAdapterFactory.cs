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

namespace Castle.Components.DictionaryAdapter
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Threading;
	using System.Diagnostics;

#if FEATURE_DICTIONARYADAPTER_XML
	using Castle.Components.DictionaryAdapter.Xml;
#endif
	using Castle.Core.Internal;

	/// <summary>
	/// Uses Reflection.Emit to expose the properties of a dictionary
	/// through a dynamic implementation of a typed interface.
	/// </summary>
	public class DictionaryAdapterFactory : IDictionaryAdapterFactory
	{
		private readonly SynchronizedDictionary<Type, DictionaryAdapterMeta> interfaceToMeta =
			new SynchronizedDictionary<Type, DictionaryAdapterMeta>();

		#region IDictionaryAdapterFactory

		/// <inheritdoc />
		public T GetAdapter<T>(IDictionary dictionary)
		{
			return (T) GetAdapter(typeof(T), dictionary);
		}

		/// <inheritdoc />
		public object GetAdapter(Type type, IDictionary dictionary)
		{
			return InternalGetAdapter(type, dictionary, null);
		}

        /// <inheritdoc />
		public object GetAdapter(Type type, IDictionary dictionary, PropertyDescriptor descriptor)
		{
			return InternalGetAdapter(type, dictionary, descriptor);
		}

		/// <inheritdoc />
		public T GetAdapter<T, R>(IDictionary<string, R> dictionary)
		{
			return (T) GetAdapter<R>(typeof(T), dictionary);
		}

		/// <inheritdoc />
		public object GetAdapter<R>(Type type, IDictionary<string, R> dictionary)
		{
			var adapter = new GenericDictionaryAdapter<R>(dictionary);
			return InternalGetAdapter(type, adapter, null);
		}

		/// <inheritdoc />
		public T GetAdapter<T>(NameValueCollection nameValues)
		{
			return GetAdapter<T>(new NameValueCollectionAdapter(nameValues));
		}

		/// <inheritdoc />
		public object GetAdapter(Type type, NameValueCollection nameValues)
		{
			return GetAdapter(type, new NameValueCollectionAdapter(nameValues));
		}

#if FEATURE_DICTIONARYADAPTER_XML
		/// <inheritdoc />
		public T GetAdapter<T>(System.Xml.XmlNode xmlNode)
		{
		    return (T)GetAdapter(typeof(T), xmlNode);
		}

		/// <inheritdoc />
		public object GetAdapter(Type type, System.Xml.XmlNode xmlNode)
		{
		    var xml = new XmlAdapter(xmlNode);
			return GetAdapter(type, new Hashtable(), new PropertyDescriptor()
				.AddBehavior(XmlMetadataBehavior.Default)
				.AddBehavior(xml));
		}
#endif

		/// <inheritdoc />
		public DictionaryAdapterMeta GetAdapterMeta(Type type)
		{
			return GetAdapterMeta(type, null as PropertyDescriptor);
		}

		/// <inheritdoc />
		public DictionaryAdapterMeta GetAdapterMeta(Type type, PropertyDescriptor descriptor)
		{
			return InternalGetAdapterMeta(type, descriptor, null);
		}

		/// <inheritdoc />
		public DictionaryAdapterMeta GetAdapterMeta(Type type, DictionaryAdapterMeta other)
		{
			return InternalGetAdapterMeta(type, null, other);
		}

		#endregion

		private DictionaryAdapterMeta InternalGetAdapterMeta(Type type,
			PropertyDescriptor descriptor, DictionaryAdapterMeta other)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.GetTypeInfo().IsInterface == false)
				throw new ArgumentException("Only interfaces can be adapted to a dictionary", "type");

			return interfaceToMeta.GetOrAdd(type, t =>
			{
				if (descriptor == null && other != null)
				{
					descriptor = other.CreateDescriptor();
				}

#if FEATURE_LEGACY_REFLECTION_API
				var appDomain = Thread.GetDomain();
				var typeBuilder = CreateTypeBuilder(type, appDomain);
#else
				var typeBuilder = CreateTypeBuilder(type);
#endif
				return CreateAdapterMeta(type, typeBuilder, descriptor);
			});
		}

		private object InternalGetAdapter(Type type, IDictionary dictionary, PropertyDescriptor descriptor)
		{
			var meta = InternalGetAdapterMeta(type, descriptor, null);
			return meta.CreateInstance(dictionary, descriptor);
		}

		#region Type Builders

#if FEATURE_LEGACY_REFLECTION_API
		private static TypeBuilder CreateTypeBuilder(Type type, AppDomain appDomain)
		{
			var assemblyName = new AssemblyName("CastleDictionaryAdapterAssembly");
			var assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule("CastleDictionaryAdapterModule");
			return CreateAdapterType(type, moduleBuilder);
		}
#else
		private static TypeBuilder CreateTypeBuilder(Type type)
		{
			var assemblyName = new AssemblyName("CastleDictionaryAdapterAssembly");
			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule("CastleDictionaryAdapterModule");
			return CreateAdapterType(type, moduleBuilder);
		}

#endif

		private static TypeBuilder CreateAdapterType(Type type, ModuleBuilder moduleBuilder)
		{
			var typeBuilder = moduleBuilder.DefineType("CastleDictionaryAdapterType",
				TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit);
			typeBuilder.AddInterfaceImplementation(type);
			typeBuilder.SetParent(typeof(DictionaryAdapterBase));

			var attribCtorParams = new[] { typeof(Type) };
			var attribCtorInfo = typeof(DictionaryAdapterAttribute).GetConstructor(attribCtorParams);
			var attribBuilder = new CustomAttributeBuilder(attribCtorInfo, new[] { type });
			typeBuilder.SetCustomAttribute(attribBuilder);

			var debugAttribCtorParams = new[] { typeof(string) };
			var debugAttribCtorInfo = typeof(DebuggerDisplayAttribute).GetConstructor(debugAttribCtorParams);
			var debugAttribBuilder = new CustomAttributeBuilder(debugAttribCtorInfo, new[] { "Type: {Meta.Type.FullName,nq}" });
			typeBuilder.SetCustomAttribute(debugAttribBuilder);
			return typeBuilder;
		}

		private DictionaryAdapterMeta CreateAdapterMeta(Type type, TypeBuilder typeBuilder, PropertyDescriptor descriptor)
		{
			var binding = FieldAttributes.Public | FieldAttributes.Static;
			var metaField = typeBuilder.DefineField("__meta", typeof(DictionaryAdapterMeta), binding);
			var constructor = CreateAdapterConstructor(typeBuilder);
			CreateAdapterFactoryMethod(typeBuilder, constructor);

			object[] typeBehaviors;
			var initializers = new PropertyDescriptor();
			var propertyMap = GetPropertyDescriptors(type, initializers, out typeBehaviors);

			if (descriptor != null)
			{
#if DOTNET40
				initializers.AddBehaviors(descriptor.MetaInitializers);
#else
				initializers.AddBehaviors(descriptor.MetaInitializers.Cast<IDictionaryBehavior>());
#endif
				typeBehaviors = typeBehaviors.Union(descriptor.Annotations).ToArray();
			}

			CreateMetaProperty(typeBuilder, AdapterGetMeta, metaField);

			foreach (var property in propertyMap)
			{
				CreateAdapterProperty(typeBuilder, property.Value);
			}

#if FEATURE_LEGACY_REFLECTION_API
			var implementation = typeBuilder.CreateType();
			var creator = (Func<DictionaryAdapterInstance, IDictionaryAdapter>)Delegate.CreateDelegate
			(
				typeof(Func<DictionaryAdapterInstance, IDictionaryAdapter>),
				implementation,
				"__Create"
			);
#else
			var implementation = typeBuilder.CreateTypeInfo().AsType();
			var creator = (Func<DictionaryAdapterInstance, IDictionaryAdapter>)implementation
				.GetTypeInfo().GetDeclaredMethod("__Create")
				.CreateDelegate(typeof(Func<DictionaryAdapterInstance, IDictionaryAdapter>));
#endif

			var meta = new DictionaryAdapterMeta(type, implementation, typeBehaviors,
				initializers.MetaInitializers.ToArray(), initializers.Initializers.ToArray(),
				propertyMap, this, creator);

#if FEATURE_LEGACY_REFLECTION_API
			const BindingFlags metaBindings = BindingFlags.Public | BindingFlags.Static | BindingFlags.SetField;
			implementation.InvokeMember("__meta", metaBindings, null, null, new[] { meta });
#else
			const BindingFlags metaBindings = BindingFlags.Public | BindingFlags.Static;
			var field = implementation.GetField("__meta", metaBindings);
			field.SetValue(implementation, meta);
#endif
			return meta;
		}

		private static readonly PropertyInfo AdapterGetMeta = typeof(IDictionaryAdapter).GetProperty("Meta");

		#endregion

		#region Constructors

		private static ConstructorInfo CreateAdapterConstructor(TypeBuilder typeBuilder)
		{
			var constructorBuilder = typeBuilder.DefineConstructor(
				MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard,
				ConstructorParameterTypes
				);

			var ilGenerator = constructorBuilder.GetILGenerator();

			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldarg_1);
			ilGenerator.Emit(OpCodes.Call, BaseCtor);
			ilGenerator.Emit(OpCodes.Ret);

			return constructorBuilder;
		}

		private static void CreateAdapterFactoryMethod(TypeBuilder typeBuilder, ConstructorInfo constructor)
		{
			var factoryBuilder = typeBuilder.DefineMethod
			(
				"__Create",
				MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
				typeof(IDictionaryAdapter),
				ConstructorParameterTypes
			);

			var ilGenerator = factoryBuilder.GetILGenerator();
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Newobj, constructor);
			ilGenerator.Emit(OpCodes.Ret);
 		}

		private static readonly ConstructorInfo BaseCtor = typeof(DictionaryAdapterBase).GetConstructors()[0];
		private static readonly Type[] ConstructorParameterTypes = { typeof(DictionaryAdapterInstance) };

		#endregion

		#region Properties

		private static void CreateMetaProperty(TypeBuilder typeBuilder, PropertyInfo prop, FieldInfo field)
		{
			const MethodAttributes propAttribs = MethodAttributes.Public | MethodAttributes.SpecialName |
												 MethodAttributes.HideBySig | MethodAttributes.ReuseSlot |
												 MethodAttributes.Virtual   | MethodAttributes.Final;

			var getMethodBuilder = typeBuilder.DefineMethod("get_" + prop.Name, propAttribs, prop.PropertyType, null);

			var getILGenerator = getMethodBuilder.GetILGenerator();
			if (field.IsStatic)
			{
				getILGenerator.Emit(OpCodes.Ldsfld, field);
			}
			else
			{
				getILGenerator.Emit(OpCodes.Ldarg_0);
				getILGenerator.Emit(OpCodes.Ldfld, field);
			}
			getILGenerator.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(getMethodBuilder, prop.GetGetMethod());
		}

		private static void CreateAdapterProperty(TypeBuilder typeBuilder, PropertyDescriptor descriptor)
		{
			var property = descriptor.Property;
			var propertyBuilder = typeBuilder.DefineProperty(property.Name, property.Attributes, property.PropertyType, null);
			const MethodAttributes propAttribs = MethodAttributes.Public | MethodAttributes.SpecialName |
												 MethodAttributes.HideBySig | MethodAttributes.Virtual;

			if (property.CanRead)
			{
				CreatePropertyGetMethod(typeBuilder, propertyBuilder, descriptor, propAttribs);
			}

			if (property.CanWrite)
			{
				CreatePropertySetMethod(typeBuilder, propertyBuilder, descriptor, propAttribs);
			}
		}

		private static void PreparePropertyMethod(PropertyDescriptor descriptor, ILGenerator propILGenerator)
		{
			propILGenerator.DeclareLocal(typeof(String));
			propILGenerator.DeclareLocal(typeof(object));

			// key = propertyInfo.Name
			propILGenerator.Emit(OpCodes.Ldstr, descriptor.PropertyName);
			propILGenerator.Emit(OpCodes.Stloc_0);
		}

		#endregion

		#region Getters

		private static void CreatePropertyGetMethod(TypeBuilder typeBuilder, PropertyBuilder propertyBuilder, 
			PropertyDescriptor descriptor, MethodAttributes propAttribs)
		{
			var getMethodBuilder = typeBuilder.DefineMethod("get_" + descriptor.PropertyName, propAttribs, descriptor.PropertyType, null);
			var getILGenerator = getMethodBuilder.GetILGenerator();
			var returnDefault = getILGenerator.DefineLabel();
			var storeResult = getILGenerator.DefineLabel();
			var loadResult = getILGenerator.DefineLabel();

			PreparePropertyMethod(descriptor, getILGenerator);

			var result = getILGenerator.DeclareLocal(descriptor.PropertyType);

			// value = GetProperty(key, false)
			getILGenerator.Emit(OpCodes.Ldarg_0);
			getILGenerator.Emit(OpCodes.Ldloc_0);
			getILGenerator.Emit(OpCodes.Ldc_I4_0);
			getILGenerator.Emit(OpCodes.Callvirt, AdapterGetProperty);
			getILGenerator.Emit(OpCodes.Stloc_1);

			// if (value == null) return null
			getILGenerator.Emit(OpCodes.Ldloc_1);
			getILGenerator.Emit(OpCodes.Brfalse_S, returnDefault);

			// return (propertyInfo.PropertyType) value
			getILGenerator.Emit(OpCodes.Ldloc_1);
			getILGenerator.Emit(OpCodes.Unbox_Any, descriptor.PropertyType);
			getILGenerator.Emit(OpCodes.Br_S, storeResult);

			getILGenerator.MarkLabel(returnDefault);
			getILGenerator.Emit(OpCodes.Ldloca_S, result);
			getILGenerator.Emit(OpCodes.Initobj, descriptor.PropertyType);
			getILGenerator.Emit(OpCodes.Br_S, loadResult);

			getILGenerator.MarkLabel(storeResult);
			getILGenerator.Emit(OpCodes.Stloc_S, result);

			getILGenerator.MarkLabel(loadResult);
			getILGenerator.Emit(OpCodes.Ldloc_S, result);
			getILGenerator.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(getMethodBuilder);
		}

		private static readonly MethodInfo AdapterGetProperty = typeof(IDictionaryAdapter).GetMethod("GetProperty");

		#endregion

		#region Setters

		private static void CreatePropertySetMethod(TypeBuilder typeBuilder, PropertyBuilder propertyBuilder,
			PropertyDescriptor descriptor, MethodAttributes propAttribs)
		{
			var setMethodBuilder = typeBuilder.DefineMethod("set_" + descriptor.PropertyName, propAttribs, null, new[] {descriptor.PropertyType});
			var setILGenerator = setMethodBuilder.GetILGenerator();
			PreparePropertyMethod(descriptor, setILGenerator);

			setILGenerator.Emit(OpCodes.Ldarg_1);
			if (descriptor.PropertyType.GetTypeInfo().IsValueType)
			{
				setILGenerator.Emit(OpCodes.Box, descriptor.PropertyType);
			}
			setILGenerator.Emit(OpCodes.Stloc_1);

			// ignore = SetProperty(key, ref value)
			setILGenerator.Emit(OpCodes.Ldarg_0);
			setILGenerator.Emit(OpCodes.Ldloc_0);
			setILGenerator.Emit(OpCodes.Ldloca_S, 1);
			setILGenerator.Emit(OpCodes.Callvirt, AdapterSetProperty);
			setILGenerator.Emit(OpCodes.Pop);
			setILGenerator.Emit(OpCodes.Ret);

			propertyBuilder.SetSetMethod(setMethodBuilder);
		}

		private static readonly MethodInfo AdapterSetProperty = typeof(IDictionaryAdapter).GetMethod("SetProperty");

		#endregion

		#region Descriptors

		private static Dictionary<String, PropertyDescriptor> GetPropertyDescriptors(Type type, PropertyDescriptor initializers, out object[] typeBehaviors)
		{
			var propertyMap = new Dictionary<String, PropertyDescriptor>();
			var interfaceBehaviors = typeBehaviors = ExpandBehaviors(InterfaceAttributeUtil.GetAttributes(type, true)).ToArray();
			var defaultFetch = typeBehaviors.OfType<FetchAttribute>().Select(b => (bool?)b.Fetch).FirstOrDefault().GetValueOrDefault();

#if DOTNET40
			initializers.AddBehaviors(typeBehaviors.OfType<IDictionaryMetaInitializer>())
						.AddBehaviors(typeBehaviors.OfType<IDictionaryInitializer>());
#else
			initializers.AddBehaviors(typeBehaviors.OfType<IDictionaryMetaInitializer>().Cast<IDictionaryBehavior>())
						.AddBehaviors(typeBehaviors.OfType<IDictionaryInitializer>    ().Cast<IDictionaryBehavior>());
#endif

			CollectProperties(type, (property, reflectedType) =>
			{
				var propertyBehaviors = ExpandBehaviors(property.GetCustomAttributes(false)).ToArray();
				var propertyDescriptor = new PropertyDescriptor(property, propertyBehaviors)
					.AddBehaviors(propertyBehaviors.OfType<IDictionaryBehavior>())
					.AddBehaviors(interfaceBehaviors.OfType<IDictionaryBehavior>().Where(b => b is IDictionaryKeyBuilder == false));
				var expandedBehaviors = ExpandBehaviors(InterfaceAttributeUtil
					.GetAttributes(reflectedType, true))
#if DOTNET40
					.OfType<IDictionaryKeyBuilder>();
#else
					.OfType<IDictionaryKeyBuilder>()
					.Cast<IDictionaryBehavior>();
#endif
				propertyDescriptor = propertyDescriptor.AddBehaviors(expandedBehaviors);

				AddDefaultGetter(propertyDescriptor);

				var propertyFetch = propertyBehaviors.OfType<FetchAttribute>().Select(b => (bool?)b.Fetch).FirstOrDefault();
				propertyDescriptor.IfExists = propertyBehaviors.OfType<IfExistsAttribute>().Any();
				propertyDescriptor.Fetch = propertyFetch.GetValueOrDefault(defaultFetch);

				foreach (var descriptorInitializer in propertyDescriptor.Behaviors.OfType<IPropertyDescriptorInitializer>())
				{
					descriptorInitializer.Initialize(propertyDescriptor, propertyBehaviors);
				}

#if DOTNET40
				initializers.AddBehaviors(propertyBehaviors.OfType<IDictionaryMetaInitializer>());
#else
				initializers.AddBehaviors(propertyBehaviors.OfType<IDictionaryMetaInitializer>().Cast<IDictionaryBehavior>());
#endif

				PropertyDescriptor existingDescriptor;
				if (propertyMap.TryGetValue(property.Name, out existingDescriptor))
				{
					var existingProperty = existingDescriptor.Property;
					if (existingProperty.PropertyType == property.PropertyType)
					{
						if (property.CanRead && property.CanWrite)
						{
							propertyMap[property.Name] = propertyDescriptor;
						}
						return;
					}
				}

				propertyMap.Add(property.Name, propertyDescriptor);
			});

			return propertyMap;
		}

		private static IEnumerable<object> ExpandBehaviors(IEnumerable<object> behaviors)
		{
			foreach (var behavior in behaviors)
			{
				if (behavior is IDictionaryBehaviorBuilder)
				{
					foreach (var build in ((IDictionaryBehaviorBuilder)behavior).BuildBehaviors())
						yield return build;
				}
				else
				{
					yield return behavior;
				}
			}
		}

		private static void CollectProperties(Type currentType, Action<PropertyInfo, Type> onProperty)
		{
			var types = new List<Type>();
			types.Add(currentType);
			types.AddRange(currentType.GetInterfaces());
			const BindingFlags publicBindings = BindingFlags.Public | BindingFlags.Instance;

			foreach (var reflectedType in types.Where(t => InfrastructureTypes.Contains(t) == false))
			foreach (var property in reflectedType.GetProperties(publicBindings))
			{
				onProperty(property, reflectedType);
			}
		}

		private static void AddDefaultGetter(PropertyDescriptor descriptor)
		{
			if (descriptor.TypeConverter != null)
				descriptor.AddBehavior(new DefaultPropertyGetter(descriptor.TypeConverter));
		}

		private static readonly HashSet<Type> InfrastructureTypes =	new HashSet<Type>
			{
				typeof (IEditableObject), typeof (IDictionaryEdit), typeof (IChangeTracking),
				typeof (IRevertibleChangeTracking), typeof (IDictionaryNotify),
#if FEATURE_IDATAERRORINFO
				typeof (IDataErrorInfo),
#endif
				typeof (IDictionaryValidate), typeof (IDictionaryAdapter)
			};

		#endregion
	}
}