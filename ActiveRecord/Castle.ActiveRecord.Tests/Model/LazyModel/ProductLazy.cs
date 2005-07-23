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

namespace Castle.ActiveRecord.Tests.Model.LazyModel
{
	using System;

	using Iesi.Collections;


	[ActiveRecord]
	public class ProductLazy : ActiveRecordValidationBase
	{
		private int id;
		private ISet categories = new HashedSet();

		public ProductLazy()
		{
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[HasMany( typeof(CategoryLazy), Lazy=true )]
		public ISet Categories
		{
			get { return categories; }
			set { categories = value; }
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll( typeof(ProductLazy) );
		}

		public static ProductLazy[] FindAll()
		{
			return (ProductLazy[]) ActiveRecordBase.FindAll( typeof(ProductLazy) );
		}

		public static ProductLazy Find(int id)
		{
			return (ProductLazy) ActiveRecordBase.FindByPrimaryKey( typeof(ProductLazy), id );
		}
	}

	[ActiveRecord]
	public class CategoryLazy : ActiveRecordValidationBase
	{
		private int id;
		private String name;
		private ProductLazy product;

		public CategoryLazy()
		{
		}

		public CategoryLazy(String name)
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

		[BelongsTo("product_id")]
		public ProductLazy Product
		{
			get { return product; }
			set { product = value; }
		}
	}
}
