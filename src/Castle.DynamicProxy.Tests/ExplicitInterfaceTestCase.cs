// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using Castle.DynamicProxy.Tests.Interceptors;

	using NUnit.Framework;

	[TestFixture]
	public class ExplicitInterfaceTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void ExplicitInterfaceMethods_AreIgnored_OnClassProxy()
		{
			LogInvocationInterceptor interceptor = new LogInvocationInterceptor();
			ClassWithExplicitInterface instance = generator.CreateClassProxy<ClassWithExplicitInterface>(interceptor);

			instance.DoVirtual();
			int result = ((ISimpleInterface) instance).Do ();
			instance.DoVirtual();

			Assert.AreEqual(2, interceptor.Invocations.Count);
			Assert.AreEqual("DoVirtual", interceptor.Invocations[0]);
			Assert.AreEqual("DoVirtual", interceptor.Invocations[1]);

			Assert.AreEqual(5, result);
		}

		[Test]
		[Ignore ("TODO - allow proxying of explicit interfaces _with_ base calls")]
		public void ExplicitInterface_AsAdditionalInterfaceToProxy_OnClassProxy_WithBaseCalls()
		{
			LogInvocationInterceptor interceptor = new LogInvocationInterceptor();
			interceptor.Proceed = false;

			ClassWithExplicitInterface instance = (ClassWithExplicitInterface) generator.CreateClassProxy (typeof (ClassWithExplicitInterface), 
				new[] { typeof(ISimpleInterface) }, interceptor);

			instance.DoVirtual();
			int result = ((ISimpleInterface) instance).Do ();
			instance.DoVirtual();

			Assert.AreEqual(3, interceptor.Invocations.Count);
			Assert.AreEqual("DoVirtual", interceptor.Invocations[0]);
			Assert.AreEqual("Do", interceptor.Invocations[1]);
			Assert.AreEqual("DoVirtual", interceptor.Invocations[2]);

			Assert.AreEqual (5, result); // indicates that original method was called
		}

		[Test]
		public void ExplicitInterface_AsAdditionalInterfaceToProxy_OnClassProxy_WithoutBaseCalls()
		{
			LogInvocationInterceptor interceptor = new LogInvocationInterceptor ();
			interceptor.Proceed = false;

			ClassWithExplicitInterface instance = (ClassWithExplicitInterface) generator.CreateClassProxy (typeof (ClassWithExplicitInterface),
				new[] { typeof (ISimpleInterface) }, interceptor);

			instance.DoVirtual ();
			int result = ((ISimpleInterface) instance).Do ();
			instance.DoVirtual ();

			Assert.AreEqual (3, interceptor.Invocations.Count);
			Assert.AreEqual ("DoVirtual", interceptor.Invocations[0]);
			Assert.AreEqual ("Do", interceptor.Invocations[1]);
			Assert.AreEqual ("DoVirtual", interceptor.Invocations[2]);

			Assert.AreEqual (0, result); // indicates that original method was not called
		}

		[Test]
		public void ExplicitInterface_properties_should_be_public_interface()
		{
			var interceptor = new LogInvocationInterceptor { Proceed = false };

			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(ISimpleInterfaceWithProperty), interceptor);
			Assert.IsNotEmpty(proxy.GetType().GetProperties());
		}

		[Test]
		public void ExplicitInterface_properties_should_be_public_class()
		{
			var interceptor = new LogInvocationInterceptor { Proceed = false };

			var proxy = generator.CreateClassProxy(typeof(ExplicitInterfaceWithPropertyImplementation), new[] { typeof(ISimpleInterfaceWithProperty) },
			                                       interceptor);
			Assert.IsNotEmpty(proxy.GetType().GetProperties());
		}
	}

	public class ExplicitInterfaceWithPropertyImplementation: ISimpleInterfaceWithProperty
	{
		public int Age
		{
			get { throw new NotImplementedException(); }
		}
	}
}