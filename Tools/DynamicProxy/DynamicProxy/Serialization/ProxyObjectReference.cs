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
	using System.Runtime.Serialization;

	/// <summary>
	/// Summary description for ProxyObjectReference.
	/// </summary>
	[Serializable]
	public class ProxyObjectReference : IObjectReference, ISerializable
	{
		private Type m_baseType;
		private IInterceptor m_interceptor;

		protected ProxyObjectReference(SerializationInfo info, StreamingContext context)
		{
			m_interceptor = (IInterceptor) info.GetValue("interceptor", typeof(IInterceptor) );
			m_baseType = (Type) info.GetValue("baseType", typeof(Type) );
		}

		public object GetRealObject(StreamingContext context)
		{
			ProxyGenerator generator = new ProxyGenerator();
			
			return generator.CreateClassProxy( m_baseType, m_interceptor );
		}

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
		}

		#endregion
	}
}
