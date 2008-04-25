namespace Castle.ActiveRecord.Tests.Model.CompositeUserType
{
	using System;
	using System.Data;
	using NHibernate;
	using NHibernate.Engine;
	using NHibernate.Type;
	using NHibernate.UserTypes;

	/// <summary>
	/// Extracted from the NHibernate.Model
	/// </summary>
	public class DoubleStringType : ICompositeUserType
	{
		public System.Type ReturnedClass
		{
			get { return typeof(string[]); }
		}

		public new bool Equals(object x, object y)
		{
			if (x == y) return true;
			if (x == null || y == null) return false;
			string[] lhs = (string[]) x;
			string[] rhs = (string[]) y;

			return lhs[0].Equals(rhs[0]) && lhs[1].Equals(rhs[1]);
		}

		public int GetHashCode(object x)
		{
			unchecked
			{
				string[] a = (string[]) x;
				return a[0].GetHashCode() + 31 * a[1].GetHashCode();
			}
		}

		public Object DeepCopy(Object x)
		{
			if (x == null) return null;
			string[] result = new string[2];
			string[] input = (string[]) x;
			result[0] = input[0];
			result[1] = input[1];
			return result;
		}

		public bool IsMutable
		{
			get { return true; }
		}

		public Object NullSafeGet(IDataReader rs, string[] names, ISessionImplementor session, Object owner)
		{
			string first = (string) NHibernateUtil.String.NullSafeGet(rs, names[0], session, owner);
			string second = (string) NHibernateUtil.String.NullSafeGet(rs, names[1], session, owner);

			return (first == null && second == null) ? null : new string[] { first, second };
		}

		public void NullSafeSet(IDbCommand st, Object value, int index, ISessionImplementor session)
		{
			string[] strings = (value == null) ? new string[2] : (string[]) value;

			NHibernateUtil.String.NullSafeSet(st, strings[0], index, session);
			NHibernateUtil.String.NullSafeSet(st, strings[1], index + 1, session);
		}

		public string[] PropertyNames
		{
			get { return new string[] { "s1", "s2" }; }
		}

		public IType[] PropertyTypes
		{
			get { return new IType[] { NHibernateUtil.String, NHibernateUtil.String }; }
		}

		public Object GetPropertyValue(Object component, int property)
		{
			return ((string[]) component)[property];
		}

		public void SetPropertyValue(Object component, int property, Object value)
		{
			((string[]) component)[property] = (string) value;
		}

		public object Assemble(object cached, ISessionImplementor session, object owner)
		{
			return DeepCopy(cached);
		}

		public object Disassemble(Object value, ISessionImplementor session)
		{
			return DeepCopy(value);
		}

		public object Replace(object original, object target, ISessionImplementor session, object owner)
		{
			return DeepCopy(original);
		}
	}
}
