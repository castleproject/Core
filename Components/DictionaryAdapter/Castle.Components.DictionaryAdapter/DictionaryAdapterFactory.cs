// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Threading;
	using System.Collections.Specialized;

	/// <summary>
	/// Uses Reflection.Emit to expose the properties of a dictionary
	/// through a dynamic implementation of a typed interface.
	/// </summary>
	public class DictionaryAdapterFactory : IDictionaryAdapterFactory
	{
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
		/// <param name="keyBuilder">The dictionary key builder.</param>
		/// <returns>An implementation of the typed interface bound to the dictionary.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		public object GetAdapter(Type type, IDictionary dictionary,
		                         IDictionaryKeyBuilder keyBuilder)
		{
			return InternalGetAdapter(type, dictionary, keyBuilder);
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

		#region Dynamic Type Building

		private object InternalGetAdapter(Type type, IDictionary dictionary,
		                                  IDictionaryKeyBuilder keyBuilder)
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
				adapterAssembly = CreateAdapterAssembly(type, typeBuilder, dictionary);
			}

			return GetExistingAdapter(type, adapterAssembly, dictionary, keyBuilder);
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

			return typeBuilder;
		}

		private static Assembly CreateAdapterAssembly(
			Type type, TypeBuilder typeBuilder, IDictionary dictionary)
		{
			FieldBuilder factoryField = typeBuilder.DefineField(
				"factory", typeof(DictionaryAdapterFactory), FieldAttributes.Private);
			FieldBuilder dictionaryField = typeBuilder.DefineField(
				"dictionary", typeof(IDictionary), FieldAttributes.Private);
			FieldBuilder keyBuilderField = typeBuilder.DefineField(
				"keyBuilder", typeof(IDictionaryKeyBuilder), FieldAttributes.Private);
			FieldBuilder propertyMapField = typeBuilder.DefineField(
				"propertyMap", typeof(Dictionary<String, object[]>),
				FieldAttributes.Private | FieldAttributes.Static);

			CreateAdapterConstructor(typeBuilder, factoryField, dictionaryField,
			                         keyBuilderField);

			Dictionary<String, object[]> propertyMap = GetPropertyInfoMap(type, dictionary);

			foreach(KeyValuePair<String, object[]> property in propertyMap)
			{
				String key = (String) property.Value[1];
				PropertyInfo propertyInfo = (PropertyInfo) property.Value[0];
				CreateAdapterProperty(typeBuilder, factoryField, dictionaryField,
				                      keyBuilderField, propertyMapField, key,
				                      propertyInfo);
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
			FieldInfo dictionaryField, FieldInfo keyBuilderField)
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
			constructorBuilder.DefineParameter(3, ParameterAttributes.None, "keyBuilder");

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
			ilGenerator.Emit(OpCodes.Stfld, keyBuilderField);
			ilGenerator.Emit(OpCodes.Ret);
		}

		#endregion

		#region CreateAdapterProperty

		private static void CreateAdapterProperty(
			TypeBuilder typeBuilder, FieldInfo factoryField, 
			FieldInfo dictionaryField, FieldInfo keyBuilderField, 
			FieldInfo propertyMapField, String key, PropertyInfo property)
		{
			PropertyBuilder propertyBuilder =
				typeBuilder.DefineProperty(property.Name, property.Attributes,
				                           property.PropertyType, null);
			
			MethodAttributes propertyMethodAttributes =
				MethodAttributes.Public | MethodAttributes.SpecialName |
				MethodAttributes.HideBySig | MethodAttributes.Virtual;

			if (property.CanRead)
			{
				CreatePropertyGetMethod(
					typeBuilder, factoryField, dictionaryField,
					keyBuilderField, propertyMapField, propertyBuilder,
					key, property, propertyMethodAttributes);
			}

			if (property.CanWrite)
			{
				CreatePropertySetMethod(
					typeBuilder, factoryField, dictionaryField,
					keyBuilderField, propertyMapField, propertyBuilder,
					key, property, propertyMethodAttributes);
			}
		}

		private static void PreparePropertyMethod(
			PropertyInfo property, String key, FieldInfo dictionaryField,
			FieldInfo propertyMapField, FieldInfo keyBuilderField,
			ILGenerator propILGenerator, Label start,
			out LocalBuilder propertyInfo)
		{
			propILGenerator.DeclareLocal(typeof(String));
			propILGenerator.DeclareLocal(typeof(object));
			propILGenerator.DeclareLocal(typeof(object[]));

			propertyInfo = propILGenerator.DeclareLocal(typeof(PropertyInfo));

			// propertyDesc = propertyMap[key]
			propILGenerator.Emit(OpCodes.Ldsfld, propertyMapField);
			propILGenerator.Emit(OpCodes.Ldstr, property.Name);
			propILGenerator.Emit(OpCodes.Callvirt, PropertyMapGetItem);
			propILGenerator.Emit(OpCodes.Stloc_2);

			// propertyInfo = (PropertyInfo) propertyDesc[0]
			propILGenerator.Emit(OpCodes.Ldloc_2);
			propILGenerator.Emit(OpCodes.Ldc_I4_0);
			propILGenerator.Emit(OpCodes.Ldelem_Ref);
			propILGenerator.Emit(OpCodes.Castclass, typeof(PropertyInfo));
			propILGenerator.Emit(OpCodes.Stloc_S, propertyInfo);

			propILGenerator.Emit(OpCodes.Ldstr, key);
			propILGenerator.Emit(OpCodes.Stloc_0);

			propILGenerator.Emit(OpCodes.Ldarg_0);
			propILGenerator.Emit(OpCodes.Ldfld, keyBuilderField);
			propILGenerator.Emit(OpCodes.Brfalse_S, start);

			propILGenerator.Emit(OpCodes.Ldarg_0);
			propILGenerator.Emit(OpCodes.Ldfld, keyBuilderField);
			propILGenerator.Emit(OpCodes.Ldarg_0);
			propILGenerator.Emit(OpCodes.Ldfld, dictionaryField);
			propILGenerator.Emit(OpCodes.Ldloc_0);
			propILGenerator.Emit(OpCodes.Ldloc_S, propertyInfo);
			propILGenerator.Emit(OpCodes.Callvirt, PropertyKeyBuilder);
			propILGenerator.Emit(OpCodes.Stloc_0);
		}

		#endregion

		#region CreatePropertyGetMethod

		private static void CreatePropertyGetMethod(
			TypeBuilder typeBuilder, FieldInfo factoryField,
			FieldInfo dictionaryField, FieldInfo keyBuilderField, 
			FieldInfo propertyMapField, PropertyBuilder propertyBuilder,
			String key, PropertyInfo property,
			MethodAttributes propertyMethodAttributes)
		{
			MethodBuilder getMethodBuilder = typeBuilder.DefineMethod(
				"get_" + property.Name, propertyMethodAttributes,
				property.PropertyType, null);

			ILGenerator getILGenerator = getMethodBuilder.GetILGenerator();

			Label getValue = getILGenerator.DefineLabel();
			Label checkForNull = getILGenerator.DefineLabel();
			Label returnDefault = getILGenerator.DefineLabel();
			Label storeResult = getILGenerator.DefineLabel();
			Label loadResult = getILGenerator.DefineLabel();

			LocalBuilder propertyInfo;
			PreparePropertyMethod(property, key, dictionaryField, propertyMapField,
			                      keyBuilderField, getILGenerator,
			                      getValue, out propertyInfo);
			LocalBuilder result = getILGenerator.DeclareLocal(property.PropertyType);
			LocalBuilder getter = 
				getILGenerator.DeclareLocal(typeof(IDictionaryPropertyGetter));

			// value = dictionary[name]
			getILGenerator.MarkLabel(getValue);
			getILGenerator.Emit(OpCodes.Ldarg_0);
			getILGenerator.Emit(OpCodes.Ldfld, dictionaryField);
			getILGenerator.Emit(OpCodes.Ldloc_0);
			getILGenerator.Emit(OpCodes.Callvirt, DictionaryGetItem);
			getILGenerator.Emit(OpCodes.Stloc_1);

			// propertyGetter = (IDictionaryPropertyGetter) propertyDesc[1]
			getILGenerator.Emit(OpCodes.Ldloc_2);
			getILGenerator.Emit(OpCodes.Ldc_I4_2);
			getILGenerator.Emit(OpCodes.Ldelem_Ref);
			getILGenerator.Emit(OpCodes.Castclass, typeof(IDictionaryPropertyGetter));
			getILGenerator.Emit(OpCodes.Stloc_S, getter);
			
			getILGenerator.Emit(OpCodes.Ldloc_S, getter);
			getILGenerator.Emit(OpCodes.Brfalse_S, checkForNull);

			// gettter.GetPropertyValue(factory, dictionary, key, storedValue, propertyInfo)
			getILGenerator.Emit(OpCodes.Ldloc_S, getter);
			getILGenerator.Emit(OpCodes.Ldarg_0);
			getILGenerator.Emit(OpCodes.Ldfld, factoryField);
			getILGenerator.Emit(OpCodes.Ldarg_0);
			getILGenerator.Emit(OpCodes.Ldfld, dictionaryField);
			getILGenerator.Emit(OpCodes.Ldloc_0);
			getILGenerator.Emit(OpCodes.Ldloc_1);
			getILGenerator.Emit(OpCodes.Ldloc_S, propertyInfo);
			getILGenerator.Emit(OpCodes.Callvirt, PropertyValueGetter);
			getILGenerator.Emit(OpCodes.Stloc_1);

			// if (value == null) return null
			getILGenerator.MarkLabel(checkForNull);
			getILGenerator.Emit(OpCodes.Ldloc_1);
			getILGenerator.Emit(OpCodes.Brfalse_S, returnDefault);

			// return (PropertyType) value
			getILGenerator.Emit(OpCodes.Ldloc_1);
			getILGenerator.Emit(OpCodes.Unbox_Any, property.PropertyType);
			getILGenerator.Emit(OpCodes.Br_S, storeResult);

			getILGenerator.MarkLabel(returnDefault);
			getILGenerator.Emit(OpCodes.Ldloca_S, result);
			getILGenerator.Emit(OpCodes.Initobj, property.PropertyType);
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
			FieldInfo dictionaryField, FieldInfo keyBuilderField,
			FieldInfo propertyMapField, PropertyBuilder propertyBuilder,
			String key, PropertyInfo property,
			MethodAttributes propertyMethodAttributes)
		{
			MethodBuilder setMethodBuilder = typeBuilder.DefineMethod(
				"set_" + property.Name, propertyMethodAttributes, null,
				new Type[] {property.PropertyType});

			ILGenerator setILGenerator = setMethodBuilder.GetILGenerator();
			Label setValue = setILGenerator.DefineLabel();
			Label storeResult = setILGenerator.DefineLabel();

			LocalBuilder propertyInfo;
			PreparePropertyMethod(property, key, dictionaryField, propertyMapField,
			                      keyBuilderField, setILGenerator,
			                      setValue, out propertyInfo);
			LocalBuilder setter = 
				setILGenerator.DeclareLocal(typeof(IDictionaryPropertySetter));

			// propertySetter = (IDictionaryPropertySetter) propertyDesc[1]
			setILGenerator.MarkLabel(setValue);
			setILGenerator.Emit(OpCodes.Ldloc_2);
			setILGenerator.Emit(OpCodes.Ldc_I4_3);
			setILGenerator.Emit(OpCodes.Ldelem_Ref);
			setILGenerator.Emit(OpCodes.Castclass, typeof(IDictionaryPropertySetter));
			setILGenerator.Emit(OpCodes.Stloc_S, setter);

			setILGenerator.Emit(OpCodes.Ldarg_1);
			if (property.PropertyType.IsValueType)
			{
				setILGenerator.Emit(OpCodes.Box, property.PropertyType);
			}
			setILGenerator.Emit(OpCodes.Stloc_1);

			setILGenerator.Emit(OpCodes.Ldloc_S, setter);
			setILGenerator.Emit(OpCodes.Brfalse_S, storeResult);

			// settter.SetPropertyValue(factory, dictionary, key, value, propertyInfo)
			setILGenerator.Emit(OpCodes.Ldloc_S, setter);
			setILGenerator.Emit(OpCodes.Ldarg_0);
			setILGenerator.Emit(OpCodes.Ldfld, factoryField);
			setILGenerator.Emit(OpCodes.Ldarg_0);
			setILGenerator.Emit(OpCodes.Ldfld, dictionaryField);
			setILGenerator.Emit(OpCodes.Ldloc_0);
			setILGenerator.Emit(OpCodes.Ldloc_1);
			setILGenerator.Emit(OpCodes.Ldloc_S, propertyInfo);
			setILGenerator.Emit(OpCodes.Callvirt, PropertyValueSetter);
			setILGenerator.Emit(OpCodes.Stloc_1);

			setILGenerator.MarkLabel(storeResult);
			setILGenerator.Emit(OpCodes.Ldarg_0);
			setILGenerator.Emit(OpCodes.Ldfld, dictionaryField);
			setILGenerator.Emit(OpCodes.Ldloc_0);
			setILGenerator.Emit(OpCodes.Ldloc_1);
			setILGenerator.Emit(OpCodes.Callvirt, DictionarySetItem);

			setILGenerator.Emit(OpCodes.Ret);

			propertyBuilder.SetSetMethod(setMethodBuilder);
		}

		#endregion

		#region Property Descriptors

		private static Dictionary<String, object[]> GetPropertyInfoMap(
			Type type, IDictionary dictionary)
		{
			Dictionary<String, object[]> prop2key = new Dictionary<String, object[]>();
			IDictionaryPropertySetter typeSetter = GetTypePropertySetter(type);

			RecursivelyDiscoverProperties(
				type, delegate(PropertyInfo property)
				           {
				           	String key = property.Name;
				           	key = ApplyKeyBuilders(dictionary, key, property,
				           	                       CollectMemberKeyBuilders(property));
				           	key = ApplyKeyBuilders(dictionary, key, property,
				           	                       CollectTypeKeyBuilders(property.ReflectedType));

				           	IDictionaryPropertyGetter getter = GetPropertyGetter(property);
							IDictionaryPropertySetter setter = GetMemberPropertySetter(property);

				           	object[] propertyDescriptor = new object[]
				           		{property, key, getter, setter ?? typeSetter};
				           	prop2key.Add(property.Name, propertyDescriptor);
				           });

			return prop2key;
		}

		private static String ApplyKeyBuilders(IDictionary dictionary, String key,
		                                       PropertyInfo property,
		                                       IEnumerable<IDictionaryKeyBuilder> builders)
		{
			if (builders != null)
			{
				foreach(IDictionaryKeyBuilder builder in builders)
				{
					key = builder.Apply(dictionary, key, property);
				}
			}
			return key;
		}

		private static List<IDictionaryKeyBuilder> CollectTypeKeyBuilders(Type type)
		{
			List<IDictionaryKeyBuilder> builders = CollectMemberKeyBuilders(type);

			if (builders == null)
			{
				foreach(Type baseInterface in type.GetInterfaces())
				{
					builders = CollectTypeKeyBuilders(baseInterface);
					if (builders != null)
					{
						break;
					}
				}
			}

			return builders;
		}

		private static List<IDictionaryKeyBuilder> CollectMemberKeyBuilders(MemberInfo member)
		{
			List<IDictionaryKeyBuilder> builders = null;
			object[] attributes = member.GetCustomAttributes(typeof(IDictionaryKeyBuilder), false);

			if (attributes.Length > 0)
			{
				builders = new List<IDictionaryKeyBuilder>();
				foreach(IDictionaryKeyBuilder builder in attributes)
				{
					builders.Add(builder);
				}
			}

			return builders;
		}

		private static IDictionaryPropertyGetter GetPropertyGetter(PropertyInfo property)
		{
			object[] getters = property.GetCustomAttributes(typeof(IDictionaryPropertyGetter), false);
			if (getters.Length > 0)
			{
				return (IDictionaryPropertyGetter)getters[0];
			}
			return DefaultPropertyGetter.Instance;
		}

		private static IDictionaryPropertySetter GetTypePropertySetter(Type type)
		{
			IDictionaryPropertySetter setter = GetMemberPropertySetter(type);

			if (setter == null)
			{
				foreach (Type baseInterface in type.GetInterfaces())
				{
					setter = GetMemberPropertySetter(baseInterface);
					if (setter != null)
					{
						break;
					}
				}
			}

			return setter;
		}

		private static IDictionaryPropertySetter GetMemberPropertySetter(MemberInfo member)
		{
			object[] setters = member.GetCustomAttributes(typeof(IDictionaryPropertySetter), false);
			if (setters.Length > 0)
			{
				return (IDictionaryPropertySetter)setters[0];
			}
			return null;
		}


		private static void RecursivelyDiscoverProperties(
			Type currentType, Action<PropertyInfo> onProperty)
		{
			List<Type> types = new List<Type>();
			types.Add(currentType);
			types.AddRange(currentType.GetInterfaces());

			foreach (Type type in types)
			{
				foreach (PropertyInfo property in type.GetProperties(
					BindingFlags.Public | BindingFlags.Instance))
				{
					onProperty(property);
				}
			}
		}

		#endregion

		#region Assembly Support 

		private static String GetAdapterAssemblyName(Type type)
		{
			return type.Assembly.GetName().Name + "." + type.FullName + ".DictionaryAdapter";
		}

		private static String GetAdapterFullTypeName(Type type)
		{
			return type.Namespace + "." + GetAdapterTypeName(type);
		}

		private static String GetAdapterTypeName(Type type)
		{
			return type.Name.Substring(1) + "DictionaryAdapter";
		}

		private object GetExistingAdapter(Type type, Assembly assembly,
		                                  IDictionary dictionary,
		                                  IDictionaryKeyBuilder keyBuilder)
		{
			String adapterFullTypeName = GetAdapterFullTypeName(type);
			return Activator.CreateInstance(assembly.GetType(adapterFullTypeName, true),
			                                this, dictionary, keyBuilder);
		}

		private static Assembly GetExistingAdapterAssembly(AppDomain appDomain, String assemblyName)
		{
			return Array.Find(appDomain.GetAssemblies(),
			                  delegate(Assembly assembly) { return assembly.GetName().Name == assemblyName; });
		}

		#endregion

		#region MethodInfo Cache

		private static readonly MethodInfo DictionaryGetItem =
			typeof(IDictionary).GetMethod("get_Item", new Type[] {typeof(Object)});

		private static readonly MethodInfo DictionarySetItem =
			typeof(IDictionary).GetMethod("set_Item", new Type[] {typeof(Object), typeof(Object)});

		private static readonly MethodInfo PropertyMapGetItem =
			typeof(Dictionary<String, object[]>).GetMethod("get_Item", new Type[] { typeof(String) });

		private static readonly MethodInfo PropertyKeyBuilder =
			typeof(IDictionaryKeyBuilder).GetMethod(
				"Apply", new Type[]
				         	{
				         		typeof(IDictionary), typeof(String), typeof(PropertyInfo)
				         	});

		private static readonly MethodInfo PropertyValueGetter =
			typeof(IDictionaryPropertyGetter).GetMethod(
				"GetPropertyValue", new Type[]
				                    	{
				                    		typeof(DictionaryAdapterFactory),
				                    		typeof(IDictionary), typeof(String), typeof(object),
				                    		typeof(PropertyInfo)
				                    	});

		private static readonly MethodInfo PropertyValueSetter =
			typeof(IDictionaryPropertySetter).GetMethod(
				"SetPropertyValue", new Type[]
				                    	{
				                    		typeof(DictionaryAdapterFactory),
				                    		typeof(IDictionary), typeof(String), typeof(object),
				                    		typeof(PropertyInfo)
				                    	});

		#endregion
	}
}