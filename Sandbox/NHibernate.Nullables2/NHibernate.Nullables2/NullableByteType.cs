using System;
using NHibernate.Type;
using NHibernate.SqlTypes;

namespace NHibernate.Nullables2
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a byte?.
	/// </summary>
	public class NullableByteType : NullableBaseType
	{

		public NullableByteType() : base(new ByteSqlType())
		{
		}

		public override bool Equals(object x, object y)
		{
			//get boxed values.
            byte? xTyped = (byte?)x;
			return xTyped.Equals(y);
		}

		public override object NullValue
		{
			get { return new byte?(); }
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
			get { return "byte?"; }
		} 		
        
        public override System.Type ReturnedClass
		{
			get { return typeof(byte?); }
		}

		public override object DeepCopyNotNull(object val)
		{
			return val;
		}

		public override object Get(System.Data.IDataReader rs, int index)
		{
			//TODO: perhaps NullableByte has a method/operator/contructor that will take an object.
			object value = rs[index];

			if( value==DBNull.Value ) 
			{
				return new byte?();
			}
			else 
			{
				return new byte?(Convert.ToByte(value));
			}
		}

		public override void Set(System.Data.IDbCommand cmd, object value, int index)
		{
			System.Data.IDataParameter parameter = (System.Data.IDataParameter)cmd.Parameters[index];
            byte? nullableValue = (byte?)value;
           
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
			return new byte?(Byte.Parse(xml));
		}
	}
}