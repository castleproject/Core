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

namespace Castle.MonoRail.Framework.Tests.Actions
{
	using System.Reflection;
	using NUnit.Framework;
	using Castle.MonoRail.Framework.Descriptors;
	using Castle.MonoRail.Framework.Test;

	[TestFixture]
	public class ActionMethodExecutorTestCase
	{
		[Test]
		public void ShouldReflectAbsenceOfConfigurationOnMetaDescriptor()
		{
			BaseController controller = new BaseController();
			ActionMetaDescriptor actionMeta = new ActionMetaDescriptor();

			ActionMethodExecutor executor = new ActionMethodExecutor(GetActionMethod(controller), actionMeta);

			Assert.IsFalse(executor.ShouldSkipAllFilters);
			Assert.IsFalse(executor.ShouldSkipRescues);
			Assert.IsFalse(executor.ShouldSkipFilter(typeof(DummyFilter)));
			Assert.IsNull(executor.LayoutOverride);
			Assert.AreEqual(0, executor.Resources.Length);
		}

		[Test]
		public void ShouldReturnTrueToSkipRescueReflectingMeta()
		{
			BaseController controller = new BaseController();
			ActionMetaDescriptor actionMeta = new ActionMetaDescriptor();
			actionMeta.SkipRescue = new SkipRescueAttribute();

			ActionMethodExecutor executor = new ActionMethodExecutor(GetActionMethod(controller), actionMeta);

			Assert.IsTrue(executor.ShouldSkipRescues);
			Assert.IsFalse(executor.ShouldSkipAllFilters);
			Assert.IsFalse(executor.ShouldSkipFilter(typeof(DummyFilter)));
			Assert.IsNull(executor.LayoutOverride);
			Assert.AreEqual(0, executor.Resources.Length);
		}

		[Test]
		public void ShouldReturnTrueToSkipAllFiltersReflectingMeta()
		{
			BaseController controller = new BaseController();
			ActionMetaDescriptor actionMeta = new ActionMetaDescriptor();
			actionMeta.SkipFilters.Add(new SkipFilterAttribute());

			ActionMethodExecutor executor = new ActionMethodExecutor(GetActionMethod(controller), actionMeta);

			Assert.IsTrue(executor.ShouldSkipAllFilters);
			Assert.IsFalse(executor.ShouldSkipRescues);
			Assert.IsFalse(executor.ShouldSkipFilter(typeof(DummyFilter)));
			Assert.IsNull(executor.LayoutOverride);
			Assert.AreEqual(0, executor.Resources.Length);
		}

		[Test]
		public void ShouldReturnTrueToSkipSpecifiedFiltersReflectingMeta()
		{
			BaseController controller = new BaseController();
			ActionMetaDescriptor actionMeta = new ActionMetaDescriptor();
			actionMeta.SkipFilters.Add(new SkipFilterAttribute(typeof(DummyFilter)));

			ActionMethodExecutor executor = new ActionMethodExecutor(GetActionMethod(controller), actionMeta);

			Assert.IsTrue(executor.ShouldSkipFilter(typeof(DummyFilter)));
			Assert.IsFalse(executor.ShouldSkipRescues);
			Assert.IsFalse(executor.ShouldSkipAllFilters);
			Assert.IsNull(executor.LayoutOverride);
			Assert.AreEqual(0, executor.Resources.Length);
		}

		[Test]
		public void ShouldReturnLayoutReflectingMeta()
		{
			BaseController controller = new BaseController();
			ActionMetaDescriptor actionMeta = new ActionMetaDescriptor();
			actionMeta.Layout = new LayoutDescriptor("layoutname");

			ActionMethodExecutor executor = new ActionMethodExecutor(GetActionMethod(controller), actionMeta);

			Assert.IsFalse(executor.ShouldSkipFilter(typeof(DummyFilter)));
			Assert.IsFalse(executor.ShouldSkipRescues);
			Assert.IsFalse(executor.ShouldSkipAllFilters);
			Assert.AreEqual("layoutname", executor.LayoutOverride[0]);
			Assert.AreEqual(0, executor.Resources.Length);
		}

		[Test]
		public void ShouldReturnResourcesReflectingMeta()
		{
			BaseController controller = new BaseController();
			ActionMetaDescriptor actionMeta = new ActionMetaDescriptor();
			actionMeta.Resources = new ResourceDescriptor[] { new ResourceDescriptor(typeof(BaseController), "name", "resname", "cult", "assm") };

			ActionMethodExecutor executor = new ActionMethodExecutor(GetActionMethod(controller), actionMeta);

			Assert.IsFalse(executor.ShouldSkipFilter(typeof(DummyFilter)));
			Assert.IsFalse(executor.ShouldSkipRescues);
			Assert.IsFalse(executor.ShouldSkipAllFilters);
			Assert.IsNull(executor.LayoutOverride);
			Assert.AreEqual(1, executor.Resources.Length);
		}

		[Test]
		public void ExecutesActionAndReturnValue()
		{
			BaseController controller = new BaseController();
			ActionMetaDescriptor actionMeta = new ActionMetaDescriptor();

			ActionMethodExecutor executor = new ActionMethodExecutor(GetActionMethod(controller), actionMeta);

			StubRequest req = new StubRequest();
			StubResponse res = new StubResponse();
			StubMonoRailServices services = new StubMonoRailServices();
			IEngineContext engineContext = new StubEngineContext(req, res, services, new UrlInfo("area", "controller", "action"));
			object retVal = executor.Execute(engineContext, controller, new ControllerContext());

			Assert.IsTrue(controller.WasExecuted);
			Assert.AreEqual(1, retVal);
		}

		private MethodInfo GetActionMethod(object controller)
		{
			return controller.GetType().GetMethod("Action1");
		}

		public class BaseController : Controller
		{
			private bool wasExecuted;

			public object Action1()
			{
				wasExecuted = true;
				return 1;
			}

			public bool WasExecuted
			{
				get { return wasExecuted; }
			}
		}

		public class DummyFilter : Filter
		{
		}
	}
}
