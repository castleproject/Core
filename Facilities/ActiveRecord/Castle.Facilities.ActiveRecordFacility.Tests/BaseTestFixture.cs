using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace Castle.Facilities.ActiveRecord.Tests
{
	public abstract class BaseTestFixture
	{
		protected Configuration cfg;
		protected Dialect dialect;
		protected ISessionFactory sessions;

		[TearDown]
		public void Teardown()
		{
			DropSchema();
		}

		public void ExportSchema()
		{
			new SchemaExport( cfg ).Create( true, true );
		}

		public void DropSchema()
		{
			new SchemaExport( cfg ).Drop( true, true );
		}
	}
}
