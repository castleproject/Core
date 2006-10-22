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

namespace FormHelperSample.Models
{
	using System;

	public class Product
	{
		private int id;
		private String name;
		private float price;
		private bool isAvailable;
		private Supplier supplier;

		public Product()
		{
		}

		public Product(int id, string name, float price, Supplier supplier)
		{
			this.id = id;
			this.name = name;
			this.price = price;
			this.supplier = supplier;
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

		public float Price
		{
			get { return price; }
			set { price = value; }
		}

		public bool IsAvailable
		{
			get { return isAvailable; }
			set { isAvailable = value; }
		}

		public Supplier Supplier
		{
			get { return supplier; }
			set { supplier = value; }
		}
	}
}
