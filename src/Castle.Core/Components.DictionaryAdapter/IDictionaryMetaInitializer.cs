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

namespace Castle.Components.DictionaryAdapter
{
	/// <summary>
	///  Contract for dictionary meta-data initialization.
	/// </summary>
	public interface IDictionaryMetaInitializer : IDictionaryBehavior
	{
		/// <summary>
		///		Initializes the given <see cref="DictionaryAdapterMeta"/> object.
		/// </summary>
		/// <param name="factory">The dictionary adapter factory.</param>
		/// <param name="dictionaryMeta">The dictionary adapter meta.</param>
		/// 
		void Initialize(IDictionaryAdapterFactory factory, DictionaryAdapterMeta dictionaryMeta);

		/// <summary>
		///		Determines whether the given behavior should be included in a new
		///		<see cref="DictionaryAdapterMeta"/> object.
		/// </summary>
		/// <param name="behavior">A dictionary behavior or annotation.</param>
		/// <returns>True if the behavior should be included; otherwise, false.</returns>
		/// <remarks>
		///		<see cref="IDictionaryMetaInitializer"/> behaviors are always included,
		///		regardless of the result of this method.
		///	</remarks>
		/// 
		bool ShouldHaveBehavior(object behavior);
	}
}
