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
	using System.Reflection;

	/// <summary>
	/// Creates <see cref="RescueDescriptor"/> from attributes 
	/// associated with the <see cref="Controller"/>
	/// </summary>
	public class DefaultRescueDescriptorProvider : IRescueDescriptorProvider
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

		public RescueDescriptor[] CollectRescues(MemberInfo member)
		{
			object[] attributes = member.GetCustomAttributes(typeof(IRescueDescriptorBuilder), true);

			ArrayList descriptors = new ArrayList();

			foreach(IRescueDescriptorBuilder builder in attributes)
			{
				descriptors.AddRange(builder.BuildRescueDescriptors());
			}

			return (RescueDescriptor[]) descriptors.ToArray(typeof(RescueDescriptor));
		}
	}
}
