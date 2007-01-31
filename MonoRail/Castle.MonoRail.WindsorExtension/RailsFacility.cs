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
	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Controllers;
	using Castle.MonoRail.Framework.Services;
	using Castle.MonoRail.Framework.Services.Utils;

	/// <summary>
	/// Facility responsible for registering the controllers in
	/// the controllerTree.
	/// </summary>
	public class RailsFacility : AbstractFacility
	{
		private IControllerTree controllerTree;
		private IViewComponentTree componentTree;

		public RailsFacility()
		{
		}

		protected override void Init()
		{
			Kernel.AddComponent("rails.controllertree", typeof(IControllerTree), typeof(DefaultControllerTree));
			Kernel.AddComponent("rails.wizardpagefactory", typeof(IWizardPageFactory), typeof(DefaultWizardPageFactory));
			Kernel.AddComponent("rails.viewcomponenttree", typeof(IViewComponentTree), typeof(DefaultViewComponentTree));

			controllerTree = (IControllerTree)Kernel["rails.controllertree"];
			componentTree = (IViewComponentTree)Kernel["rails.viewcomponenttree"];

			Kernel.ComponentModelCreated += new ComponentModelDelegate(OnComponentModelCreated);

			MonoRailConfiguration.GetConfig().ServiceEntries.RegisterService(
				ServiceIdentification.ControllerTree, typeof(ControllerTreeAccessor));

			AddBuiltInControllers();
		}

		protected virtual void AddBuiltInControllers()
		{
			Kernel.AddComponent("files", typeof(FilesController), typeof(FilesController));
		}

		private void OnComponentModelCreated(ComponentModel model)
		{
			bool isController = typeof(Controller).IsAssignableFrom(model.Implementation);
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
				componentTree.AddViewComponent(model.Name, model.Implementation);
			}
		}
	}
}
