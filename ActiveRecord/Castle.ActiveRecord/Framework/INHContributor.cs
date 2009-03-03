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

namespace Castle.ActiveRecord.Framework
{
	using System;
	using NHibernate.Cfg;

	/// <summary>
	/// Defines an abstract base class for <see cref="INHContributor"/> which applies
	/// to all root types by default.
	/// </summary>
	public abstract class AbstractNHContributor : INHContributor
	{

		private Predicate<Type> _appliesToRootType = ((type) => true);

		/// <summary>
		/// Implements <see cref="INHContributor.AppliesToRootType"/>
		/// </summary>
		public Predicate<Type> AppliesToRootType
		{
			get { return _appliesToRootType; }
			set { _appliesToRootType = value; }
		}

		/// <summary>
		/// The actual contribution method.
		/// </summary>
		/// <param name="configuration">The configuration to be modified.</param>
		public abstract void Contribute(Configuration configuration);
	}
}
