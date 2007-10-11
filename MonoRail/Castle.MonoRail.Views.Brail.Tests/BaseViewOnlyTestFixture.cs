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
	using System.Collections;
	using System.Configuration;
	using System.IO;
	using Framework;
	using TestSupport;

	public class BaseViewOnlyTestFixture : BaseControllerTest
	{
		protected string ProcessView(string templatePath)
		{
			return ProcessView(new Hashtable(), templatePath);
		}

		protected string ProcessView(IDictionary dictionary, string templatePath)
		{
			BooViewEngine bve = new BooViewEngine();
			bve.Options = new BooViewEngineOptions();
			bve.Options.SaveDirectory = Environment.CurrentDirectory;
			bve.Options.SaveToDisk = true;
			bve.Options.Debug = true;
			string viewPath = Path.Combine(ConfigurationManager.AppSettings["tests.src"], "Views");
			bve.Service(new ViewSourceLoaderServiceProvider(viewPath));
			bve.Initialize();
			StringWriter sw = new StringWriter();
			DummyController controller = new DummyController();

			controller.PropertyBag = dictionary;
			PrepareController(controller, "", "home", "index");
			bve.Process((TextWriter) sw, Context, (Controller) controller, templatePath);
			return sw.ToString();
		}

		#region Nested type: DummyController

		private class DummyController : Controller
		{
		}

		#endregion
	}
}