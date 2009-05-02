using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Castle.MicroKernel.Registration;
using NHibernate;
using NHibernate.Cfg;
using Rhino.Mocks;

namespace Castle.Facilities.NHibernateIntegration.Tests.Issues.Facilities119
{

	[TestFixture]
	public class Fixture : IssueTestCase
	{
		protected override void ExportDatabaseSchema()
		{
			
		}
		protected override void DropDatabaseSchema()
		{
			
		}
		[Test]
		public void Configurations_can_be_obtained_via_different_ConfigurationBuilders()
		{
			var configuration1 = container.Resolve<Configuration>("sessionFactory1.cfg");
			var configuration2 = container.Resolve<Configuration>("sessionFactory2.cfg");
			var configuration3 = container.Resolve<Configuration>("sessionFactory3.cfg");
			Assert.AreEqual(configuration1.GetProperty("test"),"test1");
			Assert.AreEqual(configuration2.GetProperty("test"), "test2");
			Assert.AreEqual(configuration3.GetProperty("test"), "test3");
		}
	}
}
