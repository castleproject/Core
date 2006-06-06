using System;

using NHibernate.Type;
using NHibernate.SqlTypes;



namespace NHibernate.Nullables2
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableDecimal"/>.
	/// </summary>
	public class NullableDecimalType : NullableBaseType
	{

		public NullableDecimalType() : base(new DecimalSqlType())
		{
		}

		public override bool Equals(object x, object y)
		{
			//get boxed values.
			decimal? xTyped = (decimal?)x;
			return xTyped.Equals(y);
		}

		public override object NullValue
		{
			get { return new decimal?(); }
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
			get { return "decimal?"; }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(decimal?); }
		}

		public override object DeepCopyNotNull(object val)
		{
			return val;
		}

		public override object Get(System.Data.IDataReader rs, int index)
		{
			//TODO: perhaps NullableDecimal has a method/operator/contructor that will take an object.
			object value = rs[index];

			if( value==DBNull.Value ) 
			{
				return new decimal?();
			}
			else 
			{
				return new decimal?(Convert.ToDecimal(value));
			}
		}

		public override void Set(System.Data.IDbCommand cmd, object value, int index)
		{

			System.Data.IDataParameter parameter = (System.Data.IDataParameter)cmd.Parameters[index];
			decimal? nullableValue = (decimal?)value;

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
			return new decimal?(Decimal.Parse(xml));
		}
	}
}
