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

namespace Castle.ActiveRecord.Tests
{
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;

	[TestFixture]
	public class BugTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void SemanticVisitorBug()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(),
			                               typeof(Bank), typeof(Customer), typeof(Card));

			Recreate();
		}

		[Test]
		public void AR170Test()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(),
			                               typeof(HierarchyItem), typeof(Folder), typeof(FolderBase));

			Recreate();
		}

		[Test]
		public void InheritanceBug()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(),
			                               typeof(Parent), typeof(Child), typeof(GrandChild));
		}
	}

	#region SemanticVisitorBug related

	[ActiveRecord]
	public class Bank : ActiveRecordBase
	{
		private int id;
		private IList customers;
		private Card card;

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[BelongsTo]
		public Card Card
		{
			get { return card; }
			set { card = value; }
		}

		[HasMany(typeof(Customer))]
		public IList Customers
		{
			get { return customers; }
			set { customers = value; }
		}
	}

	[ActiveRecord]
	public class Customer : ActiveRecordBase
	{
		private int id;
		private IList cards;
		private Bank bank;

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[BelongsTo]
		public Bank Bank
		{
			get { return bank; }
			set { bank = value; }
		}

		[HasMany(typeof(Card))]
		public IList Cards
		{
			get { return cards; }
			set { cards = value; }
		}
	}

	[ActiveRecord]
	public class Card : ActiveRecordBase
	{
		private int id;
		private Customer customer;

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[BelongsTo]
		public Customer Customer
		{
			get { return customer; }
			set { customer = value; }
		}
	}

	#endregion

	#region InheritanceBug related

	public class Parent : ActiveRecordBase
	{
		private int id;

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}
	}

	public class Child : Parent
	{
	}

	[ActiveRecord]
	public class GrandChild : Child
	{
	}

	#endregion

	#region AR-170

	[ActiveRecord("HierarchyItems",
		DiscriminatorColumn = "Type",
		DiscriminatorType = "String",
		DiscriminatorValue = "HierarchyItem")]
	public class HierarchyItem : ActiveRecordBase
	{
		private int _id;
		private Folder _parent;

		[PrimaryKey]
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[BelongsTo]
		public Folder Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}
	}

	[ActiveRecord("FolderBases",
		DiscriminatorValue = "FolderBase")]
	public abstract class FolderBase : HierarchyItem
	{
		public abstract IList<HierarchyItem> Children { get; }
	}

	[ActiveRecord("PhysicalFolders",
		DiscriminatorValue = "PhysicalFolder")]
	public class Folder : FolderBase
	{
		private IList<HierarchyItem> children = new List<HierarchyItem>();

		[HasMany(Access = PropertyAccess.FieldCamelcase)]
		public override IList<HierarchyItem> Children
		{
			get { return children; }
		}
	}

	#endregion
}