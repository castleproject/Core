using System;
using System.Data;

using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Nullables2
{
	/// <summary>
	/// An <see cref="IUserType"/> that reads a <c>null</c> value from an <c>string</c>
	/// column in the database as a <see cref="String.Empty">String.Empty</see>
	/// and writes a <see cref="String.Empty">String.Empty</see> to the database
	/// as <c>null</c>.
	/// </summary>
	/// <remarks>
	/// This is intended to help with Windows Forms DataBinding and the problems associated
	/// with binding a null value.  See <a href="http://jira.nhibernate.org/browse/NH-279">
	/// NH-279</a> for the origin of this code.
	/// </remarks>
	public class EmptyStringType : IUserType
	{
		StringType stringType;

		public EmptyStringType()
		{
			stringType = (StringType)NHibernateUtil.String;
		}

		#region IUserType Members

		bool IUserType.Equals(object x, object y)
		{
			if( x==y )
			{
				return true;
			}
			if( ( x==null ) || ( y==null ) )
			{
				return false;
			}
			return x.Equals( y );
		}

		public SqlType[] SqlTypes
		{
			get
			{
				return new SqlType[] { stringType.SqlType };
			}
		}

		public DbType[] DbTypes
		{
			get
			{
				return new DbType[] { stringType.SqlType.DbType };
			}
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			if( value==null || value.Equals(string.Empty) ) 
			{
				((IDbDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
			}
			else 
			{
				stringType.Set(cmd, value, index);
			}
		}

		public System.Type ReturnedType
		{
			get
			{
				return typeof(string);
			}
		}

		public object NullSafeGet(System.Data.IDataReader rs, string[] names, object owner)
		{
			int index = rs.GetOrdinal(names[0]);

			if (rs.IsDBNull(index)) 
			{
				return string.Empty;
			}
			else 
			{
				return rs[index];
			}
		}

		public bool IsMutable
		{
			get
			{
				return false;
			}
		}

		#endregion
	}
}
