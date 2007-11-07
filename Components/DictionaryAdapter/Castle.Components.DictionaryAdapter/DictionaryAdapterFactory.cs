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
		/// <summary>
		/// Gets a typed adapter bound to the dictionary.
		/// </summary>
		/// <typeparam name="T">The typed interface.</typeparam>
		/// <param name="dictionary">The underlying source of properties.</param>
		/// <returns>An implementation of the typed interface bound to the dictionary.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		public T GetAdapter<T>(IDictionary dictionary)
		{
			return InternalGetAdapter<T>(dictionary, 
				CreateIDictionaryField, 
				CreateIDictionaryBasedPropertyGetMethod, 
				CreateIDictionaryBasedPropertySetMethod);
		}

		public T GetAdapter<T>(NameValueCollection dictionary)
		{
			return InternalGetAdapter<T>(dictionary, 
				CreateNameValueCollectionField, 
				CreateNameValueCollectionBasedPropertyGetMethod, 
				CreateNameValueCollectionBasedPropertySetMethod);
		}
		
		private static T InternalGetAdapter<T>(object dictionary, DictionaryFieldCreationDelegate CreateDictionaryField, PropertyCreationDelegate CreatePropertyGet, PropertyCreationDelegate CreatePropertySet)
		{
			if (!typeof(T).IsInterface)
			{
				throw new ArgumentException("Only interfaces can be adapted");
			}

			AppDomain appDomain = Thread.GetDomain();
			String adapterAssemblyName = GetAdapterAssemblyName<T>();
			Assembly adapterAssembly = GetExistingAdapterAssembly(appDomain, adapterAssemblyName);

			if (adapterAssembly == null)
			{
				TypeBuilder typeBuilder = CreateTypeBuilder<T>(appDomain, adapterAssemblyName);
				Type dictionaryType = dictionary is IDictionary ? typeof (IDictionary) : typeof (NameValueCollection);
				adapterAssembly = CreateAdapterAssembly<T>(typeBuilder, dictionaryType, CreateDictionaryField, CreatePropertyGet, CreatePropertySet);
			}

			return GetExistingAdapter<T>(adapterAssembly, dictionary);
		}

		private static TypeBuilder CreateTypeBuilder<T>(AppDomain appDomain, String adapterAssemblyName)
		{
			AssemblyName assemblyName = new AssemblyName(adapterAssemblyName);
			AssemblyBuilder assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(adapterAssemblyName);

			return CreateAdapterType<T>(moduleBuilder);
		}

		private static TypeBuilder CreateAdapterType<T>(ModuleBuilder moduleBuilder)
		{
			TypeBuilder typeBuilder =
				moduleBuilder.DefineType(GetAdapterFullTypeName<T>(),
										 TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit);
			typeBuilder.AddInterfaceImplementation(typeof(T));

			return typeBuilder;
		}

		private static Assembly CreateAdapterAssembly<T>(TypeBuilder typeBuilder, Type dictionaryType, DictionaryFieldCreationDelegate CreateDictionaryField, PropertyCreationDelegate CreatePropertyGet, PropertyCreationDelegate CreatePropertySet)
		{
			FieldBuilder dictionaryField = CreateDictionaryField(typeBuilder);
			CreateAdapterConstructor(typeBuilder, dictionaryField, dictionaryType);

			foreach (KeyValuePair<PropertyInfo, String> property in GetDictionaryKeyMap<T>())
			{
				CreateAdapterProperty(typeBuilder, dictionaryField, property.Key, property.Value, CreatePropertyGet, CreatePropertySet);
			}

			typeBuilder.CreateType();

			return typeBuilder.Assembly;
		}

		private static void CreateAdapterConstructor(TypeBuilder typeBuilder, FieldInfo dictionaryField, Type dictionaryType)
		{
			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
				MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard,
				new Type[] { dictionaryType });
			constructorBuilder.DefineParameter(1, ParameterAttributes.None, "dictionary");

			ILGenerator ilGenerator = constructorBuilder.GetILGenerator();

			Type objType = Type.GetType("System.Object");
			ConstructorInfo objectConstructorInfo = objType.GetConstructor(new Type[0]);

			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Call, objectConstructorInfo);
			ilGenerator.Emit(OpCodes.Nop);
			ilGenerator.Emit(OpCodes.Nop);
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldarg_1);
			ilGenerator.Emit(OpCodes.Stfld, dictionaryField);
			ilGenerator.Emit(OpCodes.Nop);
			ilGenerator.Emit(OpCodes.Ret);
		}

		delegate FieldBuilder DictionaryFieldCreationDelegate(TypeBuilder typeBuilder);
		delegate void PropertyCreationDelegate(TypeBuilder typeBuilder, FieldInfo dictionaryField,
														   PropertyBuilder propertyBuilder, PropertyInfo property,
														   String key, MethodAttributes propertyMethodAttributes);
		private static void CreateAdapterProperty(TypeBuilder typeBuilder, FieldInfo dictionaryField,
												  PropertyInfo property, String key, PropertyCreationDelegate CreatePropertyGet, PropertyCreationDelegate CreatePropertySet)
		{
			PropertyBuilder propertyBuilder =
				typeBuilder.DefineProperty(property.Name, property.Attributes, property.PropertyType, null);
			MethodAttributes propertyMethodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName |
														MethodAttributes.HideBySig | MethodAttributes.Virtual;

			if (property.CanRead)
			{
				CreatePropertyGet(typeBuilder, dictionaryField, propertyBuilder, property, key,
								   propertyMethodAttributes);
			}

			if (property.CanWrite)
			{
				CreatePropertySet(typeBuilder, dictionaryField, propertyBuilder, property, key,
								   propertyMethodAttributes);
			}
		}

		#region IDictionary
		private static FieldBuilder CreateIDictionaryField(TypeBuilder typeBuilder)
		{
			return typeBuilder.DefineField("dictionary", typeof(IDictionary), FieldAttributes.Private);
		}


		private static void CreateIDictionaryBasedPropertyGetMethod(TypeBuilder typeBuilder, FieldInfo dictionaryField,
														   PropertyBuilder propertyBuilder, PropertyInfo property,
														   String key, MethodAttributes propertyMethodAttributes)
		{
			MethodBuilder getMethodBuilder =
				typeBuilder.DefineMethod("get_" + property.Name, propertyMethodAttributes, property.PropertyType, null);

			object[] attributes = property.GetCustomAttributes(typeof(DictionaryAdapterPropertyBinderAttribute), false);
			DictionaryAdapterPropertyBinderAttribute binderAttribute = (attributes.Length > 0) ? (DictionaryAdapterPropertyBinderAttribute)attributes[0] : null;

			ILGenerator getILGenerator = getMethodBuilder.GetILGenerator();
			Label returnDefault = getILGenerator.DefineLabel();
			Label storeResult = getILGenerator.DefineLabel();
			Label loadResult = getILGenerator.DefineLabel();

			getILGenerator.DeclareLocal(typeof (object));
			getILGenerator.DeclareLocal(property.PropertyType);
			LocalBuilder local2 = (binderAttribute != null) ? getILGenerator.DeclareLocal(property.PropertyType) : null;

			getILGenerator.Emit(OpCodes.Nop);
			getILGenerator.Emit(OpCodes.Ldarg_0);
			getILGenerator.Emit(OpCodes.Ldfld, dictionaryField);
			getILGenerator.Emit(OpCodes.Ldstr, key);
			getILGenerator.Emit(OpCodes.Callvirt, typeof(IDictionary).GetMethod("get_Item", new Type[] { typeof(Object) }));

			getILGenerator.Emit(OpCodes.Stloc_0);
			getILGenerator.Emit(OpCodes.Ldloc_0);
			getILGenerator.Emit(OpCodes.Brfalse_S, returnDefault);

			if (binderAttribute != null)
			{
				getILGenerator.Emit(OpCodes.Newobj, binderAttribute.Binder.GetConstructor(new Type[] { }));
				getILGenerator.Emit(OpCodes.Ldloc_0);
				getILGenerator.Emit(OpCodes.Call, binderAttribute.Binder.GetMethod("ConvertFromDictionary", new Type[] { typeof(Object) }));
				getILGenerator.Emit(OpCodes.Unbox_Any, property.PropertyType);
				getILGenerator.Emit(OpCodes.Br_S, storeResult);

				getILGenerator.MarkLabel(returnDefault);
				getILGenerator.Emit(OpCodes.Ldloca_S, local2);
				getILGenerator.Emit(OpCodes.Initobj, property.PropertyType);
				getILGenerator.Emit(OpCodes.Ldloc_2);
			}
			else
			{				
				getILGenerator.Emit(OpCodes.Ldloc_0);
				getILGenerator.Emit(OpCodes.Unbox_Any, property.PropertyType);
				getILGenerator.Emit(OpCodes.Br_S, storeResult);

				getILGenerator.MarkLabel(returnDefault);
				getILGenerator.Emit(OpCodes.Ldloca_S, 1);
				getILGenerator.Emit(OpCodes.Initobj, property.PropertyType);
				getILGenerator.Emit(OpCodes.Br_S, loadResult);
			}

			getILGenerator.MarkLabel(storeResult);
			getILGenerator.Emit(OpCodes.Stloc_1);
			getILGenerator.Emit(OpCodes.Br_S, loadResult);

			getILGenerator.MarkLabel(loadResult);
			getILGenerator.Emit(OpCodes.Ldloc_1);
			
			getILGenerator.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(getMethodBuilder);
		}

		private static void CreateIDictionaryBasedPropertySetMethod(TypeBuilder typeBuilder, FieldInfo dictionaryField,
														   PropertyBuilder propertyBuilder, PropertyInfo property,
														   String key, MethodAttributes propertyMethodAttributes)
		{
			MethodBuilder setMethodBuilder =
				typeBuilder.DefineMethod("set_" + property.Name, propertyMethodAttributes, null,
										 new Type[] { property.PropertyType });

			
			object[] attributes = property.GetCustomAttributes(typeof (DictionaryAdapterPropertyBinderAttribute), false);
			DictionaryAdapterPropertyBinderAttribute binderAttribute = (attributes.Length > 0) ? (DictionaryAdapterPropertyBinderAttribute) attributes[0] : null;

			ILGenerator setILGenerator = setMethodBuilder.GetILGenerator();

			setILGenerator.Emit(OpCodes.Nop);
			setILGenerator.Emit(OpCodes.Ldarg_0);
			setILGenerator.Emit(OpCodes.Ldfld, dictionaryField);
			setILGenerator.Emit(OpCodes.Ldstr, key);

			if (binderAttribute != null)
			{
				setILGenerator.Emit(OpCodes.Newobj, binderAttribute.Binder.GetConstructor(new Type[] {}));
			}

			setILGenerator.Emit(OpCodes.Ldarg_1);

			if (property.PropertyType.IsValueType)
			{
				setILGenerator.Emit(OpCodes.Box, property.PropertyType);
			}

			if (binderAttribute != null)
			{
				setILGenerator.Emit(OpCodes.Callvirt, binderAttribute.Binder.GetMethod("ConvertFromInterface", new Type[] { typeof(Object) }));
			}

			setILGenerator.Emit(OpCodes.Callvirt, typeof(IDictionary).GetMethod("set_Item", new Type[] {typeof(Object), typeof(Object)}));
			setILGenerator.Emit(OpCodes.Nop);
			setILGenerator.Emit(OpCodes.Ret);

			propertyBuilder.SetSetMethod(setMethodBuilder);
		}
		#endregion

		#region NameValueCollection
		private static FieldBuilder CreateNameValueCollectionField(TypeBuilder typeBuilder)
		{
			return typeBuilder.DefineField("dictionary", typeof(NameValueCollection), FieldAttributes.Private);
		}


		private static void CreateNameValueCollectionBasedPropertyGetMethod(TypeBuilder typeBuilder, FieldInfo dictionaryField,
														   PropertyBuilder propertyBuilder, PropertyInfo property,
														   String key, MethodAttributes propertyMethodAttributes)
		{
			MethodBuilder getMethodBuilder =
				typeBuilder.DefineMethod("get_" + property.Name, propertyMethodAttributes, property.PropertyType, null);

			object[] attributes = property.GetCustomAttributes(typeof(DictionaryAdapterPropertyBinderAttribute), false);
			DictionaryAdapterPropertyBinderAttribute binderAttribute = (attributes.Length > 0) ? (DictionaryAdapterPropertyBinderAttribute)attributes[0] : null;

			ILGenerator getILGenerator = getMethodBuilder.GetILGenerator();
			Label returnDefault = getILGenerator.DefineLabel();
			Label storeResult = getILGenerator.DefineLabel();
			Label loadResult = getILGenerator.DefineLabel();

			getILGenerator.DeclareLocal(typeof (object));
			getILGenerator.DeclareLocal(property.PropertyType);
			LocalBuilder local2 = (binderAttribute != null) ? getILGenerator.DeclareLocal(property.PropertyType) : null;

			getILGenerator.Emit(OpCodes.Nop);
			getILGenerator.Emit(OpCodes.Ldarg_0);
			getILGenerator.Emit(OpCodes.Ldfld, dictionaryField);
			getILGenerator.Emit(OpCodes.Ldstr, key);
			getILGenerator.Emit(OpCodes.Callvirt, typeof(NameValueCollection).GetMethod("get_Item", new Type[] { typeof(string) }));

			getILGenerator.Emit(OpCodes.Stloc_0);
			getILGenerator.Emit(OpCodes.Ldloc_0);
			getILGenerator.Emit(OpCodes.Brfalse_S, returnDefault);

			if (binderAttribute != null)
			{
				getILGenerator.Emit(OpCodes.Newobj, binderAttribute.Binder.GetConstructor(new Type[] { }));
				getILGenerator.Emit(OpCodes.Ldloc_0);
				getILGenerator.Emit(OpCodes.Call, binderAttribute.Binder.GetMethod("ConvertFromDictionary", new Type[] { typeof(Object) }));
				getILGenerator.Emit(OpCodes.Unbox_Any, property.PropertyType);
				getILGenerator.Emit(OpCodes.Br_S, storeResult);

				getILGenerator.MarkLabel(returnDefault);
				getILGenerator.Emit(OpCodes.Ldloca_S, local2);
				getILGenerator.Emit(OpCodes.Initobj, property.PropertyType);
				getILGenerator.Emit(OpCodes.Ldloc_2);
			}
			else
			{				
				getILGenerator.Emit(OpCodes.Ldloc_0);
				getILGenerator.Emit(OpCodes.Unbox_Any, property.PropertyType);
				getILGenerator.Emit(OpCodes.Br_S, storeResult);

				getILGenerator.MarkLabel(returnDefault);
				getILGenerator.Emit(OpCodes.Ldloca_S, 1);
				getILGenerator.Emit(OpCodes.Initobj, property.PropertyType);
				getILGenerator.Emit(OpCodes.Br_S, loadResult);
			}

			getILGenerator.MarkLabel(storeResult);
			getILGenerator.Emit(OpCodes.Stloc_1);
			getILGenerator.Emit(OpCodes.Br_S, loadResult);

			getILGenerator.MarkLabel(loadResult);
			getILGenerator.Emit(OpCodes.Ldloc_1);
			
			getILGenerator.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(getMethodBuilder);
		}

		private static void CreateNameValueCollectionBasedPropertySetMethod(TypeBuilder typeBuilder, FieldInfo dictionaryField,
														   PropertyBuilder propertyBuilder, PropertyInfo property,
														   String key, MethodAttributes propertyMethodAttributes)
		{
			MethodBuilder setMethodBuilder =
				typeBuilder.DefineMethod("set_" + property.Name, propertyMethodAttributes, null,
										 new Type[] { property.PropertyType });

			
			object[] attributes = property.GetCustomAttributes(typeof (DictionaryAdapterPropertyBinderAttribute), false);
			DictionaryAdapterPropertyBinderAttribute binderAttribute = (attributes.Length > 0) ? (DictionaryAdapterPropertyBinderAttribute) attributes[0] : null;

			ILGenerator setILGenerator = setMethodBuilder.GetILGenerator();

			setILGenerator.Emit(OpCodes.Nop);
			setILGenerator.Emit(OpCodes.Ldarg_0);
			setILGenerator.Emit(OpCodes.Ldfld, dictionaryField);
			setILGenerator.Emit(OpCodes.Ldstr, key);

			if (binderAttribute != null)
			{
				setILGenerator.Emit(OpCodes.Newobj, binderAttribute.Binder.GetConstructor(new Type[] {}));
			}

			setILGenerator.Emit(OpCodes.Ldarg_1);

			if (property.PropertyType.IsValueType)
			{
				setILGenerator.Emit(OpCodes.Box, property.PropertyType);
			}

			if (binderAttribute != null)
			{
				setILGenerator.Emit(OpCodes.Callvirt, binderAttribute.Binder.GetMethod("ConvertFromInterface", new Type[] { typeof(Object) }));
			}

			setILGenerator.Emit(OpCodes.Callvirt, typeof(NameValueCollection).GetMethod("set_Item", new Type[] { typeof(string), typeof(string) }));
			setILGenerator.Emit(OpCodes.Nop);
			setILGenerator.Emit(OpCodes.Ret);

			propertyBuilder.SetSetMethod(setMethodBuilder);
		}
		#endregion

		private static Dictionary<PropertyInfo, String> GetDictionaryKeyMap<T>()
		{
			List<IDictionaryKeyBuilder> builders, typeBuilders;
			Dictionary<PropertyInfo, String> prop2key = new Dictionary<PropertyInfo, string>();

			RecursivelyDiscoverProperties(
				typeof(T), delegate(PropertyInfo property)
						   {
							   String key = property.Name;
							   builders = CollectMemberDictionaryKeyBuilders(property);
							   typeBuilders = CollectTypeDictionaryKeyBuilders(property.ReflectedType);

							   if (builders != null)
							   {
								   key = ApplyKeyBuilders(key, property, builders);
							   }

							   if (typeBuilders != null)
							   {
								   key = ApplyKeyBuilders(key, property, typeBuilders);
							   }

							   prop2key.Add(property, key);
						   });

			return prop2key;
		}

		private static String ApplyKeyBuilders(String key, PropertyInfo property,
											   IEnumerable<IDictionaryKeyBuilder> builders)
		{
			foreach (IDictionaryKeyBuilder builder in builders)
			{
				key = builder.Apply(key, property);
			}
			return key;
		}

		private static List<IDictionaryKeyBuilder> CollectTypeDictionaryKeyBuilders(Type type)
		{
			List<IDictionaryKeyBuilder> builders = CollectMemberDictionaryKeyBuilders(type);

			if (builders == null)
			{
				foreach (Type baseInterface in type.GetInterfaces())
				{
					builders = CollectTypeDictionaryKeyBuilders(baseInterface);
					if (builders != null) break;
				}
			}

			return builders;
		}

		private static List<IDictionaryKeyBuilder> CollectMemberDictionaryKeyBuilders(MemberInfo member)
		{
			List<IDictionaryKeyBuilder> builders = null;
			object[] attributes = member.GetCustomAttributes(typeof(IDictionaryKeyBuilder), false);

			if (attributes.Length > 0)
			{
				builders = new List<IDictionaryKeyBuilder>();
				foreach (IDictionaryKeyBuilder builder in attributes)
				{
					builders.Add(builder);
				}
			}

			return builders;
		}

		private static String GetAdapterAssemblyName<T>()
		{
			return typeof(T).Assembly.GetName().Name + "." + typeof(T).FullName + ".DictionaryAdapter";
		}

		private static String GetAdapterFullTypeName<T>()
		{
			return typeof(T).Namespace + "." + GetAdapterTypeName<T>();
		}

		private static String GetAdapterTypeName<T>()
		{
			return typeof(T).Name.Substring(1) + "DictionaryAdapter";
		}

		private static T GetExistingAdapter<T>(Assembly assembly, object dictionary)
		{
			String adapterFullTypeName = GetAdapterFullTypeName<T>();
			return (T)Activator.CreateInstance(assembly.GetType(adapterFullTypeName, true), dictionary);
		}

		private static Assembly GetExistingAdapterAssembly(AppDomain appDomain, String assemblyName)
		{
			return Array.Find(appDomain.GetAssemblies(),
							  delegate(Assembly assembly)
							  {
								  return assembly.GetName().Name == assemblyName;
							  });
		}

		private static void RecursivelyDiscoverProperties(Type currentType, Action<PropertyInfo> onProperty)
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
	}
}
