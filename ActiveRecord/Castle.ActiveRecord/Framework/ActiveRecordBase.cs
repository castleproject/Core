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

	using Castle.ActiveRecord.Framework;

	/// <summary>
	/// Base class for all ActiveRecord classes. Implements 
	/// all the functionality to simplify the code on the 
	/// subclasses.
	/// </summary>
	public abstract class ActiveRecordBase
	{
		protected internal static ISessionFactoryHolder _holder;

		/// <summary>
		/// Constructs an ActiveRecordBase subclass.
		/// </summary>
		public ActiveRecordBase()
		{
		}

		#region Overridable Hooks

		/// <summary>
		/// Hook to change the object state
		/// before saving it.
		/// </summary>
		/// <param name="state"></param>
		/// <returns>Return <c>true</c> if you have changed the state. <c>false</c> otherwise</returns>
		protected internal virtual bool BeforeSave(IDictionary state)
		{
			return false;
		}

		/// <summary>
		/// Hook to transform the read data 
		/// from the database before populating 
		/// the object instance
		/// </summary>
		/// <param name="adapter"></param>
		/// <returns>Return <c>true</c> if you have changed the state. <c>false</c> otherwise</returns>
		protected internal virtual bool BeforeLoad(IDictionary adapter)
		{
			return false;
		}

		/// <summary>
		/// Hook to perform additional tasks 
		/// before removing the object instance representation
		/// from the database.
		/// </summary>
		/// <param name="adapter"></param>
		protected internal virtual void BeforeDelete(IDictionary adapter)
		{
		}

		#endregion

		#region Base static methods

		/// <summary>
		/// Finds an object instance by a unique ID
		/// </summary>
		/// <param name="targetType">The AR subclass type</param>
		/// <param name="id">ID value</param>
		/// <returns></returns>
		protected static object FindByPrimaryKey(Type targetType, object id)
		{
			ISession session = _holder.CreateSession( targetType );

			try
			{
				return session.Load( targetType, id );
			}
			catch(ObjectNotFoundException)
			{
				return null;
			}
			catch(Exception ex)
			{
				throw new ActiveRecordException("Could not perform Load (Find by id) for " + targetType.Name, ex);
			}
			finally
			{
				_holder.ReleaseSession(session);
			}
		}

		protected static Array FindAll(Type targetType, Order[] orders, params Expression[] expressions)
		{
			ISession session = _holder.CreateSession( targetType );

			try
			{
				ICriteria criteria = session.CreateCriteria(targetType);

				if (orders != null)
				{
					foreach( Order order in orders )
					{
						criteria.AddOrder( order );
					}
				}

				IList result = criteria.List();
		
				Array array = Array.CreateInstance(targetType, result.Count);
	
				int index = 0;

				foreach(object item in result)
				{
					array.SetValue(item, index++);
				}

				return array;
			}
			catch(Exception ex)
			{
				throw new ActiveRecordException("Could not perform FindAll for " + targetType.Name, ex);
			}
			finally
			{
				_holder.ReleaseSession(session);
			}
		}

		protected static Array FindAll(Type targetType, params Expression[] expressions)
		{
			ISession session = _holder.CreateSession( targetType );

			try
			{
				ICriteria criteria = session.CreateCriteria(targetType);

				foreach( Expression ex in expressions )
				{
					criteria.Add( ex );
				}

				IList result = criteria.List();
		
				Array array = Array.CreateInstance(targetType, result.Count);
	
				int index = 0;

				foreach(object item in result)
				{
					array.SetValue(item, index++);
				}

				return array;
			}
			catch(Exception ex)
			{
				throw new ActiveRecordException("Could not perform FindAll for " + targetType.Name, ex);
			}
			finally
			{
				_holder.ReleaseSession(session);
			}
		}

		protected static void DeleteAll(Type type)
		{
			ISession session = _holder.CreateSession( type );

			try
			{
				session.Delete( String.Format("from {0}", type.Name) );

				session.Flush();
			}
			catch(Exception ex)
			{
				throw new ActiveRecordException("Could not perform DeleteAll for " + type.Name, ex);
			}
			finally
			{
				_holder.ReleaseSession(session);
			}
		}

		/// <summary>
		/// Saves the instance to the database
		/// </summary>
		/// <param name="instance"></param>
		protected static void Save(object instance)
		{
			ISession session = _holder.CreateSession( instance.GetType() );

			try
			{
				session.SaveOrUpdate(instance);

				session.Flush();
			}
			catch(Exception ex)
			{
				throw new ActiveRecordException("Could not perform Save for " + instance.GetType().Name, ex);
			}
			finally
			{
				_holder.ReleaseSession(session);
			}
		}

		/// <summary>
		/// Deletes the instance from the database.
		/// </summary>
		/// <param name="instance"></param>
		protected static void Delete(object instance)
		{
			ISession session = _holder.CreateSession( instance.GetType() );

			try
			{
				session.Delete(instance);

				session.Flush();
			}
			catch(Exception ex)
			{
				throw new ActiveRecordException("Could not perform Delete for " + instance.GetType().Name, ex);
			}
			finally
			{
				_holder.ReleaseSession(session);
			}
		}

		#endregion

		/// <summary>
		/// Saves the instance information to the database.
		/// </summary>
		public virtual void Save()
		{
			Save(this);
		}

		/// <summary>
		/// Deletes the instance from the database.
		/// </summary>
		public virtual void Delete()
		{
			Delete(this);
		}
	}
}