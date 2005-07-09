using System;

namespace NVelocity.Runtime {

    /// <summary>
    /// This class defines the keys that are used in the
    /// velocity.properties file so that they can be referenced as a constant
    /// within Java code.
    /// </summary>
    /// <author> <a href="mailto:jon@latchkey.com">Jon S. Stevens</a></author>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
    public struct RuntimeConstants_Fields {
	public const System.String RUNTIME_LOG = "runtime.log";
	public const System.String RUNTIME_LOG_LOGSYSTEM = "runtime.log.logsystem";
	public const System.String RUNTIME_LOG_LOGSYSTEM_CLASS = "runtime.log.logsystem.class";
	public const System.String RUNTIME_LOG_ERROR_STACKTRACE = "runtime.log.error.stacktrace";
	public const System.String RUNTIME_LOG_WARN_STACKTRACE = "runtime.log.warn.stacktrace";
	public const System.String RUNTIME_LOG_INFO_STACKTRACE = "runtime.log.info.stacktrace";
	public const System.String RUNTIME_LOG_REFERENCE_LOG_INVALID = "runtime.log.invalid.references";
	public const System.String DEBUG_PREFIX = " [debug] ";
	public const System.String INFO_PREFIX = "  [info] ";
	public const System.String WARN_PREFIX = "  [warn] ";
	public const System.String ERROR_PREFIX = " [error] ";
	public const System.String UNKNOWN_PREFIX = " [unknown] ";
	public const System.String COUNTER_NAME = "directive.foreach.counter.name";
	public const System.String COUNTER_INITIAL_VALUE = "directive.foreach.counter.initial.value";
	public const System.String ERRORMSG_START = "directive.include.output.errormsg.start";
	public const System.String ERRORMSG_END = "directive.include.output.errormsg.end";
	public const System.String PARSE_DIRECTIVE_MAXDEPTH = "directive.parse.max.depth";
	public const System.String RESOURCE_MANAGER_CLASS = "resource.manager.class";

	/**
	 * The <code>resource.manager.cache.class</code> property
	 * specifies the name of the {@link
	 * org.apache.velocity.runtime.resource.ResourceCache}
	 * implementation to use.
	 */
	public const System.String RESOURCE_MANAGER_CACHE_CLASS = "resource.manager.cache.class";

	/**
	 * The <code>resource.manager.cache.size</code> property specifies
	 * the cache upper bound (if relevant).
	 */
	public const System.String RESOURCE_MANAGER_DEFAULTCACHE_SIZE = "resource.manager.defaultcache.size";

	public const System.String RESOURCE_MANAGER_LOGWHENFOUND = "resource.manager.logwhenfound";
	public const System.String RESOURCE_LOADER = "resource.loader";
	public const System.String FILE_RESOURCE_LOADER_PATH = "file.resource.loader.path";
	public const System.String FILE_RESOURCE_LOADER_CACHE = "file.resource.loader.cache";
	public const System.String VM_LIBRARY = "velocimacro.library";
	public const System.String VM_LIBRARY_AUTORELOAD = "velocimacro.library.autoreload";
	public const System.String VM_PERM_ALLOW_INLINE = "velocimacro.permissions.allow.inline";
	public const System.String VM_PERM_ALLOW_INLINE_REPLACE_GLOBAL = "velocimacro.permissions.allow.inline.to.replace.global";
	public const System.String VM_PERM_INLINE_LOCAL = "velocimacro.permissions.allow.inline.local.scope";
	public const System.String VM_MESSAGES_ON = "velocimacro.messages.on";
	public const System.String VM_CONTEXT_LOCALSCOPE = "velocimacro.context.localscope";
	public const System.String INTERPOLATE_STRINGLITERALS = "runtime.interpolate.string.literals";
	public const System.String INPUT_ENCODING = "input.encoding";
	public const System.String OUTPUT_ENCODING = "output.encoding";
	public const System.String ENCODING_DEFAULT = "ISO-8859-1";

	public const System.String DEFAULT_RUNTIME_PROPERTIES = "NVelocity.Runtime.Defaults.nvelocity.properties";
	public const System.String DEFAULT_RUNTIME_DIRECTIVES = "NVelocity.Runtime.Defaults.directive.properties";

	/**
	 * The default number of parser instances to create.  Configurable
	 * via the parameter named by the {@link #PARSER_POOL_SIZE}
	 * constant.
	 */
	public const int NUMBER_OF_PARSERS = 20;

	/**
	 * @see #NUMBER_OF_PARSERS
	 */
	public const System.String PARSER_POOL_SIZE = "parser.pool.size";


