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

namespace Castle.DynamicProxy.Serialization
{
	using System;
	using System.Reflection;
	using System.Runtime.Serialization;

	/// <summary>
	/// Summary description for ProxyObjectReference.
	/// </summary>
	[Serializable]
	public class ProxyObjectReference : IObjectReference, ISerializable
	{
		private Type m_baseType;
		private Type[] m_interfaces;
		private IInterceptor m_interceptor;
		private object[] m_mixins;
		private object[] m_data;
		private bool m_delegateBaseSer;

		protected ProxyObjectReference(SerializationInfo info, StreamingContext context)
		{
			m_interceptor = (IInterceptor) info.GetValue("__interceptor", typeof(IInterceptor) );
			m_baseType = (Type) info.GetValue("__baseType", typeof(Type) );
			m_interfaces = (Type[]) info.GetValue("__interfaces", typeof(Type[]) );
			m_mixins = (object[]) info.GetValue("__mixins", typeof(object[]) );

			m_delegateBaseSer = info.GetBoolean("__delegateToBase");

			if (!m_delegateBaseSer)
			{
				m_data = (object[]) info.GetValue("__data", typeof(object[]) );
			}
		}

		public object GetRealObject(StreamingContext context)
		{
			object proxy = null;

			// TODO: ProxyGenerator.Current
			ProxyGenerator generator = new ProxyGenerator();

			if (m_delegateBaseSer)
			{
				// TODO: Invoke Serialization constructor
			}
			else
			{
				if (m_mixins.Length == 0)
				{
					proxy = generator.CreateClassProxy( m_baseType, m_interceptor );
				}
				else
				{
					GeneratorContext genContext = new GeneratorContext();
					
					foreach(object mixin in m_mixins)
					{
						genContext.AddMixinInstance(mixin);
					}
					
					proxy = generator.CreateCustomClassProxy( m_baseType, m_interceptor, genContext );
				}

				MemberInfo[] members = FormatterServices.GetSerializableMembers( m_baseType );
				FormatterServices.PopulateObjectMembers(proxy, members, m_data);
			}


			return proxy;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
		}
	}
}
