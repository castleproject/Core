// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Windsor.Configuration.Interpreters.XmlProcessor.ElementProcessors
{
	using System;
	using System.Xml;

	public class DefaultElementProcessor : AbstractXmlNodeProcessor
	{
		private const string IncludeAttrName = "includeUri";
		private static readonly DefaultTextNodeProcessor textProcessor = new DefaultTextNodeProcessor();
		private static readonly IncludeElementProcessor includeProcessor = new IncludeElementProcessor();

		public override String Name
		{
			get { return ""; }
		}

		/// <summary>
		/// Processes the specified node list.
		/// </summary>
		/// <param name="nodeList">The node list.</param>
		/// <param name="engine">The engine.</param>
		public override void Process(IXmlProcessorNodeList nodeList, IXmlProcessorEngine engine)
		{
			XmlElement element = (XmlElement)nodeList.Current;

			ProcessAttributes(element, engine);

			engine.DispatchProcessAll(new DefaultXmlProcessorNodeList(element.ChildNodes));
		}

		/// <summary>
		/// Processes element attributes.
		/// if the attribute is include will append to the element
		/// all contents from the file.
		/// if the attribute has a property reference the reference will be
		/// expanded
		/// </summary>
		/// <param name="element">The element.</param>
		/// <param name="engine"></param>
		private static void ProcessAttributes(XmlElement element, IXmlProcessorEngine engine)
		{
			ProcessIncludeAttribute(element, engine);

			foreach(XmlAttribute att in element.Attributes)
			{
				textProcessor.ProcessString(att, att.Value, engine);
			}
		}

		private static void ProcessIncludeAttribute(XmlElement element, IXmlProcessorEngine engine)
		{
			XmlAttribute include = element.Attributes[IncludeAttrName];

			if (include == null) return;
			// removing the include attribute from the element
			element.Attributes.RemoveNamedItem(IncludeAttrName);

			XmlNode includeContent = includeProcessor.ProcessInclude(element, include.Value, engine);

			if (includeContent != null)
			{
				element.PrependChild(includeContent);
			}
		}
	}
}
