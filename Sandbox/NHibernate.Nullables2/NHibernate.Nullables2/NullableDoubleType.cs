using System;

using NHibernate.Type;
using NHibernate.SqlTypes;



namespace NHibernate.Nullables2
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableDouble"/>.
	/// </summary>
	public class NullableDoubleType : NullableBaseType
	{
		public NullableDoubleType() : base(new DoubleSqlType())
		{
		}

		public override bool Equals(object x, object y)
		{
			//get boxed values.
			double? xTyped = (double?)x;
			return xTyped.Equals(y);
		}

		public override object NullValue
		{
			get { return new double?(); }
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
			get { return "double?"; }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(double?); }
		}

		public override object DeepCopyNotNull(object val)
		{
			return val;
		}

		public override object Get(System.Data.IDataReader rs, int index)
		{
			//TODO: perhaps NullableDouble has a method/operator/contructor that will take an object.
			object value = rs[index];

			if( value==DBNull.Value ) 
			{
				return new double?();
			}
			else 
			{
				return new double?(Convert.ToDouble(value));
			}
		}

		public override void Set(System.Data.IDbCommand cmd, object value, int index)
		{
			System.Data.IDataParameter parameter = (System.Data.IDataParameter)cmd.Parameters[index];
			double? nullableValue = (double?)value;

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
			return new double?(Double.Parse(xml));
		}
	}
}
