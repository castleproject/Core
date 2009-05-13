// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

	[ActiveRecord("animal", DiscriminatorColumn = "animal_type", DiscriminatorType = "String", DiscriminatorValue = "animal"), JoinedBase]
	public class Animal<T> : ActiveRecordBase<T> where T : Animal<T>
	{
		private int id;
		private string name;

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
	}

	[ActiveRecord("animal_cat", DiscriminatorValue = "cat")]
	public class Cat : Animal<Cat>
	{
		private string breed;
		private int animal_id;

		[JoinedKey("animal_id")]
		public int AnimalId
		{
			get { return animal_id; }
			set { animal_id = value; }
		}

		[Property("breed")]
		public string Breed
		{
			get { return breed; }
			set { breed = value; }
		}
	}

	[ActiveRecord("animal_dog", DiscriminatorValue = "dog")]
	public class Dog : Animal<Dog>
	{
		private int animal_id;
		private string master_name;

		[JoinedKey]
		public int AnimalId
		{
			get { return animal_id; }
			set { animal_id = value; }
		}

		[Property("master_name")]
		public string MastersName
		{
			get { return master_name; }
			set { master_name = value; }
		}
	}
}
