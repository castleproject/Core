namespace NVelocity.Runtime
{
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using Commons.Collections;
	using NVelocity.Runtime.Directive;
	using NVelocity.Runtime.Log;
	using NVelocity.Runtime.Parser.Node;
	using NVelocity.Runtime.Resource;
	using NVelocity.Util;
	using NVelocity.Util.Introspection;

	/// <summary> 
	/// This is the Runtime system for Velocity. It is the
	/// single access point for all functionality in Velocity.
	/// It adheres to the mediator pattern and is the only
	/// structure that developers need to be familiar with
	/// in order to get Velocity to perform.
	/// *
	/// The Runtime will also cooperate with external
	/// systems like Turbine. Runtime properties can
	/// set and then the Runtime is initialized.
	/// *
	/// Turbine for example knows where the templates
	/// are to be loaded from, and where the velocity
	/// log file should be placed.
	/// *
	/// So in the case of Velocity cooperating with Turbine
	/// the code might look something like the following:
	/// *
	/// <pre>
	/// Runtime.setProperty(Runtime.FILE_RESOURCE_LOADER_PATH, templatePath);
	/// Runtime.setProperty(Runtime.RUNTIME_LOG, pathToVelocityLog);
	/// Runtime.init();
	/// </pre>
	/// *
	/// <pre>
	/// -----------------------------------------------------------------------
	/// N O T E S  O N  R U N T I M E  I N I T I A L I Z A T I O N
	/// -----------------------------------------------------------------------
	/// Runtime.init()
	///
	/// If Runtime.init() is called by itself the Runtime will
	/// initialize with a set of default values.
	/// -----------------------------------------------------------------------
	/// Runtime.init(String/Properties)
	/// *
	/// In this case the default velocity properties are layed down
	/// first to provide a solid base, then any properties provided
	/// in the given properties object will override the corresponding
	/// default property.
	/// -----------------------------------------------------------------------
	/// </pre>
	/// *
	/// </summary>
	public class RuntimeInstance : RuntimeConstants, RuntimeServices
	{
		private DefaultTraceListener debugOutput = new DefaultTraceListener();

		private void InitBlock()
		{
			logSystem = new PrimordialLogSystem();
			configuration = new ExtendedProperties();
		}

		public ExtendedProperties Configuration
		{
			get { return configuration; }

			set
			{
				if (overridingProperties == null)
				{
					overridingProperties = value;
				}
				else
				{
					// Avoid possible ConcurrentModificationException
					if (overridingProperties != value)
					{
						overridingProperties.Combine(value);
					}
				}
			}

		}

		public Introspector Introspector
		{
			get { return introspector; }

		}

		/// <summary>  VelocimacroFactory object to manage VMs
		/// </summary>
		private VelocimacroFactory vmFactory = null;

		///
		/// <summary>  The Runtime logger.  We start with an instance of
		/// a 'primordial logger', which just collects log messages
		/// then, when the log system is initialized, we dump
		/// all messages out of the primordial one into the real one.
		/// </summary>
		private LogSystem logSystem;

		///
		/// <summary> The Runtime parser pool
		/// </summary>
		private SimplePool parserPool;

		///
		/// <summary> Indicate whether the Runtime has been fully initialized.
		/// </summary>
		private bool initialized;

		/// <summary> These are the properties that are laid down over top
		/// of the default properties when requested.
		/// </summary>
		private ExtendedProperties overridingProperties = null;

		/// <summary> This is a hashtable of initialized directives.
		/// The directives that populate this hashtable are
		/// taken from the RUNTIME_DEFAULT_DIRECTIVES
		/// property file. This hashtable is passed
		/// to each parser that is created.
		/// </summary>
		// private Hashtable runtimeDirectives;

		/// <summary> Object that houses the configuration options for
		/// the velocity runtime. The ExtendedProperties object allows
		/// the convenient retrieval of a subset of properties.
		/// For example all the properties for a resource loader
		/// can be retrieved from the main ExtendedProperties object
		/// using something like the following:
		/// *
		/// ExtendedProperties loaderConfiguration =
		/// configuration.subset(loaderID);
		/// *
		/// And a configuration is a lot more convenient to deal
		/// with then conventional properties objects, or Maps.
		/// </summary>
		//UPGRADE_NOTE: The initialization of  'configuration' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
		private ExtendedProperties configuration;

		private ResourceManager resourceManager = null;

		/*
		*  Each runtime instance has it's own introspector
		*  to ensure that each instance is completely separate.
		*/
		private Introspector introspector = null;


		/*
		*  Opaque reference to something specificed by the 
		*  application for use in application supplied/specified
		*  pluggable components
		*/
		private Hashtable applicationAttributes = null;

		private Uberspect uberSpect;

		private IDirectiveManager directiveManager;

		public RuntimeInstance()
		{
			InitBlock();
			/*
	    *  create a VM factory, resource manager
	    *  and introspector
	    */

			vmFactory = new VelocimacroFactory(this);

			/*
	    *  make a new introspector and initialize it
	    */

			introspector = new Introspector(this);

			/*
	    * and a store for the application attributes
	    */

			applicationAttributes = new Hashtable();
		}

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

		public void init()
		{
			lock (this)
			{
				if (initialized == false)
				{
					info("************************************************************** ");
					info("Starting " + Assembly.GetExecutingAssembly().GetName());
					info("RuntimeInstance initializing.");
					initializeProperties();
					initializeLogger();
					initializeResourceManager();
					initializeDirectives();
					initializeParserPool();
					initializeIntrospection();

					/*
		    *  initialize the VM Factory.  It will use the properties 
		    * accessable from Runtime, so keep this here at the end.
		    */
					vmFactory.initVelocimacro();

					info("NVelocity successfully started.");

					initialized = true;
				}
			}
		}

		/// <summary>  Gets the classname for the Uberspect introspection package and
		/// instantiates an instance.
		/// </summary>
		private void initializeIntrospection()
		{
			String rm = getString(RuntimeConstants_Fields.UBERSPECT_CLASSNAME);

			if (rm != null && rm.Length > 0)
			{
				Object o = null;

				//UPGRADE_NOTE: Exception 'java.lang.ClassNotFoundException' was converted to 'System.Exception' which has different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1100"'
				try
				{
					//UPGRADE_TODO: Format of parameters of method 'java.lang.Class.forName' are different in the equivalent in .NET. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1092"'
					o = SupportClass.CreateNewInstance(Type.GetType(rm));
				}
				catch (System.Exception)
				{
					String err = "The specified class for Uberspect (" + rm + ") does not exist (or is not accessible to the current classlaoder.";
					error(err);
					throw new System.Exception(err);
				}

				if (!(o is Uberspect))
				{
					String err = "The specified class for Uberspect (" + rm + ") does not implement org.apache.velocity.util.introspector.Uberspect." + " Velocity not initialized correctly.";

					error(err);
					throw new System.Exception(err);
				}

				uberSpect = (Uberspect) o;

				if (uberSpect is UberspectLoggable)
				{
					((UberspectLoggable) uberSpect).RuntimeLogger = this;
				}

				uberSpect.init();
			}
			else
			{
				/*
		*  someone screwed up.  Lets not fool around...
		*/

				String err = "It appears that no class was specified as the" + " Uberspect.  Please ensure that all configuration" + " information is correct.";

				error(err);
				throw new System.Exception(err);
			}
		}

		/// <summary> Initializes the Velocity Runtime with properties file.
		/// The properties file may be in the file system proper,
		/// or the properties file may be in the classpath.
		/// </summary>
		private void setDefaultProperties()
		{
			try
			{
				// TODO: this was modified in v1.4 to use the classloader
				configuration.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream(RuntimeConstants_Fields.DEFAULT_RUNTIME_PROPERTIES));
			}
			catch (System.Exception ex)
			{
				debugOutput.WriteLine("Cannot get NVelocity Runtime default properties!\n" + ex.Message);
				debugOutput.Flush();
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
		public void setProperty(String key, Object value_)
		{
			if (overridingProperties == null)
			{
				overridingProperties = new ExtendedProperties();
			}

			overridingProperties.SetProperty(key, value_);
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
		public void addProperty(String key, Object value_)
		{
			if (overridingProperties == null)
			{
				overridingProperties = new ExtendedProperties();
			}

			overridingProperties.AddProperty(key, value_);
		}

		/// <summary> Clear the values pertaining to a particular
		/// property.
		/// *
		/// </summary>
		/// <param name="String">key of property to clear
		///
		/// </param>
		public void clearProperty(String key)
		{
			if (overridingProperties != null)
			{
				overridingProperties.ClearProperty(key);
			}
		}

		/// <summary>  Allows an external caller to get a property.  The calling
		/// routine is required to know the type, as this routine
		/// will return an Object, as that is what properties can be.
		/// *
		/// </summary>
		/// <param name="key">property to return
		///
		/// </param>
		public Object getProperty(String key)
		{
			return configuration.GetProperty(key);
		}

		/// <summary> Initialize Velocity properties, if the default
		/// properties have not been laid down first then
		/// do so. Then proceed to process any overriding
		/// properties. Laying down the default properties
		/// gives a much greater chance of having a
		/// working system.
		/// </summary>
		private void initializeProperties()
		{
			/*
	    * Always lay down the default properties first as
	    * to provide a solid base.
	    */
			if (configuration.IsInitialized() == false)
			{
				setDefaultProperties();
			}

			if (overridingProperties != null)
			{
				configuration.Combine(overridingProperties);
			}
		}

		/// <summary> Initialize the Velocity Runtime with a Properties
		/// object.
		/// *
		/// </summary>
		/// <param name="">Properties
		///
		/// </param>
		public void init(ExtendedProperties p)
		{
			overridingProperties = ExtendedProperties.ConvertProperties(p);
			init();
		}

		/// <summary> Initialize the Velocity Runtime with the name of
		/// ExtendedProperties object.
		/// *
		/// </summary>
		/// <param name="">Properties
		///
		/// </param>
		public void init(String configurationFile)
		{
			overridingProperties = new ExtendedProperties(configurationFile);
			init();
		}

		private void initializeResourceManager()
		{
			/*
			 * Which resource manager?
			 */

			String rm = getString(RuntimeConstants_Fields.RESOURCE_MANAGER_CLASS);

			if (rm != null && rm.Length > 0)
			{
				/*
				 *  if something was specified, then make one.
				 *  if that isn't a ResourceManager, consider
				 *  this a huge error and throw
				 */

				Object o = null;

				try
				{
					Type rmType = Type.GetType(rm);
					o = Activator.CreateInstance(rmType);
				}
				catch (System.Exception)
				{
					String err = "The specified class for Resourcemanager (" + rm + ") does not exist.";
					error(err);
					throw new System.Exception(err);
				}

				if (!(o is ResourceManager))
				{
					String err = "The specified class for ResourceManager (" + rm + ") does not implement ResourceManager." + " Velocity not initialized correctly.";
					error(err);
					throw new System.Exception(err);
				}

				resourceManager = (ResourceManager) o;

				resourceManager.initialize(this);
			}
			else
			{
				/*
				 *  someone screwed up.  Lets not fool around...
				 */

				String err = "It appears that no class was specified as the" + " ResourceManager.  Please ensure that all configuration" + " information is correct.";
				error(err);
				throw new System.Exception(err);
			}
		}

		/// <summary> Initialize the Velocity logging system.
		/// *
		/// @throws Exception
		/// </summary>
		private void initializeLogger()
		{
			/*
			 * Initialize the logger. We will eventually move all
			 * logging into the logging manager.
			 */

			if (logSystem is PrimordialLogSystem)
			{
				PrimordialLogSystem pls = (PrimordialLogSystem) logSystem;
				// logSystem = LogManager.createLogSystem(this);
				logSystem = new NullLogSystem();

				/*
				 * in the event of failure, lets do something to let it 
				 * limp along.
				 */
				if (logSystem == null)
				{
					logSystem = new NullLogSystem();
				}
				else
				{
					pls.DumpLogMessages(logSystem);
				}
			}
		}


		/// <summary> This methods initializes all the directives
		/// that are used by the Velocity Runtime. The
		/// directives to be initialized are listed in
		/// the RUNTIME_DEFAULT_DIRECTIVES properties
		/// file.
		///
		/// @throws Exception
		/// </summary>
		private void initializeDirectives()
		{
			initializeDirectiveManager();

			/*
			* Initialize the runtime directive table.
			* This will be used for creating parsers.
			*/
			// runtimeDirectives = new Hashtable();

			ExtendedProperties directiveProperties = new ExtendedProperties();

			/*
			* Grab the properties file with the list of directives
			* that we should initialize.
			*/

			try
			{
				directiveProperties.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream(RuntimeConstants_Fields.DEFAULT_RUNTIME_DIRECTIVES));
			}
			catch (System.Exception ex)
			{
				throw new System.Exception("Error loading directive.properties! " + "Something is very wrong if these properties " + "aren't being located. Either your Velocity " + "distribution is incomplete or your Velocity " + "jar file is corrupted!\n" + ex.Message);
			}

			/*
			* Grab all the values of the properties. These
			* are all class names for example:
			*
			* NVelocity.Runtime.Directive.Foreach
			*/
			IEnumerator directiveClasses = directiveProperties.Values.GetEnumerator();

			while (directiveClasses.MoveNext())
			{
				String directiveClass = (String) directiveClasses.Current;
				// loadDirective(directiveClass);
				directiveManager.Register(directiveClass);
			}

			/*
			*  now the user's directives
			*/
			String[] userdirective = configuration.GetStringArray("userdirective");
			for (int i = 0; i < userdirective.Length; i++)
			{
				// loadDirective(userdirective[i]);
				directiveManager.Register(userdirective[i]);
			}
		}

		private void initializeDirectiveManager()
		{
			String directiveManagerTypeName = configuration.GetString("directive.manager");

			if (directiveManagerTypeName == null)
			{
				throw new System.Exception("Looks like there's no 'directive.manager' " + 
					"configured. NVelocity can't go any further");
			}

			directiveManagerTypeName = directiveManagerTypeName.Replace(';', ',');

			Type dirMngType = Type.GetType( directiveManagerTypeName, false, false );

			if (dirMngType == null)
			{
				throw new System.Exception( 
					String.Format("The type {0} could not be resolved", directiveManagerTypeName) );
			}

			this.directiveManager = (IDirectiveManager) Activator.CreateInstance( dirMngType );
		}

		/// <summary> Initializes the Velocity parser pool.
		/// This still needs to be implemented.
		/// </summary>
		private void initializeParserPool()
		{
			int numParsers = getInt(RuntimeConstants_Fields.PARSER_POOL_SIZE, RuntimeConstants_Fields.NUMBER_OF_PARSERS);

			parserPool = new SimplePool(numParsers);

			for (int i = 0; i < numParsers; i++)
			{
				parserPool.put(createNewParser());
			}

			info("Created: " + numParsers + " parsers.");
		}

		/// <summary> Returns a JavaCC generated Parser.
		/// </summary>
		/// <returns>Parser javacc generated parser
		///
		/// </returns>
		public Parser.Parser createNewParser()
		{
			Parser.Parser parser = new Parser.Parser(this);
			parser.Directives = directiveManager;
			return parser;
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
		public SimpleNode parse(TextReader reader, String templateName)
		{
			/*
	    *  do it and dump the VM namespace for this template
	    */
			return parse(reader, templateName, true);
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
		public SimpleNode parse(TextReader reader, String templateName, bool dumpNamespace)
		{
			SimpleNode ast = null;
			Parser.Parser parser = (Parser.Parser) parserPool.get();
			bool madeNew = false;

			if (parser == null)
			{
				/*
				*  if we couldn't get a parser from the pool
				*  make one and log it.
				*/

				error("Runtime : ran out of parsers. Creating new.  " + " Please increment the parser.pool.size property." + " The current value is too small.");

				parser = createNewParser();

				if (parser != null)
				{
					madeNew = true;
				}
			}

			/*
	    *  now, if we have a parser
	    */

			if (parser != null)
			{
				try
				{
					/*
		    *  dump namespace if we are told to.  Generally, you want to 
		    *  do this - you don't in special circumstances, such as 
		    *  when a VM is getting init()-ed & parsed
		    */

					if (dumpNamespace)
					{
						dumpVMNamespace(templateName);
					}

					ast = parser.parse(reader, templateName);
				}
				finally
				{
					/*
		    *  if this came from the pool, then put back
		    */
					if (!madeNew)
					{
						parserPool.put(parser);
					}
				}
			}
			else
			{
				error("Runtime : ran out of parsers and unable to create more.");
			}
			return ast;
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
		public Template getTemplate(String name)
		{
			return getTemplate(name, getString(RuntimeConstants_Fields.INPUT_ENCODING, RuntimeConstants_Fields.ENCODING_DEFAULT));
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
		public Template getTemplate(String name, String encoding)
		{
			return (Template) resourceManager.getResource(name, ResourceManager_Fields.RESOURCE_TEMPLATE, encoding);
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
		public ContentResource getContent(String name)
		{
			/*
	    *  the encoding is irrelvant as we don't do any converstion
	    *  the bytestream should be dumped to the output stream
	    */

			return getContent(name, getString(RuntimeConstants_Fields.INPUT_ENCODING, RuntimeConstants_Fields.ENCODING_DEFAULT));
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
		public ContentResource getContent(String name, String encoding)
		{
			return (ContentResource) resourceManager.getResource(name, ResourceManager_Fields.RESOURCE_CONTENT, encoding);
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
		public String getLoaderNameForResource(String resourceName)
		{
			return resourceManager.getLoaderNameForResource(resourceName);
		}

		/// <summary> Added this to check and make sure that the configuration
		/// is initialized before trying to get properties from it.
		/// This occurs when there are errors during initialization
		/// and the default properties have yet to be layed down.
		/// </summary>
		private bool showStackTrace()
		{
			if (configuration.IsInitialized())
			{
				return getBoolean(RuntimeConstants_Fields.RUNTIME_LOG_WARN_STACKTRACE, false);
			}
			else
			{
				return false;
			}
		}

		/// <summary> Handle logging.
		/// *
		/// </summary>
		/// <param name="String">message to log
		///
		/// </param>
		private void log(int level, Object message)
		{
			String out_Renamed;

			/*
	    *  now,  see if the logging stacktrace is on
	    *  and modify the message to suit
	    */
			//UPGRADE_NOTE: Exception 'java.lang.Throwable' was converted to ' ' which has different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1100"'
			if (showStackTrace() && (message is System.Exception || message is System.Exception))
			{
				//UPGRADE_NOTE: Exception 'java.lang.Throwable' was converted to ' ' which has different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1100"'
				out_Renamed = StringUtils.stackTrace((System.Exception) message);
			}
			else
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
				out_Renamed = message.ToString();
			}

			/*
	    *  just log it, as we are guaranteed now to have some
	    *  kind of logger - save the if()
	    */
			logSystem.LogVelocityMessage(level, out_Renamed);
		}

		/// <summary> Log a warning message.
		/// *
		/// </summary>
		/// <param name="Object">message to log
		///
		/// </param>
		public void warn(Object message)
		{
			log(LogSystem_Fields.WARN_ID, message);
		}

		///
		/// <summary> Log an info message.
		/// *
		/// </summary>
		/// <param name="Object">message to log
		///
		/// </param>
		public void info(Object message)
		{
			log(LogSystem_Fields.INFO_ID, message);
		}

		/// <summary> Log an error message.
		/// *
		/// </summary>
		/// <param name="Object">message to log
		///
		/// </param>
		public void error(Object message)
		{
			log(LogSystem_Fields.ERROR_ID, message);
		}

		/// <summary> Log a debug message.
		/// *
		/// </summary>
		/// <param name="Object">message to log
		///
		/// </param>
		public void debug(Object message)
		{
			log(LogSystem_Fields.DEBUG_ID, message);
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
		public String getString(String key, String defaultValue)
		{
			return configuration.GetString(key, defaultValue);
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
		public Directive.Directive getVelocimacro(String vmName, String templateName)
		{
			return vmFactory.getVelocimacro(vmName, templateName);
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
		public bool addVelocimacro(String name, String macro, String[] argArray, String sourceTemplate)
		{
			return vmFactory.addVelocimacro(name, macro, argArray, sourceTemplate);
		}

		/// <summary>  Checks to see if a VM exists
		/// *
		/// </summary>
		/// <param name="name"> Name of velocimacro
		/// </param>
		/// <returns>boolean  True if VM by that name exists, false if not
		///
		/// </returns>
		public bool isVelocimacro(String vmName, String templateName)
		{
			return vmFactory.isVelocimacro(vmName, templateName);
		}

		/// <summary>  tells the vmFactory to dump the specified namespace.  This is to support
		/// clearing the VM list when in inline-VM-local-scope mode
		/// </summary>
		public bool dumpVMNamespace(String namespace_Renamed)
		{
			return vmFactory.dumpVMNamespace(namespace_Renamed);
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
		public String getString(String key)
		{
			return configuration.GetString(key);
		}

		/// <summary> Int property accessor method to hide the configuration implementation.
		/// *
		/// </summary>
		/// <param name="String">key property key
		/// </param>
		/// <returns>int value
		///
		/// </returns>
		public int getInt(String key)
		{
			return configuration.GetInt(key);
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
		public int getInt(String key, int defaultValue)
		{
			return configuration.GetInt(key, defaultValue);
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
		public bool getBoolean(String key, bool def)
		{
			return configuration.GetBoolean(key, def);
		}

		/// <summary> Return the velocity runtime configuration object.
		/// *
		/// </summary>
		/// <returns>ExtendedProperties configuration object which houses
		/// the velocity runtime properties.
		///
		/// </returns>
		/// <summary>  Return the Introspector for this instance
		/// </summary>
		public Object getApplicationAttribute(Object key)
		{
			return applicationAttributes[key];
		}

		public Object setApplicationAttribute(Object key, Object o)
		{
			return applicationAttributes[key] = o;
		}

		public Uberspect Uberspect
		{
			get { return uberSpect; }
		}
	}
}