using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Schema;

using NVelocity.App;
using NVelocity.Context;

namespace NVelocity.Dvsl {

    /// <summary>  <p>
    /// Class responsible for actual transformation
    /// of documents.
    /// </p>
    /// <p>
    /// Note that this class is <em>not</em> threadsafe.
    /// </p>
    /// </summary>
    /// <author> <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a></author>
    class Transformer : TransformTool {

	/// <summary>
	/// Instance of VelocityEngine we are currently using.
	/// This must be reset with a stylesheeet change
	/// </summary>
	private VelocityEngine ve = null;

	/// <summary>
	/// basic context passed to us - can contain tools
	/// and such for use.  Is protected from change via
	/// wrapping
	/// </summary>
	private IContext baseContext;

	/// <summary>
	/// context used during processing. Wraps the baseContext
	/// </summary>
	private DVSLNodeContext currentContext;

	private TemplateHandler templateHandler = null;

	/// <summary>
	/// HashMap to hold application values
	/// </summary>
	private Hashtable appValue = new Hashtable();

	/// <summary>
	/// Sole public CTOR.  We rely on the caller to give us a
	/// VelocityEngine ready with all macros registered.
	/// The context is the callers context with all tools and
	/// style drek.
	/// </summary>
	public Transformer(VelocityEngine ve, TemplateHandler th, IContext context, Hashtable applicationValues, bool validate) {
	    this.ve = ve;
	    this.baseContext = context;
	    this.templateHandler = th;

	    appValue = applicationValues;
	}

	/// <summary>
	/// Method that performs the transformation on
	/// a document
	/// </summary>
	/// <param name="reader">XML document char stream</param>
	/// <param name="writer">Writer to output transformation to</param>
	internal virtual long Transform(TextReader reader, TextWriter writer) {
	    /*
	    *  parse the document
	    */
	    XmlDocument document = new XmlDocument();
	    document.Load(reader);

	    return Transform(document, writer);
	}

	internal virtual long Transform(XmlDocument dom4jdoc, TextWriter writer) {
	    /*
	    *  wrap the document.  We do this as we let the dom4j package
	    *  decide if we have a match against "/", so we need document
	    *  to do that
	    */
	    DvslNode root = new DvslNodeImpl(dom4jdoc);
	    return Transform(root, writer);
	}

	protected internal virtual long Transform(DvslNode root, TextWriter writer) {
	    /*
	    *  wrap in a context to keep subsequent documents from
	    *  interacting with each other
	    */
	    currentContext = new DVSLNodeContext(baseContext);
	    long start = (DateTime.Now.Ticks - 621355968000000000) / 10000;

	    /*
	    *  push 'this' into the context as our TransformTool
	    *  and invoke the transformation
	    */
	    currentContext.Put("context", this);
	    Invoke(root, writer);
	    long end = (DateTime.Now.Ticks - 621355968000000000) / 10000;
	    return end - start;
	}


	private void Invoke(DvslNode element, TextWriter writer) {
	    String[] arr = new String[]{};
	    currentContext.PushNode(element);
	    templateHandler.Render(element, currentContext, writer);
	    currentContext.PopNode();
	}

	public virtual Object Get(String key) {
	    return currentContext.Get(key);
	}

	public virtual String ApplyTemplates(DvslNode node, String xpath) {
	    /*
	    *  get the nodes that was asked for
	    */
	    IList nodeset = node.SelectNodes(xpath);
	    StringWriter sw = new StringWriter();

	    for (int i = 0; i < nodeset.Count; i++) {
		DvslNode n = (DvslNode) nodeset[i];
		Invoke(n, sw);
	    }
	    return sw.ToString();
	}

	public virtual String ApplyTemplates(DvslNode node) {
	    StringWriter sw = new StringWriter();
	    Invoke(node, sw);
	    return sw.ToString();
	}

	public virtual String ApplyTemplates() {
	    return ApplyTemplates(currentContext.PeekNode(), "*|@*|text()|comment()|processing-instruction()");
	}

	public virtual String ApplyTemplates(String path) {
	    DvslNode node = currentContext.PeekNode();
	    return ApplyTemplates(node, path);
	}

	public virtual String Copy() {
	    /*
	    *  fakie, for now
	    */
	    DvslNode node = currentContext.PeekNode();
	    return node.Copy();
	}

	public virtual Object GetAppValue(Object key) {
	    return appValue[key];
	}

	public virtual Object PutAppValue(Object key, Object value) {
	    return appValue[key] = value;
	}


    }
}
