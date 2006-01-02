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

namespace Castle.DynamicProxy.Test.Classes
{
	using System;
	using System.Collections;
	using System.Data;


	public class IDataReaderProxy : IInterceptor
	{
		private IDataReader _reader = null;
		private static ArrayList _passthroughMethods = new ArrayList();

		static IDataReaderProxy()
		{
			_passthroughMethods.Add("GetType");
			_passthroughMethods.Add("ToString");
		}

		internal IDataReaderProxy(IDataReader reader)
		{
			_reader = reader;
		}

		public static IDataReader NewInstance(IDataReader reader)
		{
			object proxyCommand = null;

			IInterceptor handler = new IDataReaderProxy(reader);

			ProxyGenerator proxyGenerator = new ProxyGenerator();

			proxyCommand = proxyGenerator.CreateProxy(typeof(IDataReader), handler, reader);

			return (IDataReader) proxyCommand;
		}

		public object Intercept(IInvocation invocation, params object[] arguments)
		{
			return invocation.Method.Invoke( _reader, arguments);
		}
	}
}
