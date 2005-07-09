using System;
using System.Collections;
using System.Xml;

namespace NVelocity.Anakia {

    /// <summary> This class adds an entrypoint into XPath functionality,
    /// for Anakia.
    /// <p>
    /// All methods take a string XPath specification, along with
    /// a context, and produces a resulting java.util.List.
    /// <p>
    /// The W3C XPath Specification (http://www.w3.org/TR/xpath) refers
    /// to NodeSets repeatedly, but this implementation simply uses
    /// java.util.List to hold all Nodes.  A 'Node' is any object in
    /// a JDOM object tree, such as an org.jdom.Element, org.jdom.Document,
    /// or org.jdom.Attribute.
    /// <p>
    /// To use it in Velocity, do this:
    /// <p>
    /// <pre>
    /// #set $authors = $xpath.applyTo("document/author", $root)
    /// #foreach ($author in $authors)
    /// $author.getValue()
    /// #end
    /// #set $chapterTitles = $xpath.applyTo("document/chapter/@title", $root)
    /// #foreach ($title in $chapterTitles)
    /// $title.getValue()
    /// #end
    /// </pre>
    /// <p>
    /// In newer Anakia builds, this class is obsoleted in favor of calling
    /// <code>selectNodes()</code> on the element directly:
    /// <pre>
    /// #set $authors = $root.selectNodes("document/author")
    /// #foreach ($author in $authors)
    /// $author.getValue()
    /// #end
    /// #set $chapterTitles = $root.selectNodes("document/chapter/@title")
    /// #foreach ($title in $chapterTitles)
    /// $title.getValue()
    /// #end
    /// </pre>
    /// <p>
    ///
    /// </summary>
    /// <author> <a href="mailto:bob@werken.com">bob mcwhirter</a>
    /// </author>
    /// <author> <a href="mailto:jon@latchkey.com">Jon S. Stevens</a>
    /// </author>
    /// <author> <a href="mailto:szegedia@freemail.hu">Attila Szegedi</a>
    /// </author>
    /// <version> $Id: XPathTool.cs,v 1.2 2003/10/27 13:54:09 corts Exp $
    ///
    /// </version>
    public class XPathTool {
	/// <summary> Constructor does nothing, as this is mostly
	/// just objectified static methods
	/// </summary>
	public XPathTool() {
	    //        RuntimeSingleton.info("XPathTool::XPathTool()");
	    // intentionally left blank
	}

	/// <summary>
	/// Apply an XPath to a JDOM Document
	/// </summary>
	/// <param name="xpathSpec">The XPath to apply</param>
	/// <param name="doc">The Document context</param>
	/// <returns>A list of selected nodes</returns>
	public virtual NodeList applyTo(System.String xpathSpec, XmlDocument doc) {
	    //RuntimeSingleton.info("XPathTool::applyTo(String, Document)");
	    return new NodeList(XPathCache.getXPath(xpathSpec).applyTo(doc), false);
	}

	/// <summary>
	/// Apply an XPath to a JDOM Element
	/// </summary>
	/// <param name="xpathSpec">The XPath to apply</param>
	/// <param name="doc">The Element context</param>
	/// <returns>A list of selected nodes</returns>
	public virtual NodeList applyTo(System.String xpathSpec, XmlElement elem) {
	    //RuntimeSingleton.info("XPathTool::applyTo(String, Element)");
	    return new NodeList(XPathCache.getXPath(xpathSpec).applyTo(elem), false);
	}

	/// <summary>
	/// Apply an XPath to a nodeset
	/// </summary>
	/// <param name="xpathSpec">The XPath to apply</param>
	/// <param name="doc">The nodeset context</param>
	/// <returns>A list of selected nodes</returns>
	public virtual NodeList applyTo(System.String xpathSpec, IList nodeSet) {
	    //RuntimeSingleton.info("XPathTool::applyTo(String, List)");
	    return new NodeList(XPathCache.getXPath(xpathSpec).applyTo(nodeSet), false);
	}


    }
}
