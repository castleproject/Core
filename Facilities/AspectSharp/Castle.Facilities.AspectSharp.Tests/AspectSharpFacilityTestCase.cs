// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.Facilities.AspectSharp.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.Model.Configuration;

	using Castle.Windsor;

	using Castle.MicroKernel.SubSystems.Configuration;

	using Castle.Facilities.AspectSharp.Tests.Components;
	using Castle.Facilities.AspectSharp.Tests.Interceptors;

	/// <summary>
	/// Summary description for AspectSharpFacilityTestCase.
	/// </summary>
	[TestFixture]
	public class AspectSharpFacilityTestCase 
	{
		[SetUp]
		public void Clear()
		{
			LoggerInterceptor.Messages.Length = 0;
		}

		[Test]
		public void SimpleCase()
		{
			String contents = 
				"import Castle.Facilities.AspectSharp.Tests.Components in Castle.Facilities.AspectSharp.Tests " + 
				"import Castle.Facilities.AspectSharp.Tests.Interceptors in Castle.Facilities.AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for SimpleService " + 
				"   " + 
				"   pointcut method|property(*)" + 
				"     advice(LoggerInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			MutableConfiguration config = new MutableConfiguration("facility", contents);

			DefaultConfigurationStore store = new DefaultConfigurationStore();
			store.AddFacilityConfiguration("aop", config);

			WindsorContainer container = new WindsorContainer(store);
			container.AddFacility( "aop", new AspectSharpFacility() );

			container.AddComponent("comp1", typeof(SimpleService));

			SimpleService service = container[ typeof(SimpleService) ] as SimpleService;
			service.DoSomething();
			service.DoSomethingElse();

			Assert.AreEqual( "Enter DoSomething\r\nEnter DoSomethingElse\r\n", 
				LoggerInterceptor.Messages.ToString() );
		}

		[Test]
		public void SimpleCaseWithMixin()
		{
			String contents = 
				"import Castle.Facilities.AspectSharp.Tests.Components in Castle.Facilities.AspectSharp.Tests " + 
				"import Castle.Facilities.AspectSharp.Tests.Interceptors in Castle.Facilities.AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for SimpleService " + 
				"   include SimpleMixin" + 
				"   " + 
				"   pointcut method|property(*)" + 
				"     advice(LoggerInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			MutableConfiguration config = new MutableConfiguration("facility", contents);

			DefaultConfigurationStore store = new DefaultConfigurationStore();
			store.AddFacilityConfiguration("aop", config);

			WindsorContainer container = new WindsorContainer(store);
			container.AddFacility( "aop", new AspectSharpFacility() );
			container.AddComponent("comp1", typeof(SimpleService));

			SimpleService service = container[ typeof(SimpleService) ] as SimpleService;
			service.DoSomething();
			service.DoSomethingElse();

			Assert.AreEqual( "Enter DoSomething\r\nEnter DoSomethingElse\r\n", 
				LoggerInterceptor.Messages.ToString() );

			LoggerInterceptor.Messages.Length = 0;

			ISimpleMixin mixin = service as ISimpleMixin;
			Assert.IsNotNull(mixin);

			mixin.DoStuff();

			Assert.AreEqual( "Enter DoStuff\r\n", 
				LoggerInterceptor.Messages.ToString() );
		}
	}
}
