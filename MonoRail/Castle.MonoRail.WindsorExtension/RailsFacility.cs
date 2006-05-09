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
	using System.Web;

	using Castle.Model;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Internal.Graph;
	using Castle.MonoRail.Framework.Controllers;

	/// <summary>
	/// Facility responsible for registering the controllers in
	/// the tree.
	/// </summary>
	public class RailsFacility : AbstractFacility
	{
		private ControllerTree tree;

		public RailsFacility()
		{
		}

		protected override void Init()
		{
			Kernel.AddComponent("rails.controllertree", typeof(ControllerTree));
			Kernel.AddComponent("rails.wizardpagefactory", typeof(IWizardPageFactory), typeof(DefaultWizardPageFactory));

			tree = (ControllerTree) Kernel["rails.controllertree"];

			Kernel.ComponentModelCreated += new ComponentModelDelegate(OnComponentModelCreated);

			AddBuiltInControllers();

//			ExtractServicesFromModule();
		}

//		protected void ExtractServicesFromModule()
//		{
//			if (HttpContext.Current == null)
//			{
//				throw new FacilityException("No Http Context available while executing RailsFacility start up");
//			}
//
//			HttpModuleCollection modules = HttpContext.Current.ApplicationInstance.Modules;
//
//			foreach(String key in HttpContext.Current.ApplicationInstance.Modules.AllKeys)
//			{
//				object module = modules.Get(key);
//
//				EngineContextModule engineModule = module as EngineContextModule;
//
//				if (engineModule == null) continue;
//
//				ExtractServices(engineModule);
//
//				break;
//			}
//		}

//		protected void ExtractServices(IServiceProvider provider)
//		{
//			Kernel.Resolver.AddSubResolver(new SubDependencyResolverAdapter(provider));
//		}

		protected virtual void AddBuiltInControllers()
		{
			Kernel.AddComponent("files", typeof(FilesController), typeof(FilesController));
		}

		private void OnComponentModelCreated(ComponentModel model)
		{
			bool isController = typeof(Controller).IsAssignableFrom(model.Implementation);

			if (!isController && !typeof(ViewComponent).IsAssignableFrom(model.Implementation))
			{
				return;
			}

			// Ensure it's transient
			model.LifestyleType = LifestyleType.Transient;

			if (isController)
			{
				ControllerDescriptor descriptor = ControllerInspectionUtil.Inspect(model.Implementation);
			
				tree.AddController( descriptor.Area, descriptor.Name, model.Name );
			}
		}
	}

//	internal class SubDependencyResolverAdapter : ISubDependencyResolver
//	{
//		private readonly IServiceProvider provider;
//
//		public SubDependencyResolverAdapter(IServiceProvider provider)
//		{
//			this.provider = provider;
//		}
//
//		public object Resolve(ComponentModel model, DependencyModel dependency)
//		{
//			if (dependency.DependencyType == DependencyType.Parameter) return null;
//
//			return provider.GetService(dependency.TargetType);
//		}
//
//		public bool CanResolve(ComponentModel model, DependencyModel dependency)
//		{
//			if (dependency.DependencyType == DependencyType.Parameter) return false;
//
//			return provider.GetService(dependency.TargetType) != null;
//		}
//	}
}
