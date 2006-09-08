// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

	using Castle.ActiveRecord;
	using Iesi.Collections;

	[ActiveRecord("TSAS_Account")]
	public class Account : ActiveRecordValidationBase
	{
		private int id;
		private String name;
		private String email;
		private String password;
		private String confirmationpassword;
		private ProductLicense productLicense;
		private ISet permissions;

		public Account()
		{
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property, ValidateNotEmpty]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[Property, ValidateNotEmpty, ValidateEmail]
		public string Email
		{
			get { return email; }
			set { email = value; }
		}

		[Property, ValidateNotEmpty, ValidateConfirmation("ConfirmationPassword")]
		public string Password
		{
			get { return password; }
			set { password = value; }
		}

		[ValidateNotEmpty]
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

		[HasAndBelongsToMany( typeof(AccountPermission), Table="AccountAccountPermission", 
			 ColumnRef="permission_id", ColumnKey="account_id", Inverse=false)]
		public ISet Permissions
		{
			get { return permissions; }
			set { permissions = value; }
		}
		
		public static Account[] FindAll()
		{
			return (Account[]) ActiveRecordBase.FindAll(typeof(Account));
		}
	}
}
