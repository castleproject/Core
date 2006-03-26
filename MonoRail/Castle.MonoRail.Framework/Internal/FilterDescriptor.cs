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
		private FilterItem _item;
		private IFiltersAttribute _attribute;
		private IFilter _filterInstance;
		private bool isClone = false;

		public FilterDescriptor(IFiltersAttribute attribute, FilterItem item)
		{
			_attribute = attribute;
			_item = item;
		}

		public IFiltersAttribute Attribute
		{
			get { return _attribute; }
		}

		public Type FilterType
		{
			get { return _item.FilterType; }
		}

		public ExecuteEnum When
		{
			get { return _item.When; }
		}

		public int ExecutionOrder
		{
			get { return _item.ExecutionOrder; }
		}

		public IFilter FilterInstance
		{
			get { return _filterInstance; }
			set
			{
				if (!isClone)
					throw new InvalidOperationException("FilterInstance property could only be set on FilterDescriptor clones.");
				_filterInstance = value;
			}
		}

		public FilterDescriptor Clone()
		{
			FilterDescriptor clone = (FilterDescriptor) this.MemberwiseClone();
			clone.isClone = true;
			return clone;
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}
	}
}