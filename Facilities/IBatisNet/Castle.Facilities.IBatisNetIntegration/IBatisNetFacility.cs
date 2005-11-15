#region License
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
	using System;
	using System.Configuration;

	using Castle.Model;
	using Castle.Model.Configuration;
	using Castle.MicroKernel.Facilities;
	using Castle.Services.Transaction;

	using IBatisNet.Common.Logging;
	using IBatisNet.DataMapper;

	public class IBatisNetFacility : AbstractFacility
	{
		public static readonly String FILE_CONFIGURATION = "_IBATIS_FILE_CONFIGURATION_";
		public static readonly String FILE_CONFIGURATION_EMBEDDED = "_IBATIS_FILE_CONFIGURATION_EMBEDDED";

		private static readonly ILog _logger = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

		public IBatisNetFacility()
		{
		}

		#region IFacility Members

		protected override void Init()
		{
			if (FacilityConfig == null)
			{
				throw new ConfigurationException( "The IBatisNetFacility requires an external configuration" );
			}

			Kernel.ComponentModelBuilder.AddContributor( new AutomaticSessionInspector() );
			Kernel.AddComponent( "IBatis.session.interceptor", typeof(AutomaticSessionInterceptor) );
			Kernel.AddComponent( "IBatis.transaction.manager", typeof(ITransactionManager), typeof(DataMapperTransactionManager) );

			int factories = 0;

			foreach( IConfiguration factoryConfig in FacilityConfig.Children)
			{
				if( factoryConfig.Name == "sqlMap")
				{
					ConfigureFactory(factoryConfig);
					factories++;
				}
			}
			
			if ( factories == 0)
			{
				throw new ConfigurationException( "You need to configure at least one sqlMap for IBatisNetFacility" );
			}
		}

		#endregion

		private void ConfigureFactory( IConfiguration config )
		{
			// A name for this sqlMap
			String id = config.Attributes["id"]; 

			String fileName = config.Attributes["config"];
			if ( fileName == String.Empty )
			{
				fileName = "sqlMap.config"; // default name
			}
			
			bool isEmbedded = false;
			String embedded = config.Attributes["embedded"];
			if ( embedded != null )
			{
				try
				{
					isEmbedded = Convert.ToBoolean( embedded );
				}
				catch
				{
					isEmbedded = false;
				}
			}

			ComponentModel model = new ComponentModel(id, typeof(SqlMapper), null);
			model.ExtendedProperties.Add( FILE_CONFIGURATION, fileName );
			model.ExtendedProperties.Add( FILE_CONFIGURATION_EMBEDDED, isEmbedded );
			model.LifestyleType = LifestyleType.Singleton;
			model.CustomComponentActivator = typeof( SqlMapActivator );

			Kernel.AddCustomComponent( model );
		}
	}
}
