using System;

using NHibernate.Type;
using NHibernate.SqlTypes;



namespace NHibernate.Nullables2
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a char?.
	/// </summary>
	public class NullableCharType : NullableBaseType
	{

		public NullableCharType() : base(new StringFixedLengthSqlType(1))
		{
		}

		public override bool Equals(object x, object y)
		{
			//get boxed values.
			char? xTyped = (char?)x;
			return xTyped.Equals(y);
		}

		public override object NullValue
		{
			get { return new char?(); }
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
			get { return "char?"; }
		} 		
        
        public override System.Type ReturnedClass
		{
			get { return typeof(char?); }
		}

		public override object DeepCopyNotNull(object val)
		{
			return val;
		}

		public override object Get(System.Data.IDataReader rs, int index)
		{
			//TODO: perhaps NullableChar has a method/operator/contructor that will take an object.
			object value = rs[index];

			if( value==DBNull.Value ) 
			{
				return new char?();
			}
			else 
			{
				return new char?(Convert.ToChar(value));
			}
		}

		public override void Set(System.Data.IDbCommand cmd, object value, int index)
		{
			System.Data.IDataParameter parameter = (System.Data.IDataParameter)cmd.Parameters[index];
			char? nullableValue = (char?)value;
			
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
			return new char?(xml[0]);
		}
	}
}