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

namespace Castle.MonoRail.Framework.Test
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.IO;

	/// <summary>
	/// Used to hook a viewcomponent call to render a nested section
	/// </summary>
	/// <param name="context">The content available to the section</param>
	/// <param name="writer">The writer</param>
	public delegate void TestSectionRender(IDictionary context, TextWriter writer);

	/// <summary>
	/// Used to hook a viewcomponent call to render a view template
	/// </summary>
	/// <param name="name">view name</param>
	/// <param name="context">The content available to the view</param>
	/// <param name="writer">The writer</param>
	public delegate void TestViewRender(string name, IDictionary context, TextWriter writer);

	/// <summary>
	/// Represents a mock implementation of <see cref="IMockViewComponentContext"/> for unit test purposes.
	/// </summary>
	public class StubViewComponentContext : IMockViewComponentContext
	{
		private string viewToRender;
		private string componentName;
		private TextWriter writer;
		private IDictionary contextVars = new HybridDictionary(true);
		private IDictionary componentParameters = new HybridDictionary(true);
		private IViewEngine viewEngine;
		private IDictionary<string, TestSectionRender> section2delegate;

		/// <summary>
		/// Event that is raised when a section is rendered by the viewcomponent.
		/// </summary>
		public TestSectionRender OnBodyRender;

		/// <summary>
		/// Event that is raised when a view is rendered by the viewcomponent.
		/// </summary>
		public TestViewRender OnViewRender;

		/// <summary>
		/// Initializes a new instance of the <see cref="StubViewComponentContext"/> class.
		/// </summary>
		protected StubViewComponentContext()
		{
			section2delegate = new Dictionary<string, TestSectionRender>(StringComparer.InvariantCultureIgnoreCase);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StubViewComponentContext"/> class.
		/// </summary>
		/// <param name="componentName">Name of the component.</param>
		/// <param name="writer">The writer.</param>
		/// <param name="viewEngine">The view engine.</param>
		public StubViewComponentContext(string componentName, TextWriter writer, IViewEngine viewEngine) : this()
		{
			this.writer = writer;
			this.componentName = componentName;
			this.viewEngine = viewEngine;
		}

		/// <summary>
		/// Gets or sets the section render dictionary.
		/// </summary>
		/// <value>The section render.</value>
		public IDictionary<string, TestSectionRender> SectionRender
		{
			get { return section2delegate; }
			set { section2delegate = value; }
		}

		#region IViewComponentContext

		/// <summary>
		/// Gets the name of the component.
		/// </summary>
		/// <value>The name of the component.</value>
		public virtual string ComponentName
		{
			get { return componentName; }
		}

		/// <summary>
		/// Determines whether the current component declaration on the view
		/// has the specified section.
		/// </summary>
		/// <param name="sectionName">Name of the section.</param>
		/// <returns>
		/// 	<c>true</c> if the specified section exists; otherwise, <c>false</c>.
		/// </returns>
		public bool HasSection(string sectionName)
		{
			return section2delegate.ContainsKey(sectionName);
		}

		/// <summary>
		/// Renders the view specified to the writer.
		/// </summary>
		/// <param name="name">The view template name</param>
		/// <param name="writer">A writer to output</param>
		public void RenderView(string name, TextWriter writer)
		{
			if (OnViewRender != null)
			{
				OnViewRender(name, contextVars, writer);
			}
		}

		/// <summary>
		/// Renders the component body.
		/// </summary>
		public void RenderBody()
		{
			RenderBody(writer);
		}

		/// <summary>
		/// Renders the body into the specified <see cref="TextWriter"/>
		/// </summary>
		/// <param name="writer">The writer.</param>
		public void RenderBody(TextWriter writer)
		{
			if (OnBodyRender != null)
			{
				OnBodyRender(contextVars, writer);
			}
		}

		/// <summary>
		/// Renders the the specified section.
		/// No exception will the throw if the section cannot be found.
		/// </summary>
		/// <param name="sectionName">Name of the section.</param>
		public void RenderSection(string sectionName)
		{
			RenderSection(sectionName, writer);
		}

		/// <summary>
		/// Renders the the specified section.
		/// No exception will the throw if the section cannot be found.
		/// </summary>
		/// <param name="sectionName">Name of the section.</param>
		/// <param name="writer">The writer to output the section content.</param>
		public void RenderSection(string sectionName, TextWriter writer)
		{
			if (section2delegate.ContainsKey(sectionName))
			{
				section2delegate[sectionName](contextVars, writer);
			}
		}

		/// <summary>
		/// Gets the writer used to render the view component
		/// </summary>
		/// <value>The writer.</value>
		public virtual TextWriter Writer
		{
			get { return writer; }
		}

		/// <summary>
		/// Gets the dictionary that holds variables for the
		/// view and for the view component
		/// </summary>
		/// <value>The context vars.</value>
		public virtual IDictionary ContextVars
		{
			get { return contextVars; }
		}

		/// <summary>
		/// Gets the component parameters that the view has passed
		/// to the component
		/// </summary>
		/// <value>The component parameters.</value>
		public virtual IDictionary ComponentParameters
		{
			get { return componentParameters; }
		}

		/// <summary>
		/// Gets or sets the view to render.
		/// </summary>
		/// <value>The view to render.</value>
		public string ViewToRender
		{
			get { return viewToRender; }
			set { viewToRender = value; }
		}

		/// <summary>
		/// Gets the view engine instance.
		/// </summary>
		/// <value>The view engine.</value>
		public virtual IViewEngine ViewEngine
		{
			get { return viewEngine; }
		}

		#endregion
	}
}