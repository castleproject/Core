

namespace DiggExample.Model
{
	using System;
	using Castle.ActiveRecord;

	[ActiveRecord]
	public class MyEntity : ActiveRecordBase<MyEntity>
	{
		private int id;
		private string name;
		private int index;

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

		[Property("indx")]
		public int Index
		{
			get { return index; }
			set { index = value; }
		}

	}
}
