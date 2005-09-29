using System;

namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
	[ActiveRecord]
	public class ClassWithMappedField
	{
		private int id;

		[Field("MyCustomName")]
		private String name1;

		[PrimaryKey(Access=PropertyAccess.NosetterCamelcaseUnderscore)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}
