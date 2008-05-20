// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;

	/// <summary>
	/// Collection of <see cref="DependencyModel"/>.
	/// </summary>
#if !SILVERLIGHT
	[Serializable]
#endif
	public class DependencyModelCollection : List<DependencyModel>
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
		public DependencyModelCollection(IList<DependencyModel> dependencies)
			: base(dependencies)
		{
		}
	}
}