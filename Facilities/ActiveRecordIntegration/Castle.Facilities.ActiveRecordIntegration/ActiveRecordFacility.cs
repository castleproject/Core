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

namespace Castle.Facilities.ActiveRecordIntegration
{
	using System;
	using System.Reflection;
	using System.Collections;
	using System.Configuration;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Config;
	using Castle.MicroKernel.Facilities;
	using Castle.Core;
	using Castle.Core.Configuration;
	using Castle.Core.Logging;
	using Castle.Services.Transaction;
	
	using TransactionMode = Castle.Services.Transaction.TransactionMode;

	/// <summary>
	/// Provides integration with ActiveRecord framework.
	/// </summary>
	public class ActiveRecordFacility : AbstractFacility
	{
		private ILogger log = NullLogger.Instance;
		private int sessionFactoryCount, sessionFactoryHolderCount;
		private readonly bool skipARInitialization;
		private readonly bool skipATransactionSetup;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveRecordFacility"/> class.
		/// </summary>
		public ActiveRecordFacility() : this(false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveRecordFacility"/> class.
		/// </summary>
		public ActiveRecordFacility(bool skipARInitialization)
		{
			this.skipARInitialization = skipARInitialization;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveRecordFacility"/> class.
		/// </summary>
		public ActiveRecordFacility(bool skipARInitialization, bool skipATransactionSetup)
			: this(skipARInitialization)
		{
			this.skipATransactionSetup = skipATransactionSetup;
		}

		/// <summary>
		/// The custom initialization for the Facility.
		/// </summary>
		/// <remarks>It must be overriden.</remarks>
		protected override void Init()
		{
			if (Kernel.HasComponent(typeof(ILoggerFactory)))
			{
				log = ((ILoggerFactory) Kernel[typeof(ILoggerFactory)]).Create(GetType());
			}

			if (!skipARInitialization)
			{
				log.Debug("Initializing AR Facility");

				if (FacilityConfig == null)
				{
					log.FatalError("Configuration for AR Facility not found.");
					throw new FacilityException("Sorry, but the ActiveRecord Facility depends on a proper configuration node.");
				}

				IConfiguration assemblyConfig = FacilityConfig.Children["assemblies"];

				if (assemblyConfig == null || assemblyConfig.Children.Count == 0)
				{
					log.FatalError("No assembly specified on AR Facility config.");

					throw new FacilityException("You need to specify at least one assembly that contains " +
						"the ActiveRecord classes. For example, <assemblies><item>MyAssembly</item></assemblies>");
				}

				ConfigurationCollection assembliyConfigNodes = assemblyConfig.Children;

				ArrayList assemblies = new ArrayList(assembliyConfigNodes.Count);

				foreach (IConfiguration assemblyNode in assembliyConfigNodes)
				{
					assemblies.Add(ObtainAssembly(assemblyNode.Value));
				}

				InitializeFramework(assemblies);
			}

			if (!skipATransactionSetup)
			{
				Kernel.ComponentCreated += Kernel_ComponentCreated;
				SetUpTransactionManager();
			}
		}

		/// <summary>
		/// Performs the tasks associated with freeing, releasing, or resetting
		/// the facility resources.
		/// </summary>
		/// <remarks>It can be overriden.</remarks>
		public override void Dispose()
		{
			log.Info("AR Facility is being disposed.");
			base.Dispose();
		}

		private void InitializeFramework(ArrayList assemblies)
		{
			log.Info("Initializing ActiveRecord Framework");

			ActiveRecordStarter.ResetInitializationFlag();
			ActiveRecordStarter.SessionFactoryHolderCreated += new SessionFactoryHolderDelegate(OnSessionFactoryHolderCreated);

			try
			{
				ConfigurationSourceAdapter adapter = new ConfigurationSourceAdapter(FacilityConfig);
				
				if (log.IsDebugEnabled)
				{
					log.Debug("Is Debug enabled {0}", adapter.Debug);
					log.Debug("ThreadScopeInfo {0}", adapter.ThreadScopeInfoImplementation);
					log.Debug("NamingStrategy {0}", adapter.NamingStrategyImplementation);
					log.Debug("SessionFactoryHolder {0}", adapter.SessionFactoryHolderImplementation);
				}
				
				ActiveRecordStarter.Initialize(
					(Assembly[]) assemblies.ToArray(typeof(Assembly)), adapter);
			}
			finally
			{
				ActiveRecordStarter.SessionFactoryHolderCreated -= new SessionFactoryHolderDelegate(OnSessionFactoryHolderCreated);
			}
		}

		private void SetUpTransactionManager()
		{
			if (!Kernel.HasComponent(typeof(ITransactionManager)))
			{
				log.Info("No Transaction Manager registered on Kernel, registering AR Transaction Manager");

				Kernel.AddComponent("ar.transaction.manager",
				                    typeof(ITransactionManager), typeof(DefaultTransactionManager));
			}
		}

		private void OnNewTransaction(ITransaction transaction, TransactionMode transactionMode, IsolationMode isolationMode, bool distributedTransaction)
		{
			if (!transaction.DistributedTransaction)
			{
				transaction.Enlist(new TransactionScopeResourceAdapter(transactionMode));
			}
		}

		private Assembly ObtainAssembly(String assemblyName)
		{
			log.Debug("Loading model assembly '{0}' for AR", assemblyName);
			return Assembly.Load(assemblyName);
		}

		private void Kernel_ComponentCreated(ComponentModel model, object instance)
		{
			if (model.Service != null && model.Service == typeof(ITransactionManager))
			{
				(instance as ITransactionManager).TransactionCreated += new TransactionCreationInfoDelegate(OnNewTransaction);
			}
		}

		private void OnSessionFactoryHolderCreated(Castle.ActiveRecord.Framework.ISessionFactoryHolder holder)
		{
			holder.OnRootTypeRegistered += new RootTypeHandler(OnRootTypeRegistered);

			string componentName = "activerecord.sessionfactoryholder";

			if (Kernel.HasComponent(componentName))
			{
				componentName += "." + (++sessionFactoryHolderCount);
			}

			while(Kernel.HasComponent(componentName))
			{
				componentName =
					componentName.Substring(0, componentName.LastIndexOf('.'))
						+ (++sessionFactoryHolderCount);
			}

			log.Info("Registering SessionFactoryHolder named '{0}': {1}", componentName, holder);
			Kernel.AddComponentInstance(
				componentName, typeof(ISessionFactoryHolder), holder);
		}

		private void OnRootTypeRegistered(object sender, Type rootType)
		{
			string componentName = "activerecord.sessionfactory";

			if (Kernel.HasComponent(componentName))
			{
				componentName += "." + (++sessionFactoryCount);
			}

			while(Kernel.HasComponent(componentName))
			{
				componentName =
					componentName.Substring(0, componentName.LastIndexOf('.'))
						+ (++sessionFactoryCount);
			}

			log.Info("Registering SessionFactory named '{0}' for the root type {1}: {2}", componentName, rootType, sender);
			Kernel.AddComponentInstance(
				componentName,
				typeof(NHibernate.ISessionFactory),
				SessionFactoryDelegate.Create((ISessionFactoryHolder) sender, rootType));
		}
	}

	internal class ConfigurationSourceAdapter : InPlaceConfigurationSource
	{
		public ConfigurationSourceAdapter(IConfiguration facilityConfig)
		{
			String isWeb = facilityConfig.Attributes["isWeb"];
			String threadinfotype = facilityConfig.Attributes["threadinfotype"];
			string isDebug = facilityConfig.Attributes["isDebug"];
			string sessionfactoryholdertype = facilityConfig.Attributes["sessionfactoryholdertype"];
			string isLazyByDefault = facilityConfig.Attributes["default-lazy"];
			string pluralize = facilityConfig.Attributes["pluralizeTableNames"];
			string verifyModelsAgainstDBSchema = facilityConfig.Attributes["verifyModelsAgainstDBSchema"];
			string defaultFlushType = facilityConfig.Attributes["flush"];
			string namingstrategytype = facilityConfig.Attributes["namingstrategytype"];

			SetUpThreadInfoType(ConvertBool(isWeb), threadinfotype);
			SetDebugFlag(ConvertBool(isDebug));
			SetUpSessionFactoryHolderType(sessionfactoryholdertype);
			SetIsLazyByDefault(ConvertBool(isLazyByDefault));
			PluralizeTableNames = ConvertBool(pluralize);
			VerifyModelsAgainstDBSchema = ConvertBool(verifyModelsAgainstDBSchema);
			if (string.IsNullOrEmpty(defaultFlushType))
				SetDefaultFlushType(DefaultFlushType.Classic);
			else
				SetDefaultFlushType(defaultFlushType);
			if (!string.IsNullOrEmpty(namingstrategytype))
				SetUpNamingStrategyType(namingstrategytype);

			foreach(IConfiguration config in facilityConfig.Children)
			{
				if (!"config".Equals(config.Name)) continue;

				Type type = typeof(ActiveRecordBase);

				String typeAtt = config.Attributes["type"];

				if (typeAtt != null)
				{
					type = ObtainType(typeAtt);
				}

				Add(type, AdjustConfiguration(config));
			}
		}

		private IConfiguration AdjustConfiguration(IConfiguration config)
		{
			MutableConfiguration newConfig = new MutableConfiguration("entry");

			foreach(IConfiguration configNode in config.Children)
			{
				String key = configNode.Attributes["key"];
				String value = configNode.Attributes["value"];

				newConfig.Children.Add(new MutableConfiguration(key, value));
			}

			return newConfig;
		}

		private static Type ObtainType(String typeAtt)
		{
			Type type = Type.GetType(typeAtt, false, false);

			if (type == null)
			{
				String message = String.Format("Could not obtain type from name {0}", typeAtt);

				throw new ConfigurationErrorsException(message);
			}

			return type;
		}

		private static bool ConvertBool(string boolString)
		{
			return "true".Equals(boolString, StringComparison.OrdinalIgnoreCase);
		}
	}
}
