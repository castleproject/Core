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

namespace Castle.Core
{
	using System;
	using System.Collections;

	/// <summary>
	/// Collection of <see cref="DependencyModel"/>.
	/// </summary>
	[Serializable]
	public class DependencyModelCollection : ReadOnlyCollectionBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyModelCollection"/> class.
		/// </summary>
		public DependencyModelCollection()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyModelCollection"/> class.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		public DependencyModelCollection(DependencyModelCollection dependencies)
		{
			foreach(DependencyModel model in dependencies)
			{
				Add(model);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyModelCollection"/> class.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		public DependencyModelCollection(DependencyModel[] dependencies)
		{
			foreach(DependencyModel model in dependencies)
			{
				Add(model);
			}
		}

		/// <summary>
		/// Adds the specified model.
		/// </summary>
		/// <param name="model">The model.</param>
		public void Add(DependencyModel model)
		{
			InnerList.Add(model);
		}
		
		/// <summary>
		/// Removes the specified model.
		/// </summary>
		/// <param name="model">The model.</param>
		public void Remove(DependencyModel model)
		{
			InnerList.Remove(model);
		}

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public void Clear()
		{
			InnerList.Clear();
		}

		/// <summary>
		/// Determines whether this collection contains the the specified model.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>
		/// <c>true</c> if the collection contains the specified model; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(DependencyModel model)
		{
			return InnerList.Contains(model);
		}
	}
}
