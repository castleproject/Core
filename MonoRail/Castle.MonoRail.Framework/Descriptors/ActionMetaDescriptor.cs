// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;

	/// <summary>
	/// Holds the meta information for an action
	/// </summary>
	[Serializable]
	public class ActionMetaDescriptor : BaseMetaDescriptor
	{
		private SkipRescueAttribute skipRescue;
		private AccessibleThroughAttribute accessibleThrough;
		private IList<SkipFilterAttribute> skipFilters = new List<SkipFilterAttribute>();
		private TransformFilterDescriptor[] transformFilters = new TransformFilterDescriptor[0];

		/// <summary>
		/// Gets or sets the skip rescue associated with this action.
		/// </summary>
		/// <value>The skip rescue.</value>
		public SkipRescueAttribute SkipRescue
		{
			get { return skipRescue; }
			set { skipRescue = value; }
		}

		/// <summary>
		/// Gets or sets the accessible through definition associated with this action.
		/// </summary>
		/// <value>The accessible through.</value>
		public AccessibleThroughAttribute AccessibleThrough
		{
			get { return accessibleThrough; }
			set { accessibleThrough = value; }
		}

		/// <summary>
		/// Gets the skip filters associated with this action.
		/// </summary>
		/// <value>The skip filters.</value>
		public IList<SkipFilterAttribute> SkipFilters
		{
			get { return skipFilters; }
		}

		/// <summary>
		/// Gets or sets the transform filters associated with this action.
		/// </summary>
		/// <value>The transform filters.</value>
		public TransformFilterDescriptor[] TransformFilters
		{
			get { return transformFilters; }
			set { transformFilters = value; }
		}
	}
}
