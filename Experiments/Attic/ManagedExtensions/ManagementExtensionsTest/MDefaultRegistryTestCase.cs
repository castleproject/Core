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
	/// Summary description for MDefaultRegistryTestCase.
	/// </summary>
	[TestFixture]
	public class MDefaultRegistryTestCase : Assertion
	{
		[Test]
		public void TestUse()
		{
			MDefaultRegistry registry = new MDefaultRegistry(new MDefaultServer());
			AssertEquals( 0, registry.Count );

			DummyHttpServer instance = new DummyHttpServer();
			ManagedObjectName name = new ManagedObjectName("domain.org");

			ManagedInstance minstance = 
				registry.RegisterManagedObject(instance, name);

			AssertNotNull( minstance );
			AssertNotNull( minstance.TypeName );
			AssertEquals( name, minstance.Name );
			AssertEquals( 1, registry.Count );
			AssertEquals( instance, registry[name] );
		
			registry.UnregisterManagedObject( name );

			AssertEquals( 0, registry.Count );
		}

		[Test]
		public void TestRegistration()
		{
			MDefaultRegistry registry = new MDefaultRegistry(new MDefaultServer());

			DummyLifecycledService service = new DummyLifecycledService();

			ManagedObjectName name = new ManagedObjectName("domain.org:name=Service");

			registry.RegisterManagedObject( service, name );
			registry.UnregisterManagedObject( name );

			AssertEquals( 0, service.beforeRegisterCalled );
			AssertEquals( 1, service.afterRegisterCalled );
			AssertEquals( 2, service.beforeDeregisterCalled );
			AssertEquals( 3, service.afterDeregisterCalled );
		}
	}
}
