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

namespace Castle.MonoRail.WindsorExtension
{
	using System;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Services;
	using Castle.Windsor;


	public class WindsorViewComponentFactory : AbstractViewComponentFactory
	{
		private IViewEngine viewEngine;

		public override IViewEngine ViewEngine
		{
			get { return viewEngine; }
			set { viewEngine = value; }
		}

		public override ViewComponent Create(String name)
		{
			IWindsorContainer container = ContainerAccessorUtil.ObtainContainer();

			if (container.Kernel.HasComponent(name))
			{
				try
				{
					return (ViewComponent) container[name];
				}
				catch(InvalidCastException ex)
				{
					throw new RailsException("The component registered for the given key could not be casted into a ViewComponent. Key " + name, ex);
				}
			}

			return base.Create(name);
		}
	}
}
