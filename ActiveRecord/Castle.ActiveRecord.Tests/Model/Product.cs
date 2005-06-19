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
	using System.Collections;
	using Iesi.Collections;

	[ActiveRecord("Product")]
	public class Product : ActiveRecordBase
	{
		private int id;
		private string product_name;
		private float price;
		private string serial_number;
		private ISet _orders;

		[PrimaryKey(PrimaryKeyType.Native, "ProductID")]
		public int ID
		{
			get { return this.id; }
			set { this.id = value; }
		}

		[Property()]
		public string Name
		{
			get { return this.product_name; }
			set { this.product_name = value; }
		}


		[Property()]
		public string SerialNumber
		{
			get { return this.serial_number; }
			set { this.serial_number = value; }
		}

		[Property()]
		public float Price
		{
			get { return this.price; }
			set { this.price = value; }
		}


		[HasAndBelongsToMany(typeof (Order), RelationType.Set,
			Table="line_item",
			ColumnRef="order_id", ColumnKey="product_id", Inverse=true)]
		public ISet Orders
		{
			get { return _orders; }
			set { _orders = value; }
		}

		public static Product Find(int id)
		{
			return ((Product) (ActiveRecordBase.FindByPrimaryKey(typeof (Product), id)));
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll(typeof (Product));
		}
	}

	[ActiveRecord("Product")]
	public class ProductWithIDBag : ActiveRecordBase
	{
		private int id;
		private string product_name;
		private float price;
		private string serial_number;
		private IList _orders;

		[PrimaryKey(PrimaryKeyType.Native, "ProductID")]
		public int ID
		{
			get { return this.id; }
			set { this.id = value; }
		}

		[Property()]
		public string Name
		{
			get { return this.product_name; }
			set { this.product_name = value; }
		}


		[Property()]
		public string SerialNumber
		{
			get { return this.serial_number; }
			set { this.serial_number = value; }
		}

		[Property()]
		public float Price
		{
			get { return this.price; }
			set { this.price = value; }
		}


		[HasAndBelongsToMany(typeof (OrderWithIDBag), RelationType.IdBag,
			Table="line_item_non_ident",
			ColumnRef="order_id", ColumnKey="product_id"),
			CollectionID(CollectionIDType.HiLo, "line_number", "Int32"),
			Hilo(Table="testing_hilo", Column="sequence", MaxLo=150)]
		public IList Orders
		{
			get { return _orders; }
			set { _orders = value; }
		}

		public static ProductWithIDBag Find(int id)
		{
			return ((ProductWithIDBag) (ActiveRecordBase.FindByPrimaryKey(typeof (ProductWithIDBag), id)));
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll(typeof (ProductWithIDBag));
		}
	}
}
