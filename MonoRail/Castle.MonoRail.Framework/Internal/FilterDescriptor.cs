// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	public class FilterDescriptor
	{
		private FilterAttribute _attribute;
		private IFilter _filterInstance;

		public FilterDescriptor(FilterAttribute attribute)
		{
			_attribute = attribute;
		}

		public FilterAttribute Attribute
		{
			get { return _attribute; }
		}

		public Type FilterType
		{
			get { return _attribute.FilterType; }
		}

		public ExecuteEnum When
		{
			get { return _attribute.When; }
		}

		public int ExecutionOrder
		{
			get { return _attribute.ExecutionOrder; }
		}

		public IFilter FilterInstance
		{
			get { return _filterInstance; }
			set { _filterInstance = value; }
		}
	}
}
