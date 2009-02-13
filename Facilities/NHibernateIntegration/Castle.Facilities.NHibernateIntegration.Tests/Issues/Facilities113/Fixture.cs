using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;
using Rhino.Mocks;
namespace Castle.Facilities.NHibernateIntegration.Tests.Issues.Facilities113
{
	[TestFixture]
	public class Fixture : IssueTestCase
	{
		protected override string ConfigurationFile
		{
			get
			{
				return "DefaultConfiguration.xml";
			}
		}
		[Test]
		public void Calls_ConfigurationContributors_before_SessionFactory_is_initialized()
		{
			var configurator = MockRepository.GenerateMock<IConfigurationContributor>();
			var configurator2 = MockRepository.GenerateMock<IConfigurationContributor>();
			this.container.Register(Component.For<IConfigurationContributor>()
										.Named("c1")
			                        	.Instance(configurator));
			this.container.Register(Component.For<IConfigurationContributor>()
			                        	.Named("c2")
			                        	.Instance(configurator2));
			var configuration = container.Resolve<Configuration>("sessionFactory1.cfg");
			container.Resolve<ISessionFactory>("sessionFactory1");
			configurator.AssertWasCalled(x => x.Process("sessionFactory1", configuration));
			configurator2.AssertWasCalled(x => x.Process("sessionFactory1", configuration));
		}
	}
}
