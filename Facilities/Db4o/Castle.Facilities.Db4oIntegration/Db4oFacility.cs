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
	using System.Collections;
	using System.Configuration;

	using Castle.Model.Configuration;
	using Castle.Services.Transaction;
	using Castle.MicroKernel.Facilities;

	using com.db4o;
	
	/// <summary>
	/// Enable components to take advantage of the capabilities 
	/// offered by the db4objects project.
	/// </summary>
	public class Db4oFacility : AbstractFacility
	{
		internal const string DatabaseFileKey = "databaseFile";
		internal const string IdKey = "id";
		internal const string HostNameKey = "hostName";
		internal const string RemotePortKey = "remotePort";
		internal const string UserKey = "user";
		internal const string PasswordKey = "password";

		public Db4oFacility()
		{
		}

		protected override void Init()
		{
			if (FacilityConfig == null)
			{
				throw new ConfigurationException("db4o facility requires an external configuration.");
			}

			Kernel.ComponentModelBuilder.AddContributor(new AutoDb4oTransactionInspector());
			Kernel.ComponentModelBuilder.AddContributor(new ObjectContainerActivatorOverrider());

			Kernel.AddComponent("db4o.transaction.autocommit.interceptor", typeof(AutoCommitInterceptor), typeof(AutoCommitInterceptor));

			Kernel.AddComponent("db4o.transaction.manager", typeof(ITransactionManager), typeof(Db4oTransactionManager));

			ConfigureAndAddContainer();
		}

		public override void Dispose()
		{
			ObjectContainer objContainer = (ObjectContainer) Kernel[typeof(ObjectContainer)];

			objContainer.close();

			base.Dispose();
		}

		private void ConfigureAndAddContainer()
		{
			IConfiguration config = FacilityConfig.Children["container"];

			IDictionary properties = new Hashtable();

			String compKey = config.Attributes[IdKey];

			properties.Add(IdKey, compKey);
			properties.Add(DatabaseFileKey, config.Attributes[DatabaseFileKey]);

			if (config.Attributes[HostNameKey] != null)
			{
				properties.Add(HostNameKey, config.Attributes[HostNameKey]);
				properties.Add(RemotePortKey, Convert.ToInt32(config.Attributes[RemotePortKey]));
				properties.Add(UserKey, config.Attributes[UserKey]);
				properties.Add(PasswordKey, config.Attributes[PasswordKey]);
			}

			Kernel.AddComponentWithProperties(compKey, typeof(ObjectContainer), typeof(ObjectContainer), properties);
		}
	}
}
