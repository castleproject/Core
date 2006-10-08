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

namespace Castle.MonoRail.Framework
{
	using System;

	/// <summary>
	/// Declares that for the specific method (action)
	/// no filter should be applied -- or an specific filter should be
	/// skipped.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=true), Serializable]
	public class SkipFilterAttribute : Attribute
	{
		private Type filterType;

		/// <summary>
		/// Constructs a SkipFilterAttribute which skips all filters.
		/// </summary>
		public SkipFilterAttribute()
		{
		}

		/// <summary>
		/// Constructs a SkipFilterAttribute associating 
		/// the filter type that should be skipped.
		/// </summary>
		/// <param name="filterType">The filter type to be skipped</param>
		public SkipFilterAttribute(Type filterType)
		{
			this.filterType = filterType;
		}

		/// <summary>
		/// Gets the type of the filter.
		/// </summary>
		/// <value>The type of the filter.</value>
		public Type FilterType
		{
			get { return filterType; }
		}

		/// <summary>
		/// Gets a value indicating whether [blanket skip].
		/// </summary>
		/// <value><c>true</c> if [blanket skip]; otherwise, <c>false</c>.</value>
		internal bool BlanketSkip
		{
			get { return filterType == null; }
		}
	}
}
