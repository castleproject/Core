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
	/// Summary description for IntegrationTestCase.
	/// </summary>
	[TestFixture]
	public class IntegrationTestCase
	{
		[SetUp]
		public void Clear()
		{
			LoggerInterceptor.Messages.Length = 0;
		}

		[Test]
		public void AopInterceptorAndCastleInterceptor()
		{
			String contents = 
				"import Castle.Facilities.AspectSharp.Tests.Components in Castle.Facilities.AspectSharp.Tests " + 
				"import Castle.Facilities.AspectSharp.Tests.Interceptors in Castle.Facilities.AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for AnotherService " + 
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

			container.AddComponent( "interceptor1", typeof(CastleSimpleInterceptor) );
			container.AddComponent("comp1", typeof(SimpleService));
			container.AddComponent("comp2", typeof(IAnotherService), typeof(AnotherService));

			IAnotherService service = container[ typeof(IAnotherService) ] as IAnotherService;
			service.Name = "hammett";
			service.StartWork();

			Assert.AreEqual( "Enter set_Name\r\nEnter StartWork\r\n", 
				LoggerInterceptor.Messages.ToString() );

			CastleSimpleInterceptor interceptor = 
				container[ typeof(CastleSimpleInterceptor) ] as CastleSimpleInterceptor;

			Assert.AreEqual(2, interceptor.Executions);
		}

		[Test]
		public void AopInterceptorForPropertiesAndCastleInterceptor()
		{
			String contents = 
				"import Castle.Facilities.AspectSharp.Tests.Components in Castle.Facilities.AspectSharp.Tests " + 
				"import Castle.Facilities.AspectSharp.Tests.Interceptors in Castle.Facilities.AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for AnotherService " + 
				"   " + 
				"   pointcut property(*)" + 
				"     advice(LoggerInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			MutableConfiguration config = new MutableConfiguration("facility", contents);

			DefaultConfigurationStore store = new DefaultConfigurationStore();
			store.AddFacilityConfiguration("aop", config);

			WindsorContainer container = new WindsorContainer(store);
			container.AddFacility( "aop", new AspectSharpFacility() );

			container.AddComponent( "interceptor1", typeof(CastleSimpleInterceptor) );
			container.AddComponent("comp1", typeof(SimpleService));
			container.AddComponent("comp2", typeof(IAnotherService), typeof(AnotherService));

			IAnotherService service = container[ typeof(IAnotherService) ] as IAnotherService;
			service.Name = "hammett";
			service.StartWork();

			Assert.AreEqual( "Enter set_Name\r\n", 
				LoggerInterceptor.Messages.ToString() );

			CastleSimpleInterceptor interceptor = 
				container[ typeof(CastleSimpleInterceptor) ] as CastleSimpleInterceptor;

			Assert.AreEqual(2, interceptor.Executions);
		}

		[Test]
		public void TransientComponents()
		{
			String contents = 
				"import Castle.Facilities.AspectSharp.Tests.Components in Castle.Facilities.AspectSharp.Tests " + 
				"import Castle.Facilities.AspectSharp.Tests.Interceptors in Castle.Facilities.AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for AnotherService " + 
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

			container.AddComponent("interceptor1", typeof(CastleSimpleInterceptor));
			container.AddComponent("comp1", typeof(SimpleService));
			container.AddComponent("comp2", typeof(IAnotherService), typeof(AnotherService));

			for(int i=1; i < 10000; i++)
			{
				LoggerInterceptor.Messages.Length = 0;

				IAnotherService service = container[ typeof(IAnotherService) ] as IAnotherService;
				service.Name = "hammett";
				service.StartWork();

				Assert.AreEqual( "Enter set_Name\r\nEnter StartWork\r\n", 
					LoggerInterceptor.Messages.ToString() );

				CastleSimpleInterceptor interceptor = 
					container[ typeof(CastleSimpleInterceptor) ] as CastleSimpleInterceptor;

				Assert.AreEqual(2 * i, interceptor.Executions);
			}
		}
	}
}
