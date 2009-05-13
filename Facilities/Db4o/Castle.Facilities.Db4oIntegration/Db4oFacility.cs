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

namespace Castle.Facilities.Db4oIntegration
{
	using System;
	using System.Collections;
	using System.Configuration;

	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.Core.Configuration;
	using Castle.Services.Transaction;
	using Castle.MicroKernel.Facilities;

	using Db4objects.Db4o;
	
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
		internal const string ActivationDepth = "activationDepth";
		internal const string UpdateDepth = "updateDepth";
		internal const string ExceptionsOnNotStorableKey = "exceptionsOnNotStorable";
		internal const string CallConstructorsKey = "callConstructors";

		private const String ContextKey = "db40.transaction.context";

		#region Facility Life Cycle

		protected override void Init()
		{
			if (FacilityConfig == null)
			{
				String message = "db4o facility requires an external configuration.";

				throw new ConfigurationErrorsException(message);
			}

			Kernel.ComponentModelBuilder.AddContributor(new ObjectContainerActivatorOverrider());
			Kernel.ComponentCreated += new ComponentInstanceDelegate(Kernel_ComponentCreated);

			SetUpTransactionManager();

			ConfigureAndAddDb4oContainer();
		}

		/// <summary>
		/// Performs the tasks associated with freeing, releasing, or resetting
		/// the facility resources.
		/// </summary>
		/// <remarks>It can be overriden.</remarks>
		public override void Dispose()
		{
			IObjectContainer objContainer = (IObjectContainer) Kernel[typeof(IObjectContainer)];

			objContainer.Close();

			base.Dispose();
		}

		#endregion


		#region Transaction Management Related

		private void SetUpTransactionManager()
		{
			if (!Kernel.HasComponent(typeof(ITransactionManager)))
			{
				Kernel.AddComponent("db4o.transaction.manager", typeof(ITransactionManager), typeof(DefaultTransactionManager));
			}
		}

		private void Kernel_ComponentCreated(ComponentModel model, object instance)
		{
			if (model.Service != null && model.Service == typeof(ITransactionManager))
			{
				(instance as ITransactionManager).TransactionCreated += new TransactionCreationInfoDelegate(OnNewTransaction);
			}
		}

		private void OnNewTransaction(ITransaction transaction, TransactionMode transactionMode, IsolationMode isolationMode, bool distributedTransaction)
		{
			//if (!transaction.Context.Contains(ContextKey))
			{
				IObjectContainer db4oContainer = (IObjectContainer) Kernel[typeof(IObjectContainer)];

				transaction.Context[ContextKey] = true;
				transaction.Enlist(new ResourceObjectContainerAdapter(db4oContainer));
			}
		}

		#endregion


		private void ConfigureAndAddDb4oContainer()
		{
			IConfiguration config = FacilityConfig.Children["container"];

			IDictionary properties = new Hashtable();

			String compKey = config.Attributes[IdKey];

			properties.Add(IdKey, compKey);
			properties.Add(DatabaseFileKey, config.Attributes[DatabaseFileKey]);

			properties.Add(ExceptionsOnNotStorableKey, Convert.ToBoolean(config.Attributes[ExceptionsOnNotStorableKey]));
			
			if (config.Attributes[CallConstructorsKey] != null)
			{
				properties.Add(CallConstructorsKey, Convert.ToBoolean(config.Attributes[CallConstructorsKey]));
			}

			if (config.Attributes[ActivationDepth] != null)
			{
				properties.Add(ActivationDepth, Convert.ToInt32(config.Attributes[ActivationDepth]));
			}

			if (config.Attributes[UpdateDepth] != null)
			{
				properties.Add(UpdateDepth, Convert.ToInt32(config.Attributes[UpdateDepth]));
			}

			if (config.Attributes[HostNameKey] != null)
			{
				properties.Add(HostNameKey, config.Attributes[HostNameKey]);
				properties.Add(RemotePortKey, Convert.ToInt32(config.Attributes[RemotePortKey]));
				properties.Add(UserKey, config.Attributes[UserKey]);
				properties.Add(PasswordKey, config.Attributes[PasswordKey]);
			}

			Kernel.AddComponentWithExtendedProperties(compKey, typeof(IObjectContainer), typeof(IObjectContainer), properties);
		}
	}
}
