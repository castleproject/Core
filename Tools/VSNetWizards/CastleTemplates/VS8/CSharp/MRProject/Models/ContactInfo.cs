namespace !NAMESPACE!.Models
{
	using Castle.Components.Validator;

	public class ContactInfo
	{
		private string name, email, message;
		private Country country;

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

		[ValidateNonEmpty]
		public string Message
		{
			get { return message; }
			set { message = value; }
		}

		[ValidateNonEmpty]
		public Country Country
		{
			get { return country; }
			set { country = value; }
		}
	}
}
