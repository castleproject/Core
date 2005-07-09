using System;
using System.Collections;
using System.Xml;
using System.IO;
using System.Xml.XPath;
using System.Text;

namespace NVelocity.NAnt.Anakia {

    /// <summary>
    /// Provides a class for wrapping a list of JDOM objects primarily for use in template
    /// engines and other kinds of text transformation tools.
    /// It has a {@link #toString()} method that will output the XML serialized form of the
    /// nodes it contains - again focusing on template engine usage, as well as the
    /// {@link #selectNodes(String)} method that helps selecting a different set of nodes
    /// starting from the nodes in this list. The class also implements the {@link java.util.List}
    /// interface by simply delegating calls to the contained list (the {@link #subList(int, int)}
    /// method is implemented by delegating to the contained list and wrapping the returned
    /// sublist into a <code>NodeList</code>).
    /// </summary>
    /// <author><a href="mailto:szegedia@freemail.hu">Attila Szegedi</a></author>
    public class NodeList : IList, System.ICloneable {

	/// <summary>
	/// The contained nodes
	/// </summary>
	private IList nodes;

	/// <summary>
	/// Creates an empty node list.
	/// </summary>
	public NodeList() {
	    nodes = new ArrayList();
	}

	/// <summary>
	/// Creates a node list that holds a single {@link Document} node.
	/// </summary>
	public NodeList(XmlDocument document):this((System.Object) document) {}

	/// <summary>
	/// Creates a node list that holds a single {@link Element} node.
	/// </summary>
	public NodeList(XmlElement element):this((System.Object) element) {}

	public NodeList(System.Object object_Renamed) {
	    if (object_Renamed == null) {
		throw new System.ArgumentException("Cannot construct NodeList with null.");
	    }
	    nodes = new ArrayList(1);
	    nodes.Add(object_Renamed);
	}

	/// <summary>
	/// Creates a node list that holds a list of nodes.
	/// </summary>
	/// <param name="nodes">the list of nodes this template should hold. The created
	/// template will copy the passed nodes list, so changes to the passed list
	/// will not affect the model.
	/// </param>
	public NodeList(IList nodes):this(nodes, true) {}

	/// <summary>
	/// Creates a node list that holds a list of nodes.
	/// </summary>
	/// <param name="nodes">the list of nodes this template should hold.</param>
	/// <param name="copy">if true, the created template will copy the passed nodes
	/// list, so changes to the passed list will not affect the model. If false,
	/// the model will reference the passed list and will sense changes in it,
	/// altough no operations on the list will be synchronized.
	/// </param>
	public NodeList(IList nodes, bool copy) {
	    if (nodes == null) {
		throw new System.ArgumentException("Cannot initialize NodeList with null list");
	    }
	    this.nodes = copy?new ArrayList(nodes):nodes;
	}


	public NodeList(XmlNodeList list, bool copy) {
	    nodes = new ArrayList();
	    foreach(Object o in list) {
		nodes.Add(o);
	    }
	}

	public NodeList(XmlAttributeCollection attributes) {
	    nodes = new ArrayList();
	    foreach(XmlAttribute a in attributes) {
		nodes.Add(a);
	    }
	}

	/// <summary>
	/// Retrieves the underlying list used to store the nodes. Note however, that
	/// you can fully use the underlying list through the <code>List</code> interface
	/// of this class itself. You would probably access the underlying list only for
	/// synchronization purposes.
	/// </summary>
	public virtual IList List {
	    get {
		return nodes;
	    }

	}

	/// <summary>
	/// This method returns the string resulting from concatenation of string
	/// representations of its nodes. Each node is rendered using its XML
	/// serialization format. This greatly simplifies creating XML-transformation
	/// templates, as to output a node contained in variable x as XML fragment,
	/// you simply write ${x} in the template (or whatever your template engine
	/// uses as its expression syntax).
	/// </summary>
	public override System.String ToString() {
	    if (nodes.Count==0) {
		return "";
	    }

	    StringBuilder sb = new StringBuilder();
	    XmlNode last = null;
	    foreach(XmlNode node in nodes) {
		// seperate XmlAttribute node output by a space
		if (last != null && last is XmlAttribute && node is XmlAttribute) {
		    sb.Append(" ");
		}
		sb.Append(node.OuterXml);
		last = node;
	    }
	    return sb.ToString();
	}

