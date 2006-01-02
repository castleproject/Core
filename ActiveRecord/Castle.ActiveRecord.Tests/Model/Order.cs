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

namespace Castle.ActiveRecord.Tests.Model
{
	using System;
	using System.Collections;

	using Iesi.Collections;


	[ActiveRecord("`Order`")]
	public class Order : ActiveRecordBase
	{
		private int id;
		private DateTime ordered_date;
		private Boolean shipped;
		private ISet _products;

		[PrimaryKey(PrimaryKeyType.Native, "OrderID")]
		public int ID
		{
			get { return this.id; }
			set { this.id = value; }
		}

		[Property()]
		public DateTime OrderedDate
		{
			get { return this.ordered_date; }
			set { this.ordered_date = value; }
		}

		[Property()]
		public Boolean IsShipped
		{
			get { return this.shipped; }
			set { this.shipped = value; }
		}

		public static Order Find(int id)
		{
			return ((Order) (ActiveRecordBase.FindByPrimaryKey(typeof (Order), id)));
		}

		[HasAndBelongsToMany(typeof (Product), RelationType.Set,
			Table="line_item",
			ColumnRef="product_id", ColumnKey="order_id")]
		public ISet Products
		{
			get { return _products; }
			set { _products = value; }
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll(typeof (Order));
		}
	}

	[ActiveRecord("`Order`")]
	public class OrderWithIDBag : ActiveRecordBase
	{
		private int id;
		private DateTime ordered_date;
		private Boolean shipped;
		private IList _products;

		[PrimaryKey(PrimaryKeyType.Native, "OrderID")]
		public int ID
		{
			get { return this.id; }
			set { this.id = value; }
		}

		[Property()]
		public DateTime OrderedDate
		{
			get { return this.ordered_date; }
			set { this.ordered_date = value; }
		}

		[Property()]
		public Boolean IsShipped
		{
			get { return this.shipped; }
			set { this.shipped = value; }
		}

		public static OrderWithIDBag Find(int id)
		{
			return ((OrderWithIDBag) (ActiveRecordBase.FindByPrimaryKey(typeof (OrderWithIDBag), id)));
		}

		[HasAndBelongsToMany(typeof (ProductWithIDBag), RelationType.IdBag,
			Table="line_item_non_ident",
			ColumnRef="product_id", ColumnKey="order_id"),
			CollectionID(CollectionIDType.HiLo, "line_number", "Int32"),
			Hilo(Table="testing_hilo", Column="sequence", MaxLo=150)]
		public IList Products
		{
			get { return _products; }
			set { _products = value; }
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll(typeof (OrderWithIDBag));
		}
	}
}