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
	using System.Reflection;
	using System.Reflection.Emit;

	/// <summary>
	/// Summary description for DelegateBuilder.
	/// </summary>
	public class CallableDelegateBuilder
	{
		private ConstructorBuilder m_consBuilder;
		private MethodInfo m_method;
		private TypeBuilder m_typeBuilderParent;
		private TypeBuilder m_delegateBuilder;
		private MethodBuilder m_invokeMethod;
		private int m_id;

		private CallableDelegateBuilder(TypeBuilder typeBuilderParent, MethodInfo method, int id)
		{
			m_typeBuilderParent = typeBuilderParent;
			m_method = method;
			m_id = id;
		}

		public static CallableDelegateBuilder BuildForMethod( TypeBuilder parent, MethodInfo method, ModuleScope scope )
		{
			CallableDelegateBuilder builder = new CallableDelegateBuilder( parent, method, scope.InnerTypeCounter );
			return builder.Build();
		}

		protected CallableDelegateBuilder Build()
		{
			// String delegateName = String.Format("_delegate_{0}_{1}", m_typeBuilderParent.Name, m_id);
			String delegateName = String.Format("_delegate_{0}", m_id);

			TypeAttributes flags = TypeAttributes.Sealed|TypeAttributes.NestedPublic|TypeAttributes.Class;

			m_delegateBuilder = m_typeBuilderParent.DefineNestedType(delegateName, flags, 
				typeof(MulticastDelegate), new Type[] { typeof(ICallable) } );

			GenerateConstructor();
			GenerateInvoke();
			GenerateCallableImplementation();

			return this;
		}

		private void GenerateConstructor()
		{
			Type[] base_args = new Type[] { typeof (object), typeof(IntPtr) };

			m_consBuilder = m_delegateBuilder.DefineConstructor(
				MethodAttributes.SpecialName|MethodAttributes.Public|MethodAttributes.HideBySig,
				CallingConventions.Standard,
				base_args);

			m_consBuilder.SetImplementationFlags(MethodImplAttributes.Runtime|MethodImplAttributes.Managed);
		}

		public int ID
		{
			get { return m_id; }
		}

		public void CreateType()
		{
			m_delegateBuilder.CreateType();
		}

		public ConstructorBuilder Constructor
		{
			get { return m_consBuilder; }
		}

		private void GenerateInvoke()
		{
			MethodAttributes atts = MethodAttributes.HideBySig|MethodAttributes.Public|MethodAttributes.Virtual;

			ParameterInfo[] parameterInfo = m_method.GetParameters();

			Type[] parameters = new Type[parameterInfo.Length];

			for (int i = 0; i < parameterInfo.Length; i++)
			{
				parameters[i] = parameterInfo[i].ParameterType;
			}

			m_invokeMethod = m_delegateBuilder.DefineMethod("Invoke", atts, CallingConventions.Standard, m_method.ReturnType, parameters);

			m_invokeMethod.SetImplementationFlags(MethodImplAttributes.Runtime|MethodImplAttributes.Managed);
		}

		private void GenerateCallableImplementation()
		{
			MethodAttributes atts = MethodAttributes.Public|MethodAttributes.Virtual;

			ParameterInfo[] parameterInfo = m_method.GetParameters();

			MethodBuilder methodBuilder = m_delegateBuilder.DefineMethod("Call", atts, CallingConventions.Standard, typeof(object), new Type[] { typeof(object[]) });

			ILGenerator gen = methodBuilder.GetILGenerator();

			if (m_method.ReturnType != typeof(void))
			{
				gen.DeclareLocal( typeof(object) );
			}
			
			gen.Emit(OpCodes.Ldarg_0);

			for (int i = 0; i < parameterInfo.Length; i++)
			{
				ParameterInfo param = parameterInfo[i];

				gen.Emit(OpCodes.Ldarg_1);
				gen.Emit(OpCodes.Ldc_I4, i);
				gen.Emit(OpCodes.Ldelem_Ref);

				if (param.ParameterType.IsValueType)
				{
					gen.Emit(OpCodes.Unbox, param.ParameterType);
					OpCodeUtil.ConvertTypeToOpCode(gen, param.ParameterType);
				}
				else if (param.ParameterType != typeof(Object))
				{
					gen.Emit(OpCodes.Castclass, param.ParameterType);
				}
			}

			gen.Emit(OpCodes.Call, m_invokeMethod);

			if (m_method.ReturnType != typeof(void))
			{
				if (m_method.ReturnType.IsValueType)
				{
					gen.Emit(OpCodes.Box, m_method.ReturnType.UnderlyingSystemType);
				}

				gen.Emit(OpCodes.Stloc_0);

				Label label = gen.DefineLabel();
				gen.Emit(OpCodes.Br_S, label);
				gen.MarkLabel(label);
				gen.Emit(OpCodes.Ldloc_0); // Push the return value
			}
			else
			{
				gen.Emit(OpCodes.Ldnull);
			}

			gen.Emit(OpCodes.Ret);
		}
	}
}
