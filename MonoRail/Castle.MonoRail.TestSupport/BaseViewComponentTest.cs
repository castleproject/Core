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

namespace Castle.MonoRail.TestSupport
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Test;

	public abstract class BaseViewComponentTest : BaseControllerTest
	{
		private IMockViewComponentContext componentContext;
		private IViewEngine viewEngine;
		private StringWriter writer;

		public IDictionary<string, TestSectionRender> SectionRender;
		public TestSectionRender OnBodyRender; 
		public TestViewRender OnViewRender;

		protected BaseViewComponentTest()
		{
			CleanUp();
		}

		protected void PrepareViewComponent(ViewComponent component)
		{
			if (Context == null)
			{
				BuildRailsContext("", "Controller", "Action");
			}

			viewEngine = BuildViewEngine();

			componentContext = BuildViewComponentContext(component.GetType().Name);

			component.Init(Context, componentContext);
		}

		protected string Output
		{
			get { return writer.ToString(); }
		}

		protected void CleanUp()
		{
			writer = new StringWriter();
			SectionRender = new Dictionary<string, TestSectionRender>(StringComparer.InvariantCultureIgnoreCase);
			OnBodyRender = null;
			OnViewRender = null;
		}

		protected virtual IViewEngine BuildViewEngine()
		{
			return new MockViewEngine(".view", ".jsview", true, true);
		}

		protected virtual IMockViewComponentContext BuildViewComponentContext(string viewComponentName)
		{
			MockViewComponentContext compContext = new MockViewComponentContext(viewComponentName, writer, viewEngine);

			compContext.SectionRender = SectionRender;
			compContext.OnBodyRender = OnBodyRender;
			compContext.OnViewRender = OnViewRender;

			return compContext;
		}
	}
}
