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
	
	using Castle.ActiveRecord.Framework;

	using NHibernate;
	using NHibernate.Expression;

	/// <summary>
	/// Static class that holds all the functionality for Active Record. Allows the use
	/// of Active Record without inheriting from the ActiveRecordBase.
	/// </summary>
	public class DomainModel
	{
		protected internal static ISessionFactoryHolder _holder;
		protected internal static IDictionary type2Model = Hashtable.Synchronized( new Hashtable() );

		#region Internal core methods

		private static void EnsureInitialized( Type type )
		{
			if (_holder == null)
			{
				String message = String.Format("An ActiveRecord class ({0}) was used but the framework seems not " + 
					"properly initialized. Did you forget about ActiveRecordStarter.Initialize() ?", type.FullName);
				throw new ActiveRecordException( message );
			}
			if (!type2Model.Contains(type))
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
		public static void Register(Type arType, Framework.Internal.ActiveRecordModel model)
		{
			type2Model[ arType ] = model;
		}

		/// <summary>
		/// Internally used
		/// </summary>
		/// <param name="arType"></param>
		/// <returns></returns>
		public static Framework.Internal.ActiveRecordModel GetModel( Type arType )
		{
			return (Framework.Internal.ActiveRecordModel) type2Model[ arType ];
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
		public static object Execute(Type targetType, NHibernateDelegate call, object instance)
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

		public static object FindByPrimaryKey(Type targetType, object id, bool throwOnNotFound)
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
		public static object FindByPrimaryKey(Type targetType, object id)
		{
			return FindByPrimaryKey(targetType, id, true);
		}

		/// <summary>
		/// Returns all instances found for the specified type.
		/// </summary>
		/// <param name="targetType"></param>
		/// <returns></returns>
		public static Array FindAll(Type targetType)
		{
			return FindAll(targetType, (Order[]) null);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		public static Array SlicedFindAll(Type targetType, int firstResult, int maxresults, Order[] orders, params ICriterion[] criterias)
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
		public static Array SlicedFindAll(Type targetType, int firstResult, int maxresults, params ICriterion[] criterias)
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
		public static Array FindAll(Type targetType, Order[] orders, params ICriterion[] criterias)
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
		public static Array FindAll(Type targetType, params ICriterion[] criterias)
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

		public static void DeleteAll(Type type)
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
		public static void Save(object instance)
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
		public static void Create(object instance)
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
		public static void Update(object instance)
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
		public static void Delete(object instance)
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

        /// <summary>
        /// Testing hock only.
        /// </summary>
        public static ISessionFactoryHolder GetSessionFactoryHolder()
        {
            return _holder;
        }
	}
}

