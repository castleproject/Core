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

namespace Castle.DynamicProxy.Tests
{
	using Castle.DynamicProxy.Tests.InterClasses;
	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class InterfaceProxyWithTargetInterfaceAdditionalInterfacesTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void Can_call_target__method_with_out_argument()
		{
			var target = new WithRefOutAndEmpty();

			var proxy = GetProxy<IWithRefOut>(target);

			int result;
			proxy.Do(out result);
			Assert.AreEqual(5, result);
		}

		[Test]
		public void Can_call_target__method_with_ref_argument()
		{
			var target = new WithRefOutAndEmpty();

			var proxy = GetProxy<IWithRefOut>(target);

			var result = 2;
			proxy.Did(ref result);
			Assert.AreEqual(5, result);
		}

		[Test]
		public void Can_call_target__method_with_return_type()
		{
			var target = new OneAndEmpty();

			var proxy = GetProxy<IOne>(target);

			var result = proxy.OneMethod();
			Assert.AreEqual(1, result);
		}

		[Test]
		public void Can_omit_target__method_with_return_type()
		{
			var target = new Empty();

			var proxy = GetProxy<IOne>(target);

			var result = proxy.OneMethod();
			Assert.AreEqual(0, result);
		}

		private T GetProxy<T>(object target)
		{
			return (T)generator.CreateInterfaceProxyWithTargetInterface(
				typeof(IEmpty),
				new[] { typeof(T) },
				target,
				new ProxyGenerationOptions(new ProxyNothingHook()));
		}
	}
}