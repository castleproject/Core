using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core.Configuration;
using NHibernate.Cfg;

namespace Castle.Facilities.NHibernateIntegration.Internal
{
	/// <summary>
	/// The configuration builder for NHibernate's own cfg.xml
	/// </summary>
	public class XmlConfigurationBuilder : IConfigurationBuilder
	{
		#region IConfigurationBuilder Members

		/// <summary>
		/// Returns the Configuration object for the given xml
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public Configuration GetConfiguration(IConfiguration config)
		{
			string cfgFile = config.Attributes["nhibernateConfigFile"];
			Configuration cfg = new Configuration();
			cfg.Configure(cfgFile);
			return cfg;
		}

		#endregion
	}
}
