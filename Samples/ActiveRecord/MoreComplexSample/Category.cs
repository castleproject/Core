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

	[ActiveRecord]
	public class Category : ActiveRecordBase
	{
		private int id;
		private String name;
		private Category parent;
		private ISet children = new HashedSet();
		private ISet products = new HashedSet();

		public Category()
		{
		}

		public Category(string name)
		{
			this.name = name;
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

		[BelongsTo("ParentId")]
		public Category Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		[HasMany(typeof(Category), Lazy=true, Inverse=true)]
		public ISet Children
		{
			get { return children; }
			set { children = value; }
		}
		
		[HasAndBelongsToMany(typeof(Product), 
			Table="ProductCategory", ColumnKey="CategoryId", ColumnRef="ProductId", 
			Inverse=true, Lazy=true)]
		public ISet Products
		{
			get { return products; }
			set { products = value; }
		}
	}
}
