using System;

using NHibernate.Type;
using NHibernate.SqlTypes;



namespace NHibernate.Nullables2
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableInt32"/>.
	/// </summary>
	public class NullableInt32Type : NullableBaseType
	{
		public NullableInt32Type() : base(new Int32SqlType())
		{
		}

		public override bool Equals(object x, object y)
		{
			//get boxed values.
			Int32? xTyped = (Int32?)x;
			return xTyped.Equals(y);
		}

		public override object NullValue
		{
			get { return new Int32?(); }
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
			get { return "Int32?"; }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(Int32?); }
		}

		public override object DeepCopyNotNull(object val)
		{
			return val;
		}

		public override object Get(System.Data.IDataReader rs, int index)
		{
			//TODO: perhaps NullableInt32 has a method/operator/contructor that will take an object.
			object value = rs[index];

			if( value==DBNull.Value )
			{
				return new Int32?();
			}
			else 
			{
				return new Int32?(Convert.ToInt32(value));
			}
		}

		public override void Set(System.Data.IDbCommand cmd, object value, int index)
		{
			System.Data.IDataParameter parameter = (System.Data.IDataParameter)cmd.Parameters[index];
			Int32? nullableValue = (Int32?)value;

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
			return new Int32?(Int32.Parse(xml));
		}
	}
}
