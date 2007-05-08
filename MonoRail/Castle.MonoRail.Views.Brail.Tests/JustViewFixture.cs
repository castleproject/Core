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

namespace Castle.MonoRail.Views.Brail.Tests
{
	using System;
	using System.Configuration;
	using System.IO;
	using Framework;
	using NUnit.Framework;
	using TestSupport;

	[TestFixture]
	public class JustViewFixture : BaseControllerTest
	{
		[Test]
		public void CanRenderViewWithoutUsingFullMonoRailPipeline()
		{
			BooViewEngine bve = new BooViewEngine();
			string viewPath = Path.Combine(ConfigurationManager.AppSettings["tests.src"], "Views");
			bve.Service(new ViewSourceLoaderServiceProvider(viewPath));
			bve.Initialize();
			StringWriter sw = new StringWriter();
			DummyController controller = new DummyController();
			PrepareController(controller, "", "home", "index");
			bve.Process(sw, Context, controller, "home/index");
			Assert.AreEqual("Brail is wonderful", sw.ToString());
		}

		[Test]
		public void WithParameters()
		{
			BooViewEngine bve = new BooViewEngine();
			string viewPath = Path.Combine(ConfigurationManager.AppSettings["tests.src"], "Views");
			bve.Service(new ViewSourceLoaderServiceProvider(viewPath));
			bve.Initialize();
			StringWriter sw = new StringWriter();
			DummyController controller = new DummyController();
			controller.PropertyBag["list"] = new int[] {2, 5, 7, 8};
			controller.PropertyBag["name"] = "test";
			PrepareController(controller, "", "home", "index");
			bve.Process(sw, Context, controller, "home/bag");
			string expected = @"test is the name
 2
 5
 7
 8
";
			Assert.AreEqual(expected, sw.ToString());
		}

		private class DummyController : Controller
		{
		}
	}

	internal class ViewSourceLoaderServiceProvider : IServiceProvider
	{
		private string viewRootDir;

		public ViewSourceLoaderServiceProvider(string viewRootDir)
		{
			this.viewRootDir = viewRootDir;
		}

		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			if (typeof(IViewSourceLoader) == serviceType)
			{
				FileAssemblyViewSourceLoader loader = new FileAssemblyViewSourceLoader();
				loader.ViewRootDir = viewRootDir;
				return loader;
			}
			return null;
		}

		#endregion
	}
}