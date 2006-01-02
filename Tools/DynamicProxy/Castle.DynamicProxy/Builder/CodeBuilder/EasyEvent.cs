// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	/// Summary description for EasyEvent.
	/// </summary>
	[CLSCompliant(false)]
	public class EasyEvent : IEasyMember
	{
		private EventBuilder m_builder;
		private AbstractEasyType m_maintype;
		private EasyMethod m_addOnMethod;
		private EasyMethod m_removeOnMethod;
		private string m_name;

		public EasyEvent(AbstractEasyType maintype, String name, Type eventHandlerType)
		{
			m_name = name;
			m_maintype = maintype;
			m_builder = maintype.TypeBuilder.DefineEvent(
				name, EventAttributes.None, eventHandlerType);
		}

		public String Name
		{
			get { return m_name; }
		}

		public EasyMethod CreateAddOnMethod(MethodAttributes atts, params Type[] parameters)
		{
			if (m_addOnMethod == null)
			{
				m_addOnMethod =
					new EasyMethod(m_maintype, "add_" + Name, atts, new ReturnReferenceExpression(typeof(void)), ArgumentsUtil.ConvertToArgumentReference(parameters));
			}

			return m_addOnMethod;
		}

		public EasyMethod CreateRemoveOnMethod(MethodAttributes atts, params Type[] parameters)
		{
			if (m_removeOnMethod == null)
			{
				m_removeOnMethod =
					new EasyMethod(m_maintype, "remove_" + Name, atts, new ReturnReferenceExpression(typeof(void)), ArgumentsUtil.ConvertToArgumentReference(parameters));
			}

			return m_removeOnMethod;
		}

		#region IEasyMember Members

		public void Generate()
		{
			if (m_addOnMethod != null)
			{
				m_addOnMethod.Generate();
				m_builder.SetAddOnMethod(m_addOnMethod.MethodBuilder);
			}

			if (m_removeOnMethod != null)
			{
				m_removeOnMethod.Generate();
				m_builder.SetRemoveOnMethod(m_removeOnMethod.MethodBuilder);
			}
		}

		public void EnsureValidCodeBlock()
		{
			if (m_addOnMethod != null)
			{
				m_addOnMethod.EnsureValidCodeBlock();
			}

			if (m_removeOnMethod != null)
			{
				m_removeOnMethod.EnsureValidCodeBlock();
			}
		}

		public MethodBase Member
		{
			get { return null; }
		}

		public Type ReturnType
		{
			get { throw new Exception("TBD"); }
		}

		#endregion
	}
}