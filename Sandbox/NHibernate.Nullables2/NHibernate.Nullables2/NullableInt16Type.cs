using System;

using NHibernate.Type;
using NHibernate.SqlTypes;



namespace NHibernate.Nullables2
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableInt16"/>.
	/// </summary>
	public class NullableInt16Type : NullableBaseType
	{
		public NullableInt16Type() : base(new Int16SqlType())
		{
		}

		public override bool Equals(object x, object y)
		{
			//get boxed values.
			Int16? xTyped = (Int16?)x;
			return xTyped.Equals(y);
		}

		public override object NullValue
		{
			get { return new Int16?(); }
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
			get { return "Int16?"; }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(Int16?); }
		}

		public override object DeepCopyNotNull(object val)
		{
			return val;
		}

		public override object Get(System.Data.IDataReader rs, int index)
		{
			//TODO: perhaps NullableInt16 has a method/operator/contructor that will take an object.
			object value = rs[index];

			if( value==DBNull.Value ) 
			{
				return new Int16?();
			}
			else 
			{
				return new Int16?(Convert.ToInt16(value));
			}
		}

		public override void Set(System.Data.IDbCommand cmd, object value, int index)
		{
			System.Data.IDataParameter parameter = (System.Data.IDataParameter)cmd.Parameters[index];
			Int16? nullableValue = (Int16?)value;

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
			return new Int16?(Int16.Parse(xml));
		}
	}
}
