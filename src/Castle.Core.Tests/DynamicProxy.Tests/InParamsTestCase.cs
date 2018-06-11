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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.DynamicProxy.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class InParamsTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void By_value_parameter_cannot_modify_argument_var()
		{
			var x = new ReadOnlyStruct(1);
			var original = new ReadOnlyStruct(x.Value);

			var different = new ReadOnlyStruct(x.Value + 100);
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IByValue>(new SetArgumentValueInterceptor(different));
			proxy.Method(x);

			Assert.AreEqual(original.Value, x.Value);
		}

		[Test]
		public void By_referece_In_parameter_cannot_modify_argument_var()
		{
			var x = new ReadOnlyStruct(1);
			var original = new ReadOnlyStruct(x.Value);

			var different = new ReadOnlyStruct(x.Value + 100);
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IByReadOnlyRef>(new SetArgumentValueInterceptor(different));
			proxy.Method(in x);

			Assert.AreEqual(original.Value, x.Value);
		}

		[Test]
		public void By_reference_Ref_parameter_can_modify_argument_var()
		{
			var x = new ReadOnlyStruct(1);
			var original = new ReadOnlyStruct(x.Value);

			var different = new ReadOnlyStruct(x.Value + 100);
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IByRef>(new SetArgumentValueInterceptor(different));
			proxy.Method(ref x);

			Assert.AreNotEqual(original.Value, x.Value);
		}

		[Test]
		public void By_reference_Out_parameter_can_modify_argument_var()
		{
			var x = new ReadOnlyStruct(1);
			var original = new ReadOnlyStruct(x.Value);

			var different = new ReadOnlyStruct(x.Value + 100);
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IOut>(new SetArgumentValueInterceptor(different));
			proxy.Method(out x);

			Assert.AreNotEqual(original.Value, x.Value);
		}

		public readonly struct ReadOnlyStruct
		{
			private readonly int value;

			public ReadOnlyStruct(int value)
			{
				this.value = value;
			}

			public int Value => this.value;
		}

		public interface IByValue
		{
			void Method(ReadOnlyStruct arg);
		}

		public interface IByReadOnlyRef
		{
			void Method(in ReadOnlyStruct arg);
		}

		public interface IByRef
		{
			void Method(ref ReadOnlyStruct arg);
		}

		public interface IOut
		{
			void Method(out ReadOnlyStruct arg);
		}

		public sealed class SetArgumentValueInterceptor : IInterceptor
		{
			private object value;

			public SetArgumentValueInterceptor(object value)
			{
				this.value = value;
			}

			public void Intercept(IInvocation invocation)
			{
				invocation.SetArgumentValue(0, value);
			}
		}
	}
}
