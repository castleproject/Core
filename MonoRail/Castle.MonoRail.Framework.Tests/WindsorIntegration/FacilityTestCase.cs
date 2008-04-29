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

namespace Castle.MonoRail.Framework.Tests.WindsorIntegration
{
	using System;
	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;
	using Castle.MonoRail.WindsorExtension;
	using NUnit.Framework;

	[TestFixture]
	public class FacilityTestCase
	{
		private WindsorContainer container;

		[SetUp]
		public void Init()
		{
			container = new WindsorContainer();
			container.AddFacility("mr", new MonoRailFacility());
		}

		[Test]
		public void FacilityRegisterMRServices()
		{
			Assert.IsTrue(container.Kernel.HasComponent(typeof(IControllerTree)));
			Assert.IsTrue(container.Kernel.HasComponent(typeof(IViewComponentRegistry)));
			Assert.IsTrue(container.Kernel.HasComponent(typeof(IControllerFactory)));
			Assert.IsTrue(container.Kernel.HasComponent(typeof(IFilterFactory)));
			Assert.IsTrue(container.Kernel.HasComponent(typeof(IViewComponentFactory)));
			Assert.IsTrue(container.Kernel.HasComponent(typeof(IDynamicActionProviderFactory)));
		}

		[Test]
		public void FacilityRegisterCustomWizardPageFactory()
		{
			Assert.IsTrue(container.Kernel.HasComponent(typeof(IWizardPageFactory)));
		}

		[Test]
		public void FacilityDetectsControllerBeingRegistered()
		{
			IControllerTree tree = container.Resolve<IControllerTree>();

			container.Register(
				Component.For<HomeController>().
					Named("home.controller")
					);

			Type controllerType = tree.GetController("", "home");
			Assert.IsNotNull(controllerType);
			Assert.AreEqual(typeof(HomeController), controllerType);

			container.Register(
				Component.For<DummyController>().
					Named("dummy.controller")
					);

			controllerType = tree.GetController("", "dummy");
			Assert.IsNotNull(controllerType);
			Assert.AreEqual(typeof(DummyController), controllerType);
		}

		[Test]
		public void ControllersAreMadeTransient()
		{
			container.Register(
				Component.For<HomeController>().
					Named("home.controller")
					);

			IHandler handler = container.Kernel.GetHandler("home.controller");
			Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void FacilityDetectsViewComponentsRegistered()
		{
			IViewComponentRegistry registry = container.Resolve<IViewComponentRegistry>();

			container.Register(
				Component.For<DummyComponent>().
					Named("my.component")
					);

			Type componentType = registry.GetViewComponent("my.component");
			Assert.IsNotNull(componentType);
			Assert.AreEqual(typeof(DummyComponent), componentType);
		}

		[Test]
		public void ViewComponentsAreMadeTransient()
		{
			container.Register(
				Component.For<DummyComponent>().
					Named("my.viewcomponent")
					);

			IHandler handler = container.Kernel.GetHandler("my.viewcomponent");
			Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType);
		}

		#region Controllers and ViewComponents

		class DummyComponent : ViewComponent
		{
		}

		class HomeController : Controller
		{
			
		}

		class DummyController : IController
		{
			public event ControllerHandler BeforeAction
			{
				add { throw new NotImplementedException(); }
				remove { throw new NotImplementedException(); }
			}

			public event ControllerHandler AfterAction
			{
				add { throw new NotImplementedException(); }
				remove { throw new NotImplementedException(); }
			}

			public void Process(IEngineContext engineContext, IControllerContext context)
			{
				throw new System.NotImplementedException();
			}

			public void PreSendView(object view)
			{
				throw new System.NotImplementedException();
			}

			public void PostSendView(object view)
			{
				throw new System.NotImplementedException();
			}

			public IAsyncResult BeginProcess(IEngineContext engineContext, IControllerContext context)
			{
				throw new NotImplementedException();
			}

			public void EndProcess()
			{
				throw new NotImplementedException();
			}

			public void Dispose()
			{
				throw new System.NotImplementedException();
			}
		}

		#endregion
	}
}
