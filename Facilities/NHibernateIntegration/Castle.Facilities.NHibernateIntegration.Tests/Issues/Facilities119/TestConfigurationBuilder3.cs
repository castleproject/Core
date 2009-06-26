using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Configuration;
using NHibernate.Cfg;

namespace Castle.Facilities.NHibernateIntegration.Tests.Issues.Facilities119
{

	public class TestConfigurationBuilder3 : IConfigurationBuilder
	{
		public Configuration GetConfiguration(IConfiguration config)
		{
			Configuration cfg = new Configuration();
			cfg.SetProperty("test", "test3");
			return cfg;
		}
	}
}
