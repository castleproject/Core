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
	/// Allow custom executions using the NHibernate's ISession.
	/// </summary>
	public delegate object NHibernateDelegate(ISession session, object instance);

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
		/// Invokes the specified delegate passing a valid 
		/// NHibernate session. Used for custom NHibernate queries.
		/// </summary>
		/// <param name="targetType">The target ActiveRecordType</param>
		/// <param name="call">The delegate instance</param>
		/// <param name="instance">The ActiveRecord instance</param>
		/// <returns>Whatever is returned by the delegate invocation</returns>
		protected static object Execute(Type targetType, NHibernateDelegate call, object instance)
		{
			if (targetType == null) throw new ArgumentNullException("targetType", "Target type must be informed");
			if (call == null) throw new ArgumentNullException("call", "Delegate must be passed");

			ISession session = _holder.CreateSession( targetType );

			try
			{
				return call(session, instance);
			}
			catch(Exception ex)
			{
				throw new ActiveRecordException("Error performing Execute for " + targetType.Name, ex);
			}
			finally
			{
				_holder.ReleaseSession(session);
			}
		}

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

		/// <summary>
		/// Returns all instances found for the specified type.
		/// </summary>
		/// <param name="targetType"></param>
		/// <returns></returns>
		protected static Array FindAll(Type targetType)
		{
			ISession session = _holder.CreateSession( targetType );

			try
			{
				ICriteria criteria = session.CreateCriteria(targetType);

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

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using sort orders and criterias.
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="orders"></param>
		/// <param name="criterias"></param>
		/// <returns></returns>
		protected static Array FindAll(Type targetType, Order[] orders, params ICriterion[] criterias)
		{
			ISession session = _holder.CreateSession( targetType );

			try
			{
				ICriteria criteria = session.CreateCriteria(targetType);

				foreach( ICriterion cond in criterias )
				{
					criteria.Add( cond );
				}

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

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using criterias.
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="criterias"></param>
		/// <returns></returns>
		protected static Array FindAll(Type targetType, params ICriterion[] criterias)
		{
			ISession session = _holder.CreateSession( targetType );

			try
			{
				ICriteria criteria = session.CreateCriteria(targetType);

				foreach( ICriterion ex in criterias )
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
		/// Creates (Saves) a new instance to the database.
		/// </summary>
		/// <param name="instance"></param>
		protected static void Create(object instance)
		{
			ISession session = _holder.CreateSession( instance.GetType() );

			try
			{
				session.Save(instance);

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
		/// Persists the modification on the instance
		/// state to the database.
		/// </summary>
		/// <param name="instance"></param>
		protected static void Update(object instance)
		{
			ISession session = _holder.CreateSession( instance.GetType() );

			try
			{
				session.Update(instance);

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
		/// Invokes the specified delegate passing a valid 
		/// NHibernate session. Used for custom NHibernate queries.
		/// </summary>
		/// <param name="call">The delegate instance</param>
		/// <returns>Whatever is returned by the delegate invocation</returns>
		protected internal object Execute(NHibernateDelegate call)
		{
			return Execute( this.GetType(), call, this );
		}

		/// <summary>
		/// Saves the instance information to the database.
		/// May Create or Update the instance depending 
		/// on whether it has a valid ID.
		/// </summary>
		public virtual void Save()
		{
			Save(this);
		}

		/// <summary>
		/// Creates (Saves) a new instance to the database.
		/// </summary>
		public virtual void Create()
		{
			Create(this);
		}

		/// <summary>
		/// Persists the modification on the instance
		/// state to the database.
		/// </summary>
		public virtual void Update()
		{
			Update(this);
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