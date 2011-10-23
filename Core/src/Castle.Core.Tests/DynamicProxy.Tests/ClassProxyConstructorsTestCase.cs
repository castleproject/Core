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
	using System.Collections.Generic;

	using Castle.DynamicProxy.Tests.Classes;

	using NUnit.Framework;

	[TestFixture]
	public class ClassProxyConstructorsTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void ShouldGenerateTypeWithDuplicatedBaseInterfacesClassProxy()
		{
			generator.CreateClassProxy(
				typeof(MyOwnClass),
				new Type[] { },
				ProxyGenerationOptions.Default,
				new object[] { },
				new StandardInterceptor());
		}

		[Test]
		[Ignore("I don't see any simple way of doing this...")]
		public void Should_properly_interpret_array_of_objects()
		{
			var proxy =
				(ClassWithVariousConstructors)
				generator.CreateClassProxy(typeof(ClassWithVariousConstructors), new object[] { new object[] { null } });
			Assert.AreEqual(Constructor.ArrayOfObjects, proxy.ConstructorCalled);
		}

		[Test]
		public void Should_properly_interpret_array_of_objects_and_string()
		{
			var proxy =
				(ClassWithVariousConstructors)
				generator.CreateClassProxy(typeof(ClassWithVariousConstructors), new object[] { new object[] { null }, "foo" });
			Assert.AreEqual(Constructor.ArrayOfObjectsAndSingleString, proxy.ConstructorCalled);
		}

		[Test]
		public void Should_properly_interpret_array_of_strings_and_string()
		{
			var proxy =
				(ClassWithVariousConstructors)
				generator.CreateClassProxy(typeof(ClassWithVariousConstructors), new object[] { new string[] { null }, "foo" });
			Assert.AreEqual(Constructor.ArrayAndSingleString, proxy.ConstructorCalled);
		}

		[Test]
		public void Should_properly_interpret_empty_array_as_ctor_argument()
		{
			var proxy =
				(ClassWithVariousConstructors)
				generator.CreateClassProxy(typeof(ClassWithVariousConstructors), new object[] { new string[] { } });
			Assert.AreEqual(Constructor.ArrayOfStrings, proxy.ConstructorCalled);
		}

		[Test]
		public void Can_pass_params_arguments_inline()
		{
				generator.CreateClassProxy(typeof(HasCtorWithParamsStrings), new object[] { });
		}
		[Test]
		public void Can_pass_params_arguments_inline2()
		{
			generator.CreateClassProxy(typeof(HasCtorWithParamsArgument), new object[] { });
		}

		[Test]
		public void Can_pass_params_arguments_inline_implicitly()
		{
			generator.CreateClassProxy(typeof(HasCtorWithIntAndParamsArgument), new object[] { 5 });
		}

		[Test]
		public void Should_properly_interpret_nothing_as_lack_of_ctor_arguments()
		{
			var proxy =
				(ClassWithVariousConstructors)generator.CreateClassProxy(typeof(ClassWithVariousConstructors), new IInterceptor[0]);
			Assert.AreEqual(Constructor.Default, proxy.ConstructorCalled);
		}

		[Test]
		[Ignore("I don't see any simple way of doing this...")]
		public void Should_properly_interpret_null_as_ctor_argument()
		{
			var proxy =
				(ClassWithVariousConstructors)
				generator.CreateClassProxy(typeof(ClassWithVariousConstructors), new[] { default(object) });
			Assert.AreEqual(Constructor.Object, proxy.ConstructorCalled);
		}
	}

	public abstract class MyOwnClass
	{
		public virtual void Foo<T>(List<T>[] action)
		{
		}

		/* ... */
	}
}