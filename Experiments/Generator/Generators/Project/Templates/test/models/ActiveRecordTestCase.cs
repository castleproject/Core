using System;
using NUnit.Framework;
using Castle.ActiveRecord;
using <%= ClassName %>;

namespace <%= ClassName %>.Tests.Models
{
	public class ActiveRecordTestCase {
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			Boot.InitializeActiveRecord("test", false);
		}
		
		[SetUp]
		public virtual void SetUp()
		{
			PrepareSchema();
		}

		[TearDown]
		public virtual void TearDown()
		{
			DropSchema();
		}

		protected virtual void PrepareSchema()
		{
			// If you want to delete everything from the model.
			// Remember to do it in a descendent dependency order

			// Office.DeleteAll();
			// User.DeleteAll();

			// Another approach is to always recreate the schema 
			// (please use a separate test database if you want to do that)

			ActiveRecordStarter.CreateSchema();
		}

		protected virtual void DropSchema()
		{
			ActiveRecordStarter.DropSchema();
		}
	}
}