	/**
	 *  key name for uberspector
	 */
	public const System.String UBERSPECT_CLASSNAME = "runtime.introspector.uberspect";
    }
    public interface RuntimeConstants {
	/*
	* ----------------------------------------------------------------------
	* These are public constants that are used as handles for the
	* properties that can be specified in your typical
	* nvelocity.properties file.
	* ----------------------------------------------------------------------
	*/
	/*
	* ----------------------------------------------------------------------
	* L O G G I N G  C O N F I G U R A T I O N
	* ----------------------------------------------------------------------
	*/
	///
	/// <summary> Location of the velocity log file.
	/// </summary>
	/// <summary>  externally provided logger
	/// </summary>
	/// <summary>  class of log system to use
	/// </summary>
	///
	/// <summary> Stack trace output for error messages.
	/// </summary>
	///
	/// <summary> Stack trace output for warning messages.
	/// </summary>
	///
	/// <summary> Stack trace output for info messages.
	/// </summary>
	/// <summary> Logging of invalid references.
	/// </summary>
	/// <summary>  Log message prefixes
	/// </summary>
	/// <summary> Log4Net configuration
	/// </summary>
	/// <summary> Log4Net configuration
	/// </summary>
	/// <summary> Log4Net configuration
	/// </summary>
	/// <summary> Log4Net configuration
	/// </summary>
	/// <summary> Log4Net configuration
	/// </summary>
	/// <summary> Log4Net configuration
	/// </summary>
	/// <summary> Log4Net configuration
	/// </summary>
	/// <summary> Log4Net configuration
	/// </summary>
	/// <summary> Log4Net configuration
	/// </summary>
	/// <summary> Log4Net configuration
	/// </summary>
	/// <summary> Log4Net configuration
	/// </summary>
	/// <summary> Log4Net configuration
	/// </summary>
	/*
	* ----------------------------------------------------------------------
	* D I R E C T I V E  C O N F I G U R A T I O N
	* ----------------------------------------------------------------------
	* Directive properties are of the form:
	* 
	* directive.<directive-name>.<property>
	* ----------------------------------------------------------------------
	*/
	///
	/// <summary> Initial counter value in #foreach directives.
	/// </summary>
	///
	/// <summary> Initial counter value in #foreach directives.
	/// </summary>
	/// <summary> Starting tag for error messages triggered by passing
	/// a parameter not allowed in the #include directive. Only
	/// string literals, and references are allowed.
	/// </summary>
	/// <summary> Ending tag for error messages triggered by passing
	/// a parameter not allowed in the #include directive. Only
	/// string literals, and references are allowed.
	/// </summary>
	/// <summary> Maximum recursion depth allowed for the #parse directive.
	/// </summary>
	/*
	* ----------------------------------------------------------------------
	*  R E S O U R C E   M A N A G E R   C O N F I G U R A T I O N
	* ----------------------------------------------------------------------
	*/
	/*
	* ----------------------------------------------------------------------
	* R E S O U R C E  L O A D E R  C O N F I G U R A T I O N
	* ----------------------------------------------------------------------
	*/
	/// <summary>  controls if the finding of a resource is logged
	/// </summary>
	/// <summary> Key used to retrieve the names of the resource loaders
	/// to be used. In a properties file they may appear as
	/// the following:
	/// *
	/// resource.loader = file,classpath
	/// </summary>
	/// <summary> The public handle for setting a path in
	/// the FileResourceLoader.
	/// </summary>
	/// <summary> The public handle for turning the caching on in the
	/// FileResourceLoader.
	/// </summary>
	/*
	* ----------------------------------------------------------------------
	* V E L O C I M A C R O  C O N F I G U R A T I O N
	* ----------------------------------------------------------------------
	*/
	///
	/// <summary> Name of local Velocimacro library template.
	/// </summary>
	///
	/// <summary> switch for autoloading library-sourced VMs (for development)
	/// </summary>
	///
	/// <summary> boolean (true/false) default true : allow
	/// inline (in-template) macro definitions
	/// </summary>
	/// <summary> boolean (true/false) default false : allow inline
	/// (in-template) macro definitions to replace existing
	/// </summary>
	///
	/// <summary> Switch for forcing inline macros to be local : default false.
	/// </summary>
	///
	/// <summary> Switch for VM blather : default true.
	/// </summary>
	///
	/// <summary> switch for local context in VM : default false
	/// </summary>
	/*
	* ----------------------------------------------------------------------
	* G E N E R A L  R U N T I M E  C O N F I G U R A T I O N
	* ----------------------------------------------------------------------
	*/
	/// <summary>  Switch for the interpolation facility for string literals
	/// </summary>
	///
	/// <summary> The character encoding for the templates.  Used by the parser in
	/// processing the input streams.
	/// </summary>
	/// <summary> Encoding for the output stream.  Currently used by Anakia and
	/// VelocityServlet
	/// </summary>
	/*
	* ----------------------------------------------------------------------
	* These constants are used internally by the Velocity runtime i.e.
	* the constansts listed below are strictly used in the Runtime
	* class itself.
	* ----------------------------------------------------------------------
	*/
	///
	/// <summary> Default Runtime properties.
	/// </summary>
	///
	/// <summary> Default Runtime properties
	/// </summary>
	/// <summary> Number of parsers to create
	/// </summary>
    }
}
