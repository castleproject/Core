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

namespace Castle.ActiveRecord.Tests.Model.Nested
{
	using System;
	using System.Collections;

	[ActiveRecord]
	public class Operation : ActiveRecordBase
	{
		private int id;
		private PaymentPlan paymentPlan;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Nested]
		public PaymentPlan PaymentPlan
		{
			get { return paymentPlan; }
			set { paymentPlan = value; }
		}

		public static void DeleteAll()
		{
			DeleteAll(typeof(Operation));
		}

		public static Operation[] FindAll()
		{
			return (Operation[]) FindAll(typeof(Operation));
		}

		public static Operation Find(int id)
		{
			return (Operation) FindByPrimaryKey(typeof(Operation), id);
		}
	}

	public class PaymentPlan
	{
		private DateTime expirationDate;
		private int numberOfPayments;
		private Convention convention;
		private IList payments;

		[Property]
		public DateTime ExpirationDate
		{
			get { return expirationDate; }
			set { expirationDate = value; }
		}

		[Property]
		public int NumberOfPayments
		{
			get { return numberOfPayments; }
			set { numberOfPayments = value; }
		}

		[BelongsTo()]
		public Convention Convention
		{
			get { return convention; }
			set { convention = value; }
		}

		[HasMany(typeof(Payment), "PaymentPlan", "Payment")]
		public IList Payments
		{
			get { return payments; }
			set { payments = value; }
		}
	}

	[ActiveRecord]
	public class Convention : ActiveRecordBase
	{
		private int id;
		private string description;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string Description
		{
			get { return description; }
			set { description = value; }
		}
	}

	[ActiveRecord]
	public class Payment : ActiveRecordBase
	{
		private int id;
		private int amount;
		private DateTime expiration;
//		private PaymentPlan paymentPlan;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public int Amount
		{
			get { return amount; }
			set { amount = value; }
		}

		[Property]
		public DateTime Expiration
		{
			get { return expiration; }
			set { expiration = value; }
		}

//		NH does not support a many-to-one (BelongsTo in AR) relation 
//		to a composite (nested in AR) class.
//		[BelongsTo]
//		public PaymentPlan PaymentPlan
//		{
//			get { return paymentPlan; }
//			set { paymentPlan = value; }
//		}
	}
}