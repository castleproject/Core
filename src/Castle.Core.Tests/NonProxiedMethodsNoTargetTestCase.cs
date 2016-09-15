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

	using Castle.DynamicProxy.Tests.InterClasses;
	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class NonProxiedMethodsNoTargetTestCase : BasePEVerifyTestCase
	{
		private TType CreateProxy<TType>()
		{
			var options = new ProxyGenerationOptions(new ProxyNothingHook());
			return (TType) generator.CreateInterfaceProxyWithoutTarget(typeof (TType), Type.EmptyTypes, options);
		}

		private TType CreateProxyWithAdditionalInterface<TType>(ProxyKind kind)
		{
			var options = new ProxyGenerationOptions(new ProxyNothingHook());
			var interfaces = new[] {typeof (TType)};
			switch (kind)
			{
				case ProxyKind.Class:
					return (TType) generator.CreateClassProxy(typeof (object), interfaces, options);
				case ProxyKind.WithoutTarget:
					return (TType) generator.CreateInterfaceProxyWithoutTarget(typeof (IEmpty), interfaces, options);
				case ProxyKind.WithTarget:
					return (TType) generator.CreateInterfaceProxyWithTarget(typeof (IEmpty), interfaces, new Empty(), options);
				case ProxyKind.WithTargetInterface:
					return (TType) generator.CreateInterfaceProxyWithTargetInterface(typeof (IEmpty), interfaces, new Empty(), options);
			}

			Assert.Fail("Invalid proxy kind {0}", kind);
			return default(TType);
		}

		private T CreateClassProxy<T>()
		{
			var options = new ProxyGenerationOptions(new ProxyNothingHook());
			return (T) generator.CreateClassProxy(typeof (T), Type.EmptyTypes, options);
		}

		public static readonly object[] AllKinds = {
			new object[] { ProxyKind.Class },
			new object[] { ProxyKind.WithoutTarget },
			new object[] { ProxyKind.WithTarget },
			new object[] { ProxyKind.WithTargetInterface }
		};

		[Test]
		public void Abstract_method()
		{
			var proxy = CreateClassProxy<AbstractClass>();
			string result = string.Empty;
			Assert.DoesNotThrow(() => result = proxy.Foo());
			Assert.IsNull(result);
		}

		[Test]
		[TestCaseSource("AllKinds")]
		public void AdditionalInterfaces_method(ProxyKind kind)
		{
			var proxy = CreateProxyWithAdditionalInterface<IWithRefOut>(kind);
			int result = -1;
			Assert.DoesNotThrow(() => proxy.Do(out result));
			Assert.AreEqual(0, result);

			result = -1;
			Assert.DoesNotThrow(() => proxy.Did(ref result));
			Assert.AreEqual(-1, result);
		}

		[Test]
		public void Target_method()
		{
			var proxy = CreateProxy<ISimpleInterface>();
			int result = -1;
			Assert.DoesNotThrow(() => result = proxy.Do());
			Assert.AreEqual(0, result);
		}

		[Test]
		public void Target_method_double_parameters()
		{
			var proxy = CreateProxy<IService>();
			double result = -1D;
			Assert.DoesNotThrow(() => result = proxy.Sum(1D, 2D));
			Assert.AreEqual(0D, result);
		}

		[Test]
		public void Target_method_generic_int()
		{
			var proxy = CreateProxy<IGenericInterface>();
			int result = -1;
			Assert.DoesNotThrow(() =>
			                    result = proxy.GenericMethod<int>());
			Assert.AreEqual(0, result);
		}

		[Test]
		public void Target_method_generic_out_ref_parameters_int()
		{
			var proxy = CreateProxy<IGenericWithRefOut>();
			int result = -1;
			Assert.DoesNotThrow(() => proxy.Do(out result));
			Assert.AreEqual(0, result);

			result = -1;
			Assert.DoesNotThrow(() => proxy.Did(ref result));
			Assert.AreEqual(-1, result);
		}

		[Test]
		public void Target_method_generic_out_ref_parameters_string()
		{
			var proxy = CreateProxy<IGenericWithRefOut>();
			string result = string.Empty;
			Assert.DoesNotThrow(() => proxy.Do(out result));
			Assert.IsNull(result);

			result = string.Empty;
			Assert.DoesNotThrow(() => proxy.Did(ref result));
			Assert.IsEmpty(result);
		}

		[Test]
		public void Target_method_generic_string()
		{
			var proxy = CreateProxy<IGenericInterface>();
			string result = "";
			Assert.DoesNotThrow(() => result = proxy.GenericMethod<string>());
			Assert.IsNull(result);
		}

		[Test]
		public void Target_method_IntPtr()
		{
			var proxy = CreateProxy<IFooWithIntPtr>();
			var result = new IntPtr(123);
			Assert.DoesNotThrow(() => result = proxy.Buffer(1u));
			Assert.AreEqual(IntPtr.Zero, result);
		}

		[Test]
		public void Target_method_Nullable_parameters()
		{
			var proxy = CreateProxy<INullable>();
			var result = new int?(5);
			Assert.DoesNotThrow(() => result = proxy.Get());
			Assert.IsNull(result);

			result = new int?(5);
			Assert.DoesNotThrow(() => proxy.GetOut(out result));
			Assert.IsNull(result);

			result = new int?(5);
			Assert.DoesNotThrow(() => proxy.Set(result));
		}

		[Test]
		public void Target_method_out_decimal()
		{
			var proxy = CreateProxy<IDecimalOutParam>();
			decimal result = 12M;
			Assert.DoesNotThrow(() => proxy.Dance(out result));
			Assert.AreEqual(0M, result);
		}

		[Test]
		public void Target_method_out_IntPtr()
		{
			var proxy = CreateProxy<IFooWithOutIntPtr>();
			var result = new IntPtr(123);
			Assert.DoesNotThrow(() => proxy.Bar(out result));
			Assert.AreEqual(IntPtr.Zero, result);
		}

		[Test]
		public void Target_method_out_ref_parameters()
		{
			var proxy = CreateProxy<IWithRefOut>();
			int result = -1;
			Assert.DoesNotThrow(() => proxy.Do(out result));
			Assert.AreEqual(0, result);

			result = -1;
			Assert.DoesNotThrow(() => proxy.Did(ref result));
			Assert.AreEqual(-1, result);
		}
	}
}