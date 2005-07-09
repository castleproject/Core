using System;
using System.Collections;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;
using NVelocity.Http.Context;
using NVelocity.Tool;
using NVelocity.Http.Tool;

namespace NVelocity.Http.Tool {

    /// <summary>
    /// A ToolboxManager for loading a toolbox from xml.
    ///
    /// <p>A toolbox manager is responsible for automatically filling the Velocity
    /// context with a set of view tools. This class provides the following
    /// features:</p>
    /// <ul>
    /// <li>configurable through an XML-based configuration file</li>
    /// <li>assembles a set of view tools (the toolbox) on request</li>
    /// <li>supports any class with a public constructor without parameters
    /// to be used as a view tool</li>
    /// <li>supports adding primitive data values to the context(String,Number,Boolean)</li>
    /// </ul>
    ///
    /// <p><strong>Configuration</strong></p>
    /// <p>The toolbox manager is configured through an XML-based configuration
    /// file. The configuration file is passed to the {@link #load(java.io.InputStream input)}
    /// method. The required format is shown in the following example:</p>
    /// <pre>
    /// &lt;?xml version="1.0"?&gt;
    ///
    /// &lt;toolbox&gt;
    /// &lt;tool&gt;
    /// &lt;key&gt;toolLoader&lt;/key&gt;
    /// &lt;class&gt;org.apache.velocity.tools.tools.ToolLoader&lt;/class&gt;
    /// &lt;/tool&gt;
    /// &lt;tool&gt;
    /// &lt;key&gt;math&lt;/key&gt;
    /// &lt;class&gt;org.apache.velocity.tools.tools.MathTool&lt;/class&gt;
    /// &lt;/tool&gt;
    /// &lt;data type="Number"&gt;
    /// &lt;key&gt;luckynumber&lt;/key&gt;
    /// &lt;value&gt;1.37&lt;/class&gt;
    /// &lt;/data&gt;
    /// &lt;data type="String"&gt;
    /// &lt;key&gt;greeting&lt;/key&gt;
    /// &lt;value&gt;Hello World!&lt;/class&gt;
    /// &lt;/data&gt;
    /// &lt;/toolbox&gt;
    /// </pre>
    /// </summary>
    /// <author> <a href="mailto:nathan@esha.com">Nathan Bubna</a></author>
    /// <author> <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a></author>
    public abstract class XMLToolboxManager : IToolboxManager {

	public const System.String BASE_NODE = "toolbox";
	public const System.String ELEMENT_TOOL = "tool";
	public const System.String ELEMENT_DATA = "data";
	public const System.String ELEMENT_KEY = "key";
	public const System.String ELEMENT_CLASS = "class";
	public const System.String ELEMENT_VALUE = "value";
	public const System.String ATTRIBUTE_TYPE = "type";

	private IList toolinfo;

	/// <summary>
	/// Default constructor
	/// </summary>
	public XMLToolboxManager() {
	    toolinfo = new ArrayList();
	}



	// ------------------------------- ToolboxManager interface ------------


	public virtual void AddTool(IToolInfo info) {
	    toolinfo.Add(info);
	}


	public virtual ToolboxContext getToolboxContext(System.Object initData) {
	    Hashtable toolbox = new Hashtable();

	    foreach(IToolInfo info in toolinfo) {
		toolbox[info.Key] = info.getInstance(initData);
	    }

	    return new ToolboxContext(toolbox);
	}



	// ------------------------------- toolbox loading methods ------------


	/// <summary>
	/// Default implementation logs messages to system out.
	/// </summary>
	protected internal virtual void log(System.String s) {
	    System.Console.Out.WriteLine("XMLToolboxManager - " + s);
	}


	/// <summary>
	/// Reads an XML document from an {@link InputStream}
	/// using <a href="http://dom4j.org">dom4j</a> and
	/// sets up the toolbox from that.
	///
	/// The DTD for toolbox schema is:
	/// <pre>
	/// &lt;?xml version="1.0"?&gt;
	/// &lt;!ELEMENT toolbox (tool*,data*)&gt;
	/// &lt;!ELEMENT tool    (key,class,#PCDATA)&gt;
	/// &lt;!ELEMENT data    (key,value)&gt;
	/// &lt;!ATTLIST data type (string|number|boolean) "string"&gt;
	/// &lt;!ELEMENT key     (#CDATA)&gt;
	/// &lt;!ELEMENT class   (#CDATA)&gt;
	/// &lt;!ELEMENT value   (#CDATA)&gt;
	/// </pre>
	/// </summary>
	/// <param name="input">the InputStream to read from</param>
	public virtual void  load(System.IO.Stream input) {
	    log("Loading toolbox...");
	    XmlDocument document = new XmlDocument();
	    document.Load(input);
	    XmlNodeList elements = document.SelectNodes("//" + BASE_NODE + "/*");

	    foreach(XmlElement e in elements) {
		System.String name = e.Name;

		IToolInfo info;

		if (name.ToUpper().Equals(ELEMENT_TOOL.ToUpper())) {
		    info = readToolInfo(e);
		} else if (name.ToUpper().Equals(ELEMENT_DATA.ToUpper())) {
		    info = readDataInfo(e);
		} else {
		    throw new XmlSchemaException("Unknown element: " + name, null);
		}

		AddTool(info);
		log("Added " + info.Classname + " as " + info.Key);
	    }

	    log("Toolbox loaded.");
	}


	protected internal virtual IToolInfo readToolInfo(XmlElement e) {
	    XmlNode n = e.SelectSingleNode(ELEMENT_KEY);
	    System.String key = n.InnerText;

	    n = e.SelectSingleNode(ELEMENT_CLASS);
	    System.String classname = n.InnerText;

	    return new ViewToolInfo(key, classname);
	}


	protected internal virtual IToolInfo readDataInfo(XmlElement e) {
	    XmlNode n = e.SelectSingleNode(ELEMENT_KEY);
	    System.String key = n.InnerText;

	    n = e.SelectSingleNode(ELEMENT_VALUE);
	    System.String value_Renamed = n.InnerText;

	    //System.String type = e.attributeValue(ATTRIBUTE_TYPE, DataInfo.TYPE_STRING);
	    System.String type = e.Attributes[DataInfo.TYPE_STRING].Value;

	    return new DataInfo(key, type, value_Renamed);
	}
    }
}
