using System;
using System.Collections;

namespace NVelocity.Dvsl {

    /// <summary>
    /// wrapper interface for nodes exposed in the
    /// template.  Isolates the in-VSL DOM from that
    /// of the underlying implementation
    /// </summary>
    /// <author> <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a></author>
    public interface DvslNode {

	/// <summary>
	/// returns the name of the node
	/// </summary>
	String Name {
	    get
		;
	    }

	    /// <summary>
	    /// returns the 'value' of the node
	    /// </summary>
	    String Value {
		get
		    ;
		}

		/// <summary>
		/// returns the value of the XPath
		/// expression
		/// </summary>
		Object ValueOf(String xpath);

	/// <summary>
	/// returns attribute
	/// </summary>
	String Attrib(String attribute);

	/// <summary>
	/// returns a list of nodes that satisfy
	/// the xpath
	/// </summary>
	IList SelectNodes(String xpath);

	/// <summary>  returns a single node that satisfies
	/// the xpath
	/// </summary>
	DvslNode SelectSingleNode(String xpath);

	DvslNode Get(String xpath);

	/// <summary>
	/// renders a deep copy of the XML tree
	/// below the current node to the output
	/// </summary>
	String Copy();

	/// <summary>
	/// renders a deep copy of the nodes in
	/// the list ot the output
	/// </summary>
	String Copy(IList nodeList);

	/// <summary>
	/// returns a list of all children of the current node
	/// </summary>
	IList Children();

	/// <summary>
	/// returns the 'value' of the node
	/// </summary>
	String ToString();

	/// <summary>
	/// returns the object corresponding to the node
	/// in the implementaion that we are using.
	/// use only with the greatest of care
	/// </summary>
	Object NodeImpl {
	    get
		;
	    }

	    Hashtable AttribMap {
		get
		    ;
		}

	    }
}
