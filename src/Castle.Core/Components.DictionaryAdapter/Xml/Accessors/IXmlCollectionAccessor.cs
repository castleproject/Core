using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.Collections;

namespace Castle.Components.DictionaryAdapter.Xml
{
	public interface IXmlCollectionAccessor : IXmlAccessor
	{
		void GetCollectionItems(XPathNavigator parentNode, IDictionaryAdapter parentObject, IConfigurable<XmlTypedNode> collection);
		void GetCollectionItems(XPathNavigator parentNode, IDictionaryAdapter parentObject, IList values);
		void SetCollectionItems(XPathNavigator parentNode, IEnumerable values);

		XmlTypedNode AddCollectionItem       (XmlTypedNode parentNode, IDictionaryAdapter parentObject, object value);
		XmlTypedNode InsertCollectionItem    (XmlTypedNode beforeNode, IDictionaryAdapter parentObject, object value);
		void         RemoveCollectionItem    (XmlTypedNode actualNode);
		void         RemoveAllCollectionItems(XmlTypedNode parentNode);
	}
}
