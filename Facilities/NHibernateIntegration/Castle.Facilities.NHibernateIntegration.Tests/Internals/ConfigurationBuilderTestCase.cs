namespace Castle.Facilities.NHibernateIntegration.Tests.Internals
{
	using NHibernate;
	using NHibernate.Cfg;
	using Castle.Facilities.NHibernateIntegration.Tests.Common;
	using NUnit.Framework;

	[TestFixture]
	public class ConfigurationBuilderTestCase : AbstractNHibernateTestCase
	{
		protected override string ConfigurationFile
		{
			get
			{
				return "Internals/TwoDatabaseConfiguration.xml";
			}
		}
		[Test]
		public void SaveUpdateListenerAdded()
		{
			Configuration cfg = (Configuration)container["sessionFactory4.cfg"];
			Assert.AreEqual(1, cfg.EventListeners.SaveOrUpdateEventListeners.Length);
			Assert.AreEqual(typeof(CustomSaveUpdateListener),cfg.EventListeners.SaveOrUpdateEventListeners[0].GetType());

			Assert.AreEqual(1, cfg.EventListeners.DeleteEventListeners.Length);
			Assert.AreEqual(typeof(CustomDeleteListener), cfg.EventListeners.DeleteEventListeners[0].GetType());
		}
	}
}
