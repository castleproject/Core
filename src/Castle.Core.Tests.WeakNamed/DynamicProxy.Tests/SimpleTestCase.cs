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
	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	// The tests in the following fixture are trying to establish that proxying works...
	//  * for types exclusively defined in a weak-named assembly.
	//  * in a "mixed" scenario involving types from both a weak-named and strong-named assembly.

	[TestFixture]
	public class SimpleTestCase : BasePEVerifyTestCase
	{
		private IInterceptor interceptor;

		[SetUp]
		public void Prepare_interceptor_stub()
		{
			interceptor = new DoNothingInterceptor();
		}

		[Test]
		public void Can_proxy_empty_interface()
		{
			generator.CreateInterfaceProxyWithoutTarget<IEmptyInterface>(interceptor);
		}

		[Test]
		public void Can_proxy_empty_interface_with_target()
		{
			generator.CreateInterfaceProxyWithTarget<IEmptyInterface>(new EmptyTarget(), interceptor);
		}

		[Test]
		public void Can_proxy_empty_class()
		{
			generator.CreateClassProxy<EmptyClass>(interceptor);
		}

		[Test]
		public void Can_proxy_empty_class_with_target()
		{
			generator.CreateClassProxyWithTarget<EmptyClass>(new EmptyTarget(), interceptor);
		}

		[Test]
		public void Can_proxy_single_method_interface()
		{
			generator.CreateInterfaceProxyWithoutTarget<ISingleMethodInterface>(interceptor);
		}

		[Test]
		public void Can_proxy_single_method_interface_with_target()
		{
			generator.CreateInterfaceProxyWithTarget<ISingleMethodInterface>(new SingleMethodTarget(), interceptor);
		}

		[Test]
		public void Can_proxy_single_method_class()
		{
			generator.CreateClassProxy<SingleMethodClass>(interceptor);
		}

		[Test]
		public void Can_proxy_single_method_class_with_target()
		{
			generator.CreateClassProxyWithTarget<SingleMethodClass>(new SingleMethodTarget(), interceptor);
		}

		[Test]
		public void Can_proxy_single_method_class_from_strongnamed_assembly()
		{
			generator.CreateClassProxy<AbstractClassWithMethod>(interceptor);
		}

		[Test]
		public void Can_proxy_single_method_class_from_strongnamed_assembly_and_additional_interface_from_weaknamed_assembly()
		{
			generator.CreateClassProxy(typeof(AbstractClassWithMethod), new[] { typeof(ISingleMethodInterface) }, interceptor);
		}

		[Test]
		public void Can_proxy_single_method_class_from_weaknamed_assembly_and_additional_interface_from_strongnamed_assembly()
		{
			generator.CreateClassProxy(typeof(SingleMethodClass), new[] { typeof(IBase) }, interceptor);
		}

		public interface IEmptyInterface
		{
		}

		public interface ISingleMethodInterface
		{
			void Method();
		}

		public abstract class EmptyClass
		{
		}

		public sealed class EmptyTarget : EmptyClass, IEmptyInterface
		{
		}

		public abstract class SingleMethodClass
		{
			public abstract void Method();
		}

		public sealed class SingleMethodTarget : SingleMethodClass, ISingleMethodInterface
		{
			public override void Method()
			{
			}
		}
	}
}
