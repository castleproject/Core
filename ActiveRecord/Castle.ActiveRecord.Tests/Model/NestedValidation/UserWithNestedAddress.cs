namespace Castle.ActiveRecord.Tests.Model.NestedValidation
{
	using Castle.Components.Validator;

	[ActiveRecord]
	public class UserWithNestedAddress : ActiveRecordValidationBase<UserWithNestedAddress>
	{
		private int id;
		private string email;
		private Address postalAddress = new Address();
		private Address billingAddress;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id 
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		[ValidateNonEmpty, ValidateLength(5, 5)]
		public string Email
		{
			get { return email;}
			set { email = value; }
		}

		[Nested]
		public Address PostalAddress 
		{
			get { return postalAddress; }
			set { postalAddress = value; }
		}

		[Nested]
		public Address BillingAddress {
			get { return billingAddress; }
			set { billingAddress = value; }
		}
	}

	[ActiveRecord]
	public class UserWithNestedAddressNonGeneric : ActiveRecordValidationBase
	{
		private int id;
		private string email;
		private Address postalAddress = new Address();
		private Address billingAddress;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id {
			get { return id; }
			set { id = value; }
		}

		[Property]
		[ValidateNonEmpty, ValidateLength(5, 5)]
		public string Email {
			get { return email; }
			set { email = value; }
		}

		[Nested]
		public Address PostalAddress {
			get { return postalAddress; }
			set { postalAddress = value; }
		}

		[Nested]
		public Address BillingAddress {
			get { return billingAddress; }
			set { billingAddress = value; }
		}
	}

	public class Address
	{
		private string addressLine1;
		private string country;

		[Property]
		[ValidateNonEmpty, ValidateLength(5,5)]
		public string AddressLine1
		{
			get { return addressLine1; }
			set { addressLine1 = value; }
		}

		[Property]
		[ValidateNonEmpty]
		public string Country
		{
			get { return country;}
			set { country = value; }
		}
	}
}