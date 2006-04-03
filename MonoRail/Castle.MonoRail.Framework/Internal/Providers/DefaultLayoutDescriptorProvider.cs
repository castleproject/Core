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
	using System.Reflection;

	/// <summary>
	/// Creates <see cref="LayoutDescriptor"/> from attributes 
	/// associated with the <see cref="Controller"/> and its actions
	/// </summary>
	public class DefaultLayoutDescriptorProvider : ILayoutDescriptorProvider
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

		public LayoutDescriptor CollectLayout(MemberInfo memberInfo)
		{
			object[] attributes = memberInfo.GetCustomAttributes(typeof(ILayoutDescriptorBuilder), true);

			if (attributes.Length == 1)
			{
				return (attributes[0] as ILayoutDescriptorBuilder).BuildLayoutDescriptor();
			}

			return null;
		}
	}
}
