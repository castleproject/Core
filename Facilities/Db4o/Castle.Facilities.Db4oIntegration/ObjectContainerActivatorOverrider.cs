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

namespace Castle.Facilities.Db4oIntegration
{
	using Db4objects.Db4o;

	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.ModelBuilder;
	
	/// <summary>
	/// Overrides the <see cref="IObjectContainer"/> component initialization.
	/// </summary>
	public class ObjectContainerActivatorOverrider : IContributeComponentModelConstruction
	{
		/// <summary>
		/// Overrides the <see cref="IComponentActivator"/>, if the component is the <see cref="IObjectContainer"/>.
		/// </summary>
		/// <param name="kernel">The kernel instance</param>
		/// <param name="model">The component model</param>
		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (model.Implementation == typeof(IObjectContainer))
			{
				model.CustomComponentActivator = typeof(ObjectContainerComponentActivator);
			}
		}
	}
}
