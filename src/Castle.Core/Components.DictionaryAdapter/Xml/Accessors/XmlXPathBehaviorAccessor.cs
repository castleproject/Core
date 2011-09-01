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

#if !SILVERLIGHT
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml.XPath;

	public class XmlXPathBehaviorAccessor : XmlAccessor,
		IConfigurable<XPathAttribute>
	{
	    private ICompiledPath path;

	    public XmlXPathBehaviorAccessor(PropertyDescriptor property, IXmlKnownTypeMap knownTypes)
	        : base(property.PropertyType, knownTypes) { }

	    public ICompiledPath Path
	    {
	        get { return path; }
	    }

		public bool SelectsNodes
		{
			get { return path.Expression.ReturnType == XPathResultType.NodeSet; }
		}

		public void Configure(XPathAttribute behavior)
		{
			path = behavior.Path;
		}

		public override IXmlCollectionAccessor GetCollectionAccessor(Type itemType)
		{
			return new XmlSubaccessor(this, itemType);
		}

		public override object GetPropertyValue(XPathNavigator node, IDictionaryAdapter da, bool ifExists)
		{
			return SelectsNodes
				? base.GetPropertyValue(node, da, ifExists)
				: Evaluate(node);
		}

		public override void SetPropertyValue(XPathNavigator node, object value)
		{
			if (SelectsNodes)
				base.SetPropertyValue(node, value);
			else
				throw Error.NotSupported();
		}

		private object Evaluate(XPathNavigator node)
		{
			var value = node.Evaluate(path.Expression);
			return Convert.ChangeType(value, ClrType);
		}

		protected internal override XmlIterator SelectPropertyNode(XPathNavigator node, bool create)
		{
			return new XPathMutableIterator(node, path, false);
		}

		protected internal override XmlIterator SelectCollectionNode(XPathNavigator node, bool create)
		{
			return SelectSelf(node);
		}

		protected internal override XmlIterator SelectCollectionItems(XPathNavigator node, bool create)
		{
			return new XPathMutableIterator(node, path, true);
		}
	}
}
#endif
