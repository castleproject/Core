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
	using System.Collections.Generic;

	/// <summary>
	/// Default implementation for <see cref="IStorageConfiguration"/>.
	/// </summary>
	public class DefaultStorageConfiguration : IStorageConfiguration
	{
		private readonly IDictionary<string, string> _configurationValues = new Dictionary<string, string>();

		/// <summary>
		/// The type selections writable store.
		/// </summary>
		protected readonly List<StorageTypeSelection> TypeSelectionList = new List<StorageTypeSelection>();

		#region IStorageConfiguration Members

		/// <summary>
		/// The type selections for that storage.
		/// </summary>
		public IEnumerable<StorageTypeSelection> TypeSelections
		{
			get { return TypeSelectionList; }
		}

		/// <summary>
		/// Contains the name-value-pairs for the NHibernate configuration
		/// </summary>
		public IDictionary<string, string> ConfigurationValues
		{
			get { return _configurationValues; }
		}

		#endregion

		/// <summary>
		/// Creates a type name in the form Type.Fullname, Assembly.Simple.Name
		/// </summary>
		/// <typeparam name="T">The type for which to create the name</typeparam>
		/// <returns>The created name</returns>
		protected static string GetTypeName<T>()
		{
			Type t = typeof (T);
			return string.Format("{0}, {1}", t.FullName, t.Assembly.GetName().Name);
		}
	}
}