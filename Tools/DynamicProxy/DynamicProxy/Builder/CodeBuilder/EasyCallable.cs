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

namespace Castle.DynamicProxy.Builder.CodeBuilder
{
	using System;
	using System.Reflection;

	using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
	using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;

	/// <summary>
	/// Summary description for EasyCallable.
	/// </summary>
	public class EasyCallable : EasyNested
	{
		private ArgumentReference[] m_args;
		private ReturnReferenceExpression m_returnType;
		private EasyRuntimeMethod m_invokeMethod;
		private EasyConstructor m_constructor;
		private EasyMethod m_callmethod;

		public EasyCallable( AbstractEasyType type, 
			String name,
			ReturnReferenceExpression returnType, 
			params ArgumentReference[] args ) : base(type, name, typeof(MulticastDelegate),
			new Type[] { typeof(ICallable) }, returnType, args )
		{
			m_args = args;
			m_returnType = returnType;

			GenerateConstructor();
			GenerateInvoke();
			GenerateCallableImplementation();
		}

		private void GenerateConstructor()
		{
			ArgumentReference arg1 = new ArgumentReference( typeof(object) );
			ArgumentReference arg2 = new ArgumentReference( typeof(IntPtr) );
			m_constructor = CreateRuntimeConstructor( arg1, arg2 );
		}

		private void GenerateInvoke()
		{
			m_invokeMethod = CreateRuntimeMethod( "Invoke", m_returnType, m_args );
		}

		private void GenerateCall()
		{
			ArgumentReference arg = new ArgumentReference( typeof(object[]) );
			m_callmethod = CreateMethod( "Call", 
				new ReturnReferenceExpression(typeof(object)), arg );
			
			// LocalReference localRef = method.CodeBuilder.DeclareLocal( typeof(object) );

			Expression[] arguments = new Expression[m_args.Length];

			for (int i = 0; i < m_args.Length; i++)
			{
				ArgumentReference argument = m_args[i];
				arguments[i] = new ConvertExpression( argument.Type, 
					new LoadRefArrayElementExpression(i, arg) );
			}

			MethodInvocationExpression methodInv = new MethodInvocationExpression( 
				m_invokeMethod,
				arguments );

			if (m_returnType.Type == typeof(void))
			{
				m_callmethod.CodeBuilder.AddStatement( new ExpressionStatement(methodInv) );
				m_callmethod.CodeBuilder.AddStatement( new ReturnStatement( NullExpression.Instance ) );
			}
			else
			{
				ConvertExpression conversion = new ConvertExpression( 
					m_returnType.Type, methodInv );
				m_callmethod.CodeBuilder.AddStatement( new ReturnStatement( conversion ) );
			}
		}

		private void GenerateTargetProperty()
		{
			EasyProperty property = CreateProperty("Target", typeof(object));
			EasyMethod getMethod = property.CreateGetMethod();

			MethodInfo baseMethod = typeof(MulticastDelegate).GetMethod("get_Target");

			getMethod.CodeBuilder.AddStatement( 
				new ReturnStatement( new MethodInvocationExpression(baseMethod) ) );

			//			PropertyAttributes patts = PropertyAttributes.None;
//			PropertyBuilder pbuilder = m_delegateBuilder.DefineProperty("Target", patts, typeof(Object), null);
//			
//			MethodAttributes atts = MethodAttributes.Public|MethodAttributes.Virtual|MethodAttributes.SpecialName;
//			MethodBuilder methodBuilder = m_delegateBuilder.DefineMethod("get_Target", 
//				atts, CallingConventions.Standard, typeof(object), new Type[0]);
//
//			ILGenerator gen = methodBuilder.GetILGenerator();
//
//			gen.DeclareLocal( typeof(object) );
//			gen.Emit(OpCodes.Ldarg_0);
//			gen.Emit(OpCodes.Call, typeof(MulticastDelegate).GetMethod("get_Target"));
//			gen.Emit(OpCodes.Stloc_0);
//			gen.Emit(OpCodes.Ldloc_0);
//			gen.Emit(OpCodes.Ret);
//
//			pbuilder.SetGetMethod(methodBuilder);
		}

		private void GenerateCallableImplementation()
		{
			GenerateCall();
			GenerateTargetProperty();
		}

		public EasyMethod InvokeMethod
		{
			get { return m_invokeMethod; }
		}

		public EasyMethod Callmethod
		{
			get { return m_callmethod; }
		}

		public ConstructorInfo Constructor
		{
			get { return m_constructor.Builder; }
		}
	}
}
