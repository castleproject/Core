using System;

using NHibernate.Type;
using NHibernate.SqlTypes;



namespace NHibernate.Nullables2
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableGuid"/>.
	/// </summary>
	public class NullableGuidType : NullableBaseType
	{
		public NullableGuidType() : base(new GuidSqlType())
		{
		}

		public override bool Equals(object x, object y)
		{
			//get boxed values.
			Guid? xTyped = (Guid?)x;
			return xTyped.Equals(y);
		}

		public override object NullValue
		{
			get { return new Guid?(); }
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
			get { return "Guid?"; }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(Guid?); }
		}

		public override object DeepCopyNotNull(object val)
		{
			return val;
		}

		public override object Get(System.Data.IDataReader rs, int index)
		{
			//TODO: perhaps NullableGuid has a method/operator/contructor that will take an object.
			object value = rs[index];

			if( value==DBNull.Value ) 
			{
				return new Guid?();
			}
			else if (value is Guid)
			{
				return new Guid?((Guid)value);
			}
			else 
			{
				return new Guid?(new Guid(value.ToString())); //certain DB's that have no Guid (MySQL) will return strings.
			}
		}

		public override void Set(System.Data.IDbCommand cmd, object value, int index)
		{
			System.Data.IDataParameter parameter = (System.Data.IDataParameter)cmd.Parameters[index];
			Guid? nullableValue = (Guid?)value;

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
			return new Guid?(new Guid(xml));
		}
	}
}
