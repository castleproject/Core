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
			if (!typeof(T).IsInterface)
			{
				throw new ArgumentException("Only interfaces can be adapted");
			}

			AppDomain appDomain = Thread.GetDomain();
			String adapterAssemblyName = GetAdapterAssemblyName<T>();
			Assembly adapterAssembly = GetExistingAdapterAssembly(appDomain, adapterAssemblyName);

			if (adapterAssembly == null)
			{
				adapterAssembly = CreateAdapterAssembly<T>(appDomain, adapterAssemblyName);
			}

			return GetExistingAdapter<T>(adapterAssembly, dictionary);
		}

		private static Assembly CreateAdapterAssembly<T>(AppDomain appDomain, String adapterAssemblyName)
		{
			AssemblyName assemblyName = new AssemblyName(adapterAssemblyName);
			AssemblyBuilder assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(adapterAssemblyName);

			TypeBuilder typeBuilder = CreateAdapterType<T>(moduleBuilder);
			FieldBuilder dictionaryField = CreateAdapterDictionaryField(typeBuilder);
			CreateAdapterConstructor(typeBuilder, dictionaryField);

			foreach (KeyValuePair<PropertyInfo, String> property in GetDictionaryKeyMap<T>())
			{
				CreateAdapterProperty(typeBuilder, dictionaryField, property.Key, property.Value);
			}

			typeBuilder.CreateType();

			return assemblyBuilder;
		}

		private static void CreateAdapterConstructor(TypeBuilder typeBuilder, FieldInfo dictionaryField)
		{
			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
				MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard,
				new Type[] { typeof(IDictionary) });
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

		private static FieldBuilder CreateAdapterDictionaryField(TypeBuilder typeBuilder)
		{
			return typeBuilder.DefineField("dictionary", typeof(IDictionary), FieldAttributes.Private);
		}

		private static void CreateAdapterProperty(TypeBuilder typeBuilder, FieldInfo dictionaryField,
												  PropertyInfo property, String key)
		{
			PropertyBuilder propertyBuilder =
				typeBuilder.DefineProperty(property.Name, property.Attributes, property.PropertyType, null);
			MethodAttributes propertyMethodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName |
														MethodAttributes.HideBySig | MethodAttributes.Virtual;

			if (property.CanRead)
			{
				CreateAdapterPropertyGetMethod(typeBuilder, dictionaryField, propertyBuilder, property, key,
											   propertyMethodAttributes);
			}

			if (property.CanWrite)
			{
				CreateAdapterPropertySetMethod(typeBuilder, dictionaryField, propertyBuilder, property, key,
											   propertyMethodAttributes);
			}
		}

		private static void CreateAdapterPropertyGetMethod(TypeBuilder typeBuilder, FieldInfo dictionaryField,
														   PropertyBuilder propertyBuilder, PropertyInfo property,
														   String key, MethodAttributes propertyMethodAttributes)
		{
			MethodBuilder getMethodBuilder =
				typeBuilder.DefineMethod("get_" + property.Name, propertyMethodAttributes, property.PropertyType, null);

			ILGenerator getILGenerator = getMethodBuilder.GetILGenerator();
			Label labelUnbox = getILGenerator.DefineLabel();
			Label labelLoadResult = getILGenerator.DefineLabel();

			getILGenerator.DeclareLocal(typeof(object));
			getILGenerator.DeclareLocal(property.PropertyType);
			getILGenerator.Emit(OpCodes.Ldarg_0);
			getILGenerator.Emit(OpCodes.Ldfld, dictionaryField);
			getILGenerator.Emit(OpCodes.Ldstr, key);
			getILGenerator.Emit(OpCodes.Callvirt, typeof(IDictionary).GetMethod("get_Item", new Type[] { typeof(Object) }));

			getILGenerator.Emit(OpCodes.Stloc_0);
			getILGenerator.Emit(OpCodes.Ldloc_0);
			getILGenerator.Emit(OpCodes.Ldnull);
			getILGenerator.Emit(OpCodes.Ceq);
			getILGenerator.Emit(OpCodes.Ldc_I4_0);
			getILGenerator.Emit(OpCodes.Ceq);
			getILGenerator.Emit(OpCodes.Brtrue_S, labelUnbox);
			getILGenerator.Emit(OpCodes.Ldloca_S, 1);
			getILGenerator.Emit(OpCodes.Initobj, property.PropertyType);
			getILGenerator.Emit(OpCodes.Br_S, labelLoadResult);

			getILGenerator.MarkLabel(labelUnbox);
			getILGenerator.Emit(OpCodes.Ldloc_0);
			getILGenerator.Emit(OpCodes.Unbox_Any, property.PropertyType);
			getILGenerator.Emit(OpCodes.Stloc_1);

			getILGenerator.MarkLabel(labelLoadResult);
			getILGenerator.Emit(OpCodes.Ldloc_1);
			getILGenerator.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(getMethodBuilder);
		}

		private static void CreateAdapterPropertySetMethod(TypeBuilder typeBuilder, FieldInfo dictionaryField,
														   PropertyBuilder propertyBuilder, PropertyInfo property,
														   String key, MethodAttributes propertyMethodAttributes)
		{
			MethodBuilder setMethodBuilder =
				typeBuilder.DefineMethod("set_" + property.Name, propertyMethodAttributes, null,
										 new Type[] { property.PropertyType });

			ILGenerator setILGenerator = setMethodBuilder.GetILGenerator();

			setILGenerator.Emit(OpCodes.Nop);
			setILGenerator.Emit(OpCodes.Ldarg_0);
			setILGenerator.Emit(OpCodes.Ldfld, dictionaryField);
			setILGenerator.Emit(OpCodes.Ldstr, key);
			setILGenerator.Emit(OpCodes.Ldarg_1);

			if (property.PropertyType.IsValueType)
			{
				setILGenerator.Emit(OpCodes.Box, property.PropertyType);
			}

			setILGenerator.Emit(OpCodes.Callvirt,
								typeof(IDictionary).GetMethod("set_Item", new Type[] { typeof(Object), typeof(Object) }));
			setILGenerator.Emit(OpCodes.Nop);
			setILGenerator.Emit(OpCodes.Ret);

			propertyBuilder.SetSetMethod(setMethodBuilder);
		}

		private static TypeBuilder CreateAdapterType<T>(ModuleBuilder moduleBuilder)
		{
			TypeBuilder typeBuilder =
				moduleBuilder.DefineType(GetAdapterFullTypeName<T>(),
										 TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit);
			typeBuilder.AddInterfaceImplementation(typeof(T));

			return typeBuilder;
		}

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

		private static T GetExistingAdapter<T>(Assembly assembly, IDictionary dictionary)
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
