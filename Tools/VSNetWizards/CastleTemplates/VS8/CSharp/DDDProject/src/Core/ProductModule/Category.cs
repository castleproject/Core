namespace !NAMESPACE!.Core.ProductModule
{
	using System;
	using Castle.ActiveRecord;
	using !NAMESPACE!.Core.Infraestructure;

	[ActiveRecord("Category")]
	public class Category : IIdentifiable
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

		public static Category Create(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}

			Category c = new Category();
			c.name = name;

			return c;
		}

		public override string ToString()
		{
			return Name;
		}

		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			Category category = obj as Category;
			if (category == null) return false;
			return id == category.id;
		}

		public override int GetHashCode()
		{
			return id;
		}
	}
}
