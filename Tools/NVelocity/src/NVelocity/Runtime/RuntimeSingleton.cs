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
	public class RuntimeSingleton
	{
		private static RuntimeInstance ri = new RuntimeInstance();

		/// <summary>
		/// This is the primary initialization method in the Velocity
		/// Runtime. The systems that are setup/initialized here are
		/// as follows:
		/// <list type="">
		///		<item>Logging System</item>
		///		<item>ResourceManager</item>
		///		<item>Parser Pool</item>
		///		<item>Global Cache</item>
		///		<item>Static Content Include System</item>
		///		<item>Velocimacro System</item>
		/// </list>
		/// </summary>
		public static void Init()
		{
			lock (typeof(RuntimeSingleton))
			{
				ri.Init();
			}
		}


		/// <summary>
		/// Allows an external system to set a property in
		/// the Velocity Runtime.
		/// </summary>
		/// <param name="key">property key</param>
		/// <param name="value">property value</param>
		public static void SetProperty(String key, Object value)
		{
			ri.SetProperty(key, value);
		}

		/// <summary> Add a property to the configuration. If it already
		/// exists then the value stated here will be added
		/// to the configuration entry. For example, if
		/// 
		/// <code>resource.loader = file</code>
		/// 
		/// is already present in the configuration and you
		/// 
		/// <code>addProperty("resource.loader", "classpath")</code>
		/// 
		/// Then you will end up with an ArrayList like the
		/// following:
		/// 
		/// ["file", "classpath"]
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="value">value</param>
		public static void AddProperty(String key, Object value)
		{
			ri.AddProperty(key, value);
		}

		/// <summary>
		/// Clear the values pertaining to a particular property.
		/// </summary>
		/// <param name="key">key of property to clear</param>
		public static void ClearProperty(String key)
		{
			ri.ClearProperty(key);
		}

		/// <summary>
		/// Allows an external caller to get a property.  The calling
		/// routine is required to know the type, as this routine
		/// will return an Object, as that is what properties can be.
		/// </summary>
		/// <param name="key">property to return</param>
		public static Object GetProperty(String key)
		{
			return ri.GetProperty(key);
		}

		/// <summary>
		/// Initialize the Velocity Runtime with an ExtendedProperties object.
		/// </summary>
		/// <param name="p">Properties</param>
		public static void Init(ExtendedProperties p)
		{
			ri.Init(p);
		}

		/// <summary> Initialize the Velocity Runtime with a configuration file.</summary>
		/// <param name="configurationFile">configuration file</param>
		public static void Init(String configurationFile)
		{
			ri.Init(configurationFile);
		}

		/// <summary>
		/// Returns a JavaCC generated Parser.
		/// </summary>
		/// <returns>Parser javacc generated parser</returns>
		private static Parser.Parser CreateNewParser()
		{
			return ri.CreateNewParser();
		}

		/// <summary> Parse the input and return the root of
		/// AST node structure.
		/// </summary>
		/// <remarks>
		/// In the event that it runs out of parsers in the
		/// pool, it will create and let them be GC'd
		/// dynamically, logging that it has to do that.  This
		/// is considered an exceptional condition.  It is
		/// expected that the user will set the
		/// PARSER_POOL_SIZE property appropriately for their
		/// application.  We will revisit this.
		/// </remarks>
		/// <param name="reader">TextReader retrieved by a resource loader</param>
		/// <param name="templateName">name of the template being parsed</param>
		public static SimpleNode Parse(TextReader reader, String templateName)
		{
			return ri.Parse(reader, templateName);
		}

		/// <summary>
		/// Parse the input and return the root of the AST node structure.
		/// </summary>
		/// <param name="reader">TextReader retrieved by a resource loader</param>
		/// <param name="templateName">name of the template being parsed</param>
		/// <param name="dumpNamespace">flag to dump the Velocimacro namespace for this template</param>
		public static SimpleNode Parse(TextReader reader, String templateName, bool dumpNamespace)
		{
			return ri.Parse(reader, templateName, dumpNamespace);
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
		public static Template GetTemplate(String name)
		{
			return ri.GetTemplate(name);
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
		public static Template GetTemplate(String name, String encoding)
		{
			return ri.GetTemplate(name, encoding);
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
		public static ContentResource GetContent(String name)
		{
			return ri.GetContent(name);
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
		public static ContentResource GetContent(String name, String encoding)
		{
			return ri.GetContent(name, encoding);
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
		public static String GetLoaderNameForResource(String resourceName)
		{
			return ri.GetLoaderNameForResource(resourceName);
		}


		/// <summary>
		/// Log a warning message.
		/// </summary>
		/// <param name="message">message to log</param>
		public static void Warn(Object message)
		{
			ri.Warn(message);
		}

		///
		/// <summary>
		/// Log an info message.
		/// </summary>
		/// <param name="message">message to log</param>
		public static void Info(Object message)
		{
			ri.Info(message);
		}

		/// <summary>
		/// Log an error message.
		/// </summary>
		/// <param name="message">message to log</param>
		public static void Error(Object message)
		{
			ri.Error(message);
		}

		/// <summary>
		/// Log a debug message.
		/// </summary>
		/// <param name="message">message to log</param>
		public static void Debug(Object message)
		{
			ri.Debug(message);
		}

		/// <summary> String property accessor method with default to hide the
		/// configuration implementation.
		///
		/// </summary>
		/// <param name="key">property key
		/// </param>
		/// <param name="defaultValue">default value to return if key not
		/// found in resource manager.
		/// </param>
		/// <returns>String  value of key or default
		///
		/// </returns>
		public static String getString(String key, String defaultValue)
		{
			return ri.GetString(key, defaultValue);
		}

		/// <summary>
		/// Returns the appropriate VelocimacroProxy object if vmName
		/// is a valid current Velocimacro.
		/// </summary>
		/// <param name="vmName">Name of velocimacro requested</param>
		/// <param name="templateName">Template Name</param>
		/// <returns>VelocimacroProxy</returns>
		public static Directive.Directive GetVelocimacro(String vmName, String templateName)
		{
			return ri.GetVelocimacro(vmName, templateName);
		}

		/// <summary>
		/// Adds a new Velocimacro. Usually called by Macro only while parsing.
		/// </summary>
		/// <param name="name">Name of velocimacro</param>
		/// <param name="macro">String form of macro body</param>
		/// <param name="argArray">Array of strings, containing the 
		/// <code>#macro()</code> arguments. The 0th is the name.
		/// </param>
		/// <param name="sourceTemplate">Source template</param>
		/// <returns>True if added, false if rejected for some
		/// reason (either parameters or permission settings)
		/// </returns>
		public static bool AddVelocimacro(String name, String macro, String[] argArray, String sourceTemplate)
		{
			return ri.AddVelocimacro(name, macro, argArray, sourceTemplate);
		}

		/// <summary>
		/// Checks to see if a VM exists
		/// </summary>
		/// <param name="vmName">Name of velocimacro</param>
		/// <param name="templateName">Template Name</param>
		/// <returns>True if VM by that name exists, false if not</returns>
		public static bool IsVelocimacro(String vmName, String templateName)
		{
			return ri.IsVelocimacro(vmName, templateName);
		}

		/// <summary>
		/// Tells the vmFactory to dump the specified namespace.
		/// This is to support clearing the VM list when in 
		/// inline-VM-local-scope mode
		/// </summary>
		public static bool DumpVMNamespace(String namespace_Renamed)
		{
			return ri.DumpVMNamespace(namespace_Renamed);
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

		/// <summary>
		/// String property accessor method to hide the configuration implementation.
		/// </summary>
		/// <param name="key">property key</param>
		/// <returns>Value of key or <c>null</c></returns>
		public static String GetString(String key)
		{
			return ri.GetString(key);
		}

		/// <summary>
		/// Int property accessor method to hide the configuration implementation.
		/// </summary>
		/// <param name="key">property key</param>
		/// <returns>value</returns>
		public static int GetInt(String key)
		{
			return ri.GetInt(key);
		}

		/// <summary>
		/// Int property accessor method to hide the configuration implementation.
		/// </summary>
		/// <param name="key">property key</param>
		/// <param name="defaultValue">default value</param>
		/// <returns>value</returns>
		public static int GetInt(String key, int defaultValue)
		{
			return ri.GetInt(key, defaultValue);
		}

		/// <summary>
		/// Boolean property accessor method to hide the configuration implementation.
		/// </summary>
		/// <param name="key">property key</param>
		/// <param name="def">default value if property not found</param>
		/// <returns>value of key or default value</returns>
		public static bool GetBoolean(String key, bool def)
		{
			return ri.GetBoolean(key, def);
		}

		public static IRuntimeServices RuntimeServices
		{
			get { return ri; }
		}

		/// <summary>
		/// Return the velocity runtime configuration object.
		/// </summary>
		/// <returns>
		/// ExtendedProperties configuration object which houses
		/// the velocity runtime properties.
		/// </returns>
		public static ExtendedProperties Configuration
		{
			get { return ri.Configuration; }
			set { ri.Configuration = value; }
		}

		/// <summary>
		/// Return the Introspector for this RuntimeInstance
		/// </summary>
		/// <returns>
		/// Introspector object for this runtime instance
		/// </returns>
		public static Introspector Introspector
		{
			get { return ri.Introspector; }

		}

		/// <summary>
		/// Returns the RuntimeInstance object for this singleton.
		/// For internal use only.
		/// </summary>
		/// <returns>
		/// The <see cref="RuntimeInstance"/> used by this Singleton instance.
		/// </returns>
		[Obsolete("Use the RuntimeServices property instead")]
		public static RuntimeInstance RuntimeInstance
		{
			get { return ri; }
		}


		/// <summary>
		/// <seealso cref="IRuntimeServices.GetApplicationAttribute"/>
		/// </summary>
		/// <param name="key">key</param>
		/// <returns>value</returns>
		public static Object GetApplicationAttribute(Object key)
		{
			return ri.GetApplicationAttribute(key);
		}
	}
}
