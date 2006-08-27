// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.ManagedExtensions
{
	using System;

	using Castle.Core.Configuration;

	using Castle.MicroKernel;

	using Castle.ManagementExtensions;
	using Castle.ManagementExtensions.Remote.Client;

	/// <summary>
	/// Summary description for ManagementExtensionsClientFacility.
	/// </summary>
	public class ManagementExtensionsClientFacility : IFacility
	{
		private MConnector _connector;

		public ManagementExtensionsClientFacility()
		{
		}

		public void Init(IKernel kernel, IConfiguration config)
		{
			kernel.AddComponent("managed.dispatcher.interceptor", typeof());

			_connector = MConnectorFactory.CreateConnector( "provider:http:binary:test.rem", null );

			kernel.ComponentModelBuilder.AddContributor( new Client.ManagementExtensionModelClientInspector() );
		}

		public void Terminate()
		{
			_connector.Disconnect();
			_connector.Dispose();
		}

		public MServer MServer
		{
			get { return _connector.ServerConnection; }
		}
	}
}
