#region Licence
/// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
///  
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///  
/// http://www.apache.org/licenses/LICENSE-2.0
///  
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// 
/// -- 
/// 
/// This facility was a contribution kindly 
/// donated by Gilles Bayon <gilles.bayon@gmail.com>
/// 
/// --
#endregion

namespace Castle.Facilities.IBatisNetIntegration
{
	using System.Configuration;

	using log4net;

	using Castle.Model;
	using Castle.Model.Configuration;
	using Castle.MicroKernel.Facilities;
	using Castle.Services.Transaction;

	using IBatisNet.DataMapper;


	public class IBatisNetFacility : AbstractFacility
	{
		public static readonly string FILE_CONFIGURATION = "_IBATIS_FILE_CONFIGURATION_";

		private static readonly ILog _logger = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

		public IBatisNetFacility()
		{
		}

		#region IFacility Members

		protected override void Init()
		{
			if (FacilityConfig == null)
			{
				throw new ConfigurationException(
					"The IBatisNetFacility requires an external configuration");
			}

			IConfiguration factoriesConfig = FacilityConfig.Children["sqlMap"];

			if (factoriesConfig == null)
			{
				throw new ConfigurationException(
					"You need to configure at least one sqlMap for IBatisNetFacility");
			}

			Kernel.ComponentModelBuilder.AddContributor( new AutomaticSessionInspector() );

			Kernel.AddComponent( 
				"IBatis.session.interceptor", 
				typeof(AutomaticSessionInterceptor) );

			Kernel.AddComponent( 
				"IBatis.transaction.manager", 
				typeof(ITransactionManager), typeof(DataMapperTransactionManager) );

			ConfigureFactories(factoriesConfig);
		}

		#endregion

		private void ConfigureFactories(IConfiguration config)
		{
			// A name for this sqlMap
			// sqlServerMap
			string id = config.Attributes["id"]; 

			string fileName = config.Attributes["config"];
			if (fileName == string.Empty)
			{
				fileName = "sqlMap.config"; // default name
			}

			ComponentModel model = new ComponentModel(id, typeof(SqlMapper), null);
			model.ExtendedProperties.Add(FILE_CONFIGURATION, fileName );
			model.LifestyleType = LifestyleType.Singleton;
			model.CustomComponentActivator = typeof( SqlMapActivator );

			Kernel.AddCustomComponent( model );
		}
	}
}
