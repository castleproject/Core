using System.Text;
// Copyright 2004 The Apache Software Foundation
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

namespace Apache.Avalon.DynamicProxy.Builder.CodeGenerators
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Reflection.Emit;

	/// <summary>
	/// Summary description for BaseCodeGenerator.
	/// </summary>
	public abstract class BaseCodeGenerator
	{

		private Type m_baseType = typeof (Object);
		private TypeBuilder m_typeBuilder;
		private FieldBuilder m_handlerField;
		private ConstructorBuilder m_constBuilder;
		private IList m_generated = new ArrayList();

		private GeneratorContext m_context;
		private ModuleScope m_moduleScope;

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

		protected FieldBuilder HandlerFieldBuilder
		{
			get { return m_handlerField; }
		}

		protected ConstructorBuilder DefaultConstructorBuilder
		{
			get { return m_constBuilder; }
		}

		protected Type GetFromCache( Type baseClass, Type[] interfaces )
		{
			return ModuleScope[ GenerateTypeName( baseClass, interfaces ) ] as Type;
		}

		protected void RegisterInCache( Type generatedType )
		{
			ModuleScope[ generatedType.Name ] = generatedType;
		}

		protected virtual TypeBuilder CreateTypeBuilder(Type baseType, Type[] interfaces)
		{
			String typeName = GenerateTypeName(baseType, interfaces);

			ModuleBuilder moduleBuilder = ModuleScope.ObtainDynamicModule();

			TypeAttributes flags = TypeAttributes.Public | TypeAttributes.Class; 

			if (baseType != typeof(Object))
			{
				if (baseType.IsSerializable)
				{
					flags |= TypeAttributes.Serializable;
				}
			}

			m_baseType = baseType;
			m_typeBuilder = moduleBuilder.DefineType(
				typeName, flags, baseType, interfaces);

			m_handlerField = GenerateField();
			m_constBuilder = GenerateConstructor();

			return m_typeBuilder;
		}

		protected virtual String GenerateTypeName(Type type, Type[] interfaces)
		{
			StringBuilder sb = new StringBuilder();
			foreach(Type inter in interfaces)
			{
				sb.Append('_');
				sb.Append(inter.Name);
			}
			/// Naive implementation
			return String.Format("ProxyType{0}{1}", type.Name, sb.ToString());
		}

		protected virtual void EnhanceType()
		{
			if (Context.EnhanceType != null)
			{
				Context.EnhanceType(MainTypeBuilder, HandlerFieldBuilder, DefaultConstructorBuilder);
			}
		}

		protected virtual Type[] ScreenInterfaces(Type[] interfaces)
		{
			if (Context.ScreenInterfaces != null)
			{
				interfaces = Context.ScreenInterfaces(interfaces);
			}

			return interfaces;
		}

		protected virtual Type CreateType()
		{
			Type newType = MainTypeBuilder.CreateType();

			RegisterInCache( newType );

			return newType;
		}

		/// <summary>
		/// Generates a public field holding the <see cref="IInvocationHandler"/>
		/// </summary>
		/// <returns><see cref="FieldBuilder"/> instance</returns>
		protected FieldBuilder GenerateField()
		{
			return GenerateField("handler", typeof (IInvocationHandler));
		}

		/// <summary>
		/// Generates a public field
		/// </summary>
		/// <param name="name">Field's name</param>
		/// <param name="type">Field's type</param>
		/// <returns></returns>
		protected FieldBuilder GenerateField(String name, Type type)
		{
			return m_typeBuilder.DefineField(name,
			                                 typeof (IInvocationHandler), FieldAttributes.Public);
		}

		/// <summary>
		/// Generates one public constructor receiving 
		/// the <see cref="IInvocationHandler"/> instance.
		/// </summary>
		/// <returns><see cref="ConstructorBuilder"/> instance</returns>
		protected ConstructorBuilder GenerateConstructor()
		{
			ConstructorBuilder consBuilder = m_typeBuilder.DefineConstructor(
				MethodAttributes.Public,
				CallingConventions.Standard,
				new Type[] {typeof (IInvocationHandler)});

			ILGenerator ilGenerator = consBuilder.GetILGenerator();
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Call, m_baseType.GetConstructor(new Type[0]));
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldarg_1);
			ilGenerator.Emit(OpCodes.Stfld, m_handlerField);
			ilGenerator.Emit(OpCodes.Ret);

			return consBuilder;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="interfaces"></param>
		protected void GenerateInterfaceImplementation(Type[] interfaces)
		{
			foreach(Type inter in interfaces)
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

			for(int i = 0; i < properties.Length; i++)
			{
				propertiesBuilder[i] = GeneratePropertyImplementation(properties[i]);
			}

			return propertiesBuilder;
		}

		protected virtual void GenerateMethods(Type inter, PropertyBuilder[] propertiesBuilder)
		{
			MethodInfo[] methods = inter.GetMethods();

			foreach(MethodInfo method in methods)
			{
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
			if (method.IsFinal)
			{
				return;
			}

			ParameterInfo[] parameterInfo = method.GetParameters();

			Type[] parameters = new Type[parameterInfo.Length];

			for(int i = 0; i < parameterInfo.Length; i++)
			{
				parameters[i] = parameterInfo[i].ParameterType;
			}

			MethodAttributes atts = MethodAttributes.Public | MethodAttributes.Virtual;

			if (method.Name.StartsWith("set_") || method.Name.StartsWith("get_"))
			{
				atts = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual;
			}

			MethodBuilder methodBuilder =
				m_typeBuilder.DefineMethod(method.Name, atts, CallingConventions.Standard,
				                           method.ReturnType, parameters);

			if (method.Name.StartsWith("set_") || method.Name.StartsWith("get_"))
			{
				foreach(PropertyBuilder property in properties)
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

			WriteILForMethod(method, methodBuilder, parameters, HandlerFieldBuilder);
		}

		/// <summary>
		/// Writes the stack for the method implementation. This 
		/// method generates the IL stack for property get/set method and
		/// ordinary methods.
		/// </summary>
		/// <remarks>
		/// The method implementation would be as simple as:
		/// <code>
		/// public void SomeMethod( int parameter )
		/// {
		///     MethodBase method = MethodBase.GetCurrentMethod();
		///     handler.Invoke( this, method, new object[] { parameter } );
		/// }
		/// </code>
		/// </remarks>
		/// <param name="builder"><see cref="MethodBuilder"/> being constructed.</param>
		/// <param name="parameters"></param>
		/// <param name="handlerField"></param>
		protected void WriteILForMethod(MethodInfo method, MethodBuilder builder,
		                                Type[] parameters, FieldBuilder handlerField)
		{
			int arrayPositionInStack = 1;

			ILGenerator ilGenerator = builder.GetILGenerator();

			ilGenerator.DeclareLocal(typeof (MethodBase));

			if (builder.ReturnType != typeof (void))
			{
				ilGenerator.DeclareLocal(builder.ReturnType);
				arrayPositionInStack = 2;
			}

			ilGenerator.DeclareLocal(typeof (object[]));

			ilGenerator.Emit(OpCodes.Ldtoken, method);
			ilGenerator.Emit(OpCodes.Call, typeof (MethodBase).GetMethod("GetMethodFromHandle"));

			ilGenerator.Emit(OpCodes.Stloc_0);
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldfld, handlerField);
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldloc_0);
			ilGenerator.Emit(OpCodes.Ldc_I4, parameters.Length);
			ilGenerator.Emit(OpCodes.Newarr, typeof (object));

			if (parameters.Length != 0)
			{
				ilGenerator.Emit(OpCodes.Stloc, arrayPositionInStack);
				ilGenerator.Emit(OpCodes.Ldloc, arrayPositionInStack);
			}

			for(int c = 0; c < parameters.Length; c++)
			{
				ilGenerator.Emit(OpCodes.Ldc_I4, c);
				ilGenerator.Emit(OpCodes.Ldarg, c + 1);

				if (parameters[c].IsValueType)
				{
					ilGenerator.Emit(OpCodes.Box, parameters[c].UnderlyingSystemType);
				}

				ilGenerator.Emit(OpCodes.Stelem_Ref);
				ilGenerator.Emit(OpCodes.Ldloc, arrayPositionInStack);
			}

			ilGenerator.Emit(OpCodes.Callvirt, typeof (IInvocationHandler).GetMethod("Invoke"));

			if (builder.ReturnType != typeof (void))
			{
				if (!builder.ReturnType.IsValueType)
				{
					ilGenerator.Emit(OpCodes.Castclass, builder.ReturnType);
				}
				else
				{
					ilGenerator.Emit(OpCodes.Unbox, builder.ReturnType);
					ilGenerator.Emit(ConvertTypeToOpCode(builder.ReturnType));
				}

				ilGenerator.Emit(OpCodes.Stloc, 1);

				Label label = ilGenerator.DefineLabel();
				ilGenerator.Emit(OpCodes.Br_S, label);
				ilGenerator.MarkLabel(label);
				ilGenerator.Emit(OpCodes.Ldloc, 1);
			}
			else
			{
				ilGenerator.Emit(OpCodes.Pop);
			}

			ilGenerator.Emit(OpCodes.Ret);
		}

		/// <summary>
		/// Converts a Value type to a correspondent OpCode of 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		protected virtual OpCode ConvertTypeToOpCode(Type type)
		{
			if (type.IsEnum)
			{
				Enum baseType = (Enum) Activator.CreateInstance(type);
				TypeCode code = baseType.GetTypeCode();

				switch(code)
				{
					case TypeCode.Byte:
						type = typeof (Byte);
						break;
					case TypeCode.Int16:
						type = typeof (Int16);
						break;
					case TypeCode.Int32:
						type = typeof (Int32);
						break;
					case TypeCode.Int64:
						type = typeof (Int64);
						break;
				}

				return ConvertTypeToOpCode(type);
			}

			OpCode opCode = OpCodesDictionary.Instance[ type ];

			if (Object.ReferenceEquals(opCode, OpCodesDictionary.EmptyOpCode))
			{
				throw new ArgumentException("Type " + type + " could not be converted to a OpCode");
			}

			return opCode;
		}

		public static bool NoFilterImpl(Type type, object criteria)
		{
			return true;
		}
	}
}