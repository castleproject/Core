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
	using Castle.Model;
	using Castle.Model.Configuration;
	
	using Castle.Services.Logging;
	using Castle.Services.Transaction;

	using TransactionMode = Castle.Services.Transaction.TransactionMode;

	/// <summary>
	/// Provides integration with ActiveRecord framework.
	/// </summary>
	public class ActiveRecordFacility : AbstractFacility
	{
		private ILogger log;
		private int sessionFactoryCount, sessionFactoryHolderCount;

		public ActiveRecordFacility()
		{
		}

		protected override void Init()
		{
			if (Kernel.HasComponent(typeof(ILogger)))
				log = (ILogger) Kernel[typeof(ILogger)];
			else
				log = new NullLogger();
			
			log.Debug("Initializing AR Facility");
			
			if (FacilityConfig == null)
			{
				log.FatalError("Configuration for AR Facility not found.");
				throw new FacilityException("Sorry, but the ActiveRecord Facility depends on a proper configuration node.");
			}

			ConfigurationCollection assembliyConfigNodes = FacilityConfig.Children["assemblies"].Children;

			if (assembliyConfigNodes.Count == 0)
			{
				log.FatalError("No assembly specified on AR Facility config.");

				throw new FacilityException("You need to specify at least one assembly that contains " +
					"the ActiveRecord classes. For example, <assemblies><item>MyAssembly</item></assemblies>");
			}

			ArrayList assemblies = new ArrayList(assembliyConfigNodes.Count);
			
			foreach(IConfiguration assemblyNode in assembliyConfigNodes)
			{
				assemblies.Add( ObtainAssembly( assemblyNode.Value ) );
			}

			Kernel.ComponentCreated += new Castle.MicroKernel.ComponentInstanceDelegate(Kernel_ComponentCreated);

			SetUpTransactionManager();

			InitializeFramework(assemblies);
		}

		public override void Dispose()
		{
			log.Info("AR Facility is being disposed.");
			base.Dispose();
		}

		private void InitializeFramework(ArrayList assemblies)
		{
			log.Info("Initializing ActiveRecord Framework");
			
			ActiveRecordStarter.SessionFactoryHolderCreated += new SessionFactoryHolderDelegate(OnSessionFactoryHolderCreated);

			try
			{
				ActiveRecordStarter.Initialize( 
					(Assembly[]) assemblies.ToArray( typeof(Assembly) ), 
					new ConfigurationSourceAdapter(FacilityConfig) );
			}
			finally
			{
				ActiveRecordStarter.SessionFactoryHolderCreated -= new SessionFactoryHolderDelegate(OnSessionFactoryHolderCreated);
			}
		}

		private void SetUpTransactionManager()
		{
			if (!Kernel.HasComponent( typeof(ITransactionManager) ))
			{
				log.Info("No Transaction Manager registered on Kernel, registering AR Transaction Manager");
				
				Kernel.AddComponent( "ar.transaction.manager", 
				                     typeof(ITransactionManager), typeof(ActiveRecordTransactionManager) );
			}
		}

		private void OnNewTransaction(ITransaction transaction, TransactionMode transactionMode, IsolationMode isolationMode)
		{
			transaction.Enlist( new TransactionScopeResourceAdapter(transactionMode) );
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
				componentName += "." + (++sessionFactoryHolderCount);
			
			while (Kernel.HasComponent(componentName))
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
				componentName += "." + (++sessionFactoryCount);

			while (Kernel.HasComponent(componentName))
			{
				componentName =
					componentName.Substring(0, componentName.LastIndexOf('.'))
					+ (++sessionFactoryCount);
			}

			log.Info("Registering SessionFactory named '{0}' for the root type {1}: {2}", componentName, rootType, sender);
			Kernel.AddComponentInstance(
				componentName,
				typeof(NHibernate.ISessionFactory),
				new SessionFactoryDelegate((ISessionFactoryHolder) sender, rootType));
		}
	}

	internal class ConfigurationSourceAdapter : InPlaceConfigurationSource
	{
		public ConfigurationSourceAdapter(IConfiguration facilityConfig)
		{
			String isWeb = facilityConfig.Attributes["isWeb"];
			String threadinfotype = facilityConfig.Attributes["threadinfotype"];
			string isDebug = facilityConfig.Attributes["isDebug"];

			SetUpThreadInfoType("true" == isWeb, threadinfotype);

			SetDebugFlag("true" == isDebug);

			foreach (IConfiguration config in facilityConfig.Children)
			{
				if (!"config".Equals(config.Name)) continue;

				Type type = typeof(ActiveRecordBase);
				
				String typeAtt = config.Attributes["type"];
				
				if (typeAtt != null)
				{
					type = ObtainType(typeAtt);
				}

				Add( type, AdjustConfiguration(config) );
			}
		}

		private IConfiguration AdjustConfiguration(IConfiguration config)
		{
			MutableConfiguration newConfig = new MutableConfiguration("entry");

			foreach(IConfiguration configNode in config.Children)
			{
				String key = configNode.Attributes["key"];
				String value = configNode.Attributes["value"];

				newConfig.Children.Add( new MutableConfiguration(key, value) );
			}

			return newConfig;
		}

		private static Type ObtainType(String typeAtt)
		{
			Type type = Type.GetType(typeAtt, false, false);

			if (type == null)
			{
				String message = String.Format("Could not obtain type from name {0}", typeAtt);
				throw new ConfigurationException(message);
			}

			return type;
		}
	}
}
