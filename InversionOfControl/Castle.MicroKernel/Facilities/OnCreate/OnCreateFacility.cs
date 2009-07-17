// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Facilities.OnCreate
{
	using Core;
	public delegate void OnCreateActionDelegate(IKernel kernel, object item);
	public delegate void OnCreateActionDelegate<T>(IKernel kernel, T item);

	/// <summary>
	/// This facility allows modification on objects after they are created
	/// </summary>
	public class OnCreateFacility : AbstractFacility
	{
		public const string OnCreatePropertyKey = "oncreate";
		protected override void Init()
		{
			Kernel.ComponentCreated += HandleComponentCreated;
		}

		/// <summary>
		/// Handles the ComponentCreated event and invoke the delegate stored in ExtendedProperties of <see cref="ComponentModel"/>
		/// </summary>
		/// <param name="model"></param>
		/// <param name="instance"></param>
		protected virtual void HandleComponentCreated(ComponentModel model, object instance)
		{
			if (model.ExtendedProperties.Contains(OnCreatePropertyKey))
			{
				var action = model.ExtendedProperties[OnCreatePropertyKey] as OnCreateActionDelegate;
				action(this.Kernel, instance);
			}
		}
	}
}
