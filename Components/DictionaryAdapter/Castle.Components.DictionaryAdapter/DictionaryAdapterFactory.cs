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

namespace Castle.Components.DictionaryAdapter
{
	using System;
	using System.Text;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Threading;

	/// <summary>
	/// Uses Reflection.Emit to expose the properties of a dictionary
	/// through a dynamic implementation of a typed interface.
	/// </summary>
	public class DictionaryAdapterFactory : IDictionaryAdapterFactory
	{
		private readonly IDictionary<Assembly, string> assembliesNames;

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DictionaryAdapterFactory"/> class.
		/// </summary>
		public DictionaryAdapterFactory()
		{
			assembliesNames = new Dictionary<Assembly, string>();
		}

		#endregion

		#region IDictionaryAdapterFactory

		/// <summary>
		/// Gets a typed adapter bound to the <see cref="IDictionary"/>.
		/// </summary>
		/// <typeparam name="T">The typed interface.</typeparam>
		/// <param name="dictionary">The underlying source of properties.</param>
		/// <returns>An implementation of the typed interface bound to the dictionary.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		public T GetAdapter<T>(IDictionary dictionary)
		{
			return (T) GetAdapter(typeof(T), dictionary);
		}

		/// <summary>
		/// Gets a typed adapter bound to the <see cref="IDictionary"/>.
		/// </summary>
		/// <param name="type">The typed interface.</param>
		/// <param name="dictionary">The underlying source of properties.</param>
		/// <returns>An implementation of the typed interface bound to the dictionary.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		public object GetAdapter(Type type, IDictionary dictionary)
		{
			return InternalGetAdapter(type, dictionary, null);
		}

		/// <summary>
		/// Gets a typed adapter bound to the <see cref="IDictionary"/>.
		/// </summary>
		/// <param name="type">The typed interface.</param>
		/// <param name="dictionary">The underlying source of properties.</param>
		/// <param name="descriptor">The property descriptor.</param>
		/// <returns>An implementation of the typed interface bound to the dictionary.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		public object GetAdapter(Type type, IDictionary dictionary,
		                         PropertyDescriptor descriptor)
		{
			return InternalGetAdapter(type, dictionary, descriptor);
		}

		/// <summary>
		/// Gets a typed adapter bound to the <see cref="NameValueCollection"/>.
		/// </summary>
		/// <typeparam name="T">The typed interface.</typeparam>
		/// <param name="nameValues">The underlying source of properties.</param>
		/// <returns>An implementation of the typed interface bound to the namedValues.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		public T GetAdapter<T>(NameValueCollection nameValues)
		{
			return GetAdapter<T>(new NameValueCollectionAdapter(nameValues));
		}

		/// <summary>
		/// Gets a typed adapter bound to the <see cref="NameValueCollection"/>.
		/// </summary>
		/// <param name="type">The typed interface.</param>
		/// <param name="nameValues">The underlying source of properties.</param>
		/// <returns>An implementation of the typed interface bound to the namedValues.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		public object GetAdapter(Type type, NameValueCollection nameValues)
		{
			return GetAdapter(type, new NameValueCollectionAdapter(nameValues));
		}

		#endregion

		#region Dynamic Type Generation

		private object InternalGetAdapter(Type type, IDictionary dictionary,
		                                  PropertyDescriptor descriptor)
		{
			if (!type.IsInterface)
			{
				throw new ArgumentException("Only interfaces can be adapted");
			}

			AppDomain appDomain = Thread.GetDomain();
			String adapterAssemblyName = GetAdapterAssemblyName(type);
			Assembly adapterAssembly = GetExistingAdapterAssembly(appDomain, adapterAssemblyName);

			if (adapterAssembly == null)
			{
				TypeBuilder typeBuilder = CreateTypeBuilder(type, appDomain, adapterAssemblyName);
				adapterAssembly = CreateAdapterAssembly(type, typeBuilder);
			}

			return GetExistingAdapter(type, adapterAssembly, dictionary, descriptor);
		}

		private static TypeBuilder CreateTypeBuilder(Type type, AppDomain appDomain,
		                                             String adapterAssemblyName)
		{
			AssemblyName assemblyName = new AssemblyName(adapterAssemblyName);
			AssemblyBuilder assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(adapterAssemblyName);

			return CreateAdapterType(type, moduleBuilder);
		}

