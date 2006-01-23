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

namespace Castle.Windsor.Configuration.Interpreters.XmlProcessor.ElementProcessors
{
	using System;
	using System.Xml;

	public class ChooseElementProcessor : AbstractStatementElementProcessor
	{
		private static readonly String WhenElemName = "when";
		private static readonly String OtherwiseElemName = "otherwise";

		public ChooseElementProcessor()
		{
		}

		public override String Name
		{
			get { return "choose"; }
		}

		public override void Process(IXmlProcessorNodeList nodeList, IXmlProcessorEngine engine)
		{
			XmlElement element = nodeList.Current as XmlElement;

			XmlDocumentFragment fragment = CreateFragment(element);

			foreach(XmlNode child in element.ChildNodes)
			{
				if (IgnoreNode(child)) continue;

				XmlElement elem = GetNodeAsElement(element, child);

				bool found = false;

				if (elem.Name == WhenElemName)
				{
					found = ProcessStatement(elem, engine);
				}
				else if (elem.Name == OtherwiseElemName)
				{
					found = true;
				}
				else
				{
					throw new XmlProcessorException("'{0} can not contain only 'when' and 'otherwise' elements found '{1}'", element.Name, elem.Name);
				}

				if (found)
				{
					if (elem.ChildNodes.Count > 0)
					{
						MoveChildNodes(fragment, elem);
						engine.DispatchProcessAll(new DefaultXmlProcessorNodeList(fragment.ChildNodes));
					}
					break;
				}
			}

			ReplaceItself(fragment, element);
		}
	}
}