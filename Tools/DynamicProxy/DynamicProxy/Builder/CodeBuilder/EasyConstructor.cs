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

	using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
	using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;

	/// <summary>
	/// Summary description for EasyConstructor.
	/// </summary>
	public class EasyConstructor : IEasyMember
	{
		protected ConstructorBuilder m_builder;
		private ConstructorCodeBuilder m_codebuilder;
		private AbstractEasyType m_maintype;

		internal EasyConstructor( AbstractEasyType maintype, params ArgumentReference[] arguments )
		{
			m_maintype = maintype;

			Type[] args = ArgumentsUtil.InitializeAndConvert( arguments );
			
			m_builder = maintype.TypeBuilder.DefineConstructor( 
				 MethodAttributes.Public, CallingConventions.Standard, args );
		}

		protected internal EasyConstructor()
		{
		}

		public virtual ConstructorCodeBuilder CodeBuilder
		{
			get
			{
				if (m_codebuilder == null)
				{
					m_codebuilder = new ConstructorCodeBuilder( 
						m_maintype.BaseType, m_builder.GetILGenerator() );
				}
				return m_codebuilder;
			}
		}

		internal ConstructorBuilder Builder
		{
			get { return m_builder; }
		}

		public MethodBase Member
		{
			get { return m_builder; }
		}

		public Type ReturnType
		{
			get { return typeof(void); }
		}

		public virtual void Generate()
		{
			m_codebuilder.Generate(this, m_builder.GetILGenerator());
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
