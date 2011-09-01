using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace Castle.Components.DictionaryAdapter.Xml
{
	public interface IXmlPropertyAccessor : IXmlAccessor
	{
		object GetPropertyValue(XPathNavigator parentNode, IDictionaryAdapter parentObject, bool orStub);
		void   SetPropertyValue(XPathNavigator parentNode, object value);
	}
}
