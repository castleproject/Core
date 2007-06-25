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
	using System.IO;

	public class MockViewEngine : IViewEngine
	{
		private string viewFileExtension;
		private string jsGeneratorFileExtension;
		private bool supportsJSGeneration;
		private bool xHtmlRendering;

		public MockViewEngine(string viewFileExtension, string jsGeneratorFileExtension, bool supportsJSGeneration, bool xHtmlRendering)
		{
			this.viewFileExtension = viewFileExtension;
			this.jsGeneratorFileExtension = jsGeneratorFileExtension;
			this.supportsJSGeneration = supportsJSGeneration;
			this.xHtmlRendering = xHtmlRendering;
		}

		public bool SupportsJSGeneration
		{
			get { return supportsJSGeneration; }
		}

		public bool XHtmlRendering
		{
			get { return xHtmlRendering; }
			set { xHtmlRendering = value; }
		}

		public string ViewFileExtension
		{
			get { return viewFileExtension; }
		}

		public string JSGeneratorFileExtension
		{
			get { return jsGeneratorFileExtension; }
		}

		public virtual bool HasTemplate(string templateName)
		{
			throw new NotImplementedException();
		}

		public virtual void Process(IRailsEngineContext context, Controller controller, string templateName)
		{
			throw new NotImplementedException();
		}

		public virtual void Process(TextWriter output, IRailsEngineContext context, Controller controller, string templateName)
		{
			throw new NotImplementedException();
		}

		public virtual object CreateJSGenerator(IRailsEngineContext context)
		{
			throw new NotImplementedException();
		}

		public virtual void GenerateJS(IRailsEngineContext context, Controller controller, string templateName)
		{
			throw new NotImplementedException();
		}

		public virtual void GenerateJS(TextWriter output, IRailsEngineContext context, Controller controller, string templateName)
		{
			throw new NotImplementedException();
		}

		public virtual void ProcessPartial(TextWriter output, IRailsEngineContext context, Controller controller, string partialName)
		{
			throw new NotImplementedException();
		}

		public virtual void ProcessContents(IRailsEngineContext context, Controller controller, string contents)
		{
			throw new NotImplementedException();
		}
	}
}
