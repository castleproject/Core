namespace !NAMESPACE!.Models
{
	public class Country
	{
		private int id;
		private string name;

		public Country(int id, string name)
		{
			this.id = id;
			this.name = name;
		}

		public Country()
		{
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
	}
}
