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

namespace Castle.MonoRail.Framework.Tests.Filters
{
	using NUnit.Framework;
	using Rhino.Mocks;
	using Test;

	[TestFixture]
	public class ControllerLevelFiltersTestCase
	{
		private MockRepository mockRepository = new MockRepository();
		private MockEngineContext engineContext;
		private ViewEngineManagerStub viewEngStub;
		private MockServices services;
		private IFilterFactory filterFactoryMock;

		[SetUp]
		public void Init()
		{
			MockRequest request = new MockRequest();
			MockResponse response = new MockResponse();
			services = new MockServices();
			viewEngStub = new ViewEngineManagerStub();
			services.ViewEngineManager = viewEngStub;
			filterFactoryMock = mockRepository.DynamicMock<IFilterFactory>();
			services.FilterFactory = filterFactoryMock;
			engineContext = new MockEngineContext(request, response, services, null);
		}

		[Test]
		public void Filter_BeforeActionReturningFalsePreventsActionProcessment()
		{
			ControllerWithSingleBeforeActionFilter controller = new ControllerWithSingleBeforeActionFilter();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "index",
				       services.ControllerDescriptorProvider.BuildDescriptor(controller));

			IFilter filterMock = mockRepository.DynamicMock<IFilter>();

			using(mockRepository.Record())
			{
				Expect.Call(filterFactoryMock.Create(typeof(DummyFilter))).Return(filterMock);

				Expect.Call(filterMock.Perform(ExecuteWhen.BeforeAction, engineContext, controller, context)).Return(false);

				filterFactoryMock.Release(filterMock);
				LastCall.Repeat.Once();
			}

			using(mockRepository.Playback())
			{
				controller.Process(engineContext, context);
				controller.Dispose();

				Assert.IsNull(viewEngStub.TemplateRendered);
				Assert.IsFalse(controller.indexActionExecuted);
			}
		}

		[Test]
		public void Filter_BeforeActionReturningTrueAllowsProcessToContinue()
		{
			ControllerWithSingleBeforeActionFilter controller = new ControllerWithSingleBeforeActionFilter();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "index",
				       services.ControllerDescriptorProvider.BuildDescriptor(controller));

			IFilter filterMock = mockRepository.DynamicMock<IFilter>();

			using(mockRepository.Record())
			{
				Expect.Call(filterFactoryMock.Create(typeof(DummyFilter))).Return(filterMock);

				Expect.Call(filterMock.Perform(ExecuteWhen.BeforeAction, engineContext, controller, context)).Return(true);

				filterFactoryMock.Release(filterMock);
				LastCall.Repeat.Once();
			}

			using(mockRepository.Playback())
			{
				controller.Process(engineContext, context);
				controller.Dispose();

				Assert.AreEqual("home\\index", viewEngStub.TemplateRendered);
				Assert.IsTrue(controller.indexActionExecuted);
			}
		}

		[Test]
		public void Filter_AfterActionIsRun()
		{
			ControllerWithAfterActionFilter controller = new ControllerWithAfterActionFilter();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "index",
				       services.ControllerDescriptorProvider.BuildDescriptor(controller));

			IFilter filterMock = mockRepository.DynamicMock<IFilter>();

			using(mockRepository.Record())
			{
				Expect.Call(filterFactoryMock.Create(typeof(DummyFilter))).Return(filterMock);

				Expect.Call(filterMock.Perform(ExecuteWhen.AfterAction, engineContext, controller, context)).Return(true);

				filterFactoryMock.Release(filterMock);
				LastCall.Repeat.Once();
			}

			using(mockRepository.Playback())
			{
				controller.Process(engineContext, context);
				controller.Dispose();

				Assert.AreEqual("home\\index", viewEngStub.TemplateRendered);
				Assert.IsTrue(controller.indexActionExecuted);
			}
		}

		[Test]
		public void Filter_AfterActionReturningFalsePreventsRendering()
		{
			ControllerWithAfterActionFilter controller = new ControllerWithAfterActionFilter();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "index",
				       services.ControllerDescriptorProvider.BuildDescriptor(controller));

			IFilter filterMock = mockRepository.DynamicMock<IFilter>();

			using(mockRepository.Record())
			{
				Expect.Call(filterFactoryMock.Create(typeof(DummyFilter))).Return(filterMock);

				Expect.Call(filterMock.Perform(ExecuteWhen.AfterAction, engineContext, controller, context)).Return(false);

				filterFactoryMock.Release(filterMock);
				LastCall.Repeat.Once();
			}

			using(mockRepository.Playback())
			{
				controller.Process(engineContext, context);
				controller.Dispose();

				Assert.IsNull(viewEngStub.TemplateRendered);
				Assert.IsTrue(controller.indexActionExecuted);
			}
		}

		[Test]
		public void Filter_AfterRenderingIsRun()
		{
			ControllerWithAfterRenderingFilter controller = new ControllerWithAfterRenderingFilter();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "index",
				       services.ControllerDescriptorProvider.BuildDescriptor(controller));

			IFilter filterMock = mockRepository.DynamicMock<IFilter>();

			using(mockRepository.Record())
			{
				Expect.Call(filterFactoryMock.Create(typeof(DummyFilter))).Return(filterMock);

				Expect.Call(filterMock.Perform(ExecuteWhen.AfterRendering, engineContext, controller, context)).Return(true);

				filterFactoryMock.Release(filterMock);
				LastCall.Repeat.Once();
			}

			using(mockRepository.Playback())
			{
				controller.Process(engineContext, context);
				controller.Dispose();

				Assert.AreEqual("home\\index", viewEngStub.TemplateRendered);
				Assert.IsTrue(controller.indexActionExecuted);
			}
		}

		[Test]
		public void Filter_SkipFilterAttributeSkipsTheFilter()
		{
			ControllerWithSkipFilter controller = new ControllerWithSkipFilter();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "index",
				       services.ControllerDescriptorProvider.BuildDescriptor(controller));

			IFilter filterMock = mockRepository.DynamicMock<IFilter>();

			using(mockRepository.Record())
			{
			}

			using(mockRepository.Playback())
			{
				controller.Process(engineContext, context);
				controller.Dispose();

				Assert.AreEqual("home\\index", viewEngStub.TemplateRendered);
				Assert.IsTrue(controller.indexActionExecuted);
			}
		}

		#region Controllers

		[Filter(ExecuteWhen.BeforeAction, typeof(DummyFilter))]
		private class ControllerWithSingleBeforeActionFilter : Controller
		{
			public bool indexActionExecuted;

			public void Index()
			{
				indexActionExecuted = true;
			}
		}

		[Filter(ExecuteWhen.BeforeAction, typeof(DummyFilter))]
		private class ControllerWithSkipFilter : Controller
		{
			public bool indexActionExecuted;

			[SkipFilter]
			public void Index()
			{
				indexActionExecuted = true;
			}
		}

		[Filter(ExecuteWhen.AfterAction, typeof(DummyFilter))]
		private class ControllerWithAfterActionFilter : Controller
		{
			public bool indexActionExecuted;

			public void Index()
			{
				indexActionExecuted = true;
			}
		}

		[Filter(ExecuteWhen.AfterRendering, typeof(DummyFilter))]
		private class ControllerWithAfterRenderingFilter : Controller
		{
			public bool indexActionExecuted;

			public void Index()
			{
				indexActionExecuted = true;
			}
		}

		#endregion

		private class DummyFilter : IFilter
		{
			public bool Perform(ExecuteWhen exec, IEngineContext context, IController controller,
			                    IControllerContext controllerContext)
			{
				return false;
			}
		}
	}
}