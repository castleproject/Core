using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.SessionState;

using Commons.Collections;

using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using NVelocity.IO;
using NVelocity.Runtime;
using NVelocity.Util;
using NVelocity.Http.Resource.Loader;
using NVelocity.Http.Tool;

namespace NVelocity.Http {

    /// <summary>
    /// Summary description for NVelocityHandlerSkeleton.
    /// </summary>
    public abstract class HandlerSkeleton {

	/// <summary>
	/// Key used to access the toolbox configuration file path from the
	/// Servlet init parameters.
	/// </summary>
	public const System.String INIT_TOOLBOX_KEY = "nvelocity.toolbox";

	/// <summary>
	/// Key used to access the Velocity configuration file path from the
	/// Servlet init parameters. This is the string that is looked for when
	/// getInitParameter is called.
	/// </summary>
	public const System.String INIT_PROPERTIES_KEY = "nvelocity.properties";

	/// <summary>
	/// has the NVelocity initialization taken place
	/// </summary>
	protected static Boolean initialized = false;

	/// <summary>
	/// A reference to the toolbox manager.
	/// </summary>
	protected internal ServletToolboxManager toolboxManager = null;

	/// <summary>
	/// Initializes the Velocity runtime, first calling
	/// loadConfiguration(ServletConvig) to get a
	/// java.util.Properties of configuration information
	/// and then calling Velocity.init().  Override this
	/// to do anything to the environment before the
	/// initialization of the singelton takes place, or to
	/// initialize the singleton in other ways.
	/// </summary>
	protected virtual void InitVelocity() {
	    try {
		// call the overridable method to allow the
		// derived classes a shot at altering the configuration
		// before initializing Runtime
		ExtendedProperties props = LoadConfiguration();
		Velocity.Init(props);
	    } catch( System.Exception e ) {
		throw new System.Exception("Error initializing Velocity: " + e);
	    }

	    return;
	}


	/// <summary>
	/// Loads the configuration information and returns that
	/// information as a ExtendedProperties, which will be used to
	/// initialize the Velocity runtime.
	/// <br><br>
	/// Currently, this method gets the initialization parameter
	/// VelocityServlet.INIT_PROPS_KEY, which should be a file containing
	/// the configuration information.
	/// <br><br>
	/// To configure your Servlet Spec 2.2 compliant servlet runner to pass
	/// this to you, put the following in your WEB-INF/web.xml file
	/// <br>
	/// <pre>
	///   &lt;servlet&gt;
	///     &lt;servlet-name&gt; YourServlet &lt/servlet-name&gt;
	///     &lt;servlet-class&gt; your.package.YourServlet &lt;/servlet-class&gt;
	///     &lt;init-param&gt;
	///        &lt;param-name&gt; properties &lt;/param-name&gt;
	///        &lt;param-value&gt; velocity.properties &lt;/param-value&gt;
	///     &lt;/init-param&gt;
	///   &lt;/servlet&gt;
	///  </pre>
	///
	/// Alternately, if you wish to configure an entire context in this
	/// fashion, you may use the following:
	///  <br>
	///  <pre>
	///    &lt;context-param&gt;
	///       &lt;param-name&gt; properties &lt;/param-name&gt;
	///       &lt;param-value&gt; velocity.properties &lt;/param-value&gt;
	///       &lt;description&gt; Path to Velocity configuration &lt;/description&gt;
	///    &lt;/context-param&gt;
	///   </pre>
	///
	/// Derived classes may do the same, or take advantage of this code to do the loading for them via :
	///  <pre>
	///     Properties p = super.loadConfiguration( config );
	///  </pre>
	/// and then add or modify the configuration values from the file.
	/// <br>
	/// </summary>
	/// <returns>ExtendedProperties</returns>
	protected virtual ExtendedProperties LoadConfiguration() {
	    String propsFile = System.Configuration.ConfigurationSettings.AppSettings[INIT_PROPERTIES_KEY];

	    // This will attempt to find the location of the properties
	    // file from the relative path to the WAR archive (ie:
	    // docroot). Since JServ returns null for getRealPath()
	    // because it was never implemented correctly, then we know we
	    // will not have an issue with using it this way. I don't know
	    // if this will break other servlet engines, but it probably
	    // shouldn't since WAR files are the future anyways.
	    ExtendedProperties p = new ExtendedProperties();

	    if (propsFile != null) {
		FileInfo file = new FileInfo(HttpContext.Current.Request.PhysicalApplicationPath + propsFile);
		FileStream fs = file.OpenRead();
		p.Load(fs);
		fs.Close();
	    }

	    return p;
	}


	protected virtual void LoadToolbox() {
	    // setup the toolbox if there is one
	    String key = System.Configuration.ConfigurationSettings.AppSettings[INIT_TOOLBOX_KEY];

	    if (key != null) {
		Stream fs = null;

		try {
		    // little fix up
		    if (!key.StartsWith("/")) {
			key = "/" + key;
		    }

		    // get the bits
		    FileInfo file = new FileInfo(HttpContext.Current.Request.PhysicalApplicationPath + key);
		    fs = file.OpenRead();

		    if (fs != null) {
			Velocity.Info("Using toolbox configuration file '" + key + "'");

			toolboxManager = new ServletToolboxManager(HttpContext.Current);
			toolboxManager.load(fs);

			Velocity.Info("Toolbox setup complete.");
		    }
		} catch (System.Exception e) {
		    Velocity.Error("Problem reading toolbox file properties file '" + key + "' : " + e);
		} finally {
		    try {
			if (fs != null)
			    fs.Close();
		    } catch (System.Exception) {}}
	    } else {
		Velocity.Info("No toolbox entry in configuration.");
	    }
	}


    }
}
