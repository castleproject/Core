// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Tests.Model
{
	using System;

	[ActiveRecord("entity"), JoinedBase]
	public class Entity : ActiveRecordBase
	{
		private int id;
		private string name;
		private string type;

		public Entity()
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

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll( typeof(Entity) );
		}

		public static Entity[] FindAll()
		{
			return (Entity[]) ActiveRecordBase.FindAll( typeof(Entity) );
		}

		public static Entity Find(int id)
		{
			return (Entity) ActiveRecordBase.FindByPrimaryKey( typeof(Entity), id );
		}
	}

	[ActiveRecord("entitycompany")]
	public class CompanyEntity : Entity
	{
		private byte company_type;
		private int comp_id;

		[Key("comp_id")]
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

		public new static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll( typeof(CompanyEntity) );
		}

		public new static CompanyEntity[] FindAll()
		{
			return (CompanyEntity[]) ActiveRecordBase.FindAll( typeof(CompanyEntity) );
		}

		public new static CompanyEntity Find(int id)
		{
			return (CompanyEntity) ActiveRecordBase.FindByPrimaryKey( typeof(CompanyEntity), id );
		}
	}

	[ActiveRecord("entityperson")]
	public class PersonEntity : Entity
	{
		private int person_id;

		[Key]
		public int Person_Id
		{
			get { return person_id; }
			set { person_id = value; }
		}

		public new static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll( typeof(PersonEntity) );
		}

		public new static PersonEntity[] FindAll()
		{
			return (PersonEntity[]) ActiveRecordBase.FindAll( typeof(PersonEntity) );
		}

		public new static PersonEntity Find(int id)
		{
			return (PersonEntity) ActiveRecordBase.FindByPrimaryKey( typeof(PersonEntity), id );
		}
	}
}
