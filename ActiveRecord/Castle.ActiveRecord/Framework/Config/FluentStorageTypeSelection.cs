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
	///<summary>
	/// Fluent configuration of a storage type selection
	///</summary>
	public class FluentStorageTypeSelection : StorageTypeSelection
	{
		/// <summary>
		/// Internal storage of the configuration this selection belongs to.
		/// </summary>
		protected FluentStorageConfiguration StorageConfiguration;

		/// <summary>
		/// Creates a new fluent selection configuration.
		/// </summary>
		/// <param name="storageConfiguration">The object that creates and uses the selection.</param>
		public FluentStorageTypeSelection(FluentStorageConfiguration storageConfiguration)
		{
			StorageConfiguration = storageConfiguration;
		}

		/// <summary>
		/// The storage configuration that created the current selection.
		/// </summary>
		public FluentStorageConfiguration As
		{
			get { return StorageConfiguration; }
		}

		/// <summary>
		/// Adds another selection to the storage configuration.
		/// </summary>
		public FluentStorageTypeSelection And
		{
			get { return StorageConfiguration.For; }
		}

		/// <summary>
		/// Sets the selection to be used for all types are not explicitly selected.
		/// This may be used only on one storage configuration.
		/// </summary>
		/// <returns></returns>
		public FluentStorageTypeSelection AllOtherTypes()
		{
			Default = true;
			return this;
		}

		/// <summary>
		/// Selects the type and all subtypes of the type to use the storage.
		/// </summary>
		/// <typeparam name="T">The type to choose.</typeparam>
		/// <returns>The fluent selection itself.</returns>
		public FluentStorageTypeSelection SubtypesOf<T>()
		{
			return this;
		}

		/// <summary>
		/// Selects all types in or below the namespace of the given type.
		/// </summary>
		/// <typeparam name="T">The type to select.</typeparam>
		/// <returns>The fluent selection itself.</returns>
		public FluentStorageTypeSelection InNamespaceOf<T>()
		{
			return this;
		}

		/// <summary>
		/// Sets the way the selected types are mapped. The default mapping is ActiveRecord
		/// attributes.
		/// </summary>
		/// <param name="specification">The mapping specification</param>
		/// <returns>The fluent selection itself.</returns>
		public FluentStorageTypeSelection MappedBy(IMappingSpecification specification)
		{
			return this;
		}

		/// <summary>
		/// Selects all types in the assembly of the given type.
		/// </summary>
		/// <typeparam name="T">The type to select.</typeparam>
		/// <returns>The fluent selection itself.</returns>
		public FluentStorageTypeSelection TypesInAssemblyOf<T>()
		{
			return this;
		}
	}
}