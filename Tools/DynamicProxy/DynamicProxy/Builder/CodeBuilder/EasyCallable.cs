// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
		private ArgumentReference[] _args;
		private ReturnReferenceExpression _returnType;
		private EasyRuntimeMethod _invokeMethod;
		private EasyConstructor _constructor;
		private EasyMethod _callmethod;
		private int _id;

		public EasyCallable( AbstractEasyType type, 
			int id,
			ReturnReferenceExpression returnType, 
			params ArgumentReference[] args ) : 
				base(type, 
					 String.Format("__delegate_{0}", id), 
					 typeof(MulticastDelegate),
					 new Type[] { typeof(ICallable) }, returnType, args )
		{
			_id = id;
			_args = args;
			_returnType = returnType;

			GenerateConstructor();
			GenerateInvoke();
			GenerateCallableImplementation();
		}

		private void GenerateConstructor()
		{
			ArgumentReference arg1 = new ArgumentReference( typeof(object) );
			ArgumentReference arg2 = new ArgumentReference( typeof(IntPtr) );
			_constructor = CreateRuntimeConstructor( arg1, arg2 );
		}

		private void GenerateInvoke()
		{
			_invokeMethod = CreateRuntimeMethod( "Invoke", _returnType, _args );
		}

		private void GenerateCall()
		{
			ArgumentReference arg = new ArgumentReference( typeof(object[]) );
			_callmethod = CreateMethod( "Call", 
				new ReturnReferenceExpression(typeof(object)), arg );
			
			// LocalReference localRef = method.CodeBuilder.DeclareLocal( typeof(object) );

			Expression[] arguments = new Expression[_args.Length];

			for (int i = 0; i < _args.Length; i++)
			{
				ArgumentReference argument = _args[i];
				arguments[i] = new ConvertExpression( argument.Type, 
					new LoadRefArrayElementExpression(i, arg) );
			}

			MethodInvocationExpression methodInv = new MethodInvocationExpression( 
				_invokeMethod,
				arguments );

			if (_returnType.Type == typeof(void))
			{
				_callmethod.CodeBuilder.AddStatement( new ExpressionStatement(methodInv) );
				_callmethod.CodeBuilder.AddStatement( new ReturnStatement( NullExpression.Instance ) );
			}
			else
			{
				ConvertExpression conversion = new ConvertExpression( 
					typeof(object), _returnType.Type, methodInv );
				_callmethod.CodeBuilder.AddStatement( new ReturnStatement( conversion ) );
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
//			PropertyBuilder pbuilder = _delegateBuilder.DefineProperty("Target", patts, typeof(Object), null);
//			
//			MethodAttributes atts = MethodAttributes.Public|MethodAttributes.Virtual|MethodAttributes.SpecialName;
//			MethodBuilder methodBuilder = _delegateBuilder.DefineMethod("get_Target", 
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

		public int ID
		{
			get { return _id; }
		}

		public EasyMethod InvokeMethod
		{
			get { return _invokeMethod; }
		}

		public EasyMethod Callmethod
		{
			get { return _callmethod; }
		}

		public ConstructorInfo Constructor
		{
			get { return _constructor.Builder; }
		}
	}
}
