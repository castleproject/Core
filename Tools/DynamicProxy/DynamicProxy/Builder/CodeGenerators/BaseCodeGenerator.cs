// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.DynamicProxy.Builder.CodeGenerators
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Runtime.Serialization;
	using System.Text;

	using Castle.DynamicProxy.Invocation;

	/// <summary>
	/// Summary description for BaseCodeGenerator.
	/// </summary>
	public abstract class BaseCodeGenerator
	{
		private TypeBuilder m_typeBuilder;
		private FieldBuilder m_interceptorField;
		private FieldBuilder m_cacheField;
		private ConstructorBuilder m_constBuilder;
		protected MethodBuilder m_method2Invocation;
		protected Type m_baseType = typeof (Object);
		
		private IList m_generated = new ArrayList();

		private ModuleScope m_moduleScope;
		private GeneratorContext m_context;

		protected BaseCodeGenerator(ModuleScope moduleScope) : this(moduleScope, new GeneratorContext())
		{
		}

		protected BaseCodeGenerator(ModuleScope moduleScope, GeneratorContext context)
		{
			m_moduleScope = moduleScope;
			m_context = context;
		}

		protected ModuleScope ModuleScope
		{
			get { return m_moduleScope; }
		}

		protected GeneratorContext Context
		{
			get { return m_context; }
		}

		protected TypeBuilder MainTypeBuilder
		{
			get { return m_typeBuilder; }
		}

		protected FieldBuilder InterceptorFieldBuilder
		{
			get { return m_interceptorField; }
		}

		protected FieldBuilder CacheFieldBuilder
		{
			get { return m_cacheField; }
		}

		protected ConstructorBuilder DefaultConstructorBuilder
		{
			get { return m_constBuilder; }
		}

		protected Type GetFromCache(Type baseClass, Type[] interfaces)
		{
			return ModuleScope[GenerateTypeName(baseClass, interfaces)] as Type;
		}

		protected void RegisterInCache(Type generatedType)
		{
			ModuleScope[generatedType.Name] = generatedType;
		}

		protected virtual TypeBuilder CreateTypeBuilder(Type baseType, Type[] interfaces)
		{
			String typeName = GenerateTypeName(baseType, interfaces);

			ModuleBuilder moduleBuilder = ModuleScope.ObtainDynamicModule();

			TypeAttributes flags = TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable;

			m_baseType = baseType;
			m_typeBuilder = moduleBuilder.DefineType(
				typeName, flags, baseType, interfaces);

			GenerateFields();

			m_constBuilder = GenerateConstructor();

			return m_typeBuilder;
		}

		protected virtual void GenerateFields()
		{
			m_interceptorField = GenerateField("__interceptor", typeof (IInterceptor));
			m_cacheField = GenerateField("__cache", typeof (Hashtable), 
				FieldAttributes.Family|FieldAttributes.NotSerialized);
		}

		protected virtual String GenerateTypeName(Type type, Type[] interfaces)
		{
			StringBuilder sb = new StringBuilder();
			foreach (Type inter in interfaces)
			{
				sb.Append('_');
				sb.Append(inter.Name);
			}
			/// Naive implementation
			return String.Format("ProxyType{0}{1}", type.Name, sb.ToString());
		}

		protected virtual void ImplementGetObjectData()
		{
			// To prevent re-implementation of this interface.
			m_generated.Add(typeof (ISerializable));

			Type[] args = new Type[] {typeof (SerializationInfo), typeof (StreamingContext)};
			Type[] get_type_args = new Type[] {typeof (String), typeof (bool), typeof (bool)};
			Type[] key_and_object = new Type[] {typeof (String), typeof (Object)};

			MethodAttributes attrs = MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public;
			MethodBuilder builder = MainTypeBuilder.DefineMethod("GetObjectData", attrs, typeof (void), args);

			ILGenerator generator = builder.GetILGenerator();
			LocalBuilder type_local = generator.DeclareLocal(typeof (Type));

			// type name declared
			generator.Emit(OpCodes.Ldstr, "Castle.DynamicProxy.Serialization.ProxyObjectReference, Castle.DynamicProxy");
			generator.Emit(OpCodes.Ldc_I4_1); // true
			generator.Emit(OpCodes.Ldc_I4_0); // false
			generator.Emit(OpCodes.Call, typeof (Type).GetMethod("GetType", get_type_args));

			// We set the class responsible for
			// serialize/desserialize the proxy
			generator.Emit(OpCodes.Stloc_0);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Callvirt, typeof (SerializationInfo).GetMethod("SetType"));

			// We need to save a few things to allow
			// the proxy to be reconstructed later.
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldstr, "interceptor");
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldfld, InterceptorFieldBuilder);
			generator.Emit(OpCodes.Callvirt, typeof (SerializationInfo).GetMethod("AddValue", key_and_object));

			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldstr, "baseType");
			generator.Emit(OpCodes.Ldtoken, this.m_baseType);
			generator.Emit(OpCodes.Call, typeof (Type).GetMethod("GetTypeFromHandle"));
			generator.Emit(OpCodes.Callvirt, typeof (SerializationInfo).GetMethod("AddValue", key_and_object));



			generator.Emit(OpCodes.Ret);
		}

		protected virtual void ImplementCacheInvocationCache()
		{
			Type[] args = new Type[] {typeof (ICallable), typeof (MethodInfo), typeof(object)};
			Type[] invocation_const_args = new Type[] {typeof (ICallable), typeof(object), typeof(object), typeof (MethodInfo)};
			Type[] int_invocation_const_args = new Type[] {typeof(object), typeof(object), typeof (MethodInfo)};

			MethodAttributes attrs = MethodAttributes.Private;
			m_method2Invocation = MainTypeBuilder.DefineMethod("_Method2Invocation", attrs, typeof (IInvocation), args);

			ILGenerator gen = m_method2Invocation.GetILGenerator();
			LocalBuilder inv_local = gen.DeclareLocal(typeof (IInvocation));

			Label endMethodBranch = gen.DefineLabel();
			Label interfaceInvocationBranch = gen.DefineLabel();
			Label setInCacheBranch = gen.DefineLabel();
			Label retValueBranch = gen.DefineLabel();

			// Try to obtain the invocation stored in the Hashtable
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldfld, CacheFieldBuilder);
			gen.Emit(OpCodes.Ldarg_2);
			gen.Emit(OpCodes.Callvirt, typeof(Hashtable).GetMethod("get_Item", new Type[] { typeof(object) } ));
			gen.Emit(OpCodes.Castclass, typeof(IInvocation));
			gen.Emit(OpCodes.Stloc_0);

			// If not null, goto to return
			gen.Emit(OpCodes.Ldloc_0);
			gen.Emit(OpCodes.Brtrue_S, endMethodBranch);

			// If target != this
			gen.Emit(OpCodes.Ldarg_3);
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Bne_Un_S, interfaceInvocationBranch);

			// Create a SameClassInvocation
			gen.Emit(OpCodes.Ldarg_1); // Callable
			gen.Emit(OpCodes.Ldarg_0); // this
			gen.Emit(OpCodes.Ldarg_0); // this
			gen.Emit(OpCodes.Ldarg_2); // MethodInfo
			gen.Emit(OpCodes.Newobj, typeof(SameClassInvocation).GetConstructor(invocation_const_args) );
			gen.Emit(OpCodes.Stloc_0);
			gen.Emit(OpCodes.Br_S, setInCacheBranch);

			gen.MarkLabel(interfaceInvocationBranch);
			gen.Emit(OpCodes.Ldarg_0); // this
			gen.Emit(OpCodes.Ldarg_3); // target
			gen.Emit(OpCodes.Ldarg_2); // MethodInfo
			gen.Emit(OpCodes.Newobj, typeof(InterfaceInvocation).GetConstructor(int_invocation_const_args) );
			gen.Emit(OpCodes.Stloc_0);

			gen.MarkLabel(setInCacheBranch);
			gen.Emit(OpCodes.Ldarg_0); // this
			gen.Emit(OpCodes.Ldfld, CacheFieldBuilder); 
			gen.Emit(OpCodes.Ldarg_2); // MethodInfo as key
			gen.Emit(OpCodes.Ldloc_0); // IInvocation instance as value
			gen.Emit(OpCodes.Callvirt, typeof(Hashtable).GetMethod("Add", new Type[] { typeof(object), typeof(object) } ));
			
			gen.MarkLabel(endMethodBranch);
			gen.Emit(OpCodes.Br_S, retValueBranch);

			gen.MarkLabel(retValueBranch);
			gen.Emit(OpCodes.Ldloc_0); 
			gen.Emit(OpCodes.Ret); 
		}

		protected virtual Type[] AddISerializable(Type[] interfaces)
		{
			if (Array.IndexOf(interfaces, typeof (ISerializable)) != -1)
			{
				return interfaces;
			}

			int len = interfaces.Length;
			Type[] newlist = new Type[len + 1];
			Array.Copy(interfaces, newlist, len);
			newlist[len] = typeof (ISerializable);
			return newlist;
		}

		protected virtual Type CreateType()
		{
			Type newType = MainTypeBuilder.CreateType();

			RegisterInCache(newType);

			return newType;
		}

		/// <summary>
		/// Generates a protected field
		/// </summary>
		/// <param name="name">Field's name</param>
		/// <param name="type">Field's type</param>
		/// <returns></returns>
		protected FieldBuilder GenerateField(String name, Type type)
		{
			return GenerateField(name, type, FieldAttributes.Family);
		}

		protected FieldBuilder GenerateField(String name, Type type, FieldAttributes attrs)
		{
			return m_typeBuilder.DefineField(name, type, attrs);
		}

		/// <summary>
		/// Generates one public constructor receiving 
		/// the <see cref="IInterceptor"/> instance and instantiating a hashtable
		/// </summary>
		/// <returns><see cref="ConstructorBuilder"/> instance</returns>
		protected abstract ConstructorBuilder GenerateConstructor();

		protected ConstructorInfo ObtainAvailableConstructor(Type target)
		{
			return target.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="interfaces"></param>
		protected void GenerateInterfaceImplementation(Type[] interfaces)
		{
			foreach (Type inter in interfaces)
			{
				if (!Context.ShouldSkip(inter))
				{
					GenerateTypeImplementation(inter, false);
				}
			}
		}

		/// <summary>
		/// Iterates over the interfaces and generate implementation 
		/// for each method in it.
		/// </summary>
		/// <param name="type">Type class</param>
		/// <param name="ignoreInterfaces">Interface type</param>
		protected void GenerateTypeImplementation(Type type, bool ignoreInterfaces)
		{
			if (m_generated.Contains(type))
			{
				return;
			}
			else
			{
				m_generated.Add(type);
			}

			if (!ignoreInterfaces)
			{
				Type[] baseInterfaces = type.FindInterfaces(new TypeFilter(NoFilterImpl), type);

				GenerateInterfaceImplementation(baseInterfaces);
			}

			PropertyBuilder[] propertiesBuilder = GenerateProperties(type);
			GenerateMethods(type, propertiesBuilder);
		}

		protected virtual PropertyBuilder[] GenerateProperties(Type inter)
		{
			PropertyInfo[] properties = inter.GetProperties();
			PropertyBuilder[] propertiesBuilder = new PropertyBuilder[properties.Length];

			for (int i = 0; i < properties.Length; i++)
			{
				propertiesBuilder[i] = GeneratePropertyImplementation(properties[i]);
			}

			return propertiesBuilder;
		}

		protected virtual void GenerateMethods(Type inter, PropertyBuilder[] propertiesBuilder)
		{
			MethodInfo[] methods = inter.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			foreach (MethodInfo method in methods)
			{
				if (method.IsPrivate || !method.IsVirtual || method.IsFinal)
				{
					continue;
				}
				if (method.DeclaringType.Equals(typeof (Object)) && !method.IsVirtual)
				{
					continue;
				}
				if (method.DeclaringType.Equals(typeof (Object)) && "Finalize".Equals(method.Name))
				{
					continue;
				}
				GenerateMethodImplementation(method, propertiesBuilder);
			}
		}

		/// <summary>
		/// Generate property implementation
		/// </summary>
		/// <param name="property"></param>
		protected PropertyBuilder GeneratePropertyImplementation(PropertyInfo property)
		{
			return m_typeBuilder.DefineProperty(
				property.Name, property.Attributes, property.PropertyType, null);
		}

		/// <summary>
		/// Generates implementation for each method.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="properties"></param>
		protected void GenerateMethodImplementation(
			MethodInfo method, PropertyBuilder[] properties)
		{
			ParameterInfo[] parameterInfo = method.GetParameters();

			Type[] parameters = new Type[parameterInfo.Length];

			for (int i = 0; i < parameterInfo.Length; i++)
			{
				parameters[i] = parameterInfo[i].ParameterType;
			}

			MethodAttributes atts = MethodAttributes.Virtual;

			if (method.IsPublic)
			{
				atts |= MethodAttributes.Public;
			}

			if (method.IsFamilyAndAssembly)
			{
				atts |= MethodAttributes.FamANDAssem;
			}
			else if (method.IsFamilyOrAssembly)
			{
				atts |= MethodAttributes.FamORAssem;
			}
			else if (method.IsFamily)
			{
				atts |= MethodAttributes.Family;
			}

			if (method.Name.StartsWith("set_") || method.Name.StartsWith("get_"))
			{
				atts |= MethodAttributes.SpecialName;
			}

			MethodBuilder methodBuilder =
				m_typeBuilder.DefineMethod(method.Name, atts, CallingConventions.Standard,
				                           method.ReturnType, parameters);

			if (method.Name.StartsWith("set_") || method.Name.StartsWith("get_"))
			{
				foreach (PropertyBuilder property in properties)
				{
					if (property == null)
					{
						break;
					}

					if (!property.Name.Equals(method.Name.Substring(4)))
					{
						continue;
					}

					if (methodBuilder.Name.StartsWith("set_"))
					{
						property.SetSetMethod(methodBuilder);
						break;
					}
					else
					{
						property.SetGetMethod(methodBuilder);
						break;
					}
				}
			}

			PreProcessMethod(method);
			
			WriteILForMethod(method, methodBuilder, parameters);
			
			PostProcessMethod(method);
		}

		protected virtual void PreProcessMethod(MethodInfo method)
		{
		}

		protected virtual void PostProcessMethod(MethodInfo method)
		{
		}

		/// <summary>
		/// Writes the method implementation. This 
		/// method generates the IL code for property get/set method and
		/// ordinary methods.
		/// </summary>
		/// <param name="builder"><see cref="MethodBuilder"/> being constructed.</param>
		/// <param name="parameters"></param>
		protected abstract void WriteILForMethod(MethodInfo method, MethodBuilder builder, Type[] parameters);

		public static bool NoFilterImpl(Type type, object criteria)
		{
			return true;
		}
	}
}