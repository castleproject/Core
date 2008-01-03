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

namespace Castle.MonoRail.WindsorExtension
{
	using System;
	using Castle.MicroKernel;
	using Castle.MonoRail.Framework;

	/// <summary>
	/// Custom implementation of <see cref="IControllerFactory"/>
	/// that uses the WindsorContainer to obtain the 
	/// controller instances.
	/// </summary>
	public class WindsorControllerFactory : IControllerFactory
	{
		private readonly IControllerTree controllerTree;
		private readonly IKernel kernel;

		public WindsorControllerFactory(IControllerTree controllerTree, IKernel kernel)
		{
			this.controllerTree = controllerTree;
			this.kernel = kernel;
		}

		public IController CreateController(string area, string controller)
		{
			Type implType = controllerTree.GetController(area, controller);

			if (implType == null)
			{
				throw new ControllerNotFoundException("Controller not found on the Windsor container instance. " +
					"Have you registered it? Name: '" + controller + "' area: '" + area + "'");
			}

			return CreateController(implType);
		}

		public IController CreateController(Type controllerType)
		{
			return (IController) kernel.Resolve(controllerType);
		}

		public void Release(IController controller)
		{
			kernel.ReleaseComponent(controller);
		}
	}
}
