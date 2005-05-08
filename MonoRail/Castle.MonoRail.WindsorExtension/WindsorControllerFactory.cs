// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

	using Castle.Windsor;
	using Castle.MicroKernel;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Internal.Graph;


	/// <summary>
	/// Custom implementation of <see cref="IControllerFactory"/>
	/// that uses the WindsorContainer to obtain the 
	/// controller instances.
	/// </summary>
	public class WindsorControllerFactory : IControllerFactory
	{
		#region IControllerFactory

		public Controller GetController(UrlInfo urlInfo)
		{
			IWindsorContainer container = ContainerAccessorUtil.ObtainContainer();

			ControllerTree tree = null;
			
			try
			{
				tree = (ControllerTree) container["rails.controllertree"];
			}
			catch(ComponentNotFoundException)
			{
				throw new RailsException("ControllerTree not found. Check if the RailsFacility is properly configured");
			}

			String key = (String) tree.GetController(urlInfo.Area, urlInfo.Controller);

			if (key == null || key.Length == 0)
			{
				throw new RailsException("Controller not found. Controller Name: " + urlInfo.Controller);
			}

			if (container.Kernel.HasComponent(key))
			{
				return (Controller) container[key];
			}
			
			return null;
		}

		public void Release(Controller controller)
		{
			ContainerAccessorUtil.ObtainContainer().Release(controller);
		}

		#endregion
	}
}