	/// <summary>
	/// Returns a NodeList that contains the same nodes as this node list.
	/// @throws CloneNotSupportedException if the contained list's class does
	/// not have an accessible no-arg constructor.
	/// </summary>
	public System.Object Clone() {
	    NodeList clonedList = (NodeList) base.MemberwiseClone();
	    //clonedList.cloneNodes();
	    return clonedList;
	}

	// TODO
	private void  cloneNodes() {
	    //	    System.Type listClass = nodes.Class;
	    //	    //UPGRADE_NOTE: Exception 'java.lang.InstantiationException' was converted to ' ' which has different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1100"'
	    //	    try {
	    //		System.Type temp_Class;
	    //		temp_Class = listClass;
	    //		List clonedNodes = (List) temp_Class;
	    //		clonedNodes.addAll(nodes);
	    //		nodes = clonedNodes;
	    //	    }
	    //	    catch (System.UnauthorizedAccessException e) {
	    //		//UPGRADE_NOTE: Exception 'java.lang.CloneNotSupportedException' was converted to ' ' which has different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1100"'
	    //		throw new System.Exception("Cannot clone NodeList since" + " there is no accessible no-arg constructor on class " + listClass.FullName);
	    //	    }
	    //	    catch (System.Exception e) {
	    //		// Cannot happen as listClass represents a concrete, non-primitive,
	    //		// non-array, non-void class - there's an instance of it in "nodes"
	    //		// which proves these assumptions.
	    //		throw new System.ApplicationException();
	    //	    }
	}

	/// <summary> Returns the hash code of the contained list.
	/// </summary>
	public override int GetHashCode() {
	    return nodes.GetHashCode();
	}

	/// <summary> Tests for equality with another object.
	/// </summary>
	/// <param name="o">the object to test for equality
	/// </param>
	/// <returns>true if the other object is also a NodeList and their contained
	/// {@link List} objects evaluate as equals.
	///
	/// </returns>
	public override bool Equals(System.Object o) {
	    return o is NodeList?((NodeList) o).nodes.Equals(nodes):false;
	}

	/// <summary>
	/// Applies an XPath expression to the node list and returns the resulting
	/// node list. In order for this method to work, your application must have
	/// access to <a href="http://code.werken.com">werken.xpath</a> library
	/// classes. The implementation does cache the parsed format of XPath
	/// expressions in a weak hash map, keyed by the string representation of
	/// the XPath expression. As the string object passed as the argument is
	/// usually kept in the parsed template, this ensures that each XPath
	/// expression is parsed only once during the lifetime of the template that
	/// first invoked it.
	/// </summary>
	/// <param name="xpathExpression">the XPath expression you wish to apply</param>
	/// <returns>a NodeList representing the nodes that are the result of
	/// application of the XPath to the current node list. It can be empty.
	/// </returns>
	public virtual NodeList selectNodes(System.String xpathString) {
	    NodeList nl = new NodeList();

	    foreach(XmlNode node in nodes) {
		XPathNavigator nav = node.CreateNavigator();
		if (nav.Matches(xpathString)) {
		    nl.Add(node);
		}
	    }
	    return nl;
	}

#region IList methods

	public void CopyTo(System.Array array, int index) {
	    nodes.CopyTo(array, index);
	}

	public int Count {
	    get { return nodes.Count; }
	}

	public bool IsSynchronized {
	    get { return nodes.IsSynchronized; }
	}

	public Object SyncRoot {
	    get { return nodes.SyncRoot; }
	}

	public virtual int Add(System.Object o) {
	    return nodes.Add(o);
	}

	public IEnumerator GetEnumerator() {
	    return nodes.GetEnumerator();
	}

	public void Clear() {
	    nodes.Clear();
	}

	public bool Contains(Object value) {
	    return nodes.Contains(value);
	}

	public int IndexOf(Object value) {
	    return nodes.IndexOf(value);
	}

	public void Insert(int index, Object value) {
	    nodes.Insert(index, value);
	}

	public bool IsFixedSize {
	    get { return nodes.IsFixedSize; }
	}

	public bool IsReadOnly {
	    get { return nodes.IsReadOnly; }
	}

	public void Remove(Object value) {
	    nodes.Remove(value);
	}

	public void RemoveAt(int index) {
	    nodes.RemoveAt(index);
	}

	public Object this[int index] {
	    get { return nodes[index]; }
	    set { nodes[index] = value; }
	}


#endregion



    }
}
