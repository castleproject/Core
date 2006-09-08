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

	[ActiveRecord("TSAS_ProdLicense")]
	public class ProductLicense : ActiveRecordBase
	{
		private int id;
		private DateTime created;
		private DateTime expires;
		private ISet accounts = new ListSet();

		public ProductLicense()
		{
			created = DateTime.Now;
			expires = DateTime.Now.AddDays(1);
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property(Update=false, Insert=true)]
		public DateTime Created
		{
			get { return created; }
			set { created = value; }
		}

		[Property]
		public DateTime Expires
		{
			get { return expires; }
			set { expires = value; }
		}

		[HasMany(typeof(Account), Inverse=false)]
		public ISet Accounts
		{
			get { return accounts; }
			set { accounts = value; }
		}

		public override string ToString()
		{
			return String.Format("License Created at {0} Expires at {1}", 
				created.ToShortDateString(), expires.ToShortDateString());
		}
		
		public static ProductLicense[] FindAll()
		{
			return (ProductLicense[]) ActiveRecordBase.FindAll(typeof(ProductLicense));
		}
	}
}
