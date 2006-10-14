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

namespace DataBindSample.Model
{
	using System;

	/// <summary>
	/// This is a fairly complex type to exemplify
	/// the binding of simple values (id,name, inStock), 
	/// nested objects (supplierInfo) and arrays (categories)
	/// </summary>
	public class Product
	{
		private int id;
		private String name;
		private bool inStock;
		private SupplierInfo supplierInfo;
		private Category[] categories;

		public Product()
		{
		}

		public Product(int id, string name, bool inStock)
		{
			this.id = id;
			this.name = name;
			this.inStock = inStock;
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public bool InStock
		{
			get { return inStock; }
			set { inStock = value; }
		}

		public SupplierInfo SupplierInfo
		{
			get { return supplierInfo; }
			set { supplierInfo = value; }
		}

		public Category[] Categories
		{
			get { return categories; }
			set { categories = value; }
		}
	}
}
