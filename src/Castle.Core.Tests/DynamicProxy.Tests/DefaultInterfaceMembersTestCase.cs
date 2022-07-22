// Copyright 2004-2022 Castle Project - http://www.castleproject.org/
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

#if NETCOREAPP3_0_OR_GREATER

using System.Reflection;

using NUnit.Framework;

namespace Castle.DynamicProxy.Tests
{
	[TestFixture]
	public class DefaultInterfaceMembersTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void Can_proxy_interface_with_sealed_method()
		{
			_ = generator.CreateInterfaceProxyWithoutTarget<IHaveSealedMethod>();
		}

		[Test]
		public void Can_invoke_sealed_method_in_proxied_interface()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHaveSealedMethod>();
			var invokedMethod = proxy.SealedMethod();
			Assert.AreEqual(typeof(IHaveSealedMethod).GetMethod(nameof(IHaveSealedMethod.SealedMethod)), invokedMethod);
		}

		public interface IHaveSealedMethod
		{
			sealed MethodBase SealedMethod()
			{
				return MethodBase.GetCurrentMethod();
			}
		}
	}
}

#endif
