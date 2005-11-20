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
	
	using NHibernate.Expression;


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

		public bool IsRoot
		{
			get { return Parent == null; }
		}

		/// <summary>
		/// Lazy means: load the collections content only when used.
		/// and inverse means that the other end controls 
		/// the relation
		/// </summary>
		[HasMany( typeof(Category), Inverse=true, Lazy=true, OrderBy="Name ASC" )]
		public ISet SubCategories
		{
			get { return subcategories; }
			set { subcategories = value; }
		}

		/// <summary>
		/// The Inverse is usually a source of confusion, but it
		/// more or like make this relation informative, or readonly.
		/// <see cref="Product"/> is the entity that dictates
		/// the relation.
		/// </summary>
		[HasMany( typeof(Product), Inverse=true, Lazy=true )]
		public ISet Products
		{
			get { return products; }
			set { products = value; }
		}

		/// <summary>
		/// We have overrided the implementation of 
		/// ToString only because this class in being 
		/// used on the ActiveRecord Scaffolding
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "" + name;
		}

		public static Category[] FindAll()
		{
			return (Category[]) FindAll( 
				typeof(Category), new Order[] { Order.Asc("Name") } );
		}
	}
}
