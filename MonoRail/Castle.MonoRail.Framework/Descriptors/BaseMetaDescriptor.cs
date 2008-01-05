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
	/// Common meta descriptor that represents configuration share by 
	/// controllers and actions.
	/// </summary>
	[Serializable]
	public abstract class BaseMetaDescriptor
	{
		private LayoutDescriptor layout;
		private RescueDescriptor[] rescues = new RescueDescriptor[0];
		private ResourceDescriptor[] resources = new ResourceDescriptor[0];
		private ICachePolicyConfigurer cacheConfigurer;

		/// <summary>
		/// Gets or sets the layout descriptor.
		/// </summary>
		/// <value>The layout.</value>
		public LayoutDescriptor Layout
		{
			get { return layout; }
			set { layout = value; }
		}

		/// <summary>
		/// Gets or sets the rescues descriptors.
		/// </summary>
		/// <value>The rescues.</value>
		public RescueDescriptor[] Rescues
		{
			get { return rescues; }
			set { rescues = value; }
		}

		/// <summary>
		/// Gets or sets the resources descriptors.
		/// </summary>
		/// <value>The resources.</value>
		public ResourceDescriptor[] Resources
		{
			get { return resources; }
			set { resources = value; }
		}

		/// <summary>
		/// Gets the cache configurer associated with this action.
		/// </summary>
		/// <value>The cache configurers.</value>
		public ICachePolicyConfigurer CacheConfigurer
		{
			get { return cacheConfigurer; }
			set { cacheConfigurer = value; }
		}
	}
}
