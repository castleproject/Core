using System;
using NUnit.Framework;
using <%= Namespace %>;

namespace <%= TestsNamespace %>
{
	/// <summary>
	/// Test unitaire pour <%= Name %>.
	/// </summary>
	[TestFixture]
	public class <%= ClassName %>Test : ActiveRecordTestCase
	{
		[Test]
		public void Create()
		{
			<%= ClassName %> new<%= ClassName %> = new <%= ClassName %>();
			
			// Initialize new<%= ClassName %>

			new<%= ClassName %>.Create();

			<%= ClassName %> db<%= ClassName %> = <%= ClassName %>.FindAll()[0];

			//Assert.AreEqual(new<%= ClassName %>.UnAttribut, db<%= ClassName %>.UnAttribut);
		}

		[Test]
		public void FindAll()
		{
			Create();

			Assert.AreEqual(1, <%= ClassName %>.FindAll().Length);
		}

		[Test]
		public void Delete()
		{
			Create();

			<%= ClassName %>.FindAll()[0].Delete();

			Assert.AreEqual(0, <%= ClassName %>.FindAll().Length);
		}

	}
}
