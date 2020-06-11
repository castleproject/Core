// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class InvalidProxyConstructorArgumentsException : ArgumentException
	{
		internal InvalidProxyConstructorArgumentsException(string message, Type proxyType, Type classToProxy) : base(message)
		{
			ClassToProxy = classToProxy;
			ProxyType = proxyType;
		}

		private protected InvalidProxyConstructorArgumentsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			// Binary serialization on .NET Core does not support `Type`,
			// so we need to make do with type name strings:
			ClassToProxy = Type.GetType(info.GetString("classToProxy"));
			ProxyType = Type.GetType(info.GetString("proxyType"));
		}

		public Type ClassToProxy { get; private set; }
		public Type ProxyType { get; private set; }

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("classToProxy", ClassToProxy.AssemblyQualifiedName);
			info.AddValue("proxyType", ProxyType.AssemblyQualifiedName);
		}
	}
}