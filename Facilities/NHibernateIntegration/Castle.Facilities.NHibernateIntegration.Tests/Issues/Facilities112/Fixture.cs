using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.Lifestyle;
using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;

namespace Castle.Facilities.NHibernateIntegration.Tests.Issues.Facilities112
{

	[TestFixture]
	public class LazyInitializationTestCase : IssueTestCase
	{
		protected override string ConfigurationFile
		{
			get
			{
				return "DefaultConfiguration.xml";
			}
		}

		[Test]
		public virtual void SessionFactory_is_lazily_initialized()
		{
			var handler = container.Kernel.GetHandler("sessionFactory1");
			var lifestyleManagerField = typeof (DefaultHandler).GetField("lifestyleManager",
			                                                             BindingFlags.NonPublic | BindingFlags.Instance |
			                                                             BindingFlags.GetField);
			var instanceField = typeof (SingletonLifestyleManager).GetField("instance",
			                                                                BindingFlags.NonPublic | BindingFlags.Instance |
			                                                                BindingFlags.GetField);
			var lifeStyleManager = lifestyleManagerField.GetValue(handler) as SingletonLifestyleManager;
			Assert.IsNotNull(lifeStyleManager);
			var instance = instanceField.GetValue(lifeStyleManager);
			Assert.IsNull(instance);

			var factory = container.Resolve<ISessionFactory>();

			instance = instanceField.GetValue(lifeStyleManager);
			Assert.IsNotNull(instance);
		}
		[Test]
		public virtual void SessionFactory_is_singleton()
		{
			var componentModel = container.Kernel.GetHandler("sessionFactory1").ComponentModel;
			Assert.AreEqual(LifestyleType.Singleton,componentModel.LifestyleType);
		}

	}
}
