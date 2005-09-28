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

namespace Castle.ActiveRecord.Framework
{
	using System;

	using NHibernate;
	using NHibernate.Cfg;

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
		/// Flush the session if needed.
		/// </summary>
		void FlushSession(ISession session);
		
	}
}
