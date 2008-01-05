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

namespace Castle.MonoRail.Framework.Descriptors
{
	using System;

	/// <summary>
	/// Represents the meta information and type of
	/// an implementation of <see cref="IFilter"/>.
	/// </summary>
	public class FilterDescriptor : ICloneable
	{
		private readonly Type filterType;
		private readonly ExecuteEnum when;
		private readonly int executionOrder;
		private IFilter filterInstance;
		private FilterAttribute attribute;

		/// <summary>
		/// Initializes a new instance of the <see cref="FilterDescriptor"/> class.
		/// </summary>
		/// <param name="filterType">Type of the filter.</param>
		/// <param name="when">The flag that defines when it should run.</param>
		/// <param name="executionOrder">The execution order.</param>
		/// <param name="attribute">The attribute.</param>
		public FilterDescriptor(Type filterType, ExecuteEnum when, int executionOrder, FilterAttribute attribute)
		{
			this.filterType = filterType;
			this.when = when;
			this.executionOrder = executionOrder;
			this.attribute = attribute;
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
		/// Gets the flag that defines when it should run.
		/// </summary>
		/// <value>The when.</value>
		public ExecuteEnum When
		{
			get { return when; }
		}

		/// <summary>
		/// Gets the execution order.
		/// </summary>
		/// <value>The execution order.</value>
		public int ExecutionOrder
		{
			get { return executionOrder; }
		}

		/// <summary>
		/// Gets or sets the filter instance.
		/// </summary>
		/// <value>The filter instance.</value>
		public IFilter FilterInstance
		{
			get { return filterInstance; }
			set { filterInstance = value; }
		}

		/// <summary>
		/// Gets or sets the attribute.
		/// </summary>
		/// <value>The attribute.</value>
		public FilterAttribute Attribute
		{
			get { return attribute; }
			set { attribute = value; }
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public object Clone()
		{
			return new FilterDescriptor(filterType, when, executionOrder, attribute);
		}
	}
}
