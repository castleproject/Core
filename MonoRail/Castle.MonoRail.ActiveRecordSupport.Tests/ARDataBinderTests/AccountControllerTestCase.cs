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

namespace Castle.MonoRail.ActiveRecordSupport.Tests.ARDataBinderTests
{
	using System;
	using Castle.ActiveRecord;
	using NUnit.Framework;
	using TestSiteARSupport.Model;
	using WatiN.Core;

	[TestFixture, Ignore("can't run successfully under cassini")]
	public class AccountControllerTestCase : BaseAcceptanceTestCase
	{
		private ProductLicense lic1, lic2;
		private AccountPermission perm1, perm2;
		private User user1, user2;
		private Account account;

		protected override void CreateTestData()
		{
			lic1 = new ProductLicense();
			lic2 = new ProductLicense();

			perm1 = new AccountPermission("Permission 1");
			perm2 = new AccountPermission("Permission 2");

			user1 = new User("John Doe");
			user2 = new User("Mary Jane");

			ActiveRecordMediator<ProductLicense>.Create(lic1);
			ActiveRecordMediator<ProductLicense>.Create(lic2);
			ActiveRecordMediator<AccountPermission>.Create(perm1);
			ActiveRecordMediator<AccountPermission>.Create(perm2);
			ActiveRecordMediator<User>.Create(user1);
			ActiveRecordMediator<User>.Create(user2);
		}

		[Test]
		public void CreateAccount()
		{
			ie.GoTo("http://localhost:88/Account/new.castle");

			ie.Button(Find.ByValue("Insert")).Click(); // Trying to save will spark the validation

			Assert.IsTrue(ie.Elements.Exists("advice-required-account_name"));
			Assert.IsTrue(ie.Elements.Exists("advice-required-account_email"));
			Assert.IsTrue(ie.Elements.Exists("advice-required-account_password"));

			Assert.AreEqual("This is a required field.", ie.Element("advice-required-account_name").InnerHtml);
			Assert.AreEqual("This is a required field. Please enter a valid email address. For example fred@domain.com.", ie.Element("advice-required-account_email").InnerHtml);
			Assert.AreEqual("This is a required field.", ie.Element("advice-required-account_password").InnerHtml);

			// Passwords wont match
			ie.TextField("account_password").TypeText("123");
			ie.TextField("account_confirmationpassword").TypeText("321");

			Assert.IsTrue(ie.Elements.Exists("advice-validate-same-as-account_Password-account_confirmationpassword"));
			Assert.AreEqual("Fields do not match.", ie.Element("advice-validate-same-as-account_Password-account_confirmationpassword").InnerHtml);

			// This should fix it
			ie.TextField("account_password").TypeText("123987");
			ie.TextField("account_confirmationpassword").TypeText("123987");
		
			ie.TextField("account_name").TypeText("My first account");
			ie.TextField("account_email").TypeText("johndoe@gmail.com");

			ie.SelectList("account_ProductLicense_id").SelectByValue(lic1.Id.ToString());

			ie.CheckBox("account_permissions").Checked = true;
			ie.CheckBox("account_Permissions_1_").Checked = true;

			ie.CheckBox("account_users").Checked = true;
			ie.CheckBox("account_Users_1_").Checked = true;

			ie.Button(Find.ByValue("Insert")).Click();

			int accountId = Convert.ToInt32(ie.Element("newid").InnerHtml);

			account = ActiveRecordMediator<Account>.FindByPrimaryKey(accountId);

			Assert.AreEqual("My first account", account.Name);
			Assert.AreEqual("johndoe@gmail.com", account.Email);
			Assert.AreEqual("123987", account.Password);
			
			Assert.IsNotNull(account.ProductLicense);
			Assert.AreEqual(lic1.Id, account.ProductLicense.Id);
			
			Assert.AreEqual(2, account.Permissions.Count);
			Assert.AreEqual(2, account.Users.Count);
		}

		[Test]
		public void RemovingElementsFromCollections()
		{
			CreateAccount();

			ie.GoTo("http://localhost:88/Account/edit.castle?id=" + account.Id);

			ie.TextField("account_password").TypeText("123987");
			ie.TextField("account_confirmationpassword").TypeText("123987");

			ie.CheckBox("account_Permissions_0_").Checked = false;
			ie.CheckBox("account_Permissions_1_").Checked = false;

			ie.CheckBox("account_Users_0_").Checked = false;
			ie.CheckBox("account_Users_1_").Checked = false;

			ie.Button(Find.ByValue("Update")).Click();

			ActiveRecordMediator<Account>.Refresh(account);

			Assert.AreEqual("My first account", account.Name);
			Assert.AreEqual("johndoe@gmail.com", account.Email);
			Assert.AreEqual("123987", account.Password);

			Assert.AreEqual(0, account.Permissions.Count);
			Assert.AreEqual(0, account.Users.Count);
		}
	}
}
