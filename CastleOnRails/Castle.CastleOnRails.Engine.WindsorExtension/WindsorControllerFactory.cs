// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.CastleOnRails.Engine.WindsorExtension
{
	using System;
	using System.Web;

	using Castle.Windsor;

	using Castle.CastleOnRails.Framework;

	/// <summary>
	/// Summary description for WindsorControllerFactory.
	/// </summary>
	public class WindsorControllerFactory : IControllerFactory
	{
		public WindsorControllerFactory()
		{
		}

		public Controller GetController(string name)
		{
			IWindsorContainer container = ObtainContainer();

			if (container.Kernel.HasComponent(name))
			{
				return (Controller) container.Resolve(name);
			}
			
			String other_name = name + "controller";
			
			if (container.Kernel.HasComponent(other_name))
			{
				return (Controller) container.Resolve(other_name);
			}

			other_name = name + ".controller";
			
			if (container.Kernel.HasComponent(other_name))
			{
				return (Controller) container.Resolve(other_name);
			}

			throw new RailsException( String.Format("Could not find controller for {0}",
				name) );
		}

		public void Release(Controller controller)
		{
			ObtainContainer().Release(controller);
		}

		private IWindsorContainer ObtainContainer()
		{
			IContainerAccessor containerAccessor = 
				HttpContext.Current.ApplicationInstance as IContainerAccessor;
	
			if (containerAccessor == null)
			{
				throw new RailsException("You must extend the HttpApplication in your web project " + 
					"and implement the IContainerAccessor to properly expose your container instance.");
			}
	
			IWindsorContainer container = containerAccessor.Container;
	
			if (container == null)
			{
				throw new RailsException("The container seems to be unavailable in your HttpApplication subclass.");
			}

			return container;
		}
	}
}
