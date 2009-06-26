using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Configuration;
namespace Castle.Facilities.NHibernateIntegration.Tests.Issues.Facilities119
{
	using NHibernate.Cfg;

	public class TestConfigurationBuilder2:IConfigurationBuilder
	{
		#region IConfigurationBuilder Members

		public NHibernate.Cfg.Configuration GetConfiguration(IConfiguration config)
		{
			Configuration cfg = new Configuration();
			cfg.SetProperty("test", "test2");
			return cfg;
		}

		#endregion
	}
}
