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
	/// Summary description for MDefaultServerTestCase.
	/// </summary>
	[TestFixture]
	public class MDefaultServerTestCase : Assertion
	{
		protected MDefaultServer server = new MDefaultServer();
		protected Type httpServerType = typeof(DummyHttpServer);
		protected Type smtpServerType = typeof(DummySmtpServer);

		[Test]
		public void TestInstantiate()
		{
			Object obj = server.Instantiate( httpServerType.Assembly.FullName, httpServerType.FullName );
			AssertNotNull( obj );
			AssertEquals( httpServerType, obj.GetType() );
		}

		[Test]
		public void TestCreateManagedObject()
		{
			ManagedObjectName name = new ManagedObjectName("domain.org:type=httpServer");

			try
			{
				ManagedInstance inst = server.CreateManagedObject( 
					httpServerType.Assembly.FullName, httpServerType.FullName, name );
				AssertNotNull( inst );
				AssertEquals( httpServerType.FullName, inst.TypeName );
				AssertEquals( name, inst.Name );
			}
			finally
			{
				server.UnregisterManagedObject( name );
			}
		}

		[Test]
		public void TestRegisterManagedObject()
		{
			ManagedObjectName name = new ManagedObjectName("domain.org:type=httpServer");

			try
			{
				Object httpServer = server.Instantiate( httpServerType.Assembly.FullName, httpServerType.FullName );

				ManagedInstance inst = server.RegisterManagedObject( httpServer, name );
				AssertNotNull( inst );
				AssertEquals( httpServerType.FullName, inst.TypeName );
				AssertEquals( name, inst.Name );
			}
			finally
			{
				server.UnregisterManagedObject( name );
			}
		}

		[Test]
		public void TestGetManagementInfo()
		{
			ManagedObjectName name1 = new ManagedObjectName("domain.org:type=httpServer");
			ManagedObjectName name2 = new ManagedObjectName("domain.net:type=smtpServer");

			try
			{
				Object httpServer = server.Instantiate( httpServerType.Assembly.FullName, httpServerType.FullName );
				server.RegisterManagedObject( httpServer, name1 );

				ManagementInfo info = server.GetManagementInfo( name1 );
				AssertNotNull( info );
				AssertEquals( 3, info.Operations.Count );
				AssertEquals( 1, info.Attributes.Count );

				Object smtpServer = server.Instantiate( smtpServerType.Assembly.FullName, smtpServerType.FullName );

				try
				{
					server.RegisterManagedObject( smtpServer, name1 );

					Fail("Should not allow register with same name.");
				}
				catch(InstanceAlreadyRegistredException)
				{
					// OK
				}

				server.RegisterManagedObject( smtpServer, name2 );

				info = server.GetManagementInfo( name2 );
				AssertNotNull( info );
				AssertEquals( 2, info.Operations.Count );
				AssertEquals( 1, info.Attributes.Count );
			}
			finally
			{
				server.UnregisterManagedObject( name1 );
				server.UnregisterManagedObject( name2 );
			}
		}

		[Test]
		public void TestAttributes()
		{
			ManagedObjectName name = new ManagedObjectName("domain.net:type=smtpServer");

			try
			{
				Object smtpServer = server.Instantiate( smtpServerType.Assembly.FullName, smtpServerType.FullName );

				ManagedInstance inst = server.RegisterManagedObject( smtpServer, name );

				int port = (int) server.GetAttribute(name, "Port");
				AssertEquals( 1088, port );

				server.SetAttribute( name, "Port", 25 );
				
				port = (int) server.GetAttribute(name, "Port");
				AssertEquals( 25, port );
			}
			finally
			{
				server.UnregisterManagedObject( name );
			}
		}

		[Test]
		public void TestInvoke()
		{
			ManagedObjectName name = new ManagedObjectName("domain.org:type=httpServer");

			try
			{
				Object httpServer = server.Instantiate( httpServerType.Assembly.FullName, httpServerType.FullName );

				ManagedInstance inst = server.RegisterManagedObject( httpServer, name );

				bool state = (bool) server.GetAttribute(name, "Started");
				AssertEquals( false, state );

				server.Invoke( name, "Start", null, null );
				
				state = (bool) server.GetAttribute(name, "Started");
				AssertEquals( true, state );

				server.Invoke( name, "Stop", null, null );
				state = (bool) server.GetAttribute(name, "Started");
				AssertEquals( false, state );
			}
			finally
			{
				server.UnregisterManagedObject( name );
			}
		}
	}
}
