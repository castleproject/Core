// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

	public class NVelocityViewContextAdapter : IViewComponentContext
	{
		private readonly String componentName;
		private readonly INode parentNode;

		private InternalContextAdapter context;
		private INode bodyNode;
		private TextWriter writer;
		private IDictionary componentParams;
		private String viewToRender;
		private IDictionary sections;

		public NVelocityViewContextAdapter(String componentName, INode parentNode)
		{
			this.componentName = componentName;
			this.parentNode = parentNode;
		}

		#region IViewComponentContext

		public String ComponentName
		{
			get { return componentName; }
		}

		public IDictionary ContextVars
		{
			get { return context as IDictionary; }
		}

		public IDictionary ComponentParameters
		{
			get { return componentParams; }
		}

		public String ViewToRender
		{
			get { return viewToRender; }
			set { viewToRender = value; }
		}

		public TextWriter Writer
		{
			get { return writer; }
		}

		public bool HasSection(String sectionName)
		{
			return sections != null && sections.Contains(sectionName);
		}

		public void RenderBody()
		{
			RenderBody(writer);
		}

		public void RenderSection(String sectionName)
		{
			if (HasSection(sectionName))
			{
				Directive directive = (Directive) sections[sectionName];

				directive.Render(context, writer, parentNode);
			}

			// Shall we throw exception?
		}

		public void RenderBody(TextWriter writer)
		{
			if (bodyNode == null)
			{
				throw new RailsException("This component does not have a body content to be rendered");
			}

			bodyNode.Render(context, writer);
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
