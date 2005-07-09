namespace NVelocity.Runtime
{
	using System;
	using System.IO;
	using Commons.Collections;
	using NVelocity.Runtime.Parser.Node;
	using NVelocity.Runtime.Resource;
	using NVelocity.Util.Introspection;

	/// <summary>
	/// This is the Runtime system for Velocity. It is the
	/// single access point for all functionality in Velocity.
	/// It adheres to the mediator pattern and is the only
	/// structure that developers need to be familiar with
	/// in order to get Velocity to perform.
	/// 
	/// The Runtime will also cooperate with external
	/// systems like Turbine. Runtime properties can
	/// set and then the Runtime is initialized.
	/// 
	/// Turbine for example knows where the templates
	/// are to be loaded from, and where the velocity
	/// log file should be placed.
	/// 
	/// So in the case of Velocity cooperating with Turbine
	/// the code might look something like the following:
	/// 
	/// <pre>
	/// RuntimeSingleton.setProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, templatePath);
	/// RuntimeSingleton.setProperty(RuntimeConstants.RUNTIME_LOG, pathToVelocityLog);
	/// RuntimeSingleton.init();
	/// </pre>
	/// <pre>
	/// -----------------------------------------------------------------------
	/// N O T E S  O N  R U N T I M E  I N I T I A L I Z A T I O N
	/// -----------------------------------------------------------------------
	/// RuntimeSingleton.init()
	///
	/// If Runtime.init() is called by itself the Runtime will
	/// initialize with a set of default values.
	/// -----------------------------------------------------------------------
	/// RuntimeSingleton.init(String/Properties)
	/// 
	/// In this case the default velocity properties are layed down
	/// first to provide a solid base, then any properties provided
	/// in the given properties object will override the corresponding
	/// default property.
	/// -----------------------------------------------------------------------
	/// </pre>
	/// </summary>
	/// <author><a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
	/// <author><a href="mailto:jlb@houseofdistraction.com">Jeff Bowden</a></author>
	/// <author><a href="mailto:geirm@optonline.net">Geir Magusson Jr.</a></author>
	/// <author><a href="mailto:dlr@finemaltcoding.com">Daniel Rall</a></author>
	/// <version> $Id: RuntimeSingleton.cs,v 1.5 2004/12/27 05:59:46 corts Exp $
	///
	/// </version>
	public class RuntimeSingleton : RuntimeConstants
	{
		private static RuntimeInstance ri = new RuntimeInstance();

		/*
	* This is the primary initialization method in the Velocity
	* Runtime. The systems that are setup/initialized here are
	* as follows:
	* 
	* <ul>
	*   <li>Logging System</li>
	*   <li>ResourceManager</li>
	*   <li>Parser Pool</li>
	*   <li>Global Cache</li>
	*   <li>Static Content Include System</li>
	*   <li>Velocimacro System</li>
	* </ul>
	*/

		public static void init()
		{
			lock (typeof(RuntimeSingleton))
			{
				ri.init();
			}
		}


		/// <summary> Allows an external system to set a property in
		/// the Velocity Runtime.
		/// *
		/// </summary>
		/// <param name="String">property key
		/// </param>
		/// <param name="String">property value
		///
		/// </param>
		public static void setProperty(String key, Object value_)
		{
			ri.setProperty(key, value_);
		}

		/// <summary> Allow an external system to set an ExtendedProperties
		/// object to use. This is useful where the external
		/// system also uses the ExtendedProperties class and
		/// the velocity configuration is a subset of
		/// parent application's configuration. This is
		/// the case with Turbine.
		/// *
		/// </summary>
		/// <param name="ExtendedProperties">configuration
		///
		/// </param>
		/// <summary> Add a property to the configuration. If it already
		/// exists then the value stated here will be added
		/// to the configuration entry. For example, if
		/// *
		/// resource.loader = file
		/// *
		/// is already present in the configuration and you
		/// *
		/// addProperty("resource.loader", "classpath")
		/// *
		/// Then you will end up with a Vector like the
		/// following:
		/// *
		/// ["file", "classpath"]
		/// *
		/// </summary>
		/// <param name="String">key
		/// </param>
		/// <param name="String">value
		///
		/// </param>
		public static void addProperty(String key, Object value_)
		{
			ri.addProperty(key, value_);
		}

		/// <summary> Clear the values pertaining to a particular
		/// property.
		/// *
		/// </summary>
		/// <param name="String">key of property to clear
		///
		/// </param>
		public static void clearProperty(String key)
		{
			ri.clearProperty(key);
		}

		/// <summary>  Allows an external caller to get a property.  The calling
		/// routine is required to know the type, as this routine
		/// will return an Object, as that is what properties can be.
		/// *
		/// </summary>
		/// <param name="key">property to return
		///
		/// </param>
		public static Object getProperty(String key)
		{
			return ri.getProperty(key);
		}

		/// <summary> Initialize the Velocity Runtime with a Properties
		/// object.
		/// *
		/// </summary>
		/// <param name="">Properties
		///
		/// </param>
		public static void init(ExtendedProperties p)
		{
			ri.init(p);
		}

		/// <summary> Initialize the Velocity Runtime with the name of
		/// ExtendedProperties object.
		/// *
		/// </summary>
		/// <param name="">Properties
		///
		/// </param>
		public static void init(String configurationFile)
		{
			ri.init(configurationFile);
		}

		/// <summary> Returns a JavaCC generated Parser.
		/// *
		/// </summary>
		/// <returns>Parser javacc generated parser
		///
		/// </returns>
		private static Parser.Parser createNewParser()
		{
			return ri.createNewParser();
		}

		/// <summary> Parse the input and return the root of
		/// AST node structure.
		/// <br><br>
		/// In the event that it runs out of parsers in the
		/// pool, it will create and let them be GC'd
		/// dynamically, logging that it has to do that.  This
		/// is considered an exceptional condition.  It is
		/// expected that the user will set the
		/// PARSER_POOL_SIZE property appropriately for their
		/// application.  We will revisit this.
		/// *
		/// </summary>
		/// <param name="InputStream">inputstream retrieved by a resource loader
		/// </param>
		/// <param name="String">name of the template being parsed
		///
		/// </param>
		public static SimpleNode parse(TextReader reader, String templateName)
		{
			return ri.parse(reader, templateName);
		}

		/// <summary>  Parse the input and return the root of the AST node structure.
		/// *
		/// </summary>
		/// <param name="InputStream">inputstream retrieved by a resource loader
		/// </param>
		/// <param name="String">name of the template being parsed
		/// </param>
		/// <param name="dumpNamespace">flag to dump the Velocimacro namespace for this template
		///
		/// </param>
		public static SimpleNode parse(TextReader reader, String templateName, bool dumpNamespace)
		{
			return ri.parse(reader, templateName, dumpNamespace);
		}


		/// <summary> Returns a <code>Template</code> from the resource manager.
		/// This method assumes that the character encoding of the
		/// template is set by the <code>input.encoding</code>
		/// property.  The default is "ISO-8859-1"
		/// *
		/// </summary>
		/// <param name="name">The file name of the desired template.
		/// </param>
		/// <returns>    The template.
		/// @throws ResourceNotFoundException if template not found
		/// from any available source.
		/// @throws ParseErrorException if template cannot be parsed due
		/// to syntax (or other) error.
		/// @throws Exception if an error occurs in template initialization
		///
		/// </returns>
		public static Template getTemplate(String name)
		{
			return ri.getTemplate(name);
		}

		/// <summary> Returns a <code>Template</code> from the resource manager
		/// *
		/// </summary>
		/// <param name="name">The  name of the desired template.
		/// </param>
		/// <param name="encoding">Character encoding of the template
		/// </param>
		/// <returns>    The template.
		/// @throws ResourceNotFoundException if template not found
		/// from any available source.
		/// @throws ParseErrorException if template cannot be parsed due
		/// to syntax (or other) error.
		/// @throws Exception if an error occurs in template initialization
		///
		/// </returns>
		public static Template getTemplate(String name, String encoding)
		{
			return ri.getTemplate(name, encoding);
		}

		/// <summary> Returns a static content resource from the
		/// resource manager.  Uses the current value
		/// if INPUT_ENCODING as the character encoding.
		/// *
		/// </summary>
		/// <param name="name">Name of content resource to get
		/// </param>
		/// <returns>parsed ContentResource object ready for use
		/// @throws ResourceNotFoundException if template not found
		/// from any available source.
		///
		/// </returns>
		public static ContentResource getContent(String name)
		{
			return ri.getContent(name);
		}

		/// <summary> Returns a static content resource from the
		/// resource manager.
		/// *
		/// </summary>
		/// <param name="name">Name of content resource to get
		/// </param>
		/// <param name="encoding">Character encoding to use
		/// </param>
		/// <returns>parsed ContentResource object ready for use
		/// @throws ResourceNotFoundException if template not found
		/// from any available source.
		///
		/// </returns>
		public static ContentResource getContent(String name, String encoding)
		{
			return ri.getContent(name, encoding);
		}


		/// <summary>  Determines is a template exists, and returns name of the loader that
		/// provides it.  This is a slightly less hokey way to support
		/// the Velocity.templateExists() utility method, which was broken
		/// when per-template encoding was introduced.  We can revisit this.
		/// *
		/// </summary>
		/// <param name="resourceName">Name of template or content resource
		/// </param>
		/// <returns>class name of loader than can provide it
		///
		/// </returns>
		public static String getLoaderNameForResource(String resourceName)
		{
			return ri.getLoaderNameForResource(resourceName);
		}


		/// <summary> Log a warning message.
		/// *
		/// </summary>
		/// <param name="Object">message to log
		///
		/// </param>
		public static void warn(Object message)
		{
			ri.warn(message);
		}

		///
		/// <summary> Log an info message.
		/// *
		/// </summary>
		/// <param name="Object">message to log
		///
		/// </param>
		public static void info(Object message)
		{
			ri.info(message);
		}

		/// <summary> Log an error message.
		/// *
		/// </summary>
		/// <param name="Object">message to log
		///
		/// </param>
		public static void error(Object message)
		{
			ri.error(message);
		}

		/// <summary> Log a debug message.
		/// *
		/// </summary>
		/// <param name="Object">message to log
		///
		/// </param>
		public static void debug(Object message)
		{
			ri.debug(message);
		}

		/// <summary> String property accessor method with default to hide the
		/// configuration implementation.
		///
		/// </summary>
		/// <param name="String">key property key
		/// </param>
		/// <param name="String">defaultValue  default value to return if key not
		/// found in resource manager.
		/// </param>
		/// <returns>String  value of key or default
		///
		/// </returns>
		public static String getString(String key, String defaultValue)
		{
			return ri.getString(key, defaultValue);
		}

		/// <summary> Returns the appropriate VelocimacroProxy object if strVMname
		/// is a valid current Velocimacro.
		/// *
		/// </summary>
		/// <param name="String">vmName  Name of velocimacro requested
		/// </param>
		/// <returns>String VelocimacroProxy
		///
		/// </returns>
		public static Directive.Directive getVelocimacro(String vmName, String templateName)
		{
			return ri.getVelocimacro(vmName, templateName);
		}

		/// <summary> Adds a new Velocimacro. Usually called by Macro only while parsing.
		/// *
		/// </summary>
		/// <param name="String">name  Name of velocimacro
		/// </param>
		/// <param name="String">macro  String form of macro body
		/// </param>
		/// <param name="String">argArray  Array of strings, containing the
		/// #macro() arguments.  the 0th is the name.
		/// </param>
		/// <returns>boolean  True if added, false if rejected for some
		/// reason (either parameters or permission settings)
		///
		/// </returns>
		public static bool addVelocimacro(String name, String macro, String[] argArray, String sourceTemplate)
		{
			return ri.addVelocimacro(name, macro, argArray, sourceTemplate);
		}

		/// <summary>  Checks to see if a VM exists
		/// *
		/// </summary>
		/// <param name="name"> Name of velocimacro
		/// </param>
		/// <returns>boolean  True if VM by that name exists, false if not
		///
		/// </returns>
		public static bool isVelocimacro(String vmName, String templateName)
		{
			return ri.isVelocimacro(vmName, templateName);
		}

		/// <summary>  tells the vmFactory to dump the specified namespace.  This is to support
		/// clearing the VM list when in inline-VM-local-scope mode
		/// </summary>
		public static bool dumpVMNamespace(String namespace_Renamed)
		{
			return ri.dumpVMNamespace(namespace_Renamed);
		}

		/* --------------------------------------------------------------------
		* R U N T I M E  A C C E S S O R  M E T H O D S
		* --------------------------------------------------------------------
		* These are the getXXX() methods that are a simple wrapper
		* around the configuration object. This is an attempt
		* to make a the Velocity Runtime the single access point
		* for all things Velocity, and allow the Runtime to
		* adhere as closely as possible the the Mediator pattern
		* which is the ultimate goal.
		* --------------------------------------------------------------------
		*/

		/// <summary> String property accessor method to hide the configuration implementation
		/// </summary>
		/// <param name="key"> property key
		/// </param>
		/// <returns>  value of key or null
		///
		/// </returns>
		public static String getString(String key)
		{
			return ri.getString(key);
		}

		/// <summary> Int property accessor method to hide the configuration implementation.
		/// *
		/// </summary>
		/// <param name="String">key property key
		/// </param>
		/// <returns>int value
		///
		/// </returns>
		public static int getInt(String key)
		{
			return ri.getInt(key);
		}

		/// <summary> Int property accessor method to hide the configuration implementation.
		/// *
		/// </summary>
		/// <param name="key"> property key
		/// </param>
		/// <param name="int">default value
		/// </param>
		/// <returns>int  value
		///
		/// </returns>
		public static int getInt(String key, int defaultValue)
		{
			return ri.getInt(key, defaultValue);
		}

		/// <summary> Boolean property accessor method to hide the configuration implementation.
		///
		/// </summary>
		/// <param name="String">key  property key
		/// </param>
		/// <param name="boolean">default default value if property not found
		/// </param>
		/// <returns>boolean  value of key or default value
		///
		/// </returns>
		public static bool getBoolean(String key, bool def)
		{
			return ri.getBoolean(key, def);
		}


		/**
	 * @return The RuntimeInstance used by this wrapper.
	 */

		public static RuntimeServices RuntimeServices
		{
			get { return ri; }

		}

		/// <summary> Return the velocity runtime configuration object.
		/// *
		/// </summary>
		/// <returns>ExtendedProperties configuration object which houses
		/// the velocity runtime properties.
		///
		/// </returns>
		public static ExtendedProperties Configuration
		{
			get { return ri.Configuration; }

			set { ri.Configuration = value; }

		}

		/// <summary>  Return the Introspector for this RuntimeInstance
		/// *
		/// </summary>
		/// <returns>Introspector object for this runtime instance
		///
		/// </returns>
		public static Introspector Introspector
		{
			get { return ri.Introspector; }

		}

		/// <summary>  returns the RuntimeInstance object for this singleton
		/// For internal use only :)
		/// *
		/// </summary>
		/// <returns>RuntimeInstance the RuntimeInstance used by this Singleton
		/// instance
		///
		/// </returns>
		/// 
		[Obsolete("Use getRuntimeServices() instead")]
		public static RuntimeInstance RuntimeInstance
		{
			get { return ri; }
		}


		/**
	 * @see org.apache.velocity.runtime.RuntimeServices#getApplicationAttribute(Object)
	 */

		public static Object getApplicationAttribute(Object key)
		{
			return ri.getApplicationAttribute(key);
		}

		/**
	 * @see org.apache.velocity.runtime.RuntimeServices#getUberspect()
	 */
//	public static Uberspect Uberspect {
//	    get {return ri.getUberspect(); }
//	}

	}
}