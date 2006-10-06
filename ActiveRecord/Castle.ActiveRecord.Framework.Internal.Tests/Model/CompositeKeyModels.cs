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

namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
	using System;

	#region CompositeKey2

	[Serializable]
	public class CompositeKey2
	{
		private int key1;
		private String key2;

		[KeyProperty]
		public int Key1
		{
			get { return key1; }
			set { key1 = value; }
		}

		[KeyProperty]
		public string Key2
		{
			get { return key2; }
			set { key2 = value; }
		}

		public override int GetHashCode()
		{
			return key1 + 29*(key2 != null ? key2.GetHashCode() : 0);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			CompositeKey2 compositeKey2 = obj as CompositeKey2;
			if (compositeKey2 == null)
			{
				return false;
			}
			if (key1 != compositeKey2.key1)
			{
				return false;
			}
			if (!Equals(key2, compositeKey2.key2))
			{
				return false;
			}
			return true;
		}
	}

	#endregion

	/// <summary>
	/// Simplest case for CompositeKey support on AR
	/// </summary>
	[ActiveRecord]
	public class ClassWithCompositeKey2 : ActiveRecordBase
	{
		private CompositeKey2 key;

		[CompositeKey]
		public CompositeKey2 Key
		{
			get { return this.key; }
			set { this.key = value; }
		}
	}

	#region CompositeKey3

	[Serializable]
	public class CompositeKey3
	{
		private Product product;
		private int someOtherKey;

		[BelongsTo]
		public Product Product
		{
			get { return this.product; }
			set { this.product = value; }
		}

		[KeyProperty]
		public int SomeOtherKey
		{
			get { return this.someOtherKey; }
			set { this.someOtherKey = value; }
		}

		public override int GetHashCode()
		{
			return product.GetHashCode() + 29*someOtherKey;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			CompositeKey3 compositeKey3 = obj as CompositeKey3;
			if (compositeKey3 == null)
			{
				return false;
			}
			if (!Equals(product, compositeKey3.product))
			{
				return false;
			}
			if (someOtherKey != compositeKey3.someOtherKey)
			{
				return false;
			}
			return true;
		}
	}

	#endregion

	/// <summary>
	/// In this case the composite key has a BelongsTo, which should be supported
	/// as well (must generate key-many-to-one instead of key-property)
	/// </summary>
	[ActiveRecord]
	public class ClassWithCompositeKey3 : ActiveRecordBase
	{
		private CompositeKey3 key;

		[CompositeKey]
		public CompositeKey3 Key
		{
			get { return this.key; }
			set { this.key = value; }
		}
	}

	[ActiveRecord("Product")]
	public class Product : ActiveRecordBase
	{
		private int id;
		private string product_name;
		private float price;
		private string serial_number;

		[PrimaryKey(PrimaryKeyType.Native, "ProductID")]
		public int ID
		{
			get { return this.id; }
			set { this.id = value; }
		}

		[Property]
		public string Name
		{
			get { return this.product_name; }
			set { this.product_name = value; }
		}

		[Property]
		public string SerialNumber
		{
			get { return this.serial_number; }
			set { this.serial_number = value; }
		}

		[Property]
		public float Price
		{
			get { return this.price; }
			set { this.price = value; }
		}
	}
}
