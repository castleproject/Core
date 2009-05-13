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

namespace Castle.Facilities.ActiveRecordIntegration.Tests
{
	using Castle.Core.Configuration;
	using Castle.MicroKernel.Facilities;
	using Castle.Windsor;
	using NUnit.Framework;

	[TestFixture]
	public class BugTestCase : AbstractActiveRecordTest
	{
		[Test]
		[ExpectedException(typeof(FacilityException),
			ExpectedMessage = "You need to specify at least one assembly that contains the ActiveRecord classes. For example, <assemblies><item>MyAssembly</item></assemblies>"
			)]
		public void FACILITIES66()
		{
			container = new WindsorContainer();

			IConfiguration confignode = new MutableConfiguration("facility");
			confignode.Children.Add(new MutableConfiguration("arfacility"));
			container.Kernel.ConfigurationStore.AddFacilityConfiguration("arfacility", confignode);

			container.AddFacility("arfacility", new ActiveRecordFacility());
		}
	}
}