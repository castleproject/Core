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
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Invocation;

	/// <summary>
	/// Summary description for InterfaceProxyGenerator.
	/// </summary>
	public class InterfaceProxyGenerator : BaseCodeGenerator
	{
		private static readonly Type INVOCATION_TYPE = typeof(InterfaceInvocation);

		protected FieldBuilder m_targetField;

		public InterfaceProxyGenerator(ModuleScope scope) : base(scope)
		{
		}

		public InterfaceProxyGenerator(ModuleScope scope, GeneratorContext context) : base(scope, context)
		{
		}

		protected override Type InvocationType
		{
			get { return INVOCATION_TYPE; }
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
		/// the <see cref="IInterceptor"/> instance and instantiating a HybridCollection
		/// </summary>
		protected override void GenerateConstructor()
		{
			Type[] signature = GetConstructorSignature();

			ConstructorBuilder consBuilder = MainTypeBuilder.DefineConstructor(
				MethodAttributes.Public,
				CallingConventions.Standard,
				signature);

			ILGenerator ilGenerator = consBuilder.GetILGenerator();
			
			// Calls the base constructor
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Call, ObtainAvailableConstructor(m_baseType));

			// Stores the target in the field
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldarg_2);
			ilGenerator.Emit(OpCodes.Stfld, m_targetField);

			GenerateConstructorCode(ilGenerator, OpCodes.Ldarg_2, OpCodes.Ldarg_3);

			ilGenerator.Emit(OpCodes.Ret);
		}

		protected override Type[] GetConstructorSignature()
		{
			if (Context.HasMixins)
			{
				return new Type[] { typeof(IInterceptor), typeof(object), typeof(object[]) };
			}
			else
			{
				return new Type[] { typeof(IInterceptor), typeof(object) };
			}
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
			ImplementGetObjectData();
			ImplementCacheInvocationCache();
			GenerateInterfaceImplementation( interfaces );
			GenerateConstructor();
			return CreateType();
		}

	}
}
