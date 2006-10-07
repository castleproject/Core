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

	
	[Serializable]
	public enum Polymorphism
	{
		Implicit,
		Explicit
	}

	[Serializable]
	public enum CacheEnum
	{
		Undefined,
		ReadOnly,
		ReadWrite,
		NonStrictReadWrite
	}

	[Serializable]
	public enum OuterJoinEnum
	{
		Auto,
		True,
		False
	}

	[Serializable]
	public enum ManyRelationCascadeEnum
	{
		None,
		All,
		SaveUpdate,
		Delete,
		AllDeleteOrphan
	}
}
