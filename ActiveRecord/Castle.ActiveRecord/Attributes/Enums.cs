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
	/// Define the possible fetch option values
	/// </summary>
	public enum FetchEnum
	{
		/// <summary>
		/// Let NHibernate decide what to do here
		/// </summary>
		Unspecified,
		/// <summary>
		/// Use a JOIN to load the data
		/// </summary>
		Join,
		/// <summary>
		/// Use a seperate SELECT statement to load the data
		/// </summary>
		Select,
		/// <summary>
		/// Use a seperate SELECT statement to load the data, re-running the original query in a subselect
		/// </summary>
		SubSelect
	}

	/// <summary>
	/// Defines the possible lazy option values.
	/// See http://nhforge.org/doc/nh/en/index.html#performance-fetching
	/// </summary>
	public enum FetchWhen
	{
		/// <summary>
		/// Specifies that the property should be fetched lazily when the instance variable is first accessed.
		/// </summary>
		OnInvoke,

		/// <summary>
		/// Specifies that the association will always be eagerly fetched.
		/// </summary>
		Immediate
	}

	/// <summary>
	/// Defines the cascading behaviour of this association.
	/// </summary>
	/// <remarks>
	/// Entities has associations to other objects, this may be an association to a single item (<see cref="BelongsToAttribute" />)
	/// or an association to a collection (<see cref="HasManyAttribute" />, <see cref="HasManyToAnyAttribute" />).
	/// At any rate, you are able to tell NHibernate to automatically traverse an entity's associations, and act according 
	/// to the cascade option. For instance, adding an unsaved entity to a collection with <see cref="CascadeEnum.SaveUpdate" />
	/// cascade will cause it to be saved along with its parent object, without any need for explicit instructions on our side.
	/// </remarks>
	public enum CascadeEnum
	{
		/// <summary>
		/// No cascading. This is the default.
		/// The cascade should be handled manually.
		/// </summary>
		None,
		/// <summary>
		/// Cascade save, update and delete.
		/// When the object is saved, updated or deleted, the associations will be checked
		/// and the objects found will also be saved, updated or deleted.
		/// </summary>
		All,
		/// <summary>
		/// Cascade save and update.
		/// When the object is saved or updated, the associations will be checked and any object that requires
		/// will be saved or updated (including saving or updating the associations in many-to-many scenario).
		/// </summary>
		SaveUpdate,
		/// <summary>
		/// Cascade delete.
		/// When the object is deleted, all the objects in the association will be deleted as well.
		/// </summary>
		Delete
	}

	/// <summary>
	/// Defines the cascading behaviour of this association.
	/// </summary>
	/// <remarks>
	/// Entities has associations to other objects, this may be an association to a single item (<see cref="BelongsToAttribute" />)
	/// or an association to a collection (<see cref="HasManyAttribute" />, <see cref="HasManyToAnyAttribute" />).
	/// At any rate, you are able to tell NHibernate to automatically traverse an entity's associations, and act according 
	/// to the cascade option. For instance, adding an unsaved entity to a collection with <see cref="CascadeEnum.SaveUpdate" />
	/// cascade will cause it to be saved along with its parent object, without any need for explicit instructions on our side.
	/// </remarks>
	[Serializable]
	public enum ManyRelationCascadeEnum
	{
		/// <summary>
		/// No cascading. This is the default.
		/// The cascade should be handled manually.
		/// </summary>
		None,
		/// <summary>
		/// Cascade save, update and delete.
		/// When the object is saved, updated or deleted, the associations will be checked
		/// and the objects found will also be saved, updated or deleted.
		/// </summary>
		All,
		/// <summary>
		/// Cascade save and update.
		/// When the object is saved or updated, the associations will be checked and any object that requires
		/// will be saved or updated (including saving or updating the associations in many-to-many scenario).
		/// </summary>
		SaveUpdate,
		/// <summary>
		/// Cascade delete.
		/// When the object is deleted, all the objects in the association will be deleted as well.
		/// </summary>
		Delete,
		/// <summary>
		/// Cascade save, update and delete, removing orphan children.
		/// When an object is saved, updated or deleted, the associations will be checked and all objects found
		/// will be saved, updated or deleted as well.
		/// In additional to that, when an object is removed from the association and not associated with another object (orphaned), it will also be deleted.
		/// </summary>
		AllDeleteOrphan
	}
}
