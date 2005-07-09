using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

using NVelocity.Context;
using NVelocity.Runtime.Parser.Node;

namespace NVelocity.Dvsl {

    /// <summary>
    /// Currently provides the match rule accumulation
    /// as well as the AST storage and rendering
    ///
    /// Rule stuff might be replaced with the dom4j RuleManager
    /// </summary>
    /// <author> <a href="mailto:geirm@apache.org?">Geir Magnusson Jr.</a></author>
    public class TemplateHandler {

	public TemplateHandler() {}

	private Hashtable astmap = new Hashtable();
	private IList xpathList = new ArrayList();

	public virtual void RegisterMatch(String xpath, SimpleNode node) {
	    Hashtable foo = new Hashtable();
	    foo["xpath"] = xpath;
	    foo["ast"] = node;
	    xpathList.Add(foo);
	}


	internal virtual bool Render(DvslNode node, IContext context, TextWriter writer) {
	    /*
	    *  find if we have an AST where the xpath expression mathes
	    *  for this node
	    */

	    XmlNode dom4jnode = (XmlNode) node.NodeImpl;
	    XPathNavigator nav = dom4jnode.CreateNavigator();
	    SimpleNode sn = null;

	    for (int i = 0; i < xpathList.Count; i++) {
		Hashtable m = (Hashtable) xpathList[i];

		XPathExpression expr = nav.Compile((String)m["xpath"]);

		if (nav.Matches((String)m["xpath"])) {
		    sn = (SimpleNode) m["ast"];
		    break;
		}
	    }

	    // if we found something, render
	    if (sn != null) {
		InternalContextAdapterImpl ica = new InternalContextAdapterImpl(context);
		ica.PushCurrentTemplateName(node.Name);

		try {
		    sn.render(ica, writer);
		} finally {
		    ica.PopCurrentTemplateName();
		}

		return true;
	    }

	    return false;
	}


    }
}
