using System;

using NHibernate.Type;
using NHibernate.SqlTypes;



namespace NHibernate.Nullables2
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableInt64"/>.
	/// </summary>
	public class NullableInt64Type : NullableBaseType
	{
		public NullableInt64Type() : base(new Int64SqlType())
		{
		}

		public override bool Equals(object x, object y)
		{
			//get boxed values.
			Int64? xTyped = (Int64?)x;
			return xTyped.Equals(y);
		}

		public override object NullValue
		{
			get { return new Int64?(); }
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
			get { return "Int64?"; }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(Int64?); }
		}

		public override object DeepCopyNotNull(object val)
		{
			return val;
		}

		public override object Get(System.Data.IDataReader rs, int index)
		{
			//TODO: perhaps NullableInt64 has a method/operator/contructor that will take an object.
			object value = rs[index];

			if( value==DBNull.Value )
			{
				return new Int64?();
			}
			else
			{
				return new Int64?(Convert.ToInt64(value));
			}
		}

		public override void Set(System.Data.IDbCommand cmd, object value, int index)
		{
			System.Data.IDataParameter parameter = (System.Data.IDataParameter)cmd.Parameters[index];
			Int64? nullableValue = (Int64?)value;

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
			return new Int64?(Int64.Parse(xml));
		}
	}
}
