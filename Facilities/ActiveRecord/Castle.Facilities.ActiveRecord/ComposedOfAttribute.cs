using System;

namespace Castle.Facilities.ActiveRecord
{

	public enum CompositionType
	{
		OneToOne,
		Component,
		Subclass,
		JoinedSubclass
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
	public class ComposedOfAttribute : Attribute
	{
		private CompositionType _composition = CompositionType.Subclass;
		private Type _proxy;
		private string _discriminator;
		private bool _dynamicInsert;
		private bool _dynamicUpdate;
		private string _key;
		private string _cascade;
		private bool _constrained;
		private string _outerJoin;

		public ComposedOfAttribute()
		{
		}

		public CompositionType Composition
		{
			get { return _composition; }
			set { _composition = value; }
		}

		public Type Proxy
		{
			get { return _proxy; }
			set { _proxy = value; }
		}

		public string Discriminator
		{
			get { return _discriminator; }
			set { _discriminator = value; }
		}

		public bool DynamicInsert
		{
			get { return _dynamicInsert; }
			set { _dynamicInsert = value; }
		}

		public bool DynamicUpdate
		{
			get { return _dynamicUpdate; }
			set { _dynamicUpdate = value; }
		}

		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}

		public string Cascade
		{
			get { return _cascade; }
			set { _cascade = value; }
		}

		public bool Constrained
		{
			get { return _constrained; }
			set { _constrained = value; }
		}

		public string OuterJoin
		{
			get { return _outerJoin; }
			set { _outerJoin = value; }
		}
	}
}