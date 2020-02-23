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

	using ComInteropTypes.ADODB;

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

		[Test]
		public void Can_proxy_ADO_RecorsSet()
		{
			generator.CreateInterfaceProxyWithoutTarget<Recordset>();
		}

		[Test]
		[Platform(Include = "Win", Reason = "Depends on OLE32.dll due to co-class instantiation, which is likely only available on Windows.")]
		public void Can_proxy_existing_com_object()
		{
			var command = new Command();
			var parameter = command.CreateParameter("foo");
			generator.CreateInterfaceProxyWithTargetInterface(parameter);
		}
	}
}
#endif