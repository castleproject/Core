// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Components.DictionaryAdapter
{
	using System;
	using System.Diagnostics;

	internal sealed class ListProjectionDebugView<T>
	{
		private readonly ListProjection<T> projection;

		public ListProjectionDebugView(ListProjection<T> projection)
		{
			if (projection == null)
				throw new ArgumentNullException("projection");

			this.projection = projection;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				T[] array = new T[projection.Count];
				projection.CopyTo(array, 0);
				return array;
			}
		}

		public ICollectionAdapter<T> Adapter
		{
			get { return projection.Adapter; }
		}
	}
}
