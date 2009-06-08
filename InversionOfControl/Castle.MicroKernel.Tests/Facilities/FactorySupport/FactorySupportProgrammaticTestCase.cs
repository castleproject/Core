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

namespace Castle.Facilities.FactorySupport.Tests
{
	using System;
	using System.Collections.Generic;
	using Castle.MicroKernel;
	using Core.Configuration;
	using MicroKernel.Registration;
	using NUnit.Framework;

	[TestFixture]
	public class FactorySupportProgrammaticTestCase
	{
		private IKernel kernel;
		private FactorySupportFacility facility;

		[SetUp]
		public void Init()
		{
			facility = new FactorySupportFacility();
			kernel = new DefaultKernel();
			kernel.AddFacility("factory.support", facility);
		}

		[Test]
		public void FactoryResolveWithProposedFacilityPatch()
		{
			string serviceKey = "someService";
			facility.AddFactory<ISomeService, ServiceFactory>(serviceKey, "Create");

			ISomeService service = kernel.Resolve(serviceKey,
			                                      typeof(ISomeService)) as ISomeService;

			Assert.IsTrue(ServiceFactory.CreateWasCalled);
			Assert.IsInstanceOf(typeof(ServiceImplementation), service);
		}

		[Test, Ignore("Kernel doesn't treat ${} as an special expression for config/primitives, not sure it should - leave this up for discussion")]
		public void Factory_AsAPublisherOfValues_CanBeResolvedByDependents()
		{
			string serviceKey = "someService";
			facility.AddFactory<TimeSpan, ServiceFactory>(serviceKey, "get_Something");

			kernel.Register(Component.For<SettingsConsumer>().
				Parameters(Parameter.ForKey("something").Eq("${serviceKey}")));

			SettingsConsumer consumer = kernel.Resolve<SettingsConsumer>();
		}

		class SettingsConsumer
		{
			private readonly int something;

			public SettingsConsumer(int something)
			{
				this.something = something;
			}

			public int Something
			{
				get { return something; }
			}
		}

		class Settings
		{
			private readonly int something = 1;

			public int Something
			{
				get { return something; }
			}
		}
	}

	#region Exampleclasses

	public interface ISomeService
	{
		void SomeOperation();
	}

	public class ServiceImplementation : ISomeService
	{
		private int _someValue;

		public int SomeValue
		{
			get { return _someValue; }
			set { _someValue = value; }
		}

		public ServiceImplementation(int someValue)
		{
			_someValue = someValue;
		}

		public void SomeOperation()
		{
		}
	}

	public class ServiceFactory
	{
		public static bool CreateWasCalled;

		public ServiceImplementation Create()
		{
			CreateWasCalled = true;
			return new ServiceImplementation(12);
		}
	}

	#endregion
}