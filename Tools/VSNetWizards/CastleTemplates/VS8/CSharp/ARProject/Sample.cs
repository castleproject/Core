namespace ARProject
{
	using System;
	using Castle.ActiveRecord;
	using NHibernate.Expression;

	// This is file is provided just as a starting point 
	// especially if you are new to ActiveRecord
	//
	// Feel free to delete it right away.

	[ActiveRecord("TableName")]
	public class Sample : ActiveRecordBase<Sample>
	{
		private int id;
		private String name;

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
