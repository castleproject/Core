using System;
using NUnit.Framework;
using Castle.ActiveRecord.Framework.Internal.Tests.Model;

namespace Castle.ActiveRecord.Framework.Internal.Tests
{
	[TestFixture]
	public class GraphConnectorTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void CanConnectGrandChildren()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), 
				typeof(ClassDiscriminatorA),
				typeof(DiscriminatorGrandchild),
				typeof(ClassDiscriminatorParent));

			Assert.AreEqual(
				DomainModel.GetModel(typeof(ClassDiscriminatorA)),
				DomainModel.GetModel(typeof(DiscriminatorGrandchild)).Parent
			);
		}
	}
}
