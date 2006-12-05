// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	using Castle.MonoRail.Framework;

	public class BrailViewComponentContext : IViewComponentContext
	{
		string componentName;
		IDictionary contextVars = 	new Hashtable(
#if DOTNET2
StringComparer.InvariantCultureIgnoreCase
#else
				CaseInsensitiveHashCodeProvider.Default,
				CaseInsensitiveComparer.Default
#endif
				);

		IDictionary componentParameters;

		string viewToRender;

		ICallable body;
		private readonly TextWriter default_writer;

		public string ComponentName
		{
			get { return componentName; }
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

		public ICallable Body
		{
			get { return body; }
			set { body = value; }
		}

		public TextWriter Writer
		{
			get { return default_writer; }
		}

		public BrailViewComponentContext(ICallable body, string name, TextWriter text, IDictionary parameters)
		{
			this.body = body;
			this.componentName = name;
			this.default_writer = text;
			this.componentParameters = parameters;
		}
		
		public void RenderBody()
		{
			RenderBody(default_writer);
		}
		
		public void RenderBody(TextWriter writer)
		{
			if (body == null)
				throw new RailsException("This component does not have a body content to be rendered");
			body.Call(new object[] { writer });
		}

		public bool HasSection(string sectionName)
		{
			return false;
		}

		public void RenderSection(string sectionName)
		{
			throw new NotImplementedException();
		}
	}
}
