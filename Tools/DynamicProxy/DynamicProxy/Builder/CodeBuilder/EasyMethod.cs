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
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
	using Castle.DynamicProxy.Builder.CodeBuilder.Utils;

	/// <summary>
	/// Summary description for EasyMethod.
	/// </summary>
	public class EasyMethod : IEasyMember
	{
		protected MethodBuilder m_builder;
		protected ArgumentReference[] m_arguments;
		
		private MethodCodeBuilder m_codebuilder;
		private AbstractEasyType m_maintype;

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
			m_maintype = maintype;
			m_arguments = arguments;

			Type returnType = returnRef.Type;
			Type[] args = ArgumentsUtil.InitializeAndConvert( arguments );

			m_builder = maintype.TypeBuilder.DefineMethod( name,  attrs, 
				returnType, args );
		}

		protected internal EasyMethod()
		{
		}

		public virtual MethodCodeBuilder CodeBuilder
		{
			get
			{
				if (m_codebuilder == null)
				{
					m_codebuilder = new MethodCodeBuilder( 
						m_maintype.BaseType, m_builder, m_builder.GetILGenerator() );
				}
				return m_codebuilder;
			}
		}

		public ArgumentReference[] Arguments
		{
			get { return m_arguments; }
		}

		internal MethodBuilder MethodBuilder
		{
			get { return m_builder; }
		}

		public Type ReturnType
		{
			get { return m_builder.ReturnType; }
		}

		public MethodBase Member
		{
			get { return m_builder; }
		}

		public virtual void Generate()
		{
			m_codebuilder.Generate(this, m_builder.GetILGenerator());
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
