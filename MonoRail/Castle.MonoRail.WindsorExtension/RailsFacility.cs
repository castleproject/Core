// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using Castle.Core;
	using Castle.MicroKernel.Facilities;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Descriptors;
	using Castle.MonoRail.Framework.Services;
	using Castle.MonoRail.Framework.Services.Utils;

	/// <summary>
	/// Facility responsible for registering the controllers in
	/// the controllerTree.
	/// </summary>
	public class RailsFacility : AbstractFacility
	{
		private IControllerTree controllerTree;
		private IViewComponentRegistry componentRegistry;

		protected override void Init()
		{
			RegisterWindsorLocatorStrategyWithinMonoRail();

			Kernel.AddComponent("mr.controllertree", typeof(IControllerTree), typeof(DefaultControllerTree));
			Kernel.AddComponent("mr.wizardpagefactory", typeof(IWizardPageFactory), typeof(DefaultWizardPageFactory));
			Kernel.AddComponent("mr.viewcomponentregistry", typeof(IViewComponentRegistry), typeof(DefaultViewComponentRegistry));
			Kernel.AddComponent("mr.controllerfactory", typeof(IControllerFactory), typeof(WindsorControllerFactory));
			Kernel.AddComponent("mr.filterFactory", typeof(IFilterFactory), typeof(WindsorFilterFactory));
			Kernel.AddComponent("mr.viewcompfactory", typeof(IViewComponentFactory), typeof(WindsorViewComponentFactory));

			controllerTree = Kernel.Resolve<IControllerTree>();
			componentRegistry = Kernel.Resolve<IViewComponentRegistry>();

			Kernel.ComponentModelCreated += OnComponentModelCreated;
		}

		private void RegisterWindsorLocatorStrategyWithinMonoRail()
		{
			ServiceProviderLocator.Instance.AddLocatorStrategy(new WindsorAccessorStrategy());
		}

		private void OnComponentModelCreated(ComponentModel model)
		{
			bool isController = typeof(IController).IsAssignableFrom(model.Implementation);
			bool isViewComponent = typeof(ViewComponent).IsAssignableFrom(model.Implementation);

			if (!isController && !isViewComponent)
			{
				return;
			}

			// Ensure it's transient
			model.LifestyleType = LifestyleType.Transient;
			model.InspectionBehavior = PropertiesInspectionBehavior.DeclaredOnly;

			if (isController)
			{
				ControllerDescriptor descriptor = ControllerInspectionUtil.Inspect(model.Implementation);

				controllerTree.AddController(descriptor.Area, descriptor.Name, model.Implementation);
			}

			if (isViewComponent)
			{
				componentRegistry.AddViewComponent(model.Name, model.Implementation);
			}
		}

		public class WindsorAccessorStrategy : ServiceProviderLocator.IAccessorStrategy
		{
			public IServiceProviderEx LocateProvider()
			{
				return WindsorContainerAccessorUtil.ObtainContainer();
			}
		}
	}
}
