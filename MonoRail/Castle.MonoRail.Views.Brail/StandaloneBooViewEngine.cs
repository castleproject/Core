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

namespace Castle.MonoRail.Views.Brail
{
	using System.IO;
	using Framework;
	using System.Collections;

	public class StandaloneBooViewEngine
	{
		private readonly BooViewEngine bve = new BooViewEngine();

		public StandaloneBooViewEngine(IViewSourceLoader viewSourceLoader, BooViewEngineOptions options)
		{
			bve.Options = options;
			bve.SetViewSourceLoader(viewSourceLoader);
			bve.Initialize();
		}

		public BooViewEngineOptions Options
		{
			get { return bve.Options; }
		}

		public void Process(string templateName, TextWriter output, IDictionary parameters)
		{
			DummyController controller = new DummyController();
			ControllerContext context = new ControllerContext();
			context.PropertyBag = parameters;
			bve.Process(templateName, output, null, controller, context);
		}

		#region Nested type: DummyController

		private class DummyController : Controller
		{
		}

		#endregion
	}
}