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
	using Core.Interceptor;
	using NUnit.Framework;

	[TestFixture]
	public class InterfaceProxyWithTargetInterfaceTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void When_target_does_not_implement_additional_interface_method_should_throw()
		{
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof (IOne),
																		  new[] {typeof (ITwo)},
																		  new One(),
																		  ProxyGenerationOptions.Default,new StandardInterceptor());
			Assert.Throws<NotImplementedException>(() => (proxy as ITwo).TwoMethod());
		}

		[Test]
		public void When_target_does_implement_additional_interface_method_should_forward()
		{
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),
																		  new[] { typeof(ITwo) },
																		  new OneTwo(),
																		  ProxyGenerationOptions.Default);
			int result = (proxy as ITwo).TwoMethod();
			Assert.AreEqual(2, result);
		}

		[Test]
		public void TargetInterface_methods_should_be_forwarded_to_target()
		{
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),
																		  new[] { typeof(ITwo) },
																		  new OneTwo(),
																		  ProxyGenerationOptions.Default);
			int result = (proxy as IOne).OneMethod();
			Assert.AreEqual(3, result);
		}
	}

	public interface IOne
	{
		int OneMethod();
	}

	public class OneTwo : IOne, ITwo
	{
		public int OneMethod()
		{
			return 3;
		}

		public int TwoMethod()
		{
			return 2;
		}
	}

	public class One : IOne
	{
		public int OneMethod()
		{
			return 1;
		}
	}

	public interface ITwo
	{
		int TwoMethod();
	}
}