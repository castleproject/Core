using System;

using NHibernate.Type;
using NHibernate.SqlTypes;



//Contributed by Sergey Koshcheyev

namespace NHibernate.Nullables2
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableSByte"/>.
	/// </summary>
	public class NullableSByteType : NullableBaseType
	{
		public NullableSByteType() : base(new Int16SqlType())
		{
		}

		public override bool Equals(object x, object y)
		{
			//get boxed values.
			SByte? xTyped = (SByte?)x;
			return xTyped.Equals(y);
		}

		public override object NullValue
		{
			get { return new SByte?(); }
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
			get { return "SByte?"; }
		} 		
		
		public override System.Type ReturnedClass
		{
			get { return typeof(SByte?); }
		}

		public override object DeepCopyNotNull(object val)
		{
			return val;
		}

		public override object Get(System.Data.IDataReader rs, int index)
		{
			//TODO: perhaps NullableSByte has a method/operator/contructor that will take an object.
			object value = rs[index];

			if( value==DBNull.Value ) 
			{
				return new SByte?();
			}
			else 
			{
				return new SByte?( Convert.ToSByte( value ) );
			}
		}

		public override object Get(System.Data.IDataReader rs, string name)
		{
			//TODO: perhaps NullableSByte has a method/operator/contructor that will take an object.
			object value = rs[name];

			if( value==DBNull.Value ) 
			{
				return new SByte?();
			}
			else 
			{
				
				return new SByte?( Convert.ToSByte( value ) );
			}
		}

		public override void Set(System.Data.IDbCommand cmd, object value, int index)
		{
			System.Data.IDataParameter parameter = (System.Data.IDataParameter)cmd.Parameters[index];
			SByte? nullableValue = (SByte?)value;
			
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
			return new SByte?(SByte.Parse(xml));
		}
	}
}