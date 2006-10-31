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

namespace MoreComplexSample
{
	using System;
	using Castle.ActiveRecord;
	using Iesi.Collections;

	[ActiveRecord("Tb_Customers")]
	public class Customer : ActiveRecordValidationBase
	{
		private int id;
		private String name;
		private String email;
		private ISet orders = new HashedSet();

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property("cust_Name"), ValidateNotEmpty, ValidateLength(10, 35)]
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

		[HasMany(typeof(Order), Lazy=true)]
		public ISet Orders
		{
			get { return orders; }
			set { orders = value; }
		}

		public static Customer Find(int id)
		{
			return (Customer) FindByPrimaryKey(typeof(Customer), id);
		}

		public static Customer FindByEmail(String email)
		{
			return (Customer) FindOne(typeof(Customer),
				                        NHibernate.Expression.Expression.Eq("Email", email));
		}
	}
}