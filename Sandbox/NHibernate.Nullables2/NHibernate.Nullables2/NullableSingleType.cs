using System;

using NHibernate.Type;
using NHibernate.SqlTypes;



namespace NHibernate.Nullables2
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableSingle"/>.
	/// </summary>
	public class NullableSingleType : NullableBaseType
	{
		public NullableSingleType() : base(new SingleSqlType())
		{
		}

		public override bool Equals(object x, object y)
		{
			//get boxed values.
			Single? xTyped = (Single?)x;
			
			return xTyped.Equals(y);
		}

		public override object NullValue
		{
			get { return new Single?(); }
		}

		public override bool HasNiceEquals
		{
			get
			{
				//the results from .Equals on NullalbeTypes don't suite our use of them (we want 2 nulls to be equal)
				return true; 
			}
		}

		public override bool IsMutable
		{
			get { return true; }
		}

		public override string Name
		{
			get { return "Single?"; }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(Single?); }
		}

		public override object DeepCopyNotNull(object val)
		{
			return val;
		}

		public override object Get(System.Data.IDataReader rs, int index)
		{
			//TODO: perhaps NullableSingle has a method/operator/contructor that will take an object.
			object value = rs[index];

			if( value==DBNull.Value )
			{
				return new Single?();
			}
			else 
			{
				return new Single?(Convert.ToSingle(value));
			}
		}

		public override void Set(System.Data.IDbCommand cmd, object value, int index)
		{
			System.Data.IDataParameter parameter = (System.Data.IDataParameter)cmd.Parameters[index];
			Single? nullableValue = (Single?)value;

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
			return new Single?(Single.Parse(xml));
		}
	}
}