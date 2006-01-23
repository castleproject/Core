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

namespace Castle.Windsor.Configuration.Interpreters.XmlProcessor
{
	using System.Collections;
	using System.Xml;

	public class DefaultXmlProcessorNodeList : IXmlProcessorNodeList
	{
		private IList nodes;
		private int index = -1;

		public DefaultXmlProcessorNodeList(XmlNode node)
		{
			nodes = new ArrayList();
			nodes.Add(node);
		}

		public DefaultXmlProcessorNodeList(ArrayList nodes)
		{
			this.nodes = nodes;
		}

		public DefaultXmlProcessorNodeList(XmlNodeList nodes)
		{
			this.nodes = CloneNodeList(nodes);
		}

		/// <summary>
		/// Make a shallow copy of the nodeList.
		/// </summary>
		/// <param name="nodeList">The nodeList to be copied.</param>
		/// <returns></returns>
		protected ArrayList CloneNodeList(XmlNodeList nodeList)
		{
			ArrayList nodes = new ArrayList(nodeList.Count);

			foreach(XmlNode node in nodeList)
			{
				nodes.Add(node);
			}

			return nodes;
		}

		public XmlNode Current
		{
			get { return nodes[index] as XmlNode; }
		}

		public bool MoveNext()
		{
			return ++index < nodes.Count;
		}
	}
}