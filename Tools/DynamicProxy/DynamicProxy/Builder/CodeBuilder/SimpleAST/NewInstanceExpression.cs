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

namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;

	/// <summary>
	/// Summary description for NewInstanceExpression.
	/// </summary>
	public class NewInstanceExpression : Expression
	{
		private Type m_type;
		private Type[] m_constructor_args;
		private Expression[] m_arguments;
		private ConstructorInfo m_constructor;

		public NewInstanceExpression( EasyCallable callable, params Expression[] args ) : 
			this( callable.Constructor, args )
		{
		}

		public NewInstanceExpression( ConstructorInfo constructor, params Expression[] args )
		{
			m_constructor = constructor;
			m_arguments = args;
		}

		public NewInstanceExpression( Type target, Type[] constructor_args, params Expression[] args )
		{
			m_type = target;
			m_constructor_args = constructor_args;
			m_arguments = args;
		}

		public override void Emit(IEasyMember member, ILGenerator gen)
		{
			foreach(Expression exp in m_arguments)
			{
				exp.Emit(member, gen);
			}

			if (m_constructor == null)
			{
				m_constructor = m_type.GetConstructor( m_constructor_args );
			}

			if (m_constructor == null)
			{
				throw new ApplicationException("Could not find constructor matching specified arguments");
			}

			gen.Emit(OpCodes.Newobj, m_constructor);
		}
	}
}
