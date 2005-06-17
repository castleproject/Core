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

	using Castle.ManagementExtensions;
	using Castle.ManagementExtensions.Default;
	using Castle.ManagementExtensions.Remote.Server;
	using Castle.ManagementExtensions.Remote.Client;

	/// <summary>
	/// Summary description for RemoteTestCase.
	/// </summary>
	[TestFixture]
	public class RemoteTestCase : Assertion
	{
		MServer server = null;

		[SetUp]
		public void Init()
		{
			server = MServerFactory.CreateServer("test", true);
		}

		[TearDown]
		public void Terminate()
		{
			MServerFactory.Release(server);
		}

		[Test]
		public void TestServerCreation()
		{
			MConnectorServer serverConn = 
					   MConnectorServerFactory.CreateServer( "provider:http:binary:test.rem", null, null );

			AssertNotNull( serverConn );

			ManagedObjectName name = new ManagedObjectName("connector.http:formatter=binary");
			server.RegisterManagedObject( serverConn, name );

			AssertEquals( name, serverConn.ManagedObjectName );

			AppDomain child = null;
		
			try
			{
				child = AppDomain.CreateDomain(
					"Child", 
					new System.Security.Policy.Evidence(AppDomain.CurrentDomain.Evidence), 
					AppDomain.CurrentDomain.SetupInformation);

				RemoteClient client = (RemoteClient) 
					child.CreateInstanceAndUnwrap( typeof(RemoteClient).Assembly.FullName, typeof(RemoteClient).FullName );

				AssertNotNull( client.TestClientCreation() );
			}
			finally
			{
				server.UnregisterManagedObject( name );

				if (child != null)
				{
					AppDomain.Unload(child);
				}
			}
		}

		[Test]
		public void TestTcpServerCreation()
		{
			System.Collections.Specialized.NameValueCollection props = 
				new System.Collections.Specialized.NameValueCollection();
			props.Add("port", "3131");

			MConnectorServer serverConn = 
					   MConnectorServerFactory.CreateServer( "provider:tcp:binary:test.rem", props, null );
			AssertNotNull( serverConn );

			ManagedObjectName name = new ManagedObjectName("connector.tcp:formatter=binary");
			server.RegisterManagedObject( serverConn, name );

			AssertEquals( name, serverConn.ManagedObjectName );

			AppDomain child = null;

			try
			{
				child = AppDomain.CreateDomain(
					"Child", 
					new System.Security.Policy.Evidence(AppDomain.CurrentDomain.Evidence), 
					AppDomain.CurrentDomain.SetupInformation);

				RemoteClient client = (RemoteClient) 
					child.CreateInstanceAndUnwrap( typeof(RemoteClient).Assembly.FullName, typeof(RemoteClient).FullName );

				AssertNotNull( client.TestTcpClientCreation() );
			}
			finally
			{
				server.UnregisterManagedObject( name );

				if (child != null)
				{
					AppDomain.Unload(child);
				}
			}
		}
	}

	public class RemoteClient : MarshalByRefObject
	{
		public String[] TestClientCreation()
		{
			using(MConnector connector = MConnectorFactory.CreateConnector( "provider:http:binary:test.rem", null ))
			{
				Assertion.AssertNotNull( connector );
				Assertion.AssertNotNull( connector.ServerConnection );

				MServer server = (MServer) connector.ServerConnection;
				String[] domains = server.GetDomains();
				Assertion.AssertNotNull( domains );
				return domains;
			}
		}

		public String[] TestTcpClientCreation()
		{
			System.Collections.Specialized.NameValueCollection props = 
				new System.Collections.Specialized.NameValueCollection();
			props.Add("port", "3131");

			using(MConnector connector = MConnectorFactory.CreateConnector( "provider:tcp:binary:test.rem", props ))
			{
				Assertion.AssertNotNull( connector );
				Assertion.AssertNotNull( connector.ServerConnection );

				MServer server = (MServer) connector.ServerConnection;
				Assertion.AssertNotNull( server.GetDomains() );
				return server.GetDomains();
			}
		}
	}
}
