// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.Db4oIntegration
{
	using System;
	using System.IO;

	using com.db4o;
	
	using Castle.Model;
	using Castle.MicroKernel;
	using Castle.MicroKernel.ComponentActivator;


	public class ObjectContainerComponentActivator : DefaultComponentActivator
	{
		public ObjectContainerComponentActivator(ComponentModel model, IKernel kernel, ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction) : 
			base(model, kernel, onCreation, onDestruction)
		{
		}

		protected override object Instantiate()
		{
			if (Model.ExtendedProperties[Db4oFacility.HostNameKey] != null)
			{
				string hostName = (string) Model.ExtendedProperties[Db4oFacility.HostNameKey];
				int remotePort = (int) Model.ExtendedProperties[Db4oFacility.RemotePortKey];
				string user = (string) Model.ExtendedProperties[Db4oFacility.UserKey];
				string password = (string) Model.ExtendedProperties[Db4oFacility.PasswordKey];

				return Db4o.openClient(hostName, remotePort, user, password);
			}
			else
			{
				string databaseFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, (string) Model.ExtendedProperties[Db4oFacility.DatabaseFileKey]);

				return Db4o.openFile(databaseFile);
			}
		}
	}
}
