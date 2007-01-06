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

using Directive = NVelocity.Runtime.Directive.Directive;
using IInternalContextAdapter = NVelocity.Context.IInternalContextAdapter;
using INode = NVelocity.Runtime.Parser.Node.INode;
using InternalContextAdapter = NVelocity.Context.IInternalContextAdapter;

namespace Castle.MonoRail.Framework.Views.NVelocity
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.IO;

	using Castle.MonoRail.Framework.Views.NVelocity.CustomDirectives;

	/// <summary>
	/// <see cref="IViewComponentContext"/>'s implementation for
	/// NVelocity
	/// </summary>
	public class NVelocityViewContextAdapter : IViewComponentContext
	{
		private readonly String componentName;
		private readonly INode parentNode;
		private readonly IViewEngine viewEngine;
		private readonly IViewRenderer renderer;

		private String viewToRender;
		private TextWriter writer;
		private INode bodyNode;
		private IDictionary sections;
		private IDictionary componentParams;
		private InternalContextAdapter context;

		/// <summary>
		/// Initializes a new instance of the <see cref="NVelocityViewContextAdapter"/> class.
		/// </summary>
		/// <param name="componentName">Name of the component.</param>
		/// <param name="parentNode">The parent node.</param>
		/// <param name="viewEngine">The view engine.</param>
		/// <param name="renderer">The view renderer.</param>
		public NVelocityViewContextAdapter(String componentName, INode parentNode, IViewEngine viewEngine, IViewRenderer renderer)
		{
			this.componentName = componentName;
			this.parentNode = parentNode;
			this.viewEngine = viewEngine;
			this.renderer = renderer;
		}

		#region IViewComponentContext

		/// <summary>
		/// Gets the name of the component.
		/// </summary>
		/// <value>The name of the component.</value>
		public String ComponentName
		{
			get { return componentName; }
		}

		/// <summary>
		/// Gets the dictionary that holds variables for the
		/// view and for the view component
		/// </summary>
		/// <value>The context vars.</value>
		public IDictionary ContextVars
		{
			get { return context; }
		}

		/// <summary>
		/// Gets the component parameters that the view has passed
		/// to the component
		/// </summary>
		/// <value>The component parameters.</value>
		public IDictionary ComponentParameters
		{
			get { return componentParams; }
		}

		/// <summary>
		/// Gets or sets the view to render.
		/// </summary>
		/// <value>The view to render.</value>
		public String ViewToRender
		{
			get { return viewToRender; }
			set { viewToRender = value; }
		}

		/// <summary>
		/// Gets the writer used to render the component
		/// </summary>
		/// <value>The writer.</value>
		public TextWriter Writer
		{
			get { return writer; }
		}

		/// <summary>
		/// Determines whether the current component declaration on the view
		/// has the specified section.
		/// </summary>
		/// <param name="sectionName">Name of the section.</param>
		/// <returns>
		/// 	<c>true</c> if the specified section exists; otherwise, <c>false</c>.
		/// </returns>
		public bool HasSection(String sectionName)
		{
			return sections != null && sections.Contains(sectionName);
		}

		/// <summary>
		/// Renders the component body.
		/// </summary>
		public void RenderBody()
		{
			RenderBody(writer);
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="name"></param>
		/// <param name="writer"></param>
		public void RenderView(string name, TextWriter writer)
		{
			renderer.RenderComponentView(context, name, writer, this);
		}

		/// <summary>
		/// Renders the the specified section
		/// </summary>
		/// <param name="sectionName">Name of the section.</param>
		public void RenderSection(String sectionName)
		{
			if (HasSection(sectionName))
			{
				Directive directive = (Directive) sections[sectionName];

				directive.Render(context, writer, parentNode);
			}
		}

		/// <summary>
		/// Renders the the specified section
		/// </summary>
		/// <param name="sectionName">Name of the section.</param>
		/// <param name="writer">The writer.</param>
		public void RenderSection(string sectionName, TextWriter writer)
		{
			if (HasSection(sectionName))
			{
				Directive directive = (Directive)sections[sectionName];

				directive.Render(context, writer, parentNode);
			}
		}

		/// <summary>
		/// Renders the body into the specified <see cref="TextWriter"/>
		/// </summary>
		/// <param name="writer">The writer.</param>
		public void RenderBody(TextWriter writer)
		{
			if (bodyNode == null)
			{
				throw new RailsException("This component does not have a body content to be rendered");
			}

			bodyNode.Render(context, writer);
		}

		/// <summary>
		/// Gets the view engine instance.
		/// </summary>
		/// <value>The view engine.</value>
		public IViewEngine ViewEngine
		{
			get { return viewEngine; }
		}

		#endregion

		internal IInternalContextAdapter Context
		{
			get { return context; }
			set { context = value; }
		}

		internal INode BodyNode
		{
			get { return bodyNode; }
			set { bodyNode = value; }
		}

		internal IDictionary ComponentParams
		{
			get { return componentParams; }
			set { componentParams = value; }
		}

		internal TextWriter TextWriter
		{
			set { writer = value; }
		}

		internal void RegisterSection(SubSectionDirective section)
		{
			if (sections == null)
			{
				sections = new HybridDictionary(true);
			}

			sections[section.Name] = section;
		}
	}
}
