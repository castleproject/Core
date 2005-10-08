using System;

namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
	[ActiveRecord]
	public class ClassWithMappedField
	{
		private int _id;

		[Field("MyCustomName")]
		private String name1;

		[PrimaryKey(Access=PropertyAccess.NosetterCamelcaseUnderscore)]
		public int Id
		{
			get { return _id; }
		}

		[Property(CustomAccess="CustomAccess")]
		public int Value
		{
			get { return _id;}
			set { _id = value;}
		}
	}
}
