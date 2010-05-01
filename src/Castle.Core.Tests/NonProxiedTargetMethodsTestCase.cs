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
	using System;

	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.Explicit;
	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class NonProxiedTargetMethodsTestCase : BasePEVerifyTestCase
	{
		private TType CreateProxy<TType>(ProxyKind kind, TType target)
		{
			var options = new ProxyGenerationOptions(new ProxyNothingHook());
			switch (kind)
			{
				case ProxyKind.WithTarget:
					return (TType)generator.CreateInterfaceProxyWithTarget(typeof(TType), Type.EmptyTypes, target, options);
				case ProxyKind.WithTargetInterface:
					return (TType)generator.CreateInterfaceProxyWithTargetInterface(typeof(TType), target, options);
			}

			Assert.Fail("Invalid proxy kind {0}", kind);
			return default(TType);
		}

		[Test]
		public void Target_method([Values(ProxyKind.WithTarget, ProxyKind.WithTargetInterface)] ProxyKind kind)
		{
			var proxy = CreateProxy<ISimpleInterface>(kind, new ClassWithInterface());
			int result = -1;
			Assert.DoesNotThrow(() => result = proxy.Do());
			Assert.AreEqual(5, result);
		}

		[Test]
		public void Target_method_explicit([Values(ProxyKind.WithTarget, ProxyKind.WithTargetInterface)] ProxyKind kind)
		{
			var proxy = CreateProxy<ISimpleInterface>(kind, new SimpleInterfaceExplicit());
			int result = -1;
			Assert.DoesNotThrow(() => result = proxy.Do());
			Assert.AreEqual(5, result);
		}

		[Test]
		public void Target_method_generic([Values(ProxyKind.WithTarget, ProxyKind.WithTargetInterface)] ProxyKind kind)
		{
			var proxy = CreateProxy<IGenericInterface>(kind, new GenericClass());
			int result = -1;
			Assert.DoesNotThrow(() => result = proxy.GenericMethod<int>());
			Assert.AreEqual(0, result);
		}

		[Test]
		public void Target_method_out_ref_parameters(
			[Values(ProxyKind.WithTarget, ProxyKind.WithTargetInterface)] ProxyKind kind)
		{
			var proxy = CreateProxy<IWithRefOut>(kind, new WithRefOut());
			int result = -1;
			Assert.DoesNotThrow(() => proxy.Do(out result));
			Assert.AreEqual(5, result);

			result = -1;
			Assert.DoesNotThrow(() => proxy.Did(ref result));
			Assert.AreEqual(5, result);
		}
	}
}