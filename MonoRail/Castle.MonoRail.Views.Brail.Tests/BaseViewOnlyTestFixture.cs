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
	using System.Reflection;
	using Framework;
	using TestSupport;

	public class BaseViewOnlyTestFixture : BaseControllerTest
	{
		protected static string ProcessView(string templatePath)
		{
			return ProcessView(new Hashtable(), templatePath);
		}

		protected static string ProcessView(IDictionary dictionary, string templatePath)
		{
			BooViewEngineOptions options = new BooViewEngineOptions();
			options.SaveDirectory = Environment.CurrentDirectory;
			options.SaveToDisk = true;
			options.Debug = true;
			options.BatchCompile = true;
			string viewPath = Path.Combine(ConfigurationManager.AppSettings["tests.src"], "Views");

			FileAssemblyViewSourceLoader loader = new FileAssemblyViewSourceLoader(viewPath);
			loader.AddAssemblySource(new AssemblySourceInfo(Assembly.GetExecutingAssembly().FullName,"Castle.MonoRail.Views.Brail.Tests.ResourcedViews"));
			IndependentBooViewEngine engine = new IndependentBooViewEngine(
				loader,
				options);

			StringWriter sw = new StringWriter();

			engine.Process(templatePath, sw, dictionary);

			return sw.ToString();
		}
	}
}