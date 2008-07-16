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

namespace Castle.MonoRail.Views.AspView
{
	using System.Collections;
	using Framework;
	using System.IO;

	public class ViewComponentContext : IViewComponentContext
	{
		readonly string componentName;

		readonly IDictionary componentParameters;
		IDictionary sections;
		string viewToRender;

		ViewComponentSectionRendereDelegate body;
		readonly private IViewBaseInternal callingView;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewComponentContext"/> class.
		/// </summary>
		/// <param name="callingView">The calling view.</param>
		/// <param name="body">The body.</param>
		/// <param name="name">The name.</param>
		/// <param name="parameters">The parameters.</param>
		public ViewComponentContext(IViewBaseInternal callingView, ViewComponentSectionRendereDelegate body,
										 string name, IDictionary parameters)
		{
			this.callingView = callingView;
			this.body = body;
			componentName = name;
			componentParameters = parameters;
		}

		public ViewComponentSectionRendereDelegate Body
		{
			get { return body; }
			set { body = value; }
		}

		public void RegisterSection(string name, ViewComponentSectionRendereDelegate section)
		{
			if (sections == null)
				sections = new Hashtable();
			sections[name] = section;
		}

		#region IViewComponentContext Members

		public string ComponentName
		{
			get { return componentName; }
		}

		public IDictionary ComponentParameters
		{
			get { return componentParameters; }
		}

		public IDictionary ContextVars
		{
			get { return callingView.Properties; }
		}

		public bool HasSection(string sectionName)
		{
			return sections != null && sections.Contains(sectionName);
		}

		public void RenderBody()
		{
			AssertHasBody();

			body.Invoke();
		}

		public void RenderBody(TextWriter writer)
		{
			using (callingView.SetDisposeableOutputWriter(writer))
			{
				RenderBody();
			}
		}

		public void RenderSection(string sectionName, TextWriter writer)
		{
			using (callingView.SetDisposeableOutputWriter(writer))
			{
				RenderSection(sectionName);
			}
		}

		public void RenderSection(string sectionName)
		{
			if (!HasSection(sectionName))
				return;//matching the Brail and NVelocity behavior, but maybe should throw?
			ViewComponentSectionRendereDelegate section = (ViewComponentSectionRendereDelegate)sections[sectionName];
			section.Invoke();
		}

		public void RenderView(string name, TextWriter writer)
		{
			using (callingView.SetDisposeableOutputWriter(writer))
			{
				callingView.OutputSubView(name);
			}
		}

		public IViewEngine ViewEngine
		{
			get { return callingView.ViewEngine; }
		}

		public string ViewToRender
		{
			get { return viewToRender; }
			set { viewToRender = value; }
		}

		public TextWriter Writer
		{
			get { return callingView.OutputWriter; }
		}

		#endregion

		private void AssertHasBody()
		{
			if (body == null)
			{
				throw new AspViewException("This component does not have a body content to be rendered");
			}
		}
	}
}
