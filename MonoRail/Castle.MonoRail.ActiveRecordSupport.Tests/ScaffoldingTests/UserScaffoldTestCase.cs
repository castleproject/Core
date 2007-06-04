namespace Castle.MonoRail.ActiveRecordSupport.Tests.ScaffoldingTests
{
	using System;
	using Castle.ActiveRecord;
	using Castle.MonoRail.TestSupport;
	using NUnit.Framework;
	using TestSiteARSupport.Model;
	using WatiN.Core;
	using Attribute=WatiN.Core.Attribute;

	[TestFixture]
	public class UserScaffoldTestCase : BaseAcceptanceTestCase
	{
		private Account account1, account2, account3;
		private User user;

		protected override void CreateTestData()
		{
			account1 = new Account("AdWords", "email1@gmail.net", "abc123");
			account2 = new Account("AdSense", "email1@gmail.net", "abc123");
			account3 = new Account("Orkut", "email1@gmail.net", "abc123");
		
			account1.Create();
			account2.Create();
			account3.Create();
		}

		[Test]
		public void CreateUser()
		{
			ie.GoTo("http://localhost:88/UserScaffold/new.castle");

			ie.Button(new Value("Create")).Click(); // Trying to save will spark the validation

			Assert.AreEqual("http://localhost:88/UserScaffold/new.castle", ie.Url);

			Assert.IsTrue(ie.Elements.Exists("advice-required-User_Name"));
			Assert.AreEqual("This is a required field", ie.Element("advice-required-User_Name").InnerHtml);

			ie.TextField("User_Name").TypeText("John Doe");
			ie.SelectList("User_Account_Id").SelectByValue(account1.Id.ToString());

			ie.Button(new Value("Create")).Click(); // Now it should save

			Assert.AreEqual("http://localhost:88/UserScaffold/list.castle", ie.Url);

			ElementCollection elements = ie.Elements.Filter(new Attribute("className", "idRow"));
		
			Assert.IsTrue(elements.Length > 0, "Newly added object not present on the list?");

			int id = Convert.ToInt32(elements[elements.Length - 1].InnerHtml);

			user = ActiveRecordMediator<User>.FindByPrimaryKey(id);

			Assert.AreEqual("John Doe", user.Name);
			Assert.IsNotNull(user.Account);
			Assert.AreEqual(account1.Id, user.Account.Id);
		}

		[Test]
		public void EditUser()
		{
			CreateUser();

			ie.GoTo("http://localhost:88/UserScaffold/edit.castle?id=" + user.Id);

			ie.TextField("User_Name").TypeText("");
			ie.Button(new Value("Save Changes")).Click(); // Trying to save will spark the validation

			Assert.AreEqual("http://localhost:88/UserScaffold/edit.castle?id=" + user.Id, ie.Url);

			Assert.IsTrue(ie.Elements.Exists("advice-required-User_Name"));
			Assert.AreEqual("This is a required field", ie.Element("advice-required-User_Name").InnerHtml);

			ie.TextField("User_Name").TypeText("Mary Jane");
			ie.SelectList("User_Account_Id").SelectByValue(account2.Id.ToString());

			ie.Button(new Value("Save Changes")).Click(); // Now it should save

			Assert.AreEqual("http://localhost:88/UserScaffold/list.castle", ie.Url);

			ElementCollection elements = ie.Elements.Filter(new Attribute("className", "idRow"));

			Assert.IsTrue(elements.Length > 0, "Changed object not present on the list?");

			int id = Convert.ToInt32(elements[elements.Length - 1].InnerHtml);

			user = ActiveRecordMediator<User>.FindByPrimaryKey(id);

			Assert.AreEqual("Mary Jane", user.Name);
			Assert.IsNotNull(user.Account);
			Assert.AreEqual(account2.Id, user.Account.Id);
		}
	}
}
