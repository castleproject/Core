namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
	using System.Collections.Generic;

	[ActiveRecord]
	public class IdBagPrimitive
	{
		private int id;
		private IList<string> items;

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[HasAndBelongsToMany(RelationType = RelationType.IdBag, Table="IdToItems", ColumnKey="keyid", ElementType = typeof(string))]
		[CollectionID(CollectionIDType.Sequence, "col", "Int32")]
		public IList<string> Items
		{
			get { return items; }
			set { items = value; }
		}
	}
}
