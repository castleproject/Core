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
	using Castle.ManagementExtensions.Remote.Server;

	/// <summary>
	/// Summary description for ManagementExtensionsServerFacility.
	/// </summary>
	public class ManagementExtensionsServerFacility : IFacility
	{
		private IKernel _kernel;
		private MServer _mserver;
		private MConnectorServer _serverConn;

		public ManagementExtensionsServerFacility()
		{
		}

		public MServer MServer
		{
			get { return _mserver; }
		}

		#region IFacility Members

		public void Init(IKernel kernel)
		{
			_kernel = kernel;

			_mserver = MServerFactory.CreateServer( "castle.domain", false );

			_serverConn = 
				MConnectorServerFactory.CreateServer( 
					"provider:http:binary:test.rem", null, _mserver );

			_kernel.ComponentModelBuilder.AddContributor( new ManagementExtensionModelInspector() );

			_kernel.ComponentCreated += new ComponentInstanceDelegate(OnComponentCreated);
			_kernel.ComponentDestroyed += new ComponentInstanceDelegate(OnComponentDestroyed);
		}

		public void Terminate()
		{
			_serverConn.Dispose();
		}

		#endregion

		private void OnComponentCreated(Castle.Model.ComponentModel model, object instance)
		{
			if (model.ExtendedProperties.Contains( ManagementExtensionModelInspector.ComponentIsManagedKey ))
			{
				_mserver.RegisterManagedObject( instance, new ManagedObjectName( model.Name ) );
			}
		}

		private void OnComponentDestroyed(Castle.Model.ComponentModel model, object instance)
		{
			if (model.ExtendedProperties.Contains( ManagementExtensionModelInspector.ComponentIsManagedKey ))
			{
				_mserver.UnregisterManagedObject( new ManagedObjectName( model.Name ) );
			}
		}
	}
}
