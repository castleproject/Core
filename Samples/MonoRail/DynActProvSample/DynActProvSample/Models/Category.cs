namespace DynActProvSample.Models
{
	using Castle.ActiveRecord;

	[ActiveRecord]
	public class Category : ActiveRecordBase<Category>
	{
		private int id;
		private string name;

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
	}
}
