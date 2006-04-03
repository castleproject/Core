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
	using System.Collections;

	/// <summary>
	/// Creates <see cref="FilterDescriptor"/> from attributes 
	/// associated with the <see cref="Controller"/>
	/// </summary>
	public class DefaultFilterDescriptorProvider : IFilterDescriptorProvider
	{
		/// <summary>
		/// Gives a chance to the provider initialize
		/// itself and request services from the 
		/// service provider
		/// </summary>
		/// <param name="serviceProvider"></param>
		public void Init(IServiceProvider serviceProvider)
		{
		}

		public FilterDescriptor[] CollectFilters(Type controllerType)
		{
			object[] attributes = controllerType.GetCustomAttributes(typeof(IFilterDescriptorBuilder), true);

			ArrayList filters = new ArrayList();
			
			foreach(IFilterDescriptorBuilder builder in attributes)
			{
				filters.AddRange(builder.BuildFilterDescriptors());
			}

			return (FilterDescriptor[]) filters.ToArray(typeof(FilterDescriptor));
		}
	}
}
