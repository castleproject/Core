namespace NVelocity.Runtime
{
	using System;
	using NVelocity.Runtime.Resource;

	/// <summary>
	/// This class defines the keys that are used in the
	/// velocity.properties file so that they can be referenced as a constant
	/// within Java code.
	/// </summary>
	/// <author> <a href="mailto:jon@latchkey.com">Jon S. Stevens</a></author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
	public struct RuntimeConstants
	{
		public const String RUNTIME_LOG = "runtime.log";
		public const String RUNTIME_LOG_LOGSYSTEM = "runtime.log.logsystem";
		public const String RUNTIME_LOG_LOGSYSTEM_CLASS = "runtime.log.logsystem.class";
		public const String RUNTIME_LOG_ERROR_STACKTRACE = "runtime.log.error.stacktrace";
		public const String RUNTIME_LOG_WARN_STACKTRACE = "runtime.log.warn.stacktrace";
		public const String RUNTIME_LOG_INFO_STACKTRACE = "runtime.log.info.stacktrace";
		public const String RUNTIME_LOG_REFERENCE_LOG_INVALID = "runtime.log.invalid.references";
		public const String DEBUG_PREFIX = " [debug] ";
		public const String INFO_PREFIX = "  [info] ";
		public const String WARN_PREFIX = "  [warn] ";
		public const String ERROR_PREFIX = " [error] ";
		public const String UNKNOWN_PREFIX = " [unknown] ";
		public const String COUNTER_NAME = "directive.foreach.counter.name";
		public const String COUNTER_INITIAL_VALUE = "directive.foreach.counter.initial.value";
		public const String ERRORMSG_START = "directive.include.output.errormsg.start";
		public const String ERRORMSG_END = "directive.include.output.errormsg.end";
		public const String PARSE_DIRECTIVE_MAXDEPTH = "directive.parse.max.depth";
		public const String RESOURCE_MANAGER_CLASS = "resource.manager.class";

		/// <summary>
		/// The <code>resource.manager.cache.class</code> property 
		/// specifies the name of the <see cref="ResourceCache"/> implementation to use.
		/// </summary>
		public const String RESOURCE_MANAGER_CACHE_CLASS = "resource.manager.cache.class";

		/// <summary>
		/// The <code>resource.manager.cache.size</code> property 
		/// specifies the cache upper bound (if relevant).
		/// </summary>
		public const String RESOURCE_MANAGER_DEFAULTCACHE_SIZE = "resource.manager.defaultcache.size";

		public const String RESOURCE_MANAGER_LOGWHENFOUND = "resource.manager.logwhenfound";
		public const String RESOURCE_LOADER = "resource.loader";
		public const String FILE_RESOURCE_LOADER_PATH = "file.resource.loader.path";
		public const String FILE_RESOURCE_LOADER_CACHE = "file.resource.loader.cache";
		public const String VM_LIBRARY = "velocimacro.library";
		public const String VM_LIBRARY_AUTORELOAD = "velocimacro.library.autoreload";
		public const String VM_PERM_ALLOW_INLINE = "velocimacro.permissions.allow.inline";
		public const String VM_PERM_ALLOW_INLINE_REPLACE_GLOBAL = "velocimacro.permissions.allow.inline.to.replace.global";
		public const String VM_PERM_INLINE_LOCAL = "velocimacro.permissions.allow.inline.local.scope";
		public const String VM_MESSAGES_ON = "velocimacro.messages.on";
		public const String VM_CONTEXT_LOCALSCOPE = "velocimacro.context.localscope";
		public const String INTERPOLATE_STRINGLITERALS = "runtime.interpolate.string.literals";
		public const String INPUT_ENCODING = "input.encoding";
		public const String OUTPUT_ENCODING = "output.encoding";
		public const String ENCODING_DEFAULT = "ISO-8859-1";

		public const String DEFAULT_RUNTIME_PROPERTIES = "NVelocity.Runtime.Defaults.nvelocity.properties";
		public const String DEFAULT_RUNTIME_DIRECTIVES = "NVelocity.Runtime.Defaults.directive.properties";

		/// <summary>
		/// The default number of parser instances to create.
		/// Configurable via the parameter named by the <see cref="PARSER_POOL_SIZE"/> constant.
		/// </summary>
		public const int NUMBER_OF_PARSERS = 20;

		/// <summary>
		/// <see cref="NUMBER_OF_PARSERS"/>
		/// </summary>
		public const String PARSER_POOL_SIZE = "parser.pool.size";

		/// <summary>
		/// key name for uberspector
		/// </summary>
		public const String UBERSPECT_CLASSNAME = "runtime.introspector.uberspect";
	}
}