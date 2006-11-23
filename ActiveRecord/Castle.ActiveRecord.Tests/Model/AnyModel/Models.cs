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

namespace Castle.ActiveRecord.Tests.Model.AnyModel
{
	using System;
	using System.Collections;

	[ActiveRecord]
	public class Order : ActiveRecordBase
	{
		private int id;
		private IList _payments;

		public Order()
		{
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[HasManyToAny(typeof(IPayment), "pay_id", "payments_table", typeof(int),
			"Billing_Details_Type", "Billing_Details_Id", MetaType = typeof(string))]
		[Any.MetaValue("CREDIT_CARD", typeof(CreditCards))]
		[Any.MetaValue("BANK_ACCOUNT", typeof(BankAccounts))]
		public IList Payments
		{
			get { return _payments; }
			set { _payments = value; }
		}
	}

	[ActiveRecord]
	public class CreditCards : ActiveRecordBase, IPayment
	{
		private int id;
		private String name;
		private Order _order;

		public CreditCards()
		{
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[BelongsTo]
		public Order Order
		{
			get { return _order; }
			set { _order = value; }
		}

		[Property]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
	}

	[ActiveRecord]
	public class BankAccounts : ActiveRecordBase, IPayment
	{
		private int id;
		private String name;
		private bool credit;
		private Order _order;

		public BankAccounts()
		{
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[BelongsTo]
		public Order Order
		{
			get { return _order; }
			set { _order = value; }
		}

		[Property]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[Property]
		public bool Credit
		{
			get { return credit; }
			set { credit = value; }
		}
	}

	public interface IPayment
	{
		int Id { get; set; }
		Order Order { get; set; }
	}
}