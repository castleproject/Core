using System;

namespace Castle.Facilities.ActiveRecord
{
	[AttributeUsage(AttributeTargets.Property)]
	public class PrimaryKeyAttribute : Attribute
	{
		private PrimaryKeyType _generator;
		private string _column;
		private string _unsavedValue;
		private string _access;

		public PrimaryKeyAttribute()
		{
		}

		public PrimaryKeyType Generator
		{
			get { return _generator; }
			set { _generator = value; }
		}

		public string Column
		{
			get { return _column; }
			set { _column = value; }
		}

		public string UnsavedValue
		{
			get { return _unsavedValue; }
			set { _unsavedValue = value; }
		}

		public string Access
		{
			get { return _access; }
			set { _access = value; }
		}
	}

	public enum PrimaryKeyType
	{
		None,
		Identity,
		Sequence,
		HiLo,
		SeqHiLo,
		UuidHex,
		UuidString,
		Guid,
		GuidComb,
		Native,
		Assigned,
		Foreign
	}
}