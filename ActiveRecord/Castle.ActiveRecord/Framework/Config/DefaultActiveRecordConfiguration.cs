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
	using System;

	///<summary>
	/// Default configuration class for <see cref="IActiveRecordConfiguration"/>.
	///</summary>
	public class DefaultActiveRecordConfiguration : IActiveRecordConfiguration
	{
		/// <summary>
		/// The <see cref="IThreadScopeInfo"/> to use in ActiveRecord.
		/// </summary>
		public virtual Type ThreadScopeInfoImplementation { get; set; }

		/// <summary>
		/// The <see cref="ISessionFactoryHolder"/> to use in ActiveRecord.
		/// </summary>
		public virtual Type SessionfactoryHolderImplementation { get; set; }

		/// <summary>
		/// Determines the default Flush-behaviour of <see cref="ISessionScope"/>.
		/// </summary>
		public virtual DefaultFlushType DefaultFlushType { get; set; }

		/// <summary>
		/// Determines whether ActiveRecord is configured for use in web apps.
		/// </summary>
		public virtual bool WebEnabled { get; set; }

		/// <summary>
		/// Determines whether collections should be loaded lazily by default.
		/// </summary>
		public virtual bool Lazy { get; set; }

		/// <summary>
		/// Determines whether the models should be verified against the chosem data stores
		/// at initialization.
		/// </summary>
		public virtual bool Verification { get; set; }

		/// <summary>
		/// Determines whether event listeners for NHibernate.Search should be registered.
		/// </summary>
		public virtual bool Searchable { get; set; }
	}
}