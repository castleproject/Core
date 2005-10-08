using System;
using System.Text;

namespace Castle.ActiveRecord
{
	[AttributeUsage(AttributeTargets.Property), Serializable]
	public class HasManyToAnyAttribute : HasManyAttribute
	{
		private Type idType, metaType;
		private string idColumn, typeColumn;

		public string TypeColumn
		{
			get { return typeColumn; }
			set { typeColumn = value; }
		}

		public string IdColumn
		{
			get { return idColumn; }
			set { idColumn = value; }
		}

		public Type MetaType
		{
			get { return metaType; }
			set { metaType = value; }
		}

		public Type IdType
		{
			get { return idType; }
			set { idType = value; }
		}

		public HasManyToAnyAttribute(Type mapType, string keyColum, 
			string table, Type idType,
			string typeColumn, string idColumn) : base(mapType, keyColum, table)
		{
			this.idType = idType;
			this.typeColumn = typeColumn;
			this.idColumn = idColumn;
		}
	}
}
