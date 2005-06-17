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
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
	using Castle.DynamicProxy.Builder.CodeBuilder.Utils;

	/// <summary>
	/// Summary description for EasyMethod.
	/// </summary>
	public class EasyMethod : IEasyMember
	{
		protected MethodBuilder _builder;
		protected ArgumentReference[] _arguments;
		
		private MethodCodeBuilder _codebuilder;
		private AbstractEasyType _maintype;

		internal EasyMethod( AbstractEasyType maintype, String name, 
			ReturnReferenceExpression returnRef, params ArgumentReference[] arguments ) : 
			this(maintype, name, 
			MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public, 
			returnRef, arguments)
		{
		}

		internal EasyMethod( AbstractEasyType maintype, String name, 
			MethodAttributes attrs, 
			ReturnReferenceExpression returnRef, params ArgumentReference[] arguments )
		{
			_maintype = maintype;
			_arguments = arguments;

			Type returnType = returnRef.Type;
			Type[] args = ArgumentsUtil.InitializeAndConvert( arguments );

			_builder = maintype.TypeBuilder.DefineMethod( name,  attrs, 
				returnType, args );
		}

		protected internal EasyMethod()
		{
		}

		public virtual MethodCodeBuilder CodeBuilder
		{
			get
			{
				if (_codebuilder == null)
				{
					_codebuilder = new MethodCodeBuilder( 
						_maintype.BaseType, _builder, _builder.GetILGenerator() );
				}
				return _codebuilder;
			}
		}

		public ArgumentReference[] Arguments
		{
			get { return _arguments; }
		}

		internal MethodBuilder MethodBuilder
		{
			get { return _builder; }
		}

		public Type ReturnType
		{
			get { return _builder.ReturnType; }
		}

		public MethodBase Member
		{
			get { return _builder; }
		}

		public virtual void Generate()
		{
			_codebuilder.Generate(this, _builder.GetILGenerator());
		}

		public virtual void EnsureValidCodeBlock()
		{
			if (CodeBuilder.IsEmpty)
			{
				CodeBuilder.AddStatement( new NopStatement() );
				CodeBuilder.AddStatement( new ReturnStatement() );
			}
		}
	}
}
