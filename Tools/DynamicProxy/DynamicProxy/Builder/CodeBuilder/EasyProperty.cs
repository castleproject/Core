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

	/// <summary>
	/// Summary description for EasyProperty.
	/// </summary>
	public class EasyProperty : IEasyMember
	{
		private PropertyBuilder m_builder;
		private AbstractEasyType m_maintype;
		private EasyMethod m_getMethod;
		private EasyMethod m_setMethod;

		public EasyProperty( AbstractEasyType maintype, String name, Type returnType )
		{
			m_maintype = maintype;
			m_builder = maintype.TypeBuilder.DefineProperty(
				name, PropertyAttributes.None, returnType, new Type[0]);
		}

		public String Name
		{
			get { return m_builder.Name; }
		}

		public EasyMethod CreateGetMethod()
		{
			if (m_getMethod != null)
			{
				return m_getMethod;
			}

			m_getMethod = new EasyMethod(m_maintype, "get_" + m_builder.Name, 
				MethodAttributes.Public|MethodAttributes.Virtual|MethodAttributes.SpecialName,
				new ReturnReferenceExpression(ReturnType));
			
			return m_getMethod;
		}

		public EasyMethod CreateSetMethod(ArgumentReference arg)
		{
			if (m_setMethod != null)
			{
				return m_setMethod;
			}

			m_setMethod = new EasyMethod(m_maintype, "set_" + m_builder.Name, 
				MethodAttributes.Public|MethodAttributes.Virtual|MethodAttributes.SpecialName,
				new ReturnReferenceExpression( typeof(void) ), arg);
			
			return m_setMethod;
		}

		#region IEasyMember Members

		public void Generate()
		{
			if (m_setMethod != null)
			{
				m_setMethod.Generate();
				m_builder.SetSetMethod(m_setMethod.MethodBuilder);
			}
			
			if (m_getMethod != null)
			{
				m_getMethod.Generate();
				m_builder.SetGetMethod(m_getMethod.MethodBuilder);
			}
		}

		public void EnsureValidCodeBlock()
		{
			if (m_setMethod != null)
			{
				m_setMethod.EnsureValidCodeBlock();
			}
			
			if (m_getMethod != null)
			{
				m_getMethod.EnsureValidCodeBlock();
			}
		}

		public MethodBase Member
		{
			get
			{
				return null;
			}
		}

		public Type ReturnType
		{
			get
			{
				return m_builder.PropertyType;
			}
		}

		#endregion
	}
}
