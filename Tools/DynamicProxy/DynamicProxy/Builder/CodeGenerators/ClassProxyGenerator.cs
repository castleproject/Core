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
	/// Summary description for ClassProxyGenerator.
	/// </summary>
	public class ClassProxyGenerator : BaseCodeGenerator
	{
		private static readonly Type INVOCATION_TYPE = typeof(SameClassInvocation);

		public ClassProxyGenerator(ModuleScope scope) : base(scope)
		{
		}

		public ClassProxyGenerator(ModuleScope scope, GeneratorContext context) : base(scope, context)
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
			return String.Format("CProxyType{0}{1}", type.Name, sb.ToString());
		}

		/// <summary>
		/// Generates one public constructor receiving 
		/// the <see cref="IInterceptor"/> instance and instantiating a hashtable
		/// </summary>
		protected override void GenerateConstructor()
		{
			ConstructorBuilder consBuilder = MainTypeBuilder.DefineConstructor(
				MethodAttributes.Public,
				CallingConventions.Standard,
				new Type[] {typeof (IInterceptor)});

			ILGenerator ilGenerator = consBuilder.GetILGenerator();
			
			// Calls the base constructor
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Call, ObtainAvailableConstructor(m_baseType));

			GenerateConstructorCode(ilGenerator, OpCodes.Ldarg_0);

			ilGenerator.Emit(OpCodes.Ret);
		}

		public Type GenerateCode(Type baseClass)
		{
			Type[] interfaces = new Type[0];
			interfaces = AddISerializable(interfaces);

			Type cacheType = GetFromCache(baseClass, interfaces);
			
			if (cacheType != null)
			{
				return cacheType;
			}

			CreateTypeBuilder( baseClass, interfaces );
			GenerateFields();
			ImplementGetObjectData();
			ImplementCacheInvocationCache();
			GenerateTypeImplementation( baseClass, true );
			GenerateConstructor();
			return CreateType();
		}

		protected override MethodInfo GenerateMethodActualInvoke(MethodInfo method)
		{
			MethodAttributes atts = MethodAttributes.Private;

			ParameterInfo[] parameterInfo = method.GetParameters();

			Type[] parameters = new Type[parameterInfo.Length];

			for (int i = 0; i < parameterInfo.Length; i++)
			{
				parameters[i] = parameterInfo[i].ParameterType;
			}

			String name = String.Format("callback__{0}", method.Name);

			MethodBuilder baseMethodCall = MainTypeBuilder.DefineMethod(name, atts, method.ReturnType, parameters);

			ILGenerator gen = baseMethodCall.GetILGenerator();

			if (method.ReturnType != typeof(void))
			{
				gen.DeclareLocal( method.ReturnType );
			}
			
			gen.Emit(OpCodes.Ldarg_0);

			for (int i = 0; i < parameterInfo.Length; i++)
			{
				gen.Emit(OpCodes.Ldarg, i + 1);
			}

			gen.Emit(OpCodes.Call, method);

			if (method.ReturnType != typeof(void))
			{
				gen.Emit(OpCodes.Stloc_0);
				Label retBranch = gen.DefineLabel();
				gen.Emit(OpCodes.Br_S, retBranch);
				gen.MarkLabel(retBranch);
				gen.Emit(OpCodes.Ldloc_0);
			}

			gen.Emit(OpCodes.Ret);

			return baseMethodCall;
		}
	}
}
