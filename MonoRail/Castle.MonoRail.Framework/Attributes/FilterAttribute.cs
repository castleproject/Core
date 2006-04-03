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

	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Decorates a controller associating a <see cref="IFilter"/>
	/// implementation with it. More than one can be associated.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true), Serializable]
	public class FilterAttribute : Attribute, IFilterDescriptorBuilder
	{
		private readonly Type filterType;
		private readonly ExecuteEnum when;
		private int executionOrder = Int32.MaxValue;

		/// <summary>
		/// Constructs a FilterAttribute associating 
		/// the filter type and when the filter should be invoked.
		/// </summary>
		/// <param name="when"></param>
		/// <param name="filterType"></param>
		public FilterAttribute(ExecuteEnum when, Type filterType)
		{
			if (!typeof(IFilter).IsAssignableFrom(filterType))
			{
				throw new ArgumentException("The specified type does not implement IFilter");
			}

			this.filterType = filterType;
			this.when = when;
		}

		public Type FilterType
		{
			get { return filterType; }
		}

		public ExecuteEnum When
		{
			get { return when; }
		}

		public int ExecutionOrder
		{
			get { return executionOrder; }
			set { executionOrder = value; }
		}

		public FilterDescriptor[] BuildFilterDescriptors()
		{
			return new FilterDescriptor[] { new FilterDescriptor(FilterType, when, executionOrder, this) };
		}
	}
}