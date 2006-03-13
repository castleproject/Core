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

	public class PropertiesElementProcessor : AbstractXmlNodeProcessor
	{
		public override String Name
		{
			get { return "properties"; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="nodeList"></param>
		/// <param name="engine"></param>
		/// <example>
		/// <code>
		/// 	<properties>
		///			<attributes>
		///				<myAttribute>attributeValue</myAttribute>
		///			<attributes>
		///			<myProperty>propertyValue</myProperty>
		///		</properties>
		/// </code>
		/// </example>
		public override void Process(IXmlProcessorNodeList nodeList, IXmlProcessorEngine engine)
		{
			XmlElement element = nodeList.Current as XmlElement;

			IXmlProcessorNodeList childNodes = new DefaultXmlProcessorNodeList(element.ChildNodes);
			
			while(childNodes.MoveNext())
			{
				// Properties processing its a little more complicated than usual
				// since we need to support all special tags (if,else,define...)
				// plus we need to register any regular element as a property asap
				// since we should support properties that reference other properties
				// i.e. <myprop2>#{prop1}</myprop2> 			
				if (engine.HasSpecialProcessor(childNodes.Current))
				{
					// Current node its a special element so we bookmark it before processing it...
					XmlNode current = childNodes.Current;

					int pos = childNodes.CurrentPosition;

					engine.DispatchProcessCurrent(childNodes);
				
					// ...after processing we need to refresh childNodes
					// to account for any special element that affects the node tree (if,choose...)
					childNodes = new DefaultXmlProcessorNodeList(element.ChildNodes);

					// we only care about changes in the tree from the current node and forward
					// so if the new list is empty or smaller we just exit the loop
					if (pos < childNodes.Count)
					{
						childNodes.CurrentPosition = pos;

						// if the current node gets replaced in the new list we need to restart processing
						// otherwise we just continue as usual
						if (childNodes.Current != current)
						{
							childNodes.CurrentPosition -= 1;
							continue;
						}
					}
					else
					{
						break;						
					}					
				}
				else
				{
					engine.DispatchProcessCurrent(childNodes);	
				}
				
				if (IgnoreNode(childNodes.Current)) continue;

				XmlElement elem = GetNodeAsElement(element, childNodes.Current);

				engine.AddProperty(elem);					
			}

			RemoveItSelf(element);
		}
	}
}
