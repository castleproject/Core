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

	using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
	using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;

	/// <summary>
	/// Summary description for EasyConstructor.
	/// </summary>
	public class EasyConstructor : IEasyMember
	{
		protected ConstructorBuilder _builder;
		private ConstructorCodeBuilder _codebuilder;
		private AbstractEasyType _maintype;

		internal EasyConstructor( AbstractEasyType maintype, params ArgumentReference[] arguments )
		{
			_maintype = maintype;

			Type[] args = ArgumentsUtil.InitializeAndConvert( arguments );
			
			_builder = maintype.TypeBuilder.DefineConstructor( 
				 MethodAttributes.Public, CallingConventions.Standard, args );
		}

		protected internal EasyConstructor()
		{
		}

		public virtual ConstructorCodeBuilder CodeBuilder
		{
			get
			{
				if (_codebuilder == null)
				{
					_codebuilder = new ConstructorCodeBuilder( 
						_maintype.BaseType, _builder.GetILGenerator() );
				}
				return _codebuilder;
			}
		}

		internal ConstructorBuilder Builder
		{
			get { return _builder; }
		}

		public MethodBase Member
		{
			get { return _builder; }
		}

		public Type ReturnType
		{
			get { return typeof(void); }
		}

		public virtual void Generate()
		{
			_codebuilder.Generate(this, _builder.GetILGenerator());
		}

		public virtual void EnsureValidCodeBlock()
		{
			if (CodeBuilder.IsEmpty)
			{
				CodeBuilder.InvokeBaseConstructor();				
				CodeBuilder.AddStatement( new ReturnStatement() );
			}
		}
	}
}
