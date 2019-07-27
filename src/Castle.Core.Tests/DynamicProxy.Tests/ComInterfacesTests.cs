// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

#if FEATURE_TEST_COM
namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Reflection;
	using System.Runtime.InteropServices;

	using ADODB;

	using NUnit.Framework;

	[TestFixture]
	public class ComInterfacesTests:BasePEVerifyTestCase
	{
		[Test]
		public void Marshal_Release_throws_when_called_with_IntPtr_Zero()
		{
			Assert.Catch<ArgumentException>(() => Marshal.Release(IntPtr.Zero));
			//           ^^^^^^^^^^^^^^^^^
			// We're not going to be more precise because Mono and the CLR don't throw the same exception type.
			// See https://github.com/mono/mono/issues/15853.
		}

		// NOTE: You may wonder why the following tests delegate their implementation to static methods
		// in a private static class. This is because the method bodies contain references to ADODB.dll,
		// which fails to load under Mono/Linux, causing test discovery to abort early. By moving these
		// method bodies somewhere else, test discovery will succeed and we get a chance at specifying
		// how the tests should run, e.g. using the `[Platform]` attributes.

		[Test]
		[Platform(Include = "Win", Reason = "Depends on ADODB.dll, which can fail to load under Mono/Linux.")]
		public void Can_proxy_ADO_RecorsSet()
		{
			Implementation.Can_proxy_ADO_RecorsSet(generator);
		}

		[Test]
		[Platform(Include = "Win", Reason = "Depends on OLE32.dll due to co-class instantiation, which is likely only available on Windows.")]
		public void Can_proxy_existing_com_object()
		{
			Implementation.Can_proxy_existing_com_object(generator);
		}

		private static class Implementation
		{
			public static void Can_proxy_ADO_RecorsSet(ProxyGenerator generator)
			{
				generator.CreateInterfaceProxyWithoutTarget<Recordset>();
			}

			public static void Can_proxy_existing_com_object(ProxyGenerator generator)
			{
				var command = new Command();
				var parameter = ExtractActualComParameterObject(command.CreateParameter("foo"));
				generator.CreateInterfaceProxyWithTargetInterface(parameter);
			}

			private static Parameter ExtractActualComParameterObject(Parameter parameter)
			{
				var type = parameter.GetType();
				var method = type.GetMethod("GetParm", BindingFlags.Instance | BindingFlags.NonPublic);
				var actualParameter = method.Invoke(parameter, null);
				return (Parameter)actualParameter;
			}
		}
	}
}
#endif