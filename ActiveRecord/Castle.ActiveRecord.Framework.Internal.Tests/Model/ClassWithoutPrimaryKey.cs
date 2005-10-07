using System;

namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
	[ActiveRecord]
	public class ClassWithoutPrimaryKey : ActiveRecordBase
	{
		[Property]
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		int _id;

	}
}
