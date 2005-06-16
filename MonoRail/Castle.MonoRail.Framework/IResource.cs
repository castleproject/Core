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

namespace Castle.MonoRail.Framework
{
	/// <summary>
	/// Dictates the contract for resources that are publishable
	/// through the PropertyBag context.
	/// </summary>
	public interface IResource
	{
		/// <summary>
		/// Returns the object linked to the specific key.
		/// </summary>
		object this[ string key ] { get; }

		/// <summary>
		/// Returns the object linked to the specific key as a string.
		/// </summary>
		string GetString( string key );

		/// <summary>
		/// Returns the object linked to the specific key.
		/// </summary>
		object GetObject( string key );
	}

    /// <summary>
	/// Marker interface that declares that this attribute should
	/// be used as a definition for a resource in the user ResourceFactory.
	/// </summary>
	public interface IResourceDefinition
	{
	}
}
