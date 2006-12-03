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

namespace Castle.MonoRail.Views.IronView.ElementProcessor
{
	using System;
	using System.IO;
	using System.Xml;
	using Castle.MonoRail.Framework;

	/// <summary>
	/// Renders a sub template
	/// </summary>
	public class RenderPartial : IElementProcessor
	{
		public bool CanHandlePI(string name, string value)
		{
			return false;
		}

		public bool CanHandleElement(ITemplateParser parser, XmlReader reader)
		{
			return (reader.NodeType == XmlNodeType.Element && reader.LocalName == "mr:render");
		}

		public void ProcessPI(ITemplateParser parser, ITemplateContext context)
		{
			throw new NotImplementedException();
		}

		public void ProcessElement(ITemplateParser parser, ITemplateContext context)
		{
			String view = context.Reader.GetAttribute("src") + IronPythonViewEngine.TemplateExtension;
			
			IViewSourceLoader loader = 
				(IViewSourceLoader) context.ServiceProvider.GetService(typeof(IViewSourceLoader));

			IViewSource source = loader.GetViewSource(view);
			
			if (source == null)
			{
				throw new RailsException("RenderPartial: could not find view [" + view + "]");
			}

			String functionName;
			context.Engine.CompilePartialTemplate(source, view, out functionName);

			context.Script.AppendLine();
			// context.AppendIndented(functionName);
			context.AppendIndented("something");
			context.Script.Append('(');
			// context.Script.Append("controller, context, request, response, session, output, flash, siteroot");
			context.Script.Append("output");
			context.Script.Append(')');
			context.Script.AppendLine();
		}
	}
}
