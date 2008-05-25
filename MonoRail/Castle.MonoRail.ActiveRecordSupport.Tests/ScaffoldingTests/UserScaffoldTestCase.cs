// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.MonoRail.ActiveRecordSupport.Tests.ScaffoldingTests
{
	using System;
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;
	using NUnit.Framework;
	using TestSiteARSupport.Model;
	using WatiN.Core;
	using WatiN.Core.Interfaces;

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

			account1.Password = account2.Password = account3.Password = "123abcd";
			account1.ConfirmationPassword = account2.ConfirmationPassword = account3.ConfirmationPassword = "123abcd";

			account1.Create();
			account2.Create();
			account3.Create();
		}

		[Test]
		public void CreateUser()
		{
			ie.GoTo("http://localhost:88/UserScaffold/new.castle");

			ie.Button(Find.ByValue("Create")).Click(); // Trying to save will spark the validation

			Assert.AreEqual("http://localhost:88/UserScaffold/new.castle", ie.Url);

			Assert.IsTrue(ie.Elements.Exists("advice-required-User_Name"));
			Assert.AreEqual("This is a required field.", ie.Element("advice-required-User_Name").InnerHtml);

			ie.TextField("User_Name").TypeText("John Doe");
			ie.SelectList("User_Account_Id").SelectByValue(account1.Id.ToString());

			ie.Button(Find.ByValue("Create")).Click(); // Now it should save

			Assert.AreEqual("http://localhost:88/UserScaffold/list.castle", ie.Url);

			IWatiNElementCollection elements = ie.Elements.Filter(Find.ByClass("idRow"));
		
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
			ie.Button(Find.ByValue("Save Changes")).Click(); // Trying to save will spark the validation

			Assert.AreEqual("http://localhost:88/UserScaffold/edit.castle?id=" + user.Id, ie.Url);

			Assert.IsTrue(ie.Elements.Exists("advice-required-User_Name"));
			Assert.AreEqual("This is a required field.", ie.Element("advice-required-User_Name").InnerHtml);

			ie.TextField("User_Name").TypeText("Mary Jane");
			ie.SelectList("User_Account_Id").SelectByValue(account2.Id.ToString());

			ie.Button(Find.ByValue("Save Changes")).Click(); // Now it should save

			Assert.AreEqual("http://localhost:88/UserScaffold/list.castle", ie.Url);

			IWatiNElementCollection elements = ie.Elements.Filter(Find.ByClass("idRow"));

			Assert.IsTrue(elements.Length > 0, "Changed object not present on the list?");

			int id = Convert.ToInt32(elements[elements.Length - 1].InnerHtml);

			user = ActiveRecordMediator<User>.FindByPrimaryKey(id);

			Assert.AreEqual("Mary Jane", user.Name);
			Assert.IsNotNull(user.Account);
			Assert.AreEqual(account2.Id, user.Account.Id);
		}

		[Test]
		public void ConfirmAndDeleteNewlyCreatedUser()
		{
			CreateUser();

			Assert.AreEqual("http://localhost:88/UserScaffold/list.castle", ie.Url);

			IWatiNElementCollection elements = ie.Elements.Filter(Find.ByClass("deletelink"));

			Link delLink = null;

			foreach(Link link in elements)
			{
				if (link.Url.EndsWith("confirm.castle?id=" + user.Id))
				{
					delLink = link;
					break;
				}
			}

			Assert.IsNotNull(delLink);

			delLink.Click();

			ie.Button(Find.ByValue("Yes")).Click();

			try
			{
				ActiveRecordMediator<User>.Refresh(user);
			
				Assert.Fail("Expecting exception as the user was removed");
			}
			catch(ActiveRecordException)
			{
				
			}
		}
	}
}
