namespace ValidationTestSite.Models
{
	using System;
	using Castle.Components.Validator;

	public class Client
	{
		private int id;
		private string name, email;
		private DateTime dob, created;

		public Client()
		{
			dob = new DateTime(1979,7,16);
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[ValidateNonEmpty]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[ValidateNonEmpty, ValidateEmail]
		public string Email
		{
			get { return email; }
			set { email = value; }
		}

		public DateTime Dob
		{
			get { return dob; }
			set { dob = value; }
		}

		public DateTime Created
		{
			get { return created; }
			set { created = value; }
		}
	}
}
