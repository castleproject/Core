// Copyright 2003-2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.ManagementExtensions.Test
{
	using System;

	using NUnit.Framework;

	using Castle.ManagementExtensions.Default;
	using Castle.ManagementExtensions.Test.Components;

	/// <summary>
	/// Summary description for InterfacedComponentTestCase.
	/// </summary>
	[TestFixture]
	public class InterfacedComponentTestCase : Assertion
	{
		[Test]
		public void TestInfoObtation()
		{
			MDefaultServer server = new MDefaultServer();

			ManagedObjectName name1 = new ManagedObjectName("domain.org:type=dummyService");

			try
			{
				Type serviceType = typeof(DummyService);

				Object service = server.Instantiate( serviceType.Assembly.FullName, serviceType.FullName );
				server.RegisterManagedObject( service, name1 );

				ManagementInfo info = server.GetManagementInfo( name1 );
				AssertNotNull( info );
				AssertEquals( 1, info.Operations.Count );
				AssertEquals( 1, info.Attributes.Count );
			}
			finally
			{
				server.UnregisterManagedObject( name1 );
			}
		}
	}
}
