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
	using System.Text;
	using System.Collections;
	using System.Reflection;
	using System.Reflection.Emit;

	/// <summary>
	/// Summary description for InterfaceProxyGenerator.
	/// </summary>
	public class InterfaceProxyGenerator : BaseCodeGenerator
	{
		protected FieldBuilder m_targetField;

		public InterfaceProxyGenerator(ModuleScope scope) : base(scope)
		{
		}

		public InterfaceProxyGenerator(ModuleScope scope, GeneratorContext context) : base(scope, context)
		{
		}

		protected override String GenerateTypeName(Type type, Type[] interfaces)
		{
			StringBuilder sb = new StringBuilder();
			foreach (Type inter in interfaces)
			{
				sb.Append('_');
				sb.Append(inter.Name);
			}
			/// Naive implementation
			return String.Format("ProxyInterface{0}{1}", type.Name, sb.ToString());
		}

		protected override void GenerateFields()
		{
			base.GenerateFields ();

			m_targetField = GenerateField("__target", typeof (object));
		}

		/// <summary>
		/// Generates one public constructor receiving 
		/// the <see cref="IInterceptor"/> instance and instantiating a hashtable
		/// </summary>
		/// <returns><see cref="ConstructorBuilder"/> instance</returns>
		protected override ConstructorBuilder GenerateConstructor()
		{
			ConstructorBuilder consBuilder = MainTypeBuilder.DefineConstructor(
				MethodAttributes.Public,
				CallingConventions.Standard,
				new Type[] {typeof (IInterceptor), typeof(object)});

			ILGenerator ilGenerator = consBuilder.GetILGenerator();
			
			// Calls the base constructor
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Call, ObtainAvailableConstructor(m_baseType));
			
			// Stores the interceptor in the field
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldarg_1);
			ilGenerator.Emit(OpCodes.Stfld, InterceptorFieldBuilder);

			// Stores the target in the field
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldarg_2);
			ilGenerator.Emit(OpCodes.Stfld, m_targetField);

			// Instantiates the hashtable
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Newobj, typeof(Hashtable).GetConstructor( new Type[0] ));
			ilGenerator.Emit(OpCodes.Stfld, CacheFieldBuilder);

			ilGenerator.Emit(OpCodes.Ret);

			return consBuilder;
		}

		protected override void WriteILForMethod(MethodInfo method, MethodBuilder builder, Type[] parameters)
		{
			ILGenerator gen = builder.GetILGenerator();

			gen.DeclareLocal(typeof (MethodBase));   // 0
			gen.DeclareLocal(typeof (object[]));     // 1
			gen.DeclareLocal(typeof (IInvocation));  // 2

			if (builder.ReturnType != typeof (void))
			{
				gen.DeclareLocal(builder.ReturnType);// 3
			}

			// Obtains the MethodBase from ldtoken method
			gen.Emit(OpCodes.Ldtoken, method);
			gen.Emit(OpCodes.Call, typeof (MethodBase).GetMethod("GetMethodFromHandle"));
			gen.Emit(OpCodes.Stloc_0);

			// Invokes the Method2Invocation to obtain a proper IInvocation instance
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldnull);
			gen.Emit(OpCodes.Ldloc_0);
			gen.Emit(OpCodes.Castclass, typeof (MethodInfo)); // Cast MethodBase to MethodInfo
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldfld, m_targetField);
			gen.Emit(OpCodes.Call, m_method2Invocation);
			gen.Emit(OpCodes.Stloc_2);

			gen.Emit(OpCodes.Ldc_I4, parameters.Length); // push the array size
			gen.Emit(OpCodes.Newarr, typeof (object)); // creates an array of objects
			gen.Emit(OpCodes.Stloc_1); // store the array into local field
			
			// Here we set all the arguments in the array
			for( int i=0; i < parameters.Length; i++ )
			{
				gen.Emit(OpCodes.Ldloc_1); // load the array
				gen.Emit(OpCodes.Ldc_I4, i); // set the index
				gen.Emit(OpCodes.Ldarg, i + 1); // set the value
				// box if necessary
				if (parameters[i].IsValueType)
				{
					gen.Emit(OpCodes.Box, parameters[i].UnderlyingSystemType);
				}
				gen.Emit(OpCodes.Stelem_Ref); // set the value
			}

			gen.Emit(OpCodes.Ldarg_0); // push this
			gen.Emit(OpCodes.Ldfld, InterceptorFieldBuilder); // push interceptor reference
			
			gen.Emit(OpCodes.Ldloc_2); // push the invocation
			gen.Emit(OpCodes.Ldloc_1); // push the array into stack

			gen.Emit(OpCodes.Callvirt, typeof (IInterceptor).GetMethod("Intercept"));

			if (builder.ReturnType == typeof (void))
			{
				gen.Emit(OpCodes.Pop);
			}
			else
			{
				if (!builder.ReturnType.IsValueType)
				{
					gen.Emit(OpCodes.Castclass, builder.ReturnType);
				}
				else
				{
					gen.Emit(OpCodes.Unbox, builder.ReturnType);
					OpCodeUtil.ConvertTypeToOpCode(gen, builder.ReturnType);
				}

				gen.Emit(OpCodes.Stloc, 3);

				Label label = gen.DefineLabel();
				gen.Emit(OpCodes.Br_S, label);
				gen.MarkLabel(label);
				gen.Emit(OpCodes.Ldloc, 3); // Push the return value
			}

			gen.Emit(OpCodes.Ret);
		}

		public virtual Type GenerateCode(Type[] interfaces)
		{
			interfaces = AddISerializable(interfaces);

			Type cacheType = GetFromCache(typeof(Object), interfaces);
			
			if (cacheType != null)
			{
				return cacheType;
			}

			CreateTypeBuilder( typeof(Object), interfaces );
			GenerateFields();
			GenerateConstructor();
			ImplementGetObjectData();
			ImplementCacheInvocationCache();
			GenerateInterfaceImplementation( interfaces );
			return CreateType();
		}

	}
}