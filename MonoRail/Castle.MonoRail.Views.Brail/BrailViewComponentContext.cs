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
	using System;
	using System.Collections;
	using System.IO;
	using Boo.Lang;
	using Framework;

	public class BrailViewComponentContext : IViewComponentContext
	{
		private readonly TextWriter default_writer;
		private ICallable body;
		private string componentName;

		private IDictionary componentParameters;
		private BrailBase parent;
		private IDictionary sections;
		private string viewToRender;

		/// <summary>
		/// Initializes a new instance of the <see cref="BrailViewComponentContext"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="body">The body.</param>
		/// <param name="name">The name.</param>
		/// <param name="text">The text.</param>
		/// <param name="parameters">The parameters.</param>
		public BrailViewComponentContext(BrailBase parent, ICallable body,
		                                 string name, TextWriter text, IDictionary parameters)
		{
			this.parent = parent;
			this.body = body;
			componentName = name;
			default_writer = text;
			componentParameters = IgnoreNull.ReplaceIgnoreNullsWithTargets(parameters);
		}

		public ICallable Body
		{
			get { return body; }
			set { body = value; }
		}

		#region IViewComponentContext Members

		public string ComponentName
		{
			get { return componentName; }
		}

		public IDictionary ContextVars
		{
			get { return parent.Properties; }
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

		public TextWriter Writer
		{
			get { return default_writer; }
		}

		public void RenderBody()
		{
			RenderBody(default_writer);
		}

		public void RenderBody(TextWriter writer)
		{
			if (body == null)
			{
				throw new MonoRailException("This component does not have a body content to be rendered");
			}
			using(parent.SetOutputStream(writer))
			{
				body.Call(new object[] {writer});
			}
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="name"></param>
		/// <param name="writer"></param>
		public void RenderView(string name, TextWriter writer)
		{
			parent.OutputSubView(name, writer, ContextVars);
		}

		public bool HasSection(string sectionName)
		{
			return sections != null && sections.Contains(sectionName);
		}

		public void RenderSection(string sectionName)
		{
			RenderSection(sectionName, default_writer);
		}

		/// <summary>
		/// Renders the the specified section
		/// </summary>
		/// <param name="sectionName">Name of the section.</param>
		/// <param name="writer">The writer.</param>
		public void RenderSection(string sectionName, TextWriter writer)
		{
			if (HasSection(sectionName) == false)
				return; //matching the NVelocity behavior, but maybe should throw?
			ICallable callable = (ICallable) sections[sectionName];
			callable.Call(new object[] {writer});
		}

		public IViewEngine ViewEngine
		{
			get { return parent.ViewEngine; }
		}

		#endregion

		public void RegisterSection(string name, ICallable section)
		{
			if (sections == null)
			{
				sections = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			}
			sections[name] = section;
		}
	}
}