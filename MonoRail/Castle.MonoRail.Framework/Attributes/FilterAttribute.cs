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
	/// the specified filters should NOT be applied.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=true), Serializable]
	public class SkipFilterAttribute : Attribute, ISkipFilterAttribute
	{
		private Type[] filtersToSkip;

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
		public SkipFilterAttribute(params Type[] filtersToSkip)
		{
			this.filtersToSkip = filtersToSkip;
		}

		public Type[] FiltersToSkip
		{
			get { return filtersToSkip; }
		}

		[Obsolete("Use FiltersToSkip")]
		public Type FilterType
		{
			get { return filtersToSkip != null && filtersToSkip.Length > 0 ? filtersToSkip[0] : null; }
		}

		[Obsolete("Use SkipAllFilters")]
		public bool BlanketSkip
		{
			get { return SkipAllFilters; }
		}
		
		public bool SkipAllFilters
		{
			get { return filtersToSkip == null || filtersToSkip.Length == 0; }
		}
	}

	/// <summary>
	/// Decorates a controller associating a <see cref="IFilter"/>
	/// implementation with it. More than one can be associated.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true), Serializable]
	public class FilterAttribute : Attribute, IFiltersAttribute
	{
		private FilterItem[] filters;

		/// <summary>
		/// Constructs a FilterAttribute associating 
		/// the filter type and when the filter should be invoked.
		/// </summary>
		/// <param name="when"></param>
		/// <param name="filterType"></param>
		public FilterAttribute(ExecuteEnum when, Type filterType)
		{
			this.filters = new FilterItem[] { new FilterItem(filterType, when) };
		}

		public FilterItem[] GetFilters()
		{
			return filters;
		}

		public Type FilterType
		{
			get { return filters[0].FilterType; }
		}

		public ExecuteEnum When
		{
			get { return filters[0].When; }
		}

		public int ExecutionOrder
		{
			get { return filters[0].ExecutionOrder; }
			set { filters[0].ExecutionOrder = value; }
		}
	}
}