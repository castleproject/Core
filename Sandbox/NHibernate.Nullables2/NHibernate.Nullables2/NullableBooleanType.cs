using System;
using NHibernate.Type;
using NHibernate.SqlTypes;

namespace NHibernate.Nullables2
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for bool?
	/// </summary>
	public class NullableBooleanType : NullableBaseType
	{

		public NullableBooleanType() : base(new BooleanSqlType())
		{
		}

		public override bool Equals(object x, object y)
		{
			//get boxed values.
			bool? xTyped = (bool?)x;
			return xTyped.Equals(y);
		}

		public override object NullValue
		{
			get { return new bool?(); }
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
			get { return "bool?"; }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(bool?); }
		}

		public override object DeepCopyNotNull(object val)
		{
			return val;
		}

		public override object Get(System.Data.IDataReader rs, int index)
		{
			//TODO: perhaps NullableBoolean has a method/operator/contructor that will take an object.
			object value = rs[index];
            bool? nullBool =  new bool?( (bool)value);
            return nullBool;
		}

		public override void Set(System.Data.IDbCommand cmd, object value, int index)
		{
			System.Data.IDataParameter parameter = (System.Data.IDataParameter)cmd.Parameters[index];
            
            bool? nullableValue = (bool?)value;
            
            if (nullableValue.HasValue)
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
            return new bool?(Boolean.Parse(xml));
		}
	}
}
