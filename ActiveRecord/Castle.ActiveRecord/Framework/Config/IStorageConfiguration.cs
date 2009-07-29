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
	using System.Collections.Generic;

	/// <summary>
	/// Interface for configuration of a database storage.
	/// </summary>
	/// <remarks>
	/// This interface is subject to further modification. If you need to implement
	/// this interface, please inherit from <see cref="DefaultStorageConfiguration"/>.
	/// </remarks>
	public interface IStorageConfiguration
	{
		/// <summary>
		/// The type selections for that storage.
		/// </summary>
		IEnumerable<StorageTypeSelection> TypeSelections { get; }

		/// <summary>
		/// Contains the name-value-pairs for the NHibernate configuration
		/// </summary>
		IDictionary<string, string> ConfigurationValues { get; }
	}
}