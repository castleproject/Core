// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

	using NHibernate;

	/// <summary>
	/// Define session scope types
	/// </summary>
	public enum SessionScopeType
	{
		/// <summary>
		/// Undefined type of session scope.
		/// This value probably should never exist
		/// </summary>
		Undefined,
		/// <summary>
		/// Simple - non transactional session scope
		/// </summary>
		Simple,
		/// <summary>
		/// Transactional session scope
		/// </summary>
		Transactional,
		/// <summary>
		/// Custom implementation of session scope.
		/// </summary>
		Custom
	}

	/// <summary>
	/// Contract for implementation of scopes.
	/// </summary>
	/// <remarks>
	/// A scope can implement a logic that affects 
	/// AR for the scope lifetime. Session cache and
	/// transaction are the best examples, but you 
	/// can create new scopes adding new semantics.
	/// <para>
	/// The methods on this interface are mostly invoked
	/// by the <see cref="Castle.ActiveRecord.Framework.ISessionFactoryHolder"/>
	/// implementation
	/// </para>
	/// </remarks>
	public interface ISessionScope : IDisposable
	{
		/// <summary>
		/// Returns the <see cref="FlushAction"/> defined 
		/// for this scope
		/// </summary>
		FlushAction FlushAction { get; }
		
		/// <summary>
		/// Returns the <see cref="SessionScopeType"/> defined 
		/// for this scope
		/// </summary>
		SessionScopeType ScopeType { get; }
		
		/// <summary>
		/// Flushes the sessions that this scope 
		/// is maintaining
		/// </summary>
		void Flush();

		/// <summary>
		/// Evicts the specified instance from the session cache. 
		/// </summary>
		/// <param name="instance">The instance.</param>
		void Evict(object instance);

		/// <summary>
		/// This method is invoked when no session was available
		/// at and the <see cref="Castle.ActiveRecord.Framework.ISessionFactoryHolder"/>
		/// just created one. So it registers the session created 
		/// within this scope using a key. The scope implementation
		/// shouldn't make any assumption on what the key
		/// actually is as we reserve the right to change it 
		/// <seealso cref="IsKeyKnown"/>
		/// </summary>
		/// <param name="key">an object instance</param>
		/// <param name="session">An instance of <c>ISession</c></param>
		void RegisterSession(object key, ISession session);

		/// <summary>
		/// This method is invoked when the 
		/// <see cref="Castle.ActiveRecord.Framework.ISessionFactoryHolder"/>
		/// instance needs a session instance. Instead of creating one it interrogates
		/// the active scope for one. The scope implementation must check if it
		/// has a session registered for the given key. 
		/// <seealso cref="RegisterSession"/>
		/// </summary>
		/// <param name="key">an object instance</param>
		/// <returns><c>true</c> if the key exists within this scope instance</returns>
		bool IsKeyKnown(object key);

		/// <summary>
		/// This method should return the session instance associated with the key.
		/// </summary>
		/// <param name="key">an object instance</param>
		/// <returns>the session instance or null if none was found</returns>
		ISession GetSession(object key);

		/// <summary>
		/// Implementors should return true if they
		/// want that their scope implementation 
		/// be in charge of creating the session
		/// </summary>
		bool WantsToCreateTheSession { get; }

		/// <summary>
		/// If the <see cref="WantsToCreateTheSession"/> returned
		/// <c>true</c> then this method is invoked to allow 
		/// the scope to create a properly configured session
		/// </summary>
		/// <param name="sessionFactory">From where to open the session</param>
		/// <param name="interceptor">the NHibernate interceptor</param>
		/// <returns>the newly created session</returns>
		ISession OpenSession(ISessionFactory sessionFactory, IInterceptor interceptor);

		/// <summary>
		/// This method will be called if a session action fails. 
		/// The scope may then decide to use an different approach to flush/dispose it.
		/// </summary>
		/// <param name="session">The session that failed</param>
		void FailSession(ISession session);
	}
}
