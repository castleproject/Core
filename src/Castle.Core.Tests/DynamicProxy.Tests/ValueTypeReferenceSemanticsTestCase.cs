// Copyright 2004-2018 Castle Project - http://www.castleproject.org/
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.DynamicProxy.Tests
{
	using Castle.DynamicProxy.Tests.Interceptors;

	using NUnit.Framework;

	// The purpose of this test fixture is to ensure that DynamicProxy can handle the `in` parameter modifier
	// that was introduced with C# language version 7.2.
	[TestFixture]
	public class ValueTypeReferenceSemanticsTestCase : BasePEVerifyTestCase
	{
		// This test isn't interesting by itself. It only establishes a reference "baseline" for the next test.
		[Test]
		public void Can_proxy_method_having_valuetyped_parameter_without_in_modifier()
		{
			var proxy = this.generator.CreateInterfaceProxyWithoutTarget<IWithoutInModifier>(new DoNothingInterceptor());
			var readOnlyStruct = new ReadOnlyStruct();
			proxy.Method(readOnlyStruct);
		}

#if FEATURE_CUSTOMMODIFIERS

		// ^^^
		// Because the `in` parameter modifier gets encoded as a modreq,
		// tests involving it can only ever succeed on platforms supporting them.

		[Test]
		public void Can_proxy_method_having_valuetyped_parameter_with_in_modifier()
		{
			var proxy = this.generator.CreateInterfaceProxyWithoutTarget<IWithInModifier>(new DoNothingInterceptor());
			var readOnlyStruct = new ReadOnlyStruct();
			proxy.Method(in readOnlyStruct);
		}

		[Test]
		public void Can_intercept_method_having_valuetypes_parameter_with_in_modifier()
		{
			const int expectedValue = 42;

			object receivedArg = null;
			var proxy = this.generator.CreateInterfaceProxyWithoutTarget<IWithInModifier>(
				new WithCallbackInterceptor(invocation =>
				{
					receivedArg = invocation.Arguments[0];
				}));
			var readOnlyStruct = new ReadOnlyStruct(expectedValue);

			proxy.Method(in readOnlyStruct);

			Assert.IsAssignableFrom<ReadOnlyStruct>(receivedArg);
			Assert.AreEqual(expectedValue, ((ReadOnlyStruct)receivedArg).Value);
		}

		[Test]
		[ExcludeOnFramework(Framework.NetCore | Framework.NetFramework, "Fails with a MissingMethodException due to a bug in System.Reflection.Emit. See https://github.com/dotnet/corefx/issues/29254.")]
		public void Can_proxy_method_in_generic_type_having_valuetyped_parameter_with_in_modifier()
		{
			var proxy = this.generator.CreateInterfaceProxyWithoutTarget<IGenericTypeWithInModifier<bool>>(new DoNothingInterceptor());
			var readOnlyStruct = new ReadOnlyStruct();
			proxy.Method(in readOnlyStruct);
		}

		[Test]
		[ExcludeOnFramework(Framework.NetCore | Framework.NetFramework, "Fails with a MissingMethodException due to a bug in System.Reflection.Emit. See https://github.com/dotnet/corefx/issues/29254.")]
		public void Can_proxy_generic_method_in_nongeneric_type_having_valuetyped_parameter_with_in_modifier()
		{
			var proxy = this.generator.CreateInterfaceProxyWithoutTarget<IGenericMethodWithInModifier>(new DoNothingInterceptor());
			var readOnlyStruct = new ReadOnlyStruct();
			proxy.Method<bool>(in readOnlyStruct);
		}

		[Test]
		[ExcludeOnFramework(Framework.NetCore | Framework.NetFramework, "Fails with a MissingMethodException due to a bug in System.Reflection.Emit. See https://github.com/dotnet/corefx/issues/29254.")]
		public void Can_proxy_generic_method_in_generic_type_having_valuetyped_parameter_with_in_modifier()
		{
			var proxy = this.generator.CreateInterfaceProxyWithoutTarget<IGenericTypeAndMethodWithInModifier<bool>>(new DoNothingInterceptor());
			var readOnlyStruct = new ReadOnlyStruct();
			proxy.Method<int>(in readOnlyStruct);
		}

#endif

		public readonly struct ReadOnlyStruct
		{
			public ReadOnlyStruct(int value)
			{
				this.Value = value;
			}

			public int Value { get; }
		}

		public interface IWithoutInModifier
		{
			void Method(ReadOnlyStruct readOnlyStruct);
		}

		public interface IWithInModifier
		{
			void Method(in ReadOnlyStruct readOnlyStruct);
		}

		public interface IGenericMethodWithInModifier
		{
			void Method<T>(in ReadOnlyStruct readOnlyStruct);
		}

		public interface IGenericTypeWithInModifier<T>
		{
			void Method(in ReadOnlyStruct readOnlyStruct);
		}

		public interface IGenericTypeAndMethodWithInModifier<T>
		{
			void Method<U>(in ReadOnlyStruct readOnlyStruct);
		}
	}
}