		private static TypeBuilder CreateAdapterType(Type type, ModuleBuilder moduleBuilder)
		{
			TypeBuilder typeBuilder =
				moduleBuilder.DefineType(GetAdapterFullTypeName(type),
				                         TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit);
			typeBuilder.AddInterfaceImplementation(type);
			typeBuilder.AddInterfaceImplementation(typeof(IDictionaryAdapter));

			return typeBuilder;
		}

		private static Assembly CreateAdapterAssembly(Type type, TypeBuilder typeBuilder)
		{
			FieldBuilder factoryField = typeBuilder.DefineField(
				"factory", typeof(DictionaryAdapterFactory), FieldAttributes.Private);
			FieldBuilder dictionaryField = typeBuilder.DefineField(
				"dictionary", typeof(IDictionary), FieldAttributes.Private);
			FieldBuilder descriptorField = typeBuilder.DefineField(
				"descriptor", typeof(PropertyDescriptor), FieldAttributes.Private);
			FieldBuilder propertyMapField = typeBuilder.DefineField(
				"propertyMap", typeof(Dictionary<String, PropertyDescriptor>),
				FieldAttributes.Private | FieldAttributes.Static);

			CreateAdapterConstructor(typeBuilder, factoryField, dictionaryField,
			                         descriptorField);

			Dictionary<String, PropertyDescriptor> propertyMap = GetPropertyDescriptors(type);

			CreateDictionaryAdapterMeta(typeBuilder, dictionaryField, propertyMapField, factoryField, propertyMap);

			foreach(KeyValuePair<String, PropertyDescriptor> descriptor in propertyMap)
			{
				CreateAdapterProperty(typeBuilder, factoryField, dictionaryField,
				                      descriptorField, propertyMapField, descriptor.Value);
			}

			Type adapterType = typeBuilder.CreateType();
			adapterType.InvokeMember("propertyMap",
			                         BindingFlags.NonPublic | BindingFlags.Static |
			                         BindingFlags.SetField,
			                         null, null, new object[] {propertyMap});

			return typeBuilder.Assembly;
		}

		#endregion

		#region CreateAdapterConstructor

		private static void CreateAdapterConstructor(
			TypeBuilder typeBuilder, FieldInfo factoryField,
			FieldInfo dictionaryField, FieldInfo descriptorField)
		{
			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
				MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard,
				new Type[]
					{
						typeof(DictionaryAdapterFactory), typeof(IDictionary),
						typeof(IDictionaryKeyBuilder)
					});
			constructorBuilder.DefineParameter(1, ParameterAttributes.None, "factory");
			constructorBuilder.DefineParameter(2, ParameterAttributes.None, "dictionary");
			constructorBuilder.DefineParameter(3, ParameterAttributes.None, "descriptor");

			ILGenerator ilGenerator = constructorBuilder.GetILGenerator();

