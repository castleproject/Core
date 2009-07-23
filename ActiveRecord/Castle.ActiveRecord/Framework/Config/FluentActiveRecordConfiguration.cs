// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework.Config
{
	/// <summary>
	/// Allows to configure ActiveRecord fluently.
	/// </summary>
	public class FluentActiveRecordConfiguration : DefaultActiveRecordConfiguration
	{
		/// <summary>
		/// Enables ActiveRecord to be used in a ASP.NET scenario by registering a proper
		/// <see cref="IThreadScopeInfo"/>. By default, ActiveRecord cannot be used in
		/// web applications.
		/// </summary>
		/// <returns>The fluent configuration itself.</returns>
		public FluentActiveRecordConfiguration ForWeb()
		{
			WebEnabled = true;
			return this;
		}

		/// <summary>
		/// Sets the flush behaviour for <see cref="ISessionScope"/> when no
		/// other behaviour is specified in the scope itself. The default for
		/// this configuration is <see cref="DefaultFlushType.Classic"/>. See
		/// <see cref="DefaultFlushType"/> for what the options mean.
		/// </summary>
		/// <param name="flushType">The default flushing behaviour to set.</param>
		/// <returns>The fluent configuration itself.</returns>
		public FluentActiveRecordConfiguration Flush(DefaultFlushType flushType)
		{
			DefaultFlushType = flushType;
			return this;
		}

		/// <summary>
		/// Sets the <see cref="IThreadScopeInfo"/> to use. Normally, this type is
		/// set when ActiveRecord is used in web application. You should set this
		/// value only if you need a custom implementation of that interface.
		/// </summary>
		/// <typeparam name="T">The implementation to use.</typeparam>
		/// <returns>The fluent configuration itself.</returns>
		public FluentActiveRecordConfiguration UseThreadScopeInfo<T>() where T : IThreadScopeInfo
		{
			ThreadScopeInfoImplementation = typeof (T);
			return this;
		}

		/// <summary>
		/// Sets the <see cref="ISessionFactoryHolder"/> to use. You should set this if you need to
		/// use a custom implementation of that interface.
		/// </summary>
		/// <typeparam name="T">The implementation to use.</typeparam>
		/// <returns>The fluent configuration itself.</returns>
		public FluentActiveRecordConfiguration UseSessionFactoryHolder<T>() where T : ISessionFactoryHolder
		{
			SessionfactoryHolderImplementation = typeof (T);
			return this;
		}

		/// <summary>
		/// Instructs ActiveRecord to use lazy loading unless otherwise specified on the collection.
		/// By default, lazy loading is not used.
		/// </summary>
		/// <returns>The fluent configuration itself.</returns>
		public FluentActiveRecordConfiguration MakeLazyByDefault()
		{
			Lazy = true;
			return this;
		}

		/// <summary>
		/// Instructs ActiveRecord to verify the models against the data stores on initialization.
		/// By default, there is no verification
		/// </summary>
		/// <returns>The fluent configuration itself.</returns>
		public FluentActiveRecordConfiguration VerifyModels()
		{
			Verification = true;
			return this;
		}

		/// <summary>
		/// Instructs ActiveRecord to register NHibernate.Search event listeners to allow full text search.
		/// By default, there is no registration of these listeners.
		/// </summary>
		/// <returns>The fluent configuration itself.</returns>
		public FluentActiveRecordConfiguration RegisterSearch()
		{
			Searchable = true;
			return this;
		}
	}
}