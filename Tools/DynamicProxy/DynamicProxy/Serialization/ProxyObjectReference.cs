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
	/// Handles the deserialization of proxies.
	/// </summary>
	[Serializable]
	public class ProxyObjectReference : IObjectReference, ISerializable
	{
		private Type m_baseType;
		private Type[] m_interfaces;
		private IInterceptor m_interceptor;
		private object[] m_mixins;
		private object[] m_data;
		private object m_proxy;

		protected ProxyObjectReference(SerializationInfo info, StreamingContext context)
		{
			m_interceptor = (IInterceptor) info.GetValue("__interceptor", typeof(IInterceptor) );
			m_baseType = (Type) info.GetValue("__baseType", typeof(Type) );
			m_interfaces = (Type[]) info.GetValue("__interfaces", typeof(Type[]) );
			m_mixins = (object[]) info.GetValue("__mixins", typeof(object[]) );

			m_proxy = RecreateProxy(info, context);

			InvokeCallback(m_proxy);
		}

		protected virtual object RecreateProxy(SerializationInfo info, StreamingContext context)
		{
			if (m_baseType == typeof(object))
			{
				return RecreateInterfaceProxy(info, context);
			}
			else
			{
				return RecreateClassProxy(info, context);
			}
		}

		public object RecreateInterfaceProxy(SerializationInfo info, StreamingContext context)
		{
			object proxy = null;

			// TODO: ProxyGenerator.Current
			ProxyGenerator generator = new ProxyGenerator();

			object target = info.GetValue("__target", typeof(object));

			if (m_mixins.Length == 0)
			{
				proxy = generator.CreateProxy( m_interfaces, m_interceptor, target);
			}
			else
			{
				GeneratorContext genContext = new GeneratorContext();
				
				foreach(object mixin in m_mixins)
				{
					genContext.AddMixinInstance(mixin);
				}
				
				proxy = generator.CreateCustomProxy( m_interfaces, m_interceptor, target, genContext );
			}

			return proxy;
		}

		public object RecreateClassProxy(SerializationInfo info, StreamingContext context)
		{
			bool delegateBaseSer = info.GetBoolean("__delegateToBase");

			if (!delegateBaseSer)
			{
				m_data = (object[]) info.GetValue("__data", typeof(object[]) );
			}

			object proxy = null;

			// TODO: ProxyGenerator.Current
			ProxyGenerator generator = new ProxyGenerator();

			if (delegateBaseSer)
			{
				if (m_mixins.Length == 0)
				{
					Type proxy_type = generator.ProxyBuilder.CreateClassProxy( m_baseType );
					proxy = Activator.CreateInstance( proxy_type, new object[] { info, context } );
				} 
				else
				{
					GeneratorContext genContext = new GeneratorContext();
					
					foreach(object mixin in m_mixins)
					{
						genContext.AddMixinInstance(mixin);
					}
					
					Type proxy_type = generator.ProxyBuilder.CreateCustomClassProxy( 
						m_baseType, genContext );
					proxy = Activator.CreateInstance( proxy_type, new object[] { info, context } );
				}
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

		protected void InvokeCallback(object target)
		{
			if (target is IDeserializationCallback)
			{
				(target as IDeserializationCallback).OnDeserialization(this);
			}
		}

		public object GetRealObject(StreamingContext context)
		{
			return m_proxy;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			// There is no need to implement this method as 
			// this class would never be serialized.
		}
	}
}
