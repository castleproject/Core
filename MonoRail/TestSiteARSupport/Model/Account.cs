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

namespace TestSiteARSupport.Model
{
	using System;
	using System.Collections.Generic;
	using Castle.ActiveRecord;
	using Castle.Components.Validator;
	using Iesi.Collections.Generic;
	using ValidateEmailAttribute=Castle.Components.Validator.ValidateEmailAttribute;

	[ActiveRecord("TSAS_Account")]
	public class Account : ActiveRecordValidationBase
	{
		private int id;
		private String name;
		private String email;
		private String password;
		private String confirmationpassword;
		private ProductLicense productLicense;
		private ISet<AccountPermission> permissions;
		private IList<User> users = new List<User>();

		public Account()
		{
		}

		public Account(string name, string email, string password)
		{
			this.name = name;
			this.email = email;
			this.password = password;
			this.confirmationpassword = password;
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property, ValidateNonEmpty]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[Property, ValidateNonEmpty, ValidateEmail]
		public string Email
		{
			get { return email; }
			set { email = value; }
		}

		[Property, ValidateNonEmpty]
		public string Password
		{
			get { return password; }
			set { password = value; }
		}

		[ValidateSameAs("Password")]
		public string ConfirmationPassword
		{
			get { return confirmationpassword; }
			set { confirmationpassword = value; }
		}

		[BelongsTo("license_id")]
		public ProductLicense ProductLicense
		{
			get { return productLicense; }
			set { productLicense = value; }
		}

		[HasAndBelongsToMany(
			Table="AccountAccountPermission", 
			ColumnRef="permission_id", ColumnKey="account_id", Inverse=false)]
		public ISet<AccountPermission> Permissions
		{
			get { return permissions; }
			set { permissions = value; }
		}

		[HasMany]
		public IList<User> Users
		{
			get { return users; }
			set { users = value; }
		}

		public override string ToString()
		{
			return name;
		}

		public static Account[] FindAll()
		{
			return (Account[]) FindAll(typeof(Account));
		}
	}
}
