// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework
{
	using System;
	using System.Collections;

	using NHibernate;
	using NHibernate.Type;

	/// <summary>
	/// Translates the <c>IInterceptor</c>
	/// messages to instance possible hooks
	/// </summary>
	public class HookDispatcher : IInterceptor
	{
		private static readonly HookDispatcher _instance = new HookDispatcher();

		protected HookDispatcher()
		{
		}

		public static HookDispatcher Instance
		{
			get { return _instance; }
		}

		#region IInterceptor Members

		/// <summary>
		/// Called just before an object is initialized
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="id"></param>
		/// <param name="propertyNames"></param>
		/// <param name="state"></param>
		/// <param name="types"></param>
		/// <remarks>
		/// The interceptor may change the <c>state</c>, which will be propagated to the persistent
		/// object. Note that when this method is called, <c>entity</c> will be an empty
		/// uninitialized instance of the class.</remarks>
		/// <returns><c>true</c> if the user modified the <c>state</c> in any way</returns>
		public bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
            if (entity is ActiveRecordHooksBase)
			{
                return (entity as ActiveRecordHooksBase).BeforeLoad(new DictionaryAdapter(propertyNames, state));
			}

			return false;
		}

		/// <summary>
		/// Called when an object is detected to be dirty, during a flush.
		/// </summary>
		/// <param name="currentState"></param>
		/// <param name="entity"></param>
		/// <param name="id"></param>
		/// <param name="previousState"></param>
		/// <param name="propertyNames"></param>
		/// <param name="types"></param>
		/// <remarks>
		/// The interceptor may modify the detected <c>currentState</c>, which will be propagated to
		/// both the database and the persistent object. Note that all flushes end in an actual
		/// synchronization with the database, in which as the new <c>currentState</c> will be propagated
		/// to the object, but not necessarily (immediately) to the database. It is strongly recommended
		/// that the interceptor <b>not</b> modify the <c>previousState</c>.
		/// </remarks>
		/// <returns><c>true</c> if the user modified the <c>currentState</c> in any way</returns>
		public bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
		{
			return false;
		}

		/// <summary>
		/// Called before an object is saved
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="id"></param>
		/// <param name="propertyNames"></param>
		/// <param name="state"></param>
		/// <param name="types"></param>
		/// <remarks>
		/// The interceptor may modify the <c>state</c>, which will be used for the SQL <c>INSERT</c>
		/// and propagated to the persistent object
		/// </remarks>
		/// <returns><c>true</c> if the user modified the <c>state</c> in any way</returns>
		public bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
            if (entity is ActiveRecordHooksBase) {
                return (entity as ActiveRecordHooksBase).BeforeSave(new DictionaryAdapter(propertyNames, state));
            }
            return false;
		}

		/// <summary>
		/// Called before an object is deleted
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="id"></param>
		/// <param name="propertyNames"></param>
		/// <param name="state"></param>
		/// <param name="types"></param>
		/// <remarks>
		/// It is not recommended that the interceptor modify the <c>state</c>.
		/// </remarks>
		public void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
            if (entity is ActiveRecordHooksBase)
			{
                (entity as ActiveRecordHooksBase).BeforeDelete(new DictionaryAdapter(propertyNames, state));
			}
		}

		/// <summary>
		/// Called before a flush
		/// </summary>
		/// <param name="entities">The entities</param>
		public void PreFlush(ICollection entities)
		{
			
		}

		/// <summary>
		/// Called after a flush that actually ends in execution of the SQL statements required to
		/// synchronize in-memory state with the database.
		/// </summary>
		/// <param name="entities">The entitites</param>
		public void PostFlush(ICollection entities)
		{
			
		}

		/// <summary>
		/// Called when a transient entity is passed to <c>SaveOrUpdate</c>.
		/// </summary>
		/// <remarks>
		///	The return value determines if the object is saved
		///	<list>
		///		<item><c>true</c> - the entity is passed to <c>Save()</c>, resulting in an <c>INSERT</c></item>
		///		<item><c>false</c> - the entity is passed to <c>Update()</c>, resulting in an <c>UPDATE</c></item>
		///		<item><c>null</c> - Hibernate uses the <c>unsaved-value</c> mapping to determine if the object is unsaved</item>
		///	</list>
		/// </remarks>
		/// <param name="entity">A transient entity</param>
		/// <returns></returns>
		public object IsUnsaved(object entity)
		{
			return null;
		}

		/// <summary>
		/// Called from <c>Flush()</c>. The return value determines whether the entity is updated
		/// </summary>
		/// <remarks>
		///		<list>
		///			<item>an array of property indicies - the entity is dirty</item>
		///			<item>an empty array - the entity is not dirty</item>
		///			<item><c>null</c> - use Hibernate's default dirty-checking algorithm</item>
		///		</list>
		/// </remarks>
		/// <param name="entity">A persistent entity</param>
		/// <param name="currentState"></param>
		/// <param name="id"></param>
		/// <param name="previousState"></param>
		/// <param name="propertyNames"></param>
		/// <param name="types"></param>
		/// <returns>An array of dirty property indicies or <c>null</c> to choose default behavior</returns>
		public int[] FindDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
		{
			return null;
		}

		/// <summary>
		/// Instantiate the entity class. Return <c>null</c> to indicate that Hibernate should use the default
		/// constructor of the class
		/// </summary>
		/// <param name="type">A mapped type</param>
		/// <param name="id">The identifier of the new instance</param>
		/// <returns>An instance of the class, or <c>null</c> to choose default behaviour</returns>
		public object Instantiate(System.Type type, object id)
		{
			return null;
		}

		#endregion
	}
}
