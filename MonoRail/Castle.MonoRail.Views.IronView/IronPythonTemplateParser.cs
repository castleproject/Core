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

namespace Castle.MonoRail.Views.IronView
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Xml;

	public class IronPythonTemplateParser : ITemplateParser
	{
		private IElementProcessor[] processors;
		private Stack<IElementProcessor> processorStack = new Stack<IElementProcessor>();

		/// <summary>
		/// Initializes a new instance of the <see cref="IronPythonTemplateParser"/> class.
		/// </summary>
		/// <param name="processors">The processors.</param>
		public IronPythonTemplateParser(IElementProcessor[] processors)
		{
			this.processors = processors;
		}

		public String CreateScriptBlock(TextReader reader, String viewName,
		                                IServiceProvider serviceProvider)
		{
			XmlTextReader xmlReader = new XmlTextReader(reader);
			xmlReader.Namespaces = false;
			xmlReader.WhitespaceHandling = WhitespaceHandling.All;

			DefaultContext ctx = new DefaultContext(viewName, xmlReader, serviceProvider);

			ProcessReader(ctx, 0);

			return ctx.Script.ToString();
		}

		#region ITemplateParser

		public void ProcessReader(ITemplateContext context, int levelToStop)
		{
			XmlReader reader = context.Reader;
			
			while(reader.Read())
			{
				switch(reader.NodeType)
				{
					case XmlNodeType.Comment:
						break;

					case XmlNodeType.ProcessingInstruction:

						if (IsCustomPI(context))
						{
							ProcessPI(context);
						}
						else
						{
							OutputCurrentElementAsContent(context);
						}
						break;

					case XmlNodeType.Element:

						context.IncreaseDepth();

						if (IsCustomElement(reader))
						{
							ProcessCustomTag(context);
						}
						else
						{
							OutputCurrentElementAsContent(context);
						}

						if (!reader.IsEmptyElement)
						{
							context.DecreaseDepth();
						}

						break;

					case XmlNodeType.EndElement:
						
						OutputCurrentElementAsContent(context);
						
						context.DecreaseDepth();

						if (context.CurrentElementDepth == levelToStop)
						{
							return;
						}

						break;

					case XmlNodeType.Text:
					case XmlNodeType.Whitespace:
						OutputCurrentElementAsContent(context);
						break;

					case XmlNodeType.DocumentType:
						OutputCurrentElementAsContent(context);
						break;
				}
			}
		}

		public void OutputCurrentElementAsContent(ITemplateContext context)
		{
			OutputCurrentElementAsContent(context, null);
		}

		public void OutputCurrentElementAsContent(ITemplateContext context, FilterAttribute filter)
		{
			context.AppendIndented("output.Write('");

			XmlReader reader = context.Reader;
			StringBuilder script = context.Script;

			if (reader.NodeType == XmlNodeType.Element)
			{
				if (reader.IsEmptyElement)
				{
					script.Append("<" + reader.LocalName);
					OutputAttributes(reader, script, filter);
					script.Append(" />");
				}
				else
				{
					script.Append("<");
					script.Append(reader.LocalName);
					OutputAttributes(reader, script, filter);
					script.Append(">");
				}
			}
			else if (reader.NodeType == XmlNodeType.EndElement)
			{
				script.Append("</");
				script.Append(reader.LocalName);
				script.Append(">");
			}
			else if (reader.NodeType == XmlNodeType.ProcessingInstruction)
			{
				script.Append("<?");
				script.Append(reader.LocalName);
				script.Append(' ');
				script.Append(reader.Value);
				script.Append(" ?>");
			}
			else if (reader.NodeType == XmlNodeType.Text ||
			         reader.NodeType == XmlNodeType.Whitespace)
			{
				script.Append(ConvertEscapes(reader.Value));
			}

			script.AppendLine("')");
		}

		#endregion

		private bool IsCustomPI(ITemplateContext context)
		{
			foreach(IElementProcessor processor in processors)
			{
				if (processor.CanHandlePI(context.Reader.LocalName, context.Reader.Value))
				{
					processorStack.Push(processor);

					return true;
				}
			}

			return false;
		}

		private void ProcessPI(ITemplateContext context)
		{
			processorStack.Pop().ProcessPI(this, context);
		}

		private bool IsCustomElement(XmlReader reader)
		{
			foreach(IElementProcessor processor in processors)
			{
				if (processor.CanHandleElement(this, reader))
				{
					processorStack.Push(processor);

					return true;
				}
			}

			return false;
		}

		private void ProcessCustomTag(ITemplateContext context)
		{
			processorStack.Pop().ProcessElement(this, context);
		}

		private void OutputAttributes(XmlReader reader, StringBuilder script, FilterAttribute filter)
		{
			if (!reader.HasAttributes) return;

			if (!reader.MoveToFirstAttribute()) return;

			do
			{
				String name = reader.LocalName;

				if (filter != null && !filter(name))
				{
					continue;
				}

				script.Append(' ');
				script.Append(name);
				script.Append('=');
				script.Append('\"');
				script.Append(reader.Value);
				script.Append('\"');
				
			} while(reader.MoveToNextAttribute());
		}

		private string ConvertEscapes(string value)
		{
			if (value == null || value == String.Empty) return String.Empty;

			// TODO: This can be greatly optimized

			return value.Replace("\t", "\\t").Replace("\n", "\\n").Replace("\r", "\\r");
		}
	}
}