using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.XPath;


namespace NVelocity.NAnt.Anakia {

    /// <summary>
    /// A JDOM {@link Element} that is tailored for Anakia needs. It has
    /// {@link #selectNodes(String)} method as well as a {@link #toString()} that
    /// outputs the XML serialized form of the element. This way it acts in much the
    /// same way as a single-element {@link NodeList} would.
    /// </summary>
    /// <author><a href="mailto:szegedia@freemail.hu">Attila Szegedi</a></author>
    public class AnakiaXmlElement : XmlElement {

	internal AnakiaXmlElement( string prefix, string localname, string nsURI, XmlDocument doc ) : base( prefix, localname, nsURI, doc ) {}


	/// <summary>
	/// Returns the XML serialized form of this element, as produced by the default
	/// {@link XMLOutputter}.
	/// </summary>
	public override String ToString() {
	    return this.OuterXml;
	}

	/// <summary>
	/// <p>
	/// This returns the full content of the element as a NodeList which
	/// may contain objects of type <code>String</code>, <code>Element</code>,
	/// <code>Comment</code>, <code>ProcessingInstruction</code>,
	/// <code>CDATA</code>, and <code>EntityRef</code>.
	/// The List returned is "live" in document order and modifications
	/// to it affect the element's actual contents.  Whitespace content is
	/// returned in its entirety.
	/// </p>
	/// </summary>
	/// <returns>a <code>List</code> containing the mixed content of the
	/// element: may contain <code>String</code>,
	/// <code>{@link Element}</code>, <code>{@link Comment}</code>,
	/// <code>{@link ProcessingInstruction}</code>,
	/// <code>{@link CDATA}</code>, and
	/// <code>{@link EntityRef}</code> objects.
	/// </returns>
	public virtual IList GetContent() {
	    return new NodeList(base.ChildNodes, false);
	}

	/// <summary>
	/// <p>
	/// This returns a <code>NodeList</code> of all the child elements
	/// nested directly (one level deep) within this element, as
	/// <code>Element</code> objects.  If this target element has no nested
	/// elements, an empty List is returned.  The returned list is "live"
	/// in document order and changes to it affect the element's actual
	/// contents.
	/// </p>
	/// <p>
	/// This performs no recursion, so elements nested two levels
	/// deep would have to be obtained with:
	/// <pre>
	/// <code>
	/// Iterator itr = currentElement.getChildren().iterator();
	/// while (itr.hasNext()) {
	/// Element oneLevelDeep = (Element)nestedElements.next();
	/// List twoLevelsDeep = oneLevelDeep.getChildren();
	/// // Do something with these children
	/// }
	/// </code>
	/// </pre>
	/// </p>
	/// </summary>
	/// <returns>list of child <code>Element</code> objects for this element</returns>
	public virtual IList GetChildren() {
	    XmlNodeList xnl = base.ChildNodes;
	    NodeList nl = new NodeList();
	    foreach(Object o in xnl) {
		if(o is AnakiaXmlElement) {
		    nl.Add(o);
		}
	    }
	    return nl;
	}

	/// <summary>
	/// <p>
	/// This returns a <code>NodeList</code> of all the child elements
	/// nested directly (one level deep) within this element with the given
	/// local name and belonging to no namespace, returned as
	/// <code>Element</code> objects.  If this target element has no nested
	/// elements with the given name outside a namespace, an empty List
	/// is returned.  The returned list is "live" in document order
	/// and changes to it affect the element's actual contents.
	/// </p>
	/// <p>
	/// Please see the notes for <code>{@link #getChildren}</code>
	/// for a code example.
	/// </p>
	/// </summary>
	/// <param name="name">local name for the children to match</param>
	/// <returns>all matching child elements</returns>
	public virtual IList GetChildren(String name) {
	    return GetChildren(name, "");
	}

	/// <summary>
	/// <p>
	/// This returns a <code>NodeList</code> of all the child elements
	/// nested directly (one level deep) within this element with the given
	/// local name and belonging to the given Namespace, returned as
	/// <code>Element</code> objects.  If this target element has no nested
	/// elements with the given name in the given Namespace, an empty List
	/// is returned.  The returned list is "live" in document order
	/// and changes to it affect the element's actual contents.
	/// </p>
	/// <p>
	/// Please see the notes for <code>{@link #getChildren}</code>
	/// for a code example.
	/// </p>
	/// </summary>
	/// <param name="name">local name for the children to match</param>
	/// <param name="ns"><code>Namespace</code> to search within</param>
	/// <returns>all matching child elements</returns>
	public virtual IList GetChildren(System.String name, String ns) {
	    XmlNodeList xnl = base.ChildNodes;
	    NodeList nl = new NodeList();
	    foreach(Object o in xnl) {
		if(o is AnakiaXmlElement) {
		    AnakiaXmlElement element = (AnakiaXmlElement)o;
		    if (element.LocalName.Equals(name) && element.NamespaceURI.Equals(ns)) {
			nl.Add(o);
		    }
		}
	    }
	    return nl;
	}

	/// <summary>
	/// <p>
	/// This returns the complete set of attributes for this element, as a
	/// <code>NodeList</code> of <code>Attribute</code> objects in no particular
	/// order, or an empty list if there are none.
	/// The returned list is "live" and changes to it affect the
	/// element's actual attributes.
	/// </p>
	/// </summary>
	/// <returns>attributes for the element</returns>
	public virtual IList GetAttributes() {
	    return new NodeList(base.Attributes);
	}

	public virtual XmlNode GetChild(String xpath) {
	    Object o = base.SelectSingleNode(xpath);
	    if (o is IHasXmlNode) {
		return ((IHasXmlNode)o).GetNode();
	    } else {
		return (XmlNode)o;
	    }
	}


    }
}
