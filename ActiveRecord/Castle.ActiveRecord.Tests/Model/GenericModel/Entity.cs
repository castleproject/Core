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

#if DOTNET2

namespace Castle.ActiveRecord.Tests.Model.GenericModel
{
	using System;
	using System.Collections;

	using NHibernate;

    using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;

	[ActiveRecord("genericentity"), JoinedBase]
	public abstract class Entity<T> : ActiveRecordBase<T> where T : class 
	{
		private int id;
		private string name;
		private string type;

		protected Entity()
		{
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[Property]
		public string Type
		{
			get { return type; }
			set { type = value; }
		}

	}

	[ActiveRecord("genericentitycompany")]
	public class CompanyEntity : Entity<CompanyEntity>
	{
		private byte company_type;
		private int comp_id;

		[JoinedKey("comp_id")]
		public int CompId
		{
			get { return comp_id; }
			set { comp_id = value; }
		}

		[Property("company_type")]
		public byte CompanyType
		{
			get { return company_type; }
			set { company_type = value; }
		}

	}

	[ActiveRecord("genericentityperson")]
	public class PersonEntity : Entity<PersonEntity>
	{
		private int person_id;

		[JoinedKey]
		public int Person_Id
		{
			get { return person_id; }
			set { person_id = value; }
		}
	}
}

#endif