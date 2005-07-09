using System;
using System.Xml;

namespace NVelocity.Anakia {

    /// <summary> This class extends XMLOutputter in order to provide
    /// a way to walk an Element tree into a String.
    /// *
    /// </summary>
    /// <author> <a href="mailto:jon@latchkey.com">Jon S. Stevens</a>
    /// </author>
    /// <author> <a href="mailto:rubys@us.ibm.com">Sam Ruby</a>
    /// </author>
    /// <version> $Id: OutputWrapper.cs,v 1.2 2003/10/27 13:54:09 corts Exp $
    ///
    /// </version>
    public class OutputWrapper : XmlWriter {
	/// <summary>
	/// Empty constructor
	/// </summary>
	public OutputWrapper() {}

	/// <summary> This method walks an Element tree into a String. The cool
	/// thing about it is that it will strip off the first Element.
	/// For example, if you have:
	/// <p>
	/// &lt;td&gt; foo &lt;strong&gt;bar&lt;/strong&gt; ack &lt;/td&gt;
	/// </p>
	/// It will output
	/// <p>
	/// foo &lt;strong&gt;bar&lt;/strong&gt; ack &lt;/td&gt;
	/// </p>
	/// </summary>
	public virtual System.String outputString(XmlElement element, bool strip) {
	    System.IO.StringWriter buff = new System.IO.StringWriter();
	    System.String name = element.Name;

	    try {
		outputElementContent(element, buff);
	    } catch (System.IO.IOException e) {}
	    return buff.ToString();
	}
    }
}
