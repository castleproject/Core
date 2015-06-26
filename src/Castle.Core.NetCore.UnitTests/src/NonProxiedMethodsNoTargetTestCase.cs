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

	using Xunit;

	public class NonProxiedMethodsNoTargetTestCase : BasePEVerifyTestCase
	{
		private TType CreateProxy<TType>()
		{
			var options = new ProxyGenerationOptions(new ProxyNothingHook());
			return (TType)generator.CreateInterfaceProxyWithoutTarget(typeof(TType), Type.EmptyTypes, options);
		}

		private TType CreateProxyWithAdditionalInterface<TType>(ProxyKind kind)
		{
			var options = new ProxyGenerationOptions(new ProxyNothingHook());
			var interfaces = new[] { typeof(TType) };
			switch (kind)
			{
				case ProxyKind.Class:
					return (TType)generator.CreateClassProxy(typeof(object), interfaces, options);
				case ProxyKind.WithoutTarget:
					return (TType)generator.CreateInterfaceProxyWithoutTarget(typeof(IEmpty), interfaces, options);
				case ProxyKind.WithTarget:
					return (TType)generator.CreateInterfaceProxyWithTarget(typeof(IEmpty), interfaces, new Empty(), options);
				case ProxyKind.WithTargetInterface:
					return (TType)generator.CreateInterfaceProxyWithTargetInterface(typeof(IEmpty), interfaces, new Empty(), options);
			}

			Assert.True(false, string.Format("Invalid proxy kind {0}", kind));
			return default(TType);
		}

		private T CreateClassProxy<T>()
		{
			var options = new ProxyGenerationOptions(new ProxyNothingHook());
			return (T)generator.CreateClassProxy(typeof(T), Type.EmptyTypes, options);
		}

		[Fact]
		public void Abstract_method()
		{
			var proxy = CreateClassProxy<AbstractClass>();
			string result = string.Empty;
			result = proxy.Foo();
			Assert.Null(result);
		}

#if SILVERLIGHT
		[Test]
		public void AdditionalInterfaces_method()
		{
			AdditionalInterfaces_method(ProxyKind.Class);
			AdditionalInterfaces_method(ProxyKind.WithoutTarget);
			AdditionalInterfaces_method(ProxyKind.WithTarget);
			AdditionalInterfaces_method(ProxyKind.WithTargetInterface);
		}
#else
		[Theory]
		[InlineData(ProxyKind.Class)]
		[InlineData(ProxyKind.WithoutTarget)]
		[InlineData(ProxyKind.WithTarget)]
		[InlineData(ProxyKind.WithTargetInterface)]
#endif
		public void AdditionalInterfaces_method(ProxyKind kind)
		{
			var proxy = CreateProxyWithAdditionalInterface<IWithRefOut>(kind);
			int result = -1;
			proxy.Do(out result);
			Assert.Equal(0, result);

			result = -1;
			proxy.Did(ref result);
			Assert.Equal(-1, result);
		}

		[Fact]
		public void Target_method()
		{
			var proxy = CreateProxy<ISimpleInterface>();
			int result = -1;
			result = proxy.Do();
			Assert.Equal(0, result);
		}

		[Fact]
		public void Target_method_double_parameters()
		{
			var proxy = CreateProxy<IService>();
			double result = -1D;
			result = proxy.Sum(1D, 2D);
			Assert.Equal(0D, result);
		}

		[Fact]
		public void Target_method_generic_int()
		{
			var proxy = CreateProxy<IGenericInterface>();
			int result = -1;
			result = proxy.GenericMethod<int>();
			Assert.Equal(0, result);
		}

		[Fact]
		public void Target_method_generic_out_ref_parameters_int()
		{
			var proxy = CreateProxy<IGenericWithRefOut>();
			int result = -1;
			proxy.Do(out result);
			Assert.Equal(0, result);

			result = -1;
			proxy.Did(ref result);
			Assert.Equal(-1, result);
		}

		[Fact]
		public void Target_method_generic_out_ref_parameters_string()
		{
			var proxy = CreateProxy<IGenericWithRefOut>();
			string result = string.Empty;
			proxy.Do(out result);
			Assert.Null(result);

			result = string.Empty;
			proxy.Did(ref result);
			Assert.Empty(result);
		}

		[Fact]
		public void Target_method_generic_string()
		{
			var proxy = CreateProxy<IGenericInterface>();
			string result = "";
			result = proxy.GenericMethod<string>();
			Assert.Null(result);
		}

		[Fact]
		public void Target_method_IntPtr()
		{
			var proxy = CreateProxy<IFooWithIntPtr>();
			var result = new IntPtr(123);
			result = proxy.Buffer(1u);
			Assert.Equal(IntPtr.Zero, result);
		}

		[Fact]
		public void Target_method_Nullable_parameters()
		{
			var proxy = CreateProxy<INullable>();
			var result = new int?(5);
			result = proxy.Get();
			Assert.Null(result);

			result = new int?(5);
			proxy.GetOut(out result);
			Assert.Null(result);

			result = new int?(5);
			proxy.Set(result);
		}

		[Fact]
		public void Target_method_out_decimal()
		{
			var proxy = CreateProxy<IDecimalOutParam>();
			decimal result = 12M;
			proxy.Dance(out result);
			Assert.Equal(0M, result);
		}

		[Fact]
		public void Target_method_out_IntPtr()
		{
			var proxy = CreateProxy<IFooWithOutIntPtr>();
			var result = new IntPtr(123);
			proxy.Bar(out result);
			Assert.Equal(IntPtr.Zero, result);
		}

		[Fact]
		public void Target_method_out_ref_parameters()
		{
			var proxy = CreateProxy<IWithRefOut>();
			int result = -1;
			proxy.Do(out result);
			Assert.Equal(0, result);

			result = -1;
			proxy.Did(ref result);
			Assert.Equal(-1, result);
		}
	}
}