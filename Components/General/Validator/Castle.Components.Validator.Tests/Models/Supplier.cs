namespace Castle.Components.Validator.Tests.Models
{
	public class Supplier
	{
		private int id;
		private string password, confirmation, email;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[ValidateNonEmpty(ExecutionOrder=20)]
		public string Password
		{
			get { return password; }
			set { password = value; }
		}

		[ValidateNonEmpty(ExecutionOrder=1), ValidateSameAs("Password", ExecutionOrder = 30)]
		public string Confirmation
		{
			get { return confirmation; }
			set { confirmation = value; }
		}

		[ValidateNonEmpty(ExecutionOrder=2), ValidateEmail(ExecutionOrder = 10)]
		public string Email
		{
			get { return email; }
			set { email = value; }
		}
	}
}
