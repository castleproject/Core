using System;

namespace Castle.ActiveRecord
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false), Serializable]
	public class AnyAttribute : WithAccessAttribute
	{		
		public AnyAttribute(Type idType)
		{
			this.idType = idType;
		}

		private CascadeEnum cascade;
		private string typeColumn, idColumn;
		private Type idType;
		private Type metaType;
		private string index;
		private bool insert = true, 
			update = true;

		public Type IdType
		{
			get { return idType;}
			set { idType = value;}
		}

		public Type MetaType
		{
			get { return metaType;}
			set { metaType = value;}
		}

		public CascadeEnum Cascade
		{
			get { return cascade;}
			set { cascade = value;}
		}

		public string TypeColumn
		{
			get { return typeColumn;}
			set { typeColumn = value;}
		}

		public string IdColumn
		{
			get { return idColumn;}
			set { idColumn = value;}
		}

		public string Index
		{
			get { return index;}
			set { index = value;}
		}

		public bool Insert
		{
			get { return insert;}
			set { insert = value;}
		}

		public bool Update
		{
			get { return update;}
			set { update = value;}
		}
	}

	//Avoids the AnyAttribute.MetaValue problem
	public class Any
	{
		[AttributeUsage(AttributeTargets.Property, AllowMultiple=true), Serializable]
		public class MetaValueAttribute : Attribute
		{
			private string _value;
			private Type _clazz;

			public MetaValueAttribute(string value, Type clazz)
			{
				this._value = value;
				this._clazz = clazz;
			}


			public string Value
			{
				get { return _value;}
				set { this._value = value;}
			}

			public Type Class
			{
				get { return _clazz;}
				set { _clazz = value;}
			}
		}
	}
}
