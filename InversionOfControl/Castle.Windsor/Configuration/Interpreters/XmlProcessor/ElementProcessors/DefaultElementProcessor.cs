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

using System.Collections;

namespace Castle.Windsor.Configuration.Interpreters.XmlProcessor.ElementProcessors
{
	using System;
	using System.Xml;

	public class DefaultElementProcessor : AbstractXmlNodeProcessor
	{
		private static readonly String IncludeAttrName = "includeUri";
		private static readonly DefaultTextNodeProcessor textProcessor = new DefaultTextNodeProcessor();
		private static readonly IncludeElementProcessor includeProcessor = new IncludeElementProcessor();

		public DefaultElementProcessor()
		{
		}

		public override String Name
		{
			get { return ""; }
		}

		public override void Process(IXmlProcessorNodeList nodeList, IXmlProcessorEngine engine)
		{
			XmlElement element = nodeList.Current as XmlElement;

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
		private void ProcessAttributes(XmlElement element, IXmlProcessorEngine engine)
		{
			ProcessIncludeAttribute(element, engine);
            //we may add attributes to the element as we iterate over it.
			foreach(XmlAttribute att in new ArrayList(element.Attributes))
			{
				textProcessor.ProcessString(att, att.Value, engine);
			}
		}

		private void ProcessIncludeAttribute(XmlElement element, IXmlProcessorEngine engine)
		{
			XmlAttribute include = element.Attributes[IncludeAttrName] as XmlAttribute;

			if (include != null)
			{
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
}