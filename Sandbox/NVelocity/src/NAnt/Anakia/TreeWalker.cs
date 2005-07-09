using System;
using System.Collections;
using System.Xml;

namespace NVelocity.Anakia {

    /// <summary> This class allows you to walk a tree of JDOM Element objects.
    /// It first walks the tree itself starting at the Element passed
    /// into allElements() and stores each node of the tree
    /// in a Vector which allElements() returns as a result of its
    /// execution. You can then use a #foreach in Velocity to walk
    /// over the Vector and visit each Element node. However, you can
    /// achieve the same effect by calling <code>element.selectNodes("//*")</code>.
    /// *
    /// </summary>
    /// <author> <a href="mailto:jon@latchkey.com">Jon S. Stevens</a>
    /// </author>
    /// <author> <a href="mailto:szegedia@freemail.hu">Attila Szegedi</a>
    /// </author>
    /// <version> $Id: TreeWalker.cs,v 1.2 2003/10/27 13:54:09 corts Exp $
    ///
    /// </version>
    public class TreeWalker {
	/// <summary> Empty constructor
	/// </summary>
	public TreeWalker() {
	    // Left blank
	}

	/// <summary> Creates a new Vector and walks the Element tree.
	///
	/// </summary>
	/// <param name="Element">the starting Element node
	/// </param>
	/// <returns>Vector a vector of Element nodes
	///
	/// </returns>
	public virtual NodeList allElements(XmlElement e) {
	    ArrayList theElements = new ArrayList();
	    treeWalk(e, theElements);
	    return new NodeList(theElements, false);
	}

	/// <summary>
	/// A recursive method to walk the Element tree.
	/// </summary>
	/// <param name="Element">the current Element</param>
	private void treeWalk(XmlElement e, IList theElements) {
	    foreach(XmlElement child in e.ChildNodes) {
		theElements.Add(child);
		treeWalk(child, theElements);
	    }
	}
    }
}
