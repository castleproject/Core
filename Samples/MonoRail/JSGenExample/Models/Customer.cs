namespace JSGenExample.Models
{
	public class Customer
	{
		private int id;
		private string name, email;

		public Customer()
		{
		}

		public Customer(int id, string name, string email)
		{
			this.id = id;
			this.name = name;
			this.email = email;
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public string Email
		{
			get { return email; }
			set { email = value; }
		}
	}
}
