using Castle.ActiveRecord.Framework;
// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections;

	using NHibernate;
	using NHibernate.Expression;

	/// <summary>
	/// Base class for all ActiveRecord classes. Implements 
	/// all the functionality to simplify the code on the 
	/// subclasses.
	/// </summary>
	public abstract class ActiveRecordBase
	{
		protected internal static ISessionFactoryHolder _holder;

		public ActiveRecordBase()
		{
		}

		#region Overridable Hooks

		protected internal virtual bool BeforeSave(IDictionary state)
		{
			return false;
		}

		protected internal virtual bool BeforeLoad(DictionaryAdapter adapter)
		{
			return false;
		}

		protected internal virtual void BeforeDelete(DictionaryAdapter adapter)
		{
		}

		#endregion

		#region Base static methods

		protected static object FindByPrimaryKey(Type targetType, object id)
		{
			ISession sess = _holder.CreateSession( targetType );

			try
			{
				return sess.Load( targetType, id );
			}
			finally
			{
				_holder.ReleaseSession(sess);
			}
		}

		protected static Array FindAll(Type t, Order[] orders, params Expression[] expressions)
		{
			ISession sess = _holder.CreateSession( t );

			ICriteria criteria = sess.CreateCriteria(t);

			if (orders != null)
			{
				foreach( Order order in orders )
				{
					criteria.AddOrder( order );
				}
			}

			IList result = criteria.List();
		
			Array array = Array.CreateInstance(t, result.Count);
	
			int index = 0;

			foreach(object item in result)
			{
				array.SetValue(item, index++);
			}

			_holder.ReleaseSession(sess);

			return array;
		}

		protected static Array FindAll(Type t, params Expression[] expressions)
		{
			ISession sess = _holder.CreateSession( t );

			ICriteria criteria = sess.CreateCriteria(t);

			foreach( Expression ex in expressions )
			{
				criteria.Add( ex );
			}

			IList result = criteria.List();
		
			Array array = Array.CreateInstance(t, result.Count);
	
			int index = 0;

			foreach(object item in result)
			{
				array.SetValue(item, index++);
			}

			_holder.ReleaseSession(sess);

			return array;
		}

		protected static void DeleteAll(Type type)
		{
			ISession sess = _holder.CreateSession( type );

			try
			{
				sess.Delete( String.Format("from {0}", type.Name) );

				sess.Flush();
			}
			catch(Exception ex)
			{
				// Convert to AR Save Exception or something like that

				Console.WriteLine( ex.StackTrace );

				throw ex;
			}
			finally
			{
				_holder.ReleaseSession(sess);
			}
		}

		#endregion

		public virtual void Save()
		{
			Save(this);
		}

		protected void Save(object obj)
		{
			ISession sess = _holder.CreateSession( obj.GetType() );

			try
			{
				sess.Save(obj);

				sess.Flush();
			}
			catch(Exception ex)
			{
				// Convert to AR Save Exception or something like that

				Console.WriteLine( ex.StackTrace );

				throw ex;
			}
			finally
			{
				_holder.ReleaseSession(sess);
			}
		}
	}
}


