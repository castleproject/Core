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

namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
	using System;

	using Castle.ActiveRecord;

	[ActiveRecord]
	public class ClassWithAnyAttribute : ActiveRecordBase
	{
		private int _id = 0;

		[PrimaryKey(Access=PropertyAccess.NosetterCamelcaseUnderscore)]
		public int Id
		{
			get { return _id; }
		}

		[Any(typeof (long), MetaType=typeof (string),
			TypeColumn="BILLING_DETAILS_TYPE",
			IdColumn="BILLING_DETAILS_ID",
			Cascade=CascadeEnum.SaveUpdate)]
		// [Any.MetaValue("CREDIT_CARD", typeof (CreditCard))]
		[Any.MetaValue("BANK_ACCOUNT", typeof (BankAccount))]
		public IPayment PaymentMethod
		{
			get { return null; }
			set
			{
			}
		}
	}

	public interface IPayment
	{
	}

	public class CreditCard : IPayment
	{
	}

	public class BankAccount : IPayment
	{
	}

	[ActiveRecord]
	public class ClasssWithHasManyToAny : ActiveRecordBase
	{
		private int _id = 0;

		[PrimaryKey(Access = PropertyAccess.NosetterCamelcaseUnderscore)]
		public int Id
		{
			get { return _id; }
		}

		[HasManyToAny(typeof (IPayment), "pay_id", "payments_table", typeof (int), "payment_type", "payment_method_id",
			MetaType=typeof (int), RelationType=RelationType.Set)]
		public IPayment PaymentMethod
		{
			get { return null; }
			set
			{
			}
		}
	}
}