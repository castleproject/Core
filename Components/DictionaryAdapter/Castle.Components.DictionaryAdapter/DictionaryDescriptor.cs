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
	using System.Collections.Generic;
	using System.Reflection;

	public class DictionaryDescriptor : PropertyDescriptor
	{
		private List<IDictionaryInitializer> initializers;

		public DictionaryDescriptor()
		{
		}

		public DictionaryDescriptor(PropertyInfo property)
			: base(property)
		{
		}

		/// <summary>
		/// Gets the initializers.
		/// </summary>
		/// <value>The initializers.</value>
		public ICollection<IDictionaryInitializer> Initializers
		{
			get { return initializers; }
		}

		/// <summary>
		/// Adds the dictionary initializers.
		/// </summary>
		/// <param name="inits">The initializers.</param>
		public DictionaryDescriptor AddInitializer(params IDictionaryInitializer[] inits)
		{
			return AddInitializers((IEnumerable<IDictionaryInitializer>)inits);
		}

		/// <summary>
		/// Adds the dictionary initializers.
		/// </summary>
		/// <param name="inits">The initializers.</param>
		public DictionaryDescriptor AddInitializers(IEnumerable<IDictionaryInitializer> inits)
		{
			if (inits != null)
			{
				if (initializers == null)
				{
					initializers = new List<IDictionaryInitializer>(inits);
				}
				else
				{
					initializers.AddRange(inits);
				}
			}
			return this;
		}
	}
}
