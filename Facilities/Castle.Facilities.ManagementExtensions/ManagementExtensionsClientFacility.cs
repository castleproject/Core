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

namespace Castle.Facilities.ManagedExtensions
{
	using System;

	using Castle.MicroKernel;

	using Castle.ManagementExtensions;
	using Castle.ManagementExtensions.Remote.Client;

	/// <summary>
	/// Summary description for ManagementExtensionsClientFacility.
	/// </summary>
	public class ManagementExtensionsClientFacility : IFacility
	{
		MConnector _connector;

		public ManagementExtensionsClientFacility()
		{
		}

		#region IFacility Members

		public void Init(IKernel kernel)
		{
			_connector = MConnectorFactory.CreateConnector( "provider:http:binary:test.rem", null );

		
		}

		public void Terminate()
		{
			_connector.Disconnect();
			_connector.Dispose();
		}

		#endregion

		public MServer MServer
		{
			get { return _connector.ServerConnection; }
		}
	}
}
