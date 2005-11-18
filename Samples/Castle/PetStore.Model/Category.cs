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

namespace PetStore.Model
{
	using System;

	using Castle.ActiveRecord;
	
	using Iesi.Collections;


	[ActiveRecord]
	public class Category : ActiveRecordBase
	{
		private int id;
		private String name;
		private Category parent;
		private ISet subcategories = new HashedSet();
		private ISet products = new HashedSet();

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

		[BelongsTo("parent_category_id")]
		public Category Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		[HasMany( typeof(Category), Inverse=true, Lazy=true )]
		public ISet Subcategories
		{
			get { return subcategories; }
			set { subcategories = value; }
		}

		[HasMany( typeof(Product), Inverse=true, Lazy=true )]
		public ISet Products
		{
			get { return products; }
			set { products = value; }
		}

		public override string ToString()
		{
			return "" + name;
		}
	}
}
