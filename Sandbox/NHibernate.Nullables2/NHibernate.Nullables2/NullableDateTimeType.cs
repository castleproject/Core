using System;

using NHibernate.Type;
using NHibernate.SqlTypes;



namespace NHibernate.Nullables2
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a DateTime?.
	/// </summary>
	public class NullableDateTimeType : NullableBaseType
	{
		public NullableDateTimeType() : base(new DateTimeSqlType())
		{
		}

		public override bool Equals(object x, object y)
		{
			//get boxed values.
			DateTime? xTyped = (DateTime?)x;
			return xTyped.Equals(y);
		}

		public override object NullValue
		{
			get { return new DateTime?(); }
		}

		public override bool HasNiceEquals
		{
			get { return true; }
		}

		public override bool IsMutable
		{
			get { return true; }
		}

		public override string Name
		{
			get { return "DateTime?"; }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(DateTime?); }
		}

		public override object DeepCopyNotNull(object val)
		{
			return val;
		}

		public override object Get(System.Data.IDataReader rs, int index)
		{
			//TODO: perhaps NullableDateTime has a method/operator/contructor that will take an object.
			object value = rs[index];

			if( value==DBNull.Value ) 
			{
				return new DateTime?();
			}
			else 
			{
				return new DateTime?(Convert.ToDateTime(value));
			}
		}

		public override void Set(System.Data.IDbCommand cmd, object value, int index)
		{
			System.Data.IDataParameter parameter = (System.Data.IDataParameter)cmd.Parameters[index];

			DateTime? nullableValue = (DateTime?)value;

			if( nullableValue.HasValue ) 
			{
				parameter.Value = nullableValue.Value;
			}
			else 
			{
				parameter.Value = DBNull.Value;
			}
		}

		public override string ToString(object val)
		{
			return val.ToString();
		} 

		public override object FromStringValue(string xml)
		{
			return new DateTime?(DateTime.Parse(xml));
		}
	}
}
