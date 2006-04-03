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

namespace Castle.MonoRail.Framework.Internal
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

		public FilterDescriptor(Type filterType, ExecuteEnum when, int executionOrder, FilterAttribute attribute)
		{
			this.filterType = filterType;
			this.when = when;
			this.executionOrder = executionOrder;
			this.attribute = attribute;
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
		}

		public IFilter FilterInstance
		{
			get { return filterInstance; }
			set { filterInstance = value; }
		}

		public FilterAttribute Attribute
		{
			get { return attribute; }
			set { attribute = value; }
		}

		public object Clone()
		{
			return new FilterDescriptor(filterType, when, executionOrder, attribute);
		}
	}
}