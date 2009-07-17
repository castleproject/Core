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

namespace Castle.MicroKernel.Registration.Facilities
{
	using MicroKernel.Facilities.OnCreate;
	/// <summary>
	/// Adds the actions to ExtendedProperties.
	/// </summary>
	/// <typeparam name="S"></typeparam>
	public class OnCreateComponentDescriptor<S>:ComponentDescriptor<S>
	{
		private readonly OnCreateActionDelegate<S>[] actions;

		public OnCreateComponentDescriptor(OnCreateActionDelegate<S>[] actions)
		{
			this.actions = actions;
		}

		protected internal override void ApplyToModel(IKernel kernel, Castle.Core.ComponentModel model)
		{
			if (actions == null)
				return;
			string key = OnCreateFacility.OnCreatePropertyKey;
			OnCreateActionDelegate actionDelegate = delegate { };
			foreach (var action in actions)
			{
				var current = action;
				actionDelegate += (kernell, item) => current(kernell, (S)item); ;
			}
			model.ExtendedProperties[key] = actionDelegate;
		}

	}
}
