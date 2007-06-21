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

namespace Castle.MonoRail.Framework.Test
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Collections.Generic;
	using System.IO;

	public delegate void TestSectionRender(IDictionary context, TextWriter writer);
	public delegate void TestViewRender(string name, IDictionary context, TextWriter writer);

	public class MockViewComponentContext : IMockViewComponentContext
	{
		private string viewToRender;
		private string componentName;
		private TextWriter writer;
		private IDictionary contextVars = new HybridDictionary(true);
		private IDictionary componentParameters = new HybridDictionary(true);
		private IViewEngine viewEngine;
		private IDictionary<string,  TestSectionRender> section2delegate;

		public TestSectionRender OnBodyRender;
		public TestViewRender OnViewRender;

		protected MockViewComponentContext()
		{
			section2delegate = new Dictionary<string, TestSectionRender>(StringComparer.InvariantCultureIgnoreCase);
		}

		public MockViewComponentContext(string componentName, TextWriter writer, IViewEngine viewEngine) : this()
		{
			this.writer = writer;
			this.componentName = componentName;
			this.viewEngine = viewEngine;
		}

		public IDictionary<string, TestSectionRender> SectionRender
		{
			get { return section2delegate; }
			set { section2delegate = value; }
		}

		#region IViewComponentContext

		public string ComponentName
		{
			get { return componentName; }
		}

		public bool HasSection(string sectionName)
		{
			return section2delegate.ContainsKey(sectionName);
		}

		public void RenderView(string name, TextWriter writer)
		{
			if (OnViewRender != null)
			{
				OnViewRender(name, contextVars, writer);
			}
		}

		public void RenderBody()
		{
			RenderBody(writer);
		}

		public void RenderBody(TextWriter writer)
		{
			if (OnBodyRender != null)
			{
				OnBodyRender(contextVars, writer);
			}
		}

		public void RenderSection(string sectionName)
		{
			RenderSection(sectionName, writer);
		}

		public void RenderSection(string sectionName, TextWriter writer)
		{
			if (section2delegate.ContainsKey(sectionName))
			{
				section2delegate[sectionName](contextVars, writer);
			}
		}

		public TextWriter Writer
		{
			get { return writer; }
		}

		public IDictionary ContextVars
		{
			get { return contextVars; }
		}

		public IDictionary ComponentParameters
		{
			get { return componentParameters; }
		}

		public string ViewToRender
		{
			get { return viewToRender; }
			set { viewToRender = value; }
		}

		public IViewEngine ViewEngine
		{
			get { return viewEngine; }
		}

		#endregion
	}
}
