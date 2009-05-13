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

namespace Castle.MicroKernel.Tests
{
	using Castle.Core.Configuration;
	using Castle.Facilities.Startable;
	using Castle.MicroKernel.Tests.ClassComponents;
	using NUnit.Framework;

	[TestFixture]
	public class FacilityTestCase
	{
		private const string FacilityKey = "testFacility";
		private IKernel _kernel;
		private HiperFacility _facility;

		public FacilityTestCase()
		{
		}

		[SetUp]
		public void Init()
		{
			_kernel = new DefaultKernel();

			IConfiguration confignode = new MutableConfiguration("facility");
			IConfiguration facilityConf = new MutableConfiguration(FacilityKey);
			confignode.Children.Add(facilityConf);
			_kernel.ConfigurationStore.AddFacilityConfiguration(FacilityKey, confignode);

			_facility = new HiperFacility();

			Assert.IsFalse(_facility.Initialized);
			_kernel.AddFacility(FacilityKey, _facility);
		}

		[Test]
		public void Creation()
		{
			IFacility facility = _kernel.GetFacilities()[0];

			Assert.IsNotNull(facility);
			Assert.AreSame(_facility, facility);
		}

		[Test]
		public void LifeCycle()
		{
			Assert.IsFalse(_facility.Terminated);

			IFacility facility = _kernel.GetFacilities()[0];

			Assert.IsTrue(_facility.Initialized);
			Assert.IsFalse(_facility.Terminated);

			_kernel.Dispose();

			Assert.IsTrue(_facility.Initialized);
			Assert.IsTrue(_facility.Terminated);
		}

		[Test]
		public void OnCreationCallback()
		{
			StartableFacility facility = null;

			_kernel.AddFacility<StartableFacility>(delegate(StartableFacility f)
			{
				facility = f;
			});

			Assert.IsNotNull(facility);
		}
	}
}