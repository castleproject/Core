using System;
using Castle.ActiveRecord;
namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
	[ActiveRecord]
	public class ClassWithAnyAttribute : ActiveRecordBase
	{
		private int _id;
		
		[PrimaryKey(Access=PropertyAccess.NosetterCamelcaseUnderscore)]
		public int Id
		{
			get { return _id; }
		}

		[Any(typeof(long), MetaType=typeof(string),
			 TypeColumn="BILLING_DETAILS_TYPE",
			 IdColumn="BILLING_DETAILS_ID",
			 Cascade=CascadeEnum.SaveUpdate)]
		[Any.MetaValue("CREDIT_CARD", typeof(CreditCard))]
		[Any.MetaValue("BANK_ACCOUNT",typeof(BankAccount))]
		public IPayment PaymentMethod
		{
			get { return null;}
			set { }
		}
	}
	public interface IPayment {}
	public class CreditCard : IPayment 
	{
	}
	public class BankAccount : IPayment {}

	[ActiveRecord]
	public class ClasssWithHasManyToAny : ActiveRecordBase
	{
		private int _id;

		[PrimaryKey(Access = PropertyAccess.NosetterCamelcaseUnderscore)]
		public int Id
		{
			get { return _id; }
		}

		[HasManyToAny(typeof(IPayment), "pay_id", "payments_table", typeof(int), "payment_type", "payment_method_id",
			MetaType=typeof(int), RelationType=RelationType.Set)]
		public IPayment PaymentMethod
		{
			get { return null; }
			set { }
		}
	}
}
