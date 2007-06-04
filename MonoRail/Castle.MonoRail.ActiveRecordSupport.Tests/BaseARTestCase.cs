namespace Castle.MonoRail.ActiveRecordSupport.Tests
{
	using System.Reflection;
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Config;
	using NUnit.Framework;

	public abstract class BaseARTestCase
	{
		[TestFixtureSetUp]
		public virtual void InitFixture()
		{
			IConfigurationSource source = ActiveRecordSectionHandler.Instance;
			ActiveRecordStarter.ResetInitializationFlag();
			ActiveRecordStarter.Initialize(Assembly.Load("TestSiteARSupport"), source);
		}

		[TestFixtureTearDown]
		public virtual void TerminateFixture()
		{
		}

		[SetUp]
		public virtual void InitTest()
		{
			ActiveRecordStarter.CreateSchema();

			CreateTestData();
		}

		[TearDown]
		public virtual void TearDown()
		{
			ActiveRecordStarter.DropSchema();
		}

		protected virtual void CreateTestData()
		{
		}
	}
}
