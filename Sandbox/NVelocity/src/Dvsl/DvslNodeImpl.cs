using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace NVelocity.Dvsl {

    /// <summary>
    /// wrapper class for .Net nodes to implement the
    /// DVSLNode interface for template use
    /// </summary>
    /// <author> <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a></author>
    public class DvslNodeImpl : DvslNode {

	protected internal XmlNode element = null;
	protected internal Hashtable attributes = null;

	/// <summary>
	/// this is a bit yecchy - need to revamp
	/// </summary>
	public DvslNodeImpl(XmlElement e) {
	    element = e;
	}

	public DvslNodeImpl(XmlDocument e) {
	    element = e;
	}

	public DvslNodeImpl(XmlText e) {
	    element = e;
	}

	public DvslNodeImpl(XmlAttribute e) {
	    element = e;
	}

	public DvslNodeImpl(XmlComment e) {
	    element = e;
	}

	public DvslNodeImpl(XmlCDataSection e) {
	    element = e;
	}

	public DvslNodeImpl() {}


	public virtual Object NodeImpl {
	    get {
		return element;
	    }
	}


	public virtual Hashtable AttribMap {
	    get {
		/*
		*  $$$ GMJ sync issue? yes.  Do I care?
		*/
		if (attributes == null) {
		    attributes = new Hashtable();
		}

		/*
		* only Elements have attributes
		*/
		if (element is XmlElement) {
		    XmlElement e = (XmlElement) element;
		    foreach(XmlAttribute at in e.Attributes) {
			attributes[at.Name] = at.Value;
		    }
		}

		return attributes;
	    }

	}

	/// <summary>
	/// returns the name of the node
	/// </summary>
	public virtual String Name {
	    get { return element.Name; }
	}

	/// <summary>
	/// returns a specificed attributeattribute
	/// </summary>
	public virtual String Attribute(String attribute) {
	    if (element is XmlElement) {
		return ((XmlElement) element).Attributes[attribute].Value;
	    }

	    return null;
	}

	/// <summary>
	/// returns a list of nodes that satisfy the xpath
	/// </summary>
	public virtual IList SelectNodes(String xpath) {
	    XmlNodeList l = element.SelectNodes(xpath);
	    IList list = new ArrayList();

	    for (int i = 0; i < l.Count; i++) {
		XmlNode n = l[i];
		if (n != null) {
		    DvslNode dn = MakeDvslNode(n);
		    if (dn != null) {
			list.Add(dn);
		    }
		}
	    }

	    return list;
	}

	public virtual DvslNode SelectSingleNode(String xpath) {
	    XmlNode n = element.SelectSingleNode(xpath);
	    return MakeDvslNode(n);
	}

	public virtual DvslNode Get(String xpath) {
	    return SelectSingleNode(xpath);
	}

	public virtual String Value {
	    get { return element.InnerText; }
	}

	public virtual Object ValueOf(String xpath) {
	    Object o = element.CreateNavigator().Evaluate(xpath);
	    return o;
	}

	public override String ToString() {
	    return Value;
	}

	public virtual IList Children() {
	    IList list = new ArrayList();

	    if (element.NodeType == XmlNodeType.Element) {
		XmlNodeList nodes = ((XmlElement) element).ChildNodes;

		for (int i = 0; i < nodes.Count; i++)
		    list.Add(MakeDvslNode(nodes[i]));
	    }

	    return list;
	}

	/// <summary>
	/// assumes a list of DVSLNodes
	/// </summary>
	public virtual String Copy(IList nodes) {
	    if (nodes == null)
		return "";

	    StringWriter sw = new StringWriter();

	    for (int i = 0; i < nodes.Count; i++) {
		DvslNode dn = (DvslNode) nodes[i];
		sw.Write(((XmlNode)dn.NodeImpl).OuterXml);
	    }

	    return sw.ToString();
	}

	public virtual String Copy() {
	    return element.OuterXml;
	}

	public virtual String Render() {
	    try {
		StringWriter sw = new StringWriter();
		XmlTextWriter tw = new XmlTextWriter(sw);
		element.WriteContentTo(tw);
		return tw.ToString();
	    } catch (System.Exception e) {}

	    return "";
	}

	public virtual String Attrib(String name) {
	    if (element is XmlElement) {
		XmlAttribute attrib = ((XmlElement) element).Attributes[name];

		if (attrib != null) {
		    return attrib.Value;
		}
	    }

	    return null;
	}


	/// <summary>
	/// will recast all of this later
	/// </summary>
	private DvslNode MakeDvslNode(XmlNode n) {
	    if (n == null) {
		return null;
	    }

	    if (n.NodeType == XmlNodeType.Element) {
		return new DvslNodeImpl((XmlElement) n);
	    } else if (n.NodeType == XmlNodeType.Text) {
		return new DvslNodeImpl((XmlText) n);
	    } else if (n.NodeType == XmlNodeType.Attribute) {
		return new DvslNodeImpl((XmlAttribute) n);
	    } else if (n.NodeType == XmlNodeType.Comment) {
		return new DvslNodeImpl((XmlComment) n);
	    } else if (n.NodeType == XmlNodeType.CDATA) {
		return new DvslNodeImpl((XmlCDataSection) n);
	    } else if (n.NodeType == XmlNodeType.ProcessingInstruction) {
		// not a supported node type
		return null;
	    }

	    //TODO: log the unknown node type so that it can be determined is something is missing
	    return null;
	}


    }
}
