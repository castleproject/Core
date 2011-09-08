// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Components.DictionaryAdapter.Xml
{
	internal struct XmlCollectionItem
	{
		public readonly IXmlNode Node;
		public readonly object Value;

		public XmlCollectionItem(IXmlNode node)
			: this(node, null) { }

		public XmlCollectionItem(IXmlNode node, object value)
		{
			Node  = node;
			Value = value;
		}

		public bool HasValue
		{
			get { return null != Value; }
		}

		public XmlCollectionItem WithValue(object value)
		{
			return new XmlCollectionItem(Node, value);
		}
	}
}