			Type objType = Type.GetType("System.Object");
			ConstructorInfo objectConstructorInfo = objType.GetConstructor(new Type[0]);

			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Call, objectConstructorInfo);
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldarg_1);
			ilGenerator.Emit(OpCodes.Stfld, factoryField);
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldarg_2);
			ilGenerator.Emit(OpCodes.Stfld, dictionaryField);
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldarg_3);
			ilGenerator.Emit(OpCodes.Stfld, descriptorField);
			ilGenerator.Emit(OpCodes.Ret);
		}

		#endregion

		private static void CreateDictionaryAdapterMeta(
			TypeBuilder typeBuilder, FieldInfo dictionaryField,
			FieldInfo propertyMapField, FieldInfo factoryField, 
			IDictionary<String, PropertyDescriptor> propertyMap)
		{
			CreateDictionaryAdapterMetaProperty(typeBuilder, MetaDictionaryProp, dictionaryField);
			CreateDictionaryAdapterMetaProperty(typeBuilder, MetaPropertiesProp, propertyMapField);
			CreateDictionaryAdapterMetaProperty(typeBuilder, MetaFactoryProp, factoryField);

			MethodAttributes methodAttribs =
				MethodAttributes.Private | MethodAttributes.HideBySig |
				MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;

			MethodBuilder methodBuilder = typeBuilder.DefineMethod(
				MetaFetchProperties.DeclaringType.FullName + "." + MetaFetchProperties.Name,
				methodAttribs, MetaFetchProperties.ReturnType, null);

			ILGenerator iLGenerator = methodBuilder.GetILGenerator();
			foreach (PropertyDescriptor descriptor in propertyMap.Values)
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Call, descriptor.Property.GetGetMethod());
				iLGenerator.Emit(OpCodes.Pop);
			}
			iLGenerator.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(methodBuilder, MetaFetchProperties);
		}

		private static void CreateDictionaryAdapterMetaProperty(
			TypeBuilder typeBuilder, PropertyInfo metaProp, FieldInfo metaField)
		{
			MethodAttributes propAttribs =
				MethodAttributes.Private | MethodAttributes.SpecialName |
				MethodAttributes.HideBySig | MethodAttributes.NewSlot |
				MethodAttributes.Virtual | MethodAttributes.Final;

			PropertyBuilder propertyBuilder =
				typeBuilder.DefineProperty(metaProp.Name,
					metaProp.Attributes, metaProp.PropertyType, null);

			MethodBuilder getMethodBuilder = typeBuilder.DefineMethod(
				metaProp.DeclaringType.FullName + "." + metaProp.Name,
				propAttribs, metaProp.PropertyType, null);

			ILGenerator getILGenerator = getMethodBuilder.GetILGenerator();
			if (metaField.IsStatic)
			{
				getILGenerator.Emit(OpCodes.Ldsfld, metaField);
			}
			else
			{
				getILGenerator.Emit(OpCodes.Ldarg_0);
				getILGenerator.Emit(OpCodes.Ldfld, metaField);
			}
			getILGenerator.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(getMethodBuilder, metaProp.GetGetMethod());

			propertyBuilder.SetGetMethod(getMethodBuilder);
		}

		#region CreateAdapterProperty

		private static void CreateAdapterProperty(
			TypeBuilder typeBuilder, FieldInfo factoryField,
			FieldInfo dictionaryField, FieldInfo descriptorField,
			FieldInfo propertyMapField, PropertyDescriptor descriptor)
		{
			PropertyInfo property = descriptor.Property;
			PropertyBuilder propertyBuilder =
				typeBuilder.DefineProperty(property.Name, property.Attributes,
				                           property.PropertyType, null);

			MethodAttributes propAttribs =
				MethodAttributes.Public | MethodAttributes.SpecialName |
				MethodAttributes.HideBySig | MethodAttributes.Virtual;

			if (property.CanRead)
			{
				CreatePropertyGetMethod(
					typeBuilder, factoryField, dictionaryField,
					descriptorField, propertyMapField, propertyBuilder,
					descriptor, propAttribs);
			}

			if (property.CanWrite)
			{
				CreatePropertySetMethod(
					typeBuilder, factoryField, dictionaryField,
					descriptorField, propertyMapField, propertyBuilder,
					descriptor, propAttribs);
			}
		}

		private static void PreparePropertyMethod(
			PropertyDescriptor descriptor, FieldInfo dictionaryField,
			FieldInfo propertyMapField, FieldInfo descriptorField,
			ILGenerator propILGenerator, out LocalBuilder descriptorLocal)
		{
			propILGenerator.DeclareLocal(typeof(String));
			propILGenerator.DeclareLocal(typeof(object));
			descriptorLocal = propILGenerator.DeclareLocal(typeof(PropertyDescriptor));

			// key = propertyInfo.Name
			propILGenerator.Emit(OpCodes.Ldstr, descriptor.PropertyName);
			propILGenerator.Emit(OpCodes.Stloc_0);

			// descriptor = propertyMap[key]
			propILGenerator.Emit(OpCodes.Ldsfld, propertyMapField);
			propILGenerator.Emit(OpCodes.Ldstr, descriptor.PropertyName);
			propILGenerator.Emit(OpCodes.Callvirt, PropertyMapGetItem);
			propILGenerator.Emit(OpCodes.Stloc_S, descriptorLocal);

			// key = descriptor.GetKey(dictionary, key, descriptor)
			propILGenerator.Emit(OpCodes.Ldloc_S, descriptorLocal);
			propILGenerator.Emit(OpCodes.Ldarg_0);
			propILGenerator.Emit(OpCodes.Ldfld, dictionaryField);
			propILGenerator.Emit(OpCodes.Ldloc_0);
			propILGenerator.Emit(OpCodes.Ldarg_0);
			propILGenerator.Emit(OpCodes.Ldfld, descriptorField);
			propILGenerator.Emit(OpCodes.Callvirt, DescriptorGetKey);
			propILGenerator.Emit(OpCodes.Stloc_0);
		}

		#endregion

		#region CreatePropertyGetMethod

		private static void CreatePropertyGetMethod(
			TypeBuilder typeBuilder, FieldInfo factoryField,
			FieldInfo dictionaryField, FieldInfo descriptorField,
			FieldInfo propertyMapField, PropertyBuilder propertyBuilder,
			PropertyDescriptor descriptor, MethodAttributes propAttribs)
		{
			MethodBuilder getMethodBuilder = typeBuilder.DefineMethod(
				"get_" + descriptor.PropertyName, propAttribs,
				descriptor.PropertyType, null);

			ILGenerator getILGenerator = getMethodBuilder.GetILGenerator();

			Label returnDefault = getILGenerator.DefineLabel();
			Label storeResult = getILGenerator.DefineLabel();
			Label loadResult = getILGenerator.DefineLabel();
			LocalBuilder descriptorLocal;

			PreparePropertyMethod(
				descriptor, dictionaryField, propertyMapField,
				descriptorField, getILGenerator, out descriptorLocal);
			LocalBuilder result = getILGenerator.DeclareLocal(
				descriptor.PropertyType);

			// value = dictionary[key]
			getILGenerator.Emit(OpCodes.Ldarg_0);
			getILGenerator.Emit(OpCodes.Ldfld, dictionaryField);
			getILGenerator.Emit(OpCodes.Ldloc_0);
			getILGenerator.Emit(OpCodes.Callvirt, DictionaryGetItem);
			getILGenerator.Emit(OpCodes.Stloc_1);

			// value = descriptor.GetPropertyValue(factory, dictionary, key, value, descriptor)
			getILGenerator.Emit(OpCodes.Ldloc_S, descriptorLocal);
			getILGenerator.Emit(OpCodes.Ldarg_0);
			getILGenerator.Emit(OpCodes.Ldfld, factoryField);
			getILGenerator.Emit(OpCodes.Ldarg_0);
			getILGenerator.Emit(OpCodes.Ldfld, dictionaryField);
			getILGenerator.Emit(OpCodes.Ldloc_0);
			getILGenerator.Emit(OpCodes.Ldloc_1);
			getILGenerator.Emit(OpCodes.Ldarg_0);
			getILGenerator.Emit(OpCodes.Ldfld, descriptorField);
			getILGenerator.Emit(OpCodes.Callvirt, DescriptorGetValue);
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
			getILGenerator.Emit(OpCodes.Br_S, loadResult);

			getILGenerator.MarkLabel(loadResult);
			getILGenerator.Emit(OpCodes.Ldloc_S, result);

			getILGenerator.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(getMethodBuilder);
		}

		#endregion

		#region CreatePropertySetMethod

		private static void CreatePropertySetMethod(
			TypeBuilder typeBuilder, FieldInfo factoryField,
			FieldInfo dictionaryField, FieldInfo descriptorField,
			FieldInfo propertyMapField, PropertyBuilder propertyBuilder,
			PropertyDescriptor descriptor, MethodAttributes propAttribs)
		{
			MethodBuilder setMethodBuilder = typeBuilder.DefineMethod(
				"set_" + descriptor.PropertyName, propAttribs, null,
				new Type[] {descriptor.PropertyType});

			ILGenerator setILGenerator = setMethodBuilder.GetILGenerator();
			Label skipSetter = setILGenerator.DefineLabel();
			LocalBuilder descriptorLocal;

			PreparePropertyMethod(
				descriptor, dictionaryField, propertyMapField,
				descriptorField, setILGenerator, out descriptorLocal);

			setILGenerator.Emit(OpCodes.Ldarg_1);
			if (descriptor.PropertyType.IsValueType)
			{
				setILGenerator.Emit(OpCodes.Box, descriptor.PropertyType);
			}
			setILGenerator.Emit(OpCodes.Stloc_1);

			// value = descriptor.SetPropertyValue(factory, dictionary, key, value, descriptor)
			setILGenerator.Emit(OpCodes.Ldloc_S, descriptorLocal);
			setILGenerator.Emit(OpCodes.Ldarg_0);
			setILGenerator.Emit(OpCodes.Ldfld, factoryField);
			setILGenerator.Emit(OpCodes.Ldarg_0);
			setILGenerator.Emit(OpCodes.Ldfld, dictionaryField);
			setILGenerator.Emit(OpCodes.Ldloc_0);
			setILGenerator.Emit(OpCodes.Ldloca_S, 1);
			setILGenerator.Emit(OpCodes.Ldarg_0);
			setILGenerator.Emit(OpCodes.Ldfld, descriptorField);
			setILGenerator.Emit(OpCodes.Callvirt, DescriptorSetValue);
			setILGenerator.Emit(OpCodes.Brfalse_S, skipSetter);

			// dictionary[key] = value
			setILGenerator.Emit(OpCodes.Ldarg_0);
			setILGenerator.Emit(OpCodes.Ldfld, dictionaryField);
			setILGenerator.Emit(OpCodes.Ldloc_0);
			setILGenerator.Emit(OpCodes.Ldloc_1);
			setILGenerator.Emit(OpCodes.Callvirt, DictionarySetItem);

			setILGenerator.MarkLabel(skipSetter);
			setILGenerator.Emit(OpCodes.Ret);

			propertyBuilder.SetSetMethod(setMethodBuilder);
		}

		#endregion

		#region Property Descriptors

		private static Dictionary<String, PropertyDescriptor> GetPropertyDescriptors(Type type)
		{
			Dictionary<String, PropertyDescriptor> propertyMap =
				new Dictionary<String, PropertyDescriptor>();
			ICollection<IDictionaryPropertyGetter> typeGetters = 
				GetOrderedBehaviors<IDictionaryPropertyGetter>(type);
			ICollection<IDictionaryPropertySetter> typeSetters =
				GetOrderedBehaviors<IDictionaryPropertySetter>(type);

			RecursivelyDiscoverProperties(
				type, delegate(PropertyInfo property)
				      {
				      	PropertyDescriptor descriptor = new PropertyDescriptor(property);

				      	descriptor.AddKeyBuilders(GetOrderedBehaviors<IDictionaryKeyBuilder>(property));
				      	descriptor.AddKeyBuilders(GetOrderedBehaviors<IDictionaryKeyBuilder>(property.ReflectedType));

						descriptor.AddGetters(GetOrderedBehaviors<IDictionaryPropertyGetter>(property));
				      	descriptor.AddGetters(typeGetters);
						AddDefaultGetter(descriptor);

				      	descriptor.AddSetters(GetOrderedBehaviors<IDictionaryPropertySetter>(property));
				      	descriptor.AddSetters(typeSetters);

				      	propertyMap.Add(property.Name, descriptor);
				      });

			return propertyMap;
		}

		private static List<T> GetOrderedBehaviors<T>(Type type) where T : IDictionaryBehavior
		{
			List<T> behaviors = AttributesUtil.GetTypeAttributes<T>(type);
			if (behaviors != null)
			{
				behaviors.Sort(DictionaryBehaviorComparer<T>.Instance);
			}
			return behaviors;
		}

		private static List<T> GetOrderedBehaviors<T>(MemberInfo member) where T : IDictionaryBehavior
		{
			List<T> behaviors = AttributesUtil.GetAttributes<T>(member);
			if (behaviors != null)
			{
				behaviors.Sort(DictionaryBehaviorComparer<T>.Instance);
			}
			return behaviors;
		}

		private static void RecursivelyDiscoverProperties(
			Type currentType, Action<PropertyInfo> onProperty)
		{
			List<Type> types = new List<Type>();
			types.Add(currentType);
			types.AddRange(currentType.GetInterfaces());

			foreach(Type type in types)
			{
				foreach(PropertyInfo property in type.GetProperties(
					BindingFlags.Public | BindingFlags.Instance))
				{
					onProperty(property);
				}
			}
		}

		private static void AddDefaultGetter(PropertyDescriptor descriptor)
		{
			if (descriptor.TypeConverter != null)
			{
				descriptor.AddGetter(
					new DefaultPropertyGetter(descriptor.TypeConverter));
			}
		}

		#endregion

		#region Assembly Support 

		private string GetAdapterAssemblyName(Type type)
		{
			return string.Concat(GetAssemblyName( type.Assembly ), ".",
				GetSafeTypeFullName(type), ".DictionaryAdapter" );
		}

		private string GetAssemblyName(Assembly assembly)
		{
			string assemblyName;
			if (!assembliesNames.TryGetValue(assembly, out assemblyName))
			{
				assemblyName = assembly.GetName().Name;
				assembliesNames[assembly] = assemblyName;
			}

			return assemblyName;
		}

		private static String GetAdapterFullTypeName(Type type)
		{
			return type.Namespace + "." + GetAdapterTypeName(type);
		}

		private static String GetAdapterTypeName(Type type)
		{
			return GetSafeTypeName(type).Substring(1) + "DictionaryAdapter";
		}

		public static string GetSafeTypeFullName(Type type)
		{
			if (type.IsGenericTypeDefinition)
			{
				return type.FullName.Replace("`", "_");
			}

			if (type.IsGenericType)
			{
				StringBuilder sb = new StringBuilder();
				if (!string.IsNullOrEmpty(type.Namespace))
				{
					sb.Append(type.Namespace).Append(".");
				}

				AppendGenericTypeName(type, sb);
				return sb.ToString();
			}

			return type.FullName;
		}

		public static string GetSafeTypeName(Type type)
		{
			if (type.IsGenericTypeDefinition)
			{
				return type.Name.Replace("`", "_");
			}

			if (type.IsGenericType)
			{
				StringBuilder sb = new StringBuilder();
				AppendGenericTypeName(type, sb);
				return sb.ToString();
			}

			return type.Name;
		}

		private static void AppendGenericTypeName(Type type, StringBuilder sb)
		{
			// Replace back tick preceding parameter count with _ List`1 => List_1
			sb.Append(type.Name.Replace("`", "_"));

			// Append safe full name of each type argument, separated by _
			foreach (Type argument in type.GetGenericArguments())
			{
				sb.Append("_").Append(GetSafeTypeFullName(argument).Replace(".", "_"));
			}
		}

		private object GetExistingAdapter(Type type, Assembly assembly,
		                                  IDictionary dictionary,
		                                  PropertyDescriptor descriptor)
		{
			String adapterFullTypeName = GetAdapterFullTypeName(type);
			return Activator.CreateInstance(assembly.GetType(adapterFullTypeName, true),
			                                this, dictionary, descriptor);
		}

		private Assembly GetExistingAdapterAssembly( AppDomain appDomain, string assemblyName )
		{
			return Array.Find(
				appDomain.GetAssemblies(),
				assembly => GetAssemblyName( assembly ) == assemblyName );
		}

		#endregion

		#region Reflection Cache

		private static readonly MethodInfo DictionaryGetItem =
			typeof(IDictionary).GetMethod("get_Item", new Type[] {typeof(Object)});

		private static readonly MethodInfo DictionarySetItem =
			typeof(IDictionary).GetMethod("set_Item", new Type[] {typeof(Object), typeof(Object)});

		private static readonly MethodInfo PropertyMapGetItem =
			typeof(Dictionary<String, object[]>).GetMethod("get_Item", new Type[] {typeof(String)});

		private static readonly MethodInfo DescriptorGetKey =
			typeof(PropertyDescriptor).GetMethod("GetKey");

		private static readonly MethodInfo DescriptorGetValue =
			typeof(PropertyDescriptor).GetMethod("GetPropertyValue");

		private static readonly MethodInfo DescriptorSetValue =
			typeof(PropertyDescriptor).GetMethod("SetPropertyValue");

		private static readonly PropertyInfo MetaDictionaryProp =
			typeof(IDictionaryAdapter).GetProperty("Dictionary");

		private static readonly PropertyInfo MetaPropertiesProp =
			typeof(IDictionaryAdapter).GetProperty("Properties");

		private static readonly PropertyInfo MetaFactoryProp =
			typeof(IDictionaryAdapter).GetProperty("Factory");

		private static readonly MethodInfo MetaFetchProperties =
			typeof(IDictionaryAdapter).GetMethod("FetchProperties");

		#endregion
	}
}
