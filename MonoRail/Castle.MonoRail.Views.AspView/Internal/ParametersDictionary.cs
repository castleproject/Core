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

namespace Castle.MonoRail.Views.AspView.Internal
{
	using System.Collections.Specialized;

	/// <summary>
	/// A hybrid dictionary that stores keys as case insensitive strings
	/// </summary>
	public class ParametersDictionary : HybridDictionary
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="ParametersDictionary"/> class.
		/// </summary>
		public ParametersDictionary()
			: base(true)
		{

		}

		/// <summary>
		/// Adds the given <paramref name="key"/> to the dictionary
		/// and assigns a value of <c>null</c> to it.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>A reference to this instance to allow method chaining</returns>
		public ParametersDictionary N(string key)
		{
			base[key] = null;
			return this;
		}


		/// <summary>
		/// Adds the given <paramref name="key"/> to the dictionary
		/// and assigns the <paramref name="value"/> to it.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns>
		/// A reference to this instance to allow method chaining
		/// </returns>
		public ParametersDictionary N(string key, object value)
		{
			base[key] = value;
			return this;
		}

	}

}
