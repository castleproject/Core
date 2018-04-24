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
	using Castle.DynamicProxy.Tests.InterClasses;
	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class NonProxiedMixinMethodsTestCase : BasePEVerifyTestCase
	{
		private TType CreateProxyWithMixin<TType>(ProxyKind kind, params object[] mixins)
		{
			var options = new ProxyGenerationOptions(new ProxyNothingHook());
			foreach (var mixin in mixins)
			{
				options.AddMixinInstance(mixin);
			}
			switch (kind)
			{
				case ProxyKind.Class:
					return (TType) generator.CreateClassProxy(typeof (object), Type.EmptyTypes, options);
				case ProxyKind.WithoutTarget:
					return (TType) generator.CreateInterfaceProxyWithoutTarget(typeof (IEmpty), Type.EmptyTypes, options);
				case ProxyKind.WithTarget:
					return (TType) generator.CreateInterfaceProxyWithTarget(typeof (IEmpty), Type.EmptyTypes, new Empty(), options);
				case ProxyKind.WithTargetInterface:
					return (TType) generator.CreateInterfaceProxyWithTargetInterface(typeof (IEmpty), new Empty(), options);
			}

			Assert.Fail("Invalid proxy kind {0}", kind);
			return default(TType);
		}

		public static readonly object[] AllKinds = {
			new object[] { ProxyKind.Class },
			new object[] { ProxyKind.WithoutTarget },
			new object[] { ProxyKind.WithTarget },
			new object[] { ProxyKind.WithTargetInterface }
		};

		[Test]
		[TestCaseSource("AllKinds")]
		public void Mixin_method(ProxyKind kind)
		{
			var proxy = CreateProxyWithMixin<ISimpleInterface>(kind, new ClassWithInterface());
			var result = -1;
			Assert.DoesNotThrow(() => result = proxy.Do());
			Assert.AreEqual(5, result);
		}

		[Test]
		[TestCaseSource("AllKinds")]
		public void Mixin_method_explicit(ProxyKind kind)
		{
			var proxy = CreateProxyWithMixin<ISimpleInterface>(kind, new SimpleInterfaceExplicit());
			var result = -1;
			Assert.DoesNotThrow(() => result = proxy.Do());
			Assert.AreEqual(5, result);
		}

		[Test]
		[TestCaseSource("AllKinds")]
		public void Mixin_method_generic(ProxyKind kind)
		{
			var proxy = CreateProxyWithMixin<IGenericInterface>(kind, new GenericClass());
			var result = -1;
			Assert.DoesNotThrow(() => result = proxy.GenericMethod<int>());
			Assert.AreEqual(0, result);
		}

		[Test]
		[TestCaseSource("AllKinds")]
		public void Mixin_method_out_ref_parameters(ProxyKind kind)
		{
			var proxy = CreateProxyWithMixin<IWithRefOut>(kind, new WithRefOut());
			int[] result = {-1};
			Assert.DoesNotThrow(() => proxy.Did(ref result[0]));
			Assert.AreEqual(5, result[0]);

			result[0] = -1;
			Assert.DoesNotThrow(() => proxy.Do(out result[0]));
			Assert.AreEqual(5, result[0]);
		}
	}
}