// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using Castle.MicroKernel;
	using NUnit.Framework;

	[TestFixture]
	public class FactorySupportProgrammaticTestCase
	{
		[Test]
		public void FactoryResolveWithProposedFacilityPatch()
		{
			IKernel kernel = new DefaultKernel();
			FactorySupportFacility facility = new FactorySupportFacility();
			kernel.AddFacility("factory.support", facility);

			string serviceKey = "someService";
			facility.AddFactory<ISomeService, ServiceFactory>(serviceKey, "Create");

			ISomeService service = kernel.Resolve(serviceKey, 
				typeof(ISomeService)) as ISomeService;

			Assert.IsTrue(ServiceFactory.CreateWasCalled);
			Assert.IsInstanceOfType(typeof(ServiceImplementation), service);
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
