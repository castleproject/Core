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
	[Serializable]
	public abstract class ActiveRecordBase
	{
		protected internal static ISessionFactoryHolder _holder;

		/// <summary>
		/// Constructs an ActiveRecordBase subclass.
		/// </summary>
		public ActiveRecordBase()
		{
		}

		#region Internal core methods

		private static void EnsureInitialized( Type type )
		{
			if (_holder == null)
			{
				String message = String.Format("An ActiveRecord class ({0}) was used but the framework seems not " + 
					"properly initialized. Did you forget about ActiveRecordStarter.Initialize() ?", type.FullName);
				throw new ActiveRecordException( message );
			}
			if (GetModel(type) == null)
			{
				String message = String.Format("You have accessed an ActiveRecord class that wasn't properly initialized. " + 
					"The only explanation is that the call to ActiveRecordStarter.Initialize() didn't include {0} class", type.FullName);
				throw new ActiveRecordException( message );
			}
		}

		/// <summary>
		/// Internally used
		/// </summary>
		/// <param name="arType"></param>
		/// <param name="model"></param>
		internal static void Register(Type arType, Framework.Internal.ActiveRecordModel model)
		{
			Framework.Internal.ActiveRecordModel.Register(arType, model);
		}

		/// <summary>
		/// Internally used
		/// </summary>
		/// <param name="arType"></param>
		/// <returns></returns>
		internal static Framework.Internal.ActiveRecordModel GetModel( Type arType )
		{
			return Framework.Internal.ActiveRecordModel.GetModel(arType);
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
		protected internal static object Execute(Type targetType, NHibernateDelegate call, object instance)
		{
			if (targetType == null) throw new ArgumentNullException("targetType", "Target type must be informed");
			if (call == null) throw new ArgumentNullException("call", "Delegate must be passed");

			EnsureInitialized(targetType);

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
		/// <param name="throwOnNotFound"><c>true</c> if you want to catch an exception 
		/// if the object is not found</param>
		/// <returns></returns>
		/// <exception cref="ObjectNotFoundException">if <c>throwOnNotFound</c> is set to 
		/// <c>true</c> and the row is not found</exception>
		protected internal static object FindByPrimaryKey(Type targetType, object id, bool throwOnNotFound)
		{
			EnsureInitialized(targetType);

			ISession session = _holder.CreateSession( targetType );

			try
			{
				return session.Load( targetType, id );
			}
			catch(ObjectNotFoundException ex)
			{
				if (throwOnNotFound)
				{
					String message = String.Format("Could not find {0} with id {1}", targetType.Name, id);
					throw new NotFoundException(message, ex);
				}

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
		/// Finds an object instance by a unique ID
		/// </summary>
		/// <param name="targetType">The AR subclass type</param>
		/// <param name="id">ID value</param>
		/// <returns></returns>
		protected internal static object FindByPrimaryKey(Type targetType, object id)
		{
			return FindByPrimaryKey(targetType, id, true);
		}

		/// <summary>
		/// Returns all instances found for the specified type.
		/// </summary>
		/// <param name="targetType"></param>
		/// <returns></returns>
		protected internal static Array FindAll(Type targetType)
		{
			return FindAll(targetType, (Order[]) null);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		protected internal static Array SlicedFindAll(Type targetType, int firstResult, int maxresults, Order[] orders, params ICriterion[] criterias)
		{
			EnsureInitialized(targetType);

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

				criteria.SetFirstResult(firstResult);
				criteria.SetMaxResults(maxresults);

				return CreateReturnArray(criteria, targetType);
			}
			catch(Exception ex)
			{
				throw new ActiveRecordException("Could not perform SlicedFindAll for " + targetType.Name, ex);
			}
			finally
			{
				_holder.ReleaseSession(session);
			}
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		protected internal static Array SlicedFindAll(Type targetType, int firstResult, int maxresults, params ICriterion[] criterias)
		{
			return SlicedFindAll(targetType, firstResult, maxresults, null, criterias);
		}

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using sort orders and criterias.
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="orders"></param>
		/// <param name="criterias"></param>
		/// <returns></returns>
		protected internal static Array FindAll(Type targetType, Order[] orders, params ICriterion[] criterias)
		{
			EnsureInitialized(targetType);

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

				return CreateReturnArray(criteria, targetType);
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
		protected internal static Array FindAll(Type targetType, params ICriterion[] criterias)
		{
			return FindAll(targetType, null, criterias);
		}

		private static Array CreateReturnArray(ICriteria criteria, Type targetType)
		{
			IList result = criteria.List();
	
			Array array = Array.CreateInstance(targetType, result.Count);
	
			int index = 0;
	
			foreach(object item in result)
			{
				array.SetValue(item, index++);
			}
	
			return array;
		}

		protected internal static void DeleteAll(Type type)
		{
			EnsureInitialized(type);

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
		protected internal static void Save(object instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");

			EnsureInitialized(instance.GetType());

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
		protected internal static void Create(object instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");

			EnsureInitialized(instance.GetType());

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
		protected internal static void Update(object instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");

			EnsureInitialized(instance.GetType());

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
		protected internal static void Delete(object instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");

			EnsureInitialized(instance.GetType());

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

		#region Base methods

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
			ActiveRecordBase.Save(this);
		}

		/// <summary>
		/// Creates (Saves) a new instance to the database.
		/// </summary>
		public virtual void Create()
		{
			ActiveRecordBase.Create(this);
		}

		/// <summary>
		/// Persists the modification on the instance
		/// state to the database.
		/// </summary>
		public virtual void Update()
		{
			ActiveRecordBase.Update(this);
		}

		/// <summary>
		/// Deletes the instance from the database.
		/// </summary>
		public virtual void Delete()
		{
			ActiveRecordBase.Delete(this);
		}

		#endregion
	}
}