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

	/// <summary>
	/// Defines the values for optimistic locking
	/// </summary>
	[Serializable]
	public enum OptimisticLocking
	{
		/// <summary>
		/// do not use optimistic locking
		/// </summary>
		None,
		
		/// <summary>
		/// check the version/timestamp columns
		/// </summary>
		Version,
		
		/// <summary>
		/// check the changed columns
		/// </summary>
		Dirty, 
		
		/// <summary>
		/// check all columns
		/// </summary>
		All
	}

	/// <summary>
	/// Define the polymorphism options
	/// </summary>
	[Serializable]
	public enum Polymorphism
	{
		/// <summary>
		/// Implicit polymorphism
		/// </summary>
		Implicit,
		/// <summary>
		/// Explicit polymorphism
		/// </summary>
		Explicit
	}

	/// <summary>
	/// Define the caching options
	/// </summary>
	[Serializable]
	public enum CacheEnum
	{
		/// <summary>
		/// Default value, no caching
		/// </summary>
		Undefined,
		/// <summary>
		/// Read only cache - use for cases where no write are performed.
		/// </summary>
		ReadOnly,
		/// <summary>
		/// Read write cache
		/// </summary>
		ReadWrite,
		/// <summary>
		/// Read write cache with looser semantics.
		/// Check NHibernate's documentation for the detials.
		/// </summary>
		NonStrictReadWrite
	}

	/// <summary>
	/// Define outer join options
	/// </summary>
	[Serializable]
	public enum OuterJoinEnum
	{
		/// <summary>
		/// Let NHibernate decide what to do
		/// </summary>
		Auto,
		/// <summary>
		/// Use outer join
		/// </summary>
		True,
		/// <summary>
		/// Do not use outer join
		/// </summary>
		False
	}

	/// <summary>
	/// Define relation cascade options
	/// </summary>
	[Serializable]
	public enum ManyRelationCascadeEnum
	{
		/// <summary>
		/// No cascading will be done
		/// </summary>
		None,
		/// <summary>
		/// Cascade save/update/delete operation
		/// </summary>
		All,
		/// <summary>
		/// Cascade save/update operation
		/// </summary>
		SaveUpdate,
		/// <summary>
		/// Cascade delete operation
		/// </summary>
		Delete,
		/// <summary>
		/// Cascade save/update/delete operation, and remove an orphan children
		/// when deleting.
		/// </summary>
		AllDeleteOrphan
	}
}
