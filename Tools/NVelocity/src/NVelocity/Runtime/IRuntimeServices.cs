namespace NVelocity.Runtime
{
	using System;
	using System.IO;

	using Commons.Collections;

	using NVelocity.Runtime.Parser.Node;
	using NVelocity.Runtime.Resource;
	using NVelocity.Util.Introspection;

	/// <summary> Interface for internal runtime services that are needed by the
	/// various components w/in Velocity.  This was taken from the old
	/// Runtime singleton, and anything not necessary was removed.
	///
	/// Currently implemented by RuntimeInstance.
	/// </summary>
	public interface IRuntimeServices : IRuntimeLogger
	{
		/**
		* Return the velocity runtime configuration object.
		*
		* @return ExtendedProperties configuration object which houses
		*                       the velocity runtime properties.
		*/
		ExtendedProperties Configuration { get; set; }

		/**
		* Returns the configured class introspection/reflection
		* implemenation.
		*/
		IUberspect Uberspect { get; }

		/**
		* Returns the configured method introspection/reflection
		* implemenation.
		*/
		Introspector Introspector { get; }

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
		void Init();

		/// <summary>
		/// Allows an external system to set a property in
		/// the Velocity Runtime.
		/// </summary>
		/// <param name="key">property key</param>
		/// <param name="value">property value</param>
		void SetProperty(String key, Object value);

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
		/// <param name="key">key</param>
		/// <param name="value">value</param>
		void AddProperty(String key, Object value);

		/// <summary>
		/// Clear the values pertaining to a particular property.
		/// </summary>
		/// <param name="key">key of property to clear</param>
		void ClearProperty(String key);

		/// <summary>  Allows an external caller to get a property.  The calling
		/// routine is required to know the type, as this routine
		/// will return an Object, as that is what properties can be.
		/// *
		/// </summary>
		/// <param name="key">property to return
		///
		/// </param>
		Object GetProperty(String key);

		// <summary> Initialize the Velocity Runtime with a Properties
		// object.
		// *
		// </summary>
		// <param name="">Properties
		//
		// </param>
		// TODO
		//void  init(System.Configuration.AppSettingsReader p);
		
		
		/// <summary>
		/// Initialize the Velocity Runtime with the name of
		/// ExtendedProperties object.
		/// </summary>
		/// <param name="configurationFile">Properties</param>
		void Init(String configurationFile);

		/// <summary>
		/// Parse the input and return the root of
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
		/// <param name="reader">inputstream retrieved by a resource loader</param>
		/// <param name="templateName">name of the template being parsed</param>
		SimpleNode Parse(TextReader reader, String templateName);

		/// <summary>
		/// Parse the input and return the root of the AST node structure.
		/// </summary>
		/// <param name="reader">inputstream retrieved by a resource loader</param>
		/// <param name="templateName">name of the template being parsed</param>
		/// <param name="dumpNamespace">flag to dump the Velocimacro namespace for this template</param>
		SimpleNode Parse(TextReader reader, String templateName, bool dumpNamespace);

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
		Template GetTemplate(String name);

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
		Template GetTemplate(String name, String encoding);

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
		ContentResource GetContent(String name);

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
		ContentResource GetContent(String name, String encoding);

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
		String GetLoaderNameForResource(String resourceName);

		/// <summary>
		/// String property accessor method with default to hide the
		/// configuration implementation.
		/// </summary>
		/// <param name="key">key property key</param>
		/// <param name="defaultValue">default value to return if key not
		/// found in resource manager.
		/// </param>
		/// <returns>value of key or default</returns>
		String GetString(String key, String defaultValue);

		/// <summary>
		/// Returns the appropriate VelocimacroProxy object if vmName
		/// is a valid current Velocimacro.
		/// </summary>
		/// <param name="vmName">Name of velocimacro requested</param>
		/// <param name="templateName">Template Name</param>
		/// <returns>VelocimacroProxy</returns>
		Directive.Directive GetVelocimacro(String vmName, String templateName);

		/// <summary>
		/// Adds a new Velocimacro. Usually called by Macro only while parsing.
		/// </summary>
		/// <param name="name">name  Name of velocimacro</param>
		/// <param name="macro">macro  String form of macro body</param>
		/// <param name="argArray">argArray  Array of strings, containing the
		/// <c>#macro()</c> arguments.  the 0th is the name.</param>
		/// <param name="sourceTemplate">Source Template</param>
		/// <returns>
		/// True if added, false if rejected for some
		/// reason (either parameters or permission settings)
		/// </returns>
		bool AddVelocimacro(String name, String macro, String[] argArray, String sourceTemplate);

		/// <summary>
		/// Checks to see if a VM exists
		/// </summary>
		/// <param name="vmName">Name of velocimacro</param>
		/// <param name="templateName">Template Name</param>
		/// <returns>True if VM by that name exists, false if not</returns>
		bool IsVelocimacro(String vmName, String templateName);

		/// <summary>  tells the vmFactory to dump the specified namespace.  This is to support
		/// clearing the VM list when in inline-VM-local-scope mode
		/// </summary>
		bool DumpVMNamespace(String ns);

		/// <summary>
		/// String property accessor method to hide the configuration implementation.
		/// </summary>
		/// <param name="key">property key</param>
		/// <returns>value</returns>
		String GetString(String key);

		/// <summary>
		/// Int property accessor method to hide the configuration implementation.
		/// </summary>
		/// <param name="key">property key</param>
		/// <returns>value</returns>
		int GetInt(String key);

		/// <summary>
		/// Int property accessor method to hide the configuration implementation.
		/// </summary>
		/// <param name="key">property key</param>
		/// <param name="defaultValue">default value</param>
		/// <returns>value</returns>
		int GetInt(String key, int defaultValue);

		/// <summary>
		/// Boolean property accessor method to hide the configuration implementation.
		/// </summary>
		/// <param name="key">property key</param>
		/// <param name="def">default value</param>
		/// <returns>value</returns>
		bool GetBoolean(String key, bool def);

		/// <summary>
		/// Return the specified application attribute
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		Object GetApplicationAttribute(Object key);

		/// <summary>
		/// Sets the specified application attribute.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		Object SetApplicationAttribute(Object key, Object value);
	}
}