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

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections;
	
	using NHibernate;
	using NHibernate.Type;

	/// <summary>
	/// Base class for ActiveRecord entities
	/// that are interested in NHibernate's hooks.
	/// </summary>
	[Serializable]
	public abstract class ActiveRecordHooksBase : ILifecycle
    {
        /// <summary>
        /// Hook to change the object state
        /// before saving it.
        /// </summary>
        /// <param name="state"></param>
        /// <returns>Return <c>true</c> if you have changed the state. <c>false</c> otherwise</returns>
        protected virtual internal bool BeforeSave(IDictionary state)
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
        protected virtual internal bool BeforeLoad(IDictionary adapter) 
		{
            return false;
        }

        /// <summary>
        /// Hook to perform additional tasks 
        /// before removing the object instance representation
        /// from the database.
        /// </summary>
        /// <param name="adapter"></param>
        protected virtual internal void BeforeDelete(IDictionary adapter) 
		{
        }

		/// <summary>
		/// Called before a flush
		/// </summary>
		protected virtual internal void PreFlush()
		{
		}

		/// <summary>
		/// Called after a flush that actually ends in execution of the SQL statements required to
		/// synchronize in-memory state with the database.
		/// </summary>
		protected virtual internal void PostFlush()
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
		/// <returns></returns>
		protected virtual internal object IsUnsaved()
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
		/// <param name="id"></param>
		/// <param name="previousState"></param>
		/// <param name="currentState"></param>
		/// <param name="types"></param>
		/// <returns>An array of dirty property indicies or <c>null</c> to choose default behavior</returns>
		protected virtual internal int[] FindDirty(object id, IDictionary previousState, IDictionary currentState, IType[] types)
		{
			return null;
		}

		#region ILifecycle

		LifecycleVeto ILifecycle.OnSave(ISession session)
		{
			OnSave();

			return LifecycleVeto.NoVeto;
		}

		LifecycleVeto ILifecycle.OnUpdate(ISession session)
		{
			OnUpdate();

			return LifecycleVeto.NoVeto;
		}

		LifecycleVeto ILifecycle.OnDelete(ISession session)
		{
			OnDelete();

			return LifecycleVeto.NoVeto;
		}

		void ILifecycle.OnLoad(ISession session, object id)
		{
			OnLoad(id);
		}

		#endregion

		/// <summary>
		/// Lifecycle method invoked during Save of the entity
		/// </summary>
		protected virtual void OnSave()
		{
			
		}

		/// <summary>
		/// Lifecycle method invoked during Update of the entity
		/// </summary>
		protected virtual void OnUpdate()
		{
			
		}

		/// <summary>
		/// Lifecycle method invoked during Delete of the entity
		/// </summary>
		protected virtual void OnDelete()
		{
			
		}

		/// <summary>
		/// Lifecycle method invoked during Load of the entity
		/// </summary>
		protected virtual void OnLoad(object id)
		{
			
		}
	}
}
