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

namespace Castle.ActiveRecord.Framework
{
	using System;

	using NHibernate;
	using NHibernate.Cfg;

	/// <summary>
	/// Type of delegate that is called when a root type is registered.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="rootType"></param>
	public delegate void RootTypeHandler(object sender, Type rootType);

	/// <summary>
	/// Keeps an association of SessionFactories to a object model 
	/// tree;
	/// </summary>
	public interface ISessionFactoryHolder
	{
		/// <summary>
		/// Raised when a new root type is registered. 
		/// A new root type creates a new <c>ISessionFactory</c>
		/// </summary>
		event RootTypeHandler OnRootTypeRegistered;

		/// <summary>
		/// Associates a Configuration object to a root type
		/// </summary>
		/// <param name="rootType"></param>
		/// <param name="cfg"></param>
		void Register(Type rootType, Configuration cfg);

		/// <summary>
		/// Pendent
		/// </summary>
		/// <returns></returns>
		Configuration[] GetAllConfigurations();

		/// <summary>
		/// Requests the Configuration associated to the type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		Configuration GetConfiguration(Type type);

		/// <summary>
		/// Obtains the SessionFactory associated to the type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		ISessionFactory GetSessionFactory(Type type);

		/// <summary>
		/// Gets the all the session factories.
		/// </summary>
		/// <returns></returns>
		ISessionFactory[] GetSessionFactories();

		/// <summary>
		/// Creates a session for the associated type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		ISession CreateSession(Type type);

		/// <summary>
		/// Releases the specified session
		/// </summary>
		/// <param name="session"></param>
		void ReleaseSession(ISession session);

		/// <summary>
		/// Called if an action on the session fails
		/// </summary>
		/// <param name="session"></param>
		void FailSession(ISession session);

		/// <summary>
		/// Gets the type of the root.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		Type GetRootType(Type type);

		/// <summary>
		/// Gets or sets the implementation of <see cref="IThreadScopeInfo"/>
		/// </summary>
		IThreadScopeInfo ThreadScopeInfo { get; set; }

		///<summary>
		/// This method allows direct registration
		/// of a session factory to a type, bypassing the normal preperation that AR
		/// usually does. 
		/// The main usage is in testing, so you would be able to switch the session factory
		/// for each test.
		/// Note that this will override the current session factory for the baseType.
		///</summary>
		void RegisterSessionFactory(ISessionFactory sessionFactory, Type baseType);
	}
}
