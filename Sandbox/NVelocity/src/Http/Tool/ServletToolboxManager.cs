using System;
using System.Collections;
using System.Xml;
using System.Web;
using System.Web.SessionState;

using NVelocity.Http.Context;
using NVelocity.Tool;

namespace NVelocity.Http.Tool {

    /// <summary>
    /// <p>A toolbox manager for the servlet environment.</p>
    /// <p>A toolbox manager is responsible for automatically filling the Velocity
    /// context with a set of view tools. This class provides the following
    /// features:</p>
    /// <ul>
    /// <li>configurable through an XML-based configuration file</li>
    /// <li>assembles a set of view tools (the toolbox) on request</li>
    /// <li>handles different tool scopes (request, session, application)</li>
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
    /// &lt;scope&gt;application&lt;/scope&gt;
    /// &lt;class&gt;org.apache.velocity.tools.tools.ToolLoader&lt;/class&gt;
    /// &lt;/tool&gt;
    /// &lt;tool&gt;
    /// &lt;key&gt;math&lt;/key&gt;
    /// &lt;scope&gt;application&lt;/scope&gt;
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
    /// <p>The recommended location for the configuration file is the WEB-INF directory of the
    /// web application.
    /// </summary>
    /// <author> <a href="mailto:sidler@teamup.com">Gabriel Sidler</a>
    /// </author>
    /// <author> <a href="mailto:nathan@esha.com">Nathan Bubna</a>
    /// </author>
    /// <author> <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a>
    /// </author>
    public class ServletToolboxManager:XMLToolboxManager {

	// --------------------------------------------------- Properties ---------

	public const String ELEMENT_SCOPE = "scope";

	public const String SESSION_TOOLS_KEY = "NVelocity.Http.Tools.ServletToolboxManager.SessionTools";

	private HttpContext httpContext;
	private Hashtable appTools;
	private ArrayList sessionToolInfo;
	private ArrayList requestToolInfo;



	// --------------------------------------------------- Constructor --------

	/// <summary>
	/// Default constructor
	/// </summary>
	public ServletToolboxManager(HttpContext httpContext) {
	    this.httpContext = httpContext;
	    appTools = new Hashtable();
	    sessionToolInfo = new ArrayList();
	    requestToolInfo = new ArrayList();
	}



	// --------------------------------------------------- Methods ------------

	/// <summary>
	/// Overrides XMLToolboxManager to log to the servlet context
	/// </summary>
	protected internal override void log(String s) {
	    NVelocity.App.Velocity.Info("ServletToolboxManager: " + s);
	    //servletContext.log("ServletToolboxManager: " + s);
	}


	/// <summary>
	/// Overrides XMLToolboxManager to read a {@link ServletToolInfo}
	/// instead of a {@link org.apache.velocity.tools.view.ViewToolInfo}.
	/// </summary>
	protected internal override IToolInfo readToolInfo(XmlElement e) {
	    XmlNode n = e.SelectSingleNode(ELEMENT_KEY);
	    String key = n.InnerText;

	    n = e.SelectSingleNode(ELEMENT_CLASS);
	    String classname = n.InnerText;

	    String scope = ServletToolInfo.REQUEST_SCOPE;
	    n = e.SelectSingleNode(ELEMENT_SCOPE);
	    if (n != null) {
		scope = n.InnerText;
	    }

	    return new ServletToolInfo(key, classname, scope);
	}


	/// <summary>
	/// Overrides XMLToolboxManager to separate tools by scope.
	/// For this to work, we obviously override getToolboxContext as well.
	/// </summary>
	public override void AddTool(IToolInfo info) {
	    if (info is DataInfo) {
		//add static data to the appTools map
		appTools[info.Key] = info.getInstance(null);
	    } else if (info is ServletToolInfo) {
		ServletToolInfo stInfo = (ServletToolInfo) info;

		if (stInfo.Scope.ToUpper().Equals(ServletToolInfo.REQUEST_SCOPE.ToUpper())) {
		    requestToolInfo.Add(stInfo);
		} else if (stInfo.Scope.ToUpper().Equals(ServletToolInfo.SESSION_SCOPE.ToUpper())) {
		    sessionToolInfo.Add(stInfo);
		} else if (stInfo.Scope.ToUpper().Equals(ServletToolInfo.APPLICATION_SCOPE.ToUpper())) {
		    //add application scoped tools to appTools and
		    //initialize them with the ServletContext
		    appTools[stInfo.Key] = stInfo.getInstance(httpContext);
		} else {
		    log("Unknown scope: " + stInfo.Scope + " " + stInfo.Key + " will be request scoped.");
		    requestToolInfo.Add(stInfo);
		}
	    } else {
		//default is request scope
		requestToolInfo.Add(info);
	    }
	}


	/// <summary>
	/// Overrides XMLToolboxManager to handle the separate
	/// scopes.
	///
	/// Application scope tools were initialized when the toolbox was loaded.
	/// Session scope tools are initialized once per session and stored in a
	/// map in the session attributes.
	/// Request scope tools are initialized on every request.
	/// </summary>
	public override ToolboxContext getToolboxContext(Object initData) {
	    //we know the initData is a ViewContext
	    IViewContext ctx = (IViewContext)initData;

	    //create the toolbox map with the application tools in it
	    Hashtable toolbox = new Hashtable(appTools);

	    if (sessionToolInfo.Count>0) {
		//UPGRADE_TODO: Interface javax.servlet.http.HttpSession was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1095"'
		//UPGRADE_TODO: Method javax.servlet.http.HttpServletRequest.getSession was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1095"'
		HttpSessionState session = ctx.HttpContext.Session;

		//synchronize session tool initialization to avoid potential
		//conflicts from multiple simultaneous requests in the same session
		lock(session) {
		    //get the initialized session tools
		    Hashtable stmap = (Hashtable) session[SESSION_TOOLS_KEY];

		    //if session tools aren't initialized,
		    //do so and store them in the session
		    if (stmap == null) {
			stmap = new Hashtable(sessionToolInfo.Count);
			foreach(IToolInfo info in sessionToolInfo) {
			    stmap[info.Key] = info.getInstance(ctx);
			}
			session[SESSION_TOOLS_KEY] = stmap;
		    }

		    //add the initialized session tools to the toolbox
		    IDictionaryEnumerator e = stmap.GetEnumerator();
		    while(e.MoveNext()) {
			toolbox.Add(e.Key, e.Value);
		    }
		}
	    }

	    //add and initialize request tools
	    foreach(IToolInfo info in requestToolInfo) {
		toolbox[info.Key] = info.getInstance(ctx);
	    }

	    return new ToolboxContext(toolbox);
	}
    }
}
