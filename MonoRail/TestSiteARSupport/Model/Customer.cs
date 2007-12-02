namespace TestSiteARSupport.Model
{
	using Castle.ActiveRecord;

	[ActiveRecord]
	public class Customer : ActiveRecordBase<Customer>
	{
		private int id;
		private string name;
		private Address homeAddress;

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[Nested]
		public Address HomeAddress
		{
			get { return homeAddress; }
			set { homeAddress = value; }
		}
	}

	public class Address
	{
		private string street;

		[Property]
		public string Street
		{
			get { return street; }
			set { street = value; }
		}
	}
}
