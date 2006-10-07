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
	
	/// <summary>
	/// Represents an ActiveRecord Query.
	/// </summary>
	public interface IActiveRecordQuery
	{
		/// <summary>
		/// Gets the target type of this query
		/// </summary>
		Type Target { get; }

		/// <summary>
		/// Executes the specified query and return the results
		/// </summary>
		/// <param name="session">The session to execute the query in.</param>
		/// <returns></returns>
		object Execute(ISession session);

		/// <summary>
		/// Enumerates over the result of the query.
		/// Note: Only use if you expect most of your values to already exist in the second level cache!
		/// </summary>
		IEnumerable Enumerate(ISession session);
	}
}
