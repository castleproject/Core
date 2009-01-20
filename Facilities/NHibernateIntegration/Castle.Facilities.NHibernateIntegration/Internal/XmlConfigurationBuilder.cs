using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Castle.Core.Configuration;
using Castle.Core.Resource;
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
			IResource configResource = new FileAssemblyResource(cfgFile);
			Configuration cfg;
			using(XmlReader reader=XmlReader.Create(configResource.GetStreamReader()))
			{
				cfg = new Configuration();
				cfg.Configure(reader);
			}
			configResource.Dispose();
			return cfg;
		}

		#endregion
	}
}
