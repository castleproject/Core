using System;
using System.Xml;

namespace NVelocity.NAnt.Anakia {

    /// <summary>
    /// Customized XmlDocument for Anakia so that AnakiaXmlElements can be created
    /// </summary>
    public class AnakiaXmlDocument : XmlDocument {

	public override XmlElement CreateElement(string prefix, string localname, string nsURI) {
	    AnakiaXmlElement element = new AnakiaXmlElement(prefix, localname, nsURI, this);
	    return element;
	}
    }
}
