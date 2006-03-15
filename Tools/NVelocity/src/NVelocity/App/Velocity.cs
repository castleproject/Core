using NVelocity.Runtime.Resource;

namespace NVelocity.App
{
	using System;
	using System.IO;
	using System.Text;
	using Commons.Collections;
	using NVelocity.Context;
	using NVelocity.Exception;
	using NVelocity.Runtime;
	using NVelocity.Runtime.Parser;
	using NVelocity.Runtime.Parser.Node;

	/// <summary>
	/// This class provides  services to the application
	/// developer, such as :
	/// <ul>
	/// <li> Simple Velocity Runtime engine initialization methods.
	/// <li> Functions to apply the template engine to streams and strings
	/// to allow embedding and dynamic template generation.
	/// <li> Methods to access Velocimacros directly.
	/// </ul>
	/// <br><br>
	/// While the most common way to use NVelocity is via templates, as
	/// Velocity is a general-purpose template engine, there are other
	/// uses that NVelocity is well suited for, such as processing dynamically
	/// created templates, or processing content streams.
	/// <br><br>
	/// The methods herein were developed to allow easy access to the NVelocity
	/// facilities without direct spelunking of the internals.  If there is
	/// something you feel is necessary to add here, please, send a patch.
	/// </summary>
	public class Velocity
	{
		/// <summary>
		/// initialize the NVelocity runtime engine, using the default
		/// properties of the NVelocity distribution
		/// </summary>
		public static void Init()
		{
			RuntimeSingleton.Init();
		}

		/// <summary>
		/// initialize the Velocity runtime engine, using default properties
		/// plus the properties in the properties file passed in as the arg
		/// </summary>
		/// <param name="propsFilename">
		/// file containing properties to use to initialize
		/// the Velocity runtime
		/// </param>
		public static void Init(String propsFilename)
		{
			RuntimeSingleton.Init(propsFilename);
		}

		/// <summary>
		/// initialize the Velocity runtime engine, using default properties
		/// plus the properties in the passed in java.util.Properties object
		/// </summary>
		/// <param name="p">
		/// Proprties object containing initialization properties
		/// </param>
		public static void Init(ExtendedProperties p)
		{
			RuntimeSingleton.Init(p);
		}

		/// <summary>
		/// Set a Velocity Runtime property.
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="value">value</param>
		public static void SetProperty(String key, Object value)
		{
			RuntimeSingleton.SetProperty(key, value);
		}

		/// <summary>
		/// Add a Velocity Runtime property.
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="value">value</param>
		public static void AddProperty(String key, Object value)
		{
			RuntimeSingleton.AddProperty(key, value);
		}

		/// <summary>
		/// Clear a NVelocity Runtime property.
		/// </summary>
		/// <param name="key">of property to clear</param>
		public static void ClearProperty(String key)
		{
			RuntimeSingleton.ClearProperty(key);
		}

		/// <summary>
		/// Set an entire configuration at once. This is
		/// useful in cases where the parent application uses
		/// the ExtendedProperties class and the velocity configuration
		/// is a subset of the parent application's configuration.
		/// </summary>
		public static void SetExtendedProperties(ExtendedProperties value)
		{
			RuntimeSingleton.Configuration = value;
		}

		/// <summary>
		/// Get a Velocity Runtime property.
		/// </summary>
		/// <param name="key">property to retrieve</param>
		/// <returns>property value or null if the property not currently set</returns>
		public static Object GetProperty(String key)
		{
			return RuntimeSingleton.GetProperty(key);
		}

		/// <summary>
		/// renders the input string using the context into the output writer.
		/// To be used when a template is dynamically constructed, or want to use
		/// Velocity as a token replacer.
		/// </summary>
		/// <param name="context">context to use in rendering input string</param>
		/// <param name="writer"> Writer in which to render the output</param>
		/// <param name="logTag"> string to be used as the template name for log messages in case of error</param>
		/// <param name="instring">input string containing the VTL to be rendered</param>
		/// <returns>true if successful, false otherwise.  If false, see Velocity runtime log</returns>
		public static bool Evaluate(IContext context, TextWriter writer, String logTag, String instring)
		{
			return Evaluate(context, writer, logTag, new StringReader(instring));
		}

		/// <summary>
		/// Renders the input stream using the context into the output writer.
		/// To be used when a template is dynamically constructed, or want to
		/// use Velocity as a token replacer.
		/// </summary>
		/// <param name="context">context to use in rendering input string</param>
		/// <param name="writer"> Writer in which to render the output</param>
		/// <param name="logTag"> string to be used as the template name for log messages in case of error</param>
		/// <param name="instream">input stream containing the VTL to be rendered</param>
		/// <returns>true if successful, false otherwise.  If false, see Velocity runtime log</returns>
		[Obsolete("Use the overload that takes a TextReader")]
		public static bool Evaluate(IContext context, TextWriter writer, String logTag, Stream instream)
		{
			// first, parse - convert ParseException if thrown
			TextReader reader = null;
			String encoding = null;

			try
			{
				encoding = RuntimeSingleton.getString(RuntimeConstants.INPUT_ENCODING, RuntimeConstants.ENCODING_DEFAULT);
				reader = new StreamReader(new StreamReader(instream, Encoding.GetEncoding(encoding)).BaseStream);
			}
			catch (IOException uce)
			{
				String msg = "Unsupported input encoding : " + encoding + " for template " + logTag;
				throw new ParseErrorException(msg, uce);
			}

			return Evaluate(context, writer, logTag, reader);
		}

		/// <summary>
		/// Renders the input reader using the context into the output writer.
		/// To be used when a template is dynamically constructed, or want to
		/// use Velocity as a token replacer.
		/// </summary>
		/// <param name="context">context to use in rendering input string</param>
		/// <param name="writer"> Writer in which to render the output</param>
		/// <param name="logTag"> string to be used as the template name for log messages in case of error</param>
		/// <param name="reader">Reader containing the VTL to be rendered</param>
		/// <returns>true if successful, false otherwise.  If false, see Velocity runtime log</returns>
		public static bool Evaluate(IContext context, TextWriter writer, String logTag, TextReader reader)
		{
			SimpleNode nodeTree = null;

			try
			{
				nodeTree = RuntimeSingleton.Parse(reader, logTag);
			}
			catch (ParseException pex)
			{
				throw new ParseErrorException(pex.Message, pex);
			}

			// now we want to init and render
			if (nodeTree != null)
			{
				InternalContextAdapterImpl ica = new InternalContextAdapterImpl(context);

				ica.PushCurrentTemplateName(logTag);

				try
				{
					try
					{
						nodeTree.Init(ica, RuntimeSingleton.RuntimeServices);
					}
					catch (Exception e)
					{
						RuntimeSingleton.Error("Velocity.evaluate() : init exception for tag = " + logTag + " : " + e);
					}

					// now render, and let any exceptions fly
					nodeTree.Render(ica, writer);
				}
				finally
				{
					ica.PopCurrentTemplateName();
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Invokes a currently registered Velocimacro with the parms provided
		/// and places the rendered stream into the writer.
		/// 
		/// Note : currently only accepts args to the VM if they are in the context.
		/// </summary>
		/// <param name="vmName">name of Velocimacro to call</param>
		/// <param name="logTag">string to be used for template name in case of error</param>
		/// <param name="parameters">args used to invoke Velocimacro. In context key format :
		/// eg  "foo","bar" (rather than "$foo","$bar")
		/// </param>
		/// <param name="context">Context object containing data/objects used for rendering.</param>
		/// <param name="writer"> Writer for output stream</param>
		/// <returns>true if Velocimacro exists and successfully invoked, false otherwise.</returns>
		public static bool InvokeVelocimacro(String vmName, String logTag, String[] parameters, IContext context, TextWriter writer)
		{
			// check parms
			if (vmName == null || parameters == null || context == null || writer == null || logTag == null)
			{
				RuntimeSingleton.Error("Velocity.invokeVelocimacro() : invalid parameter");
				return false;
			}

			// does the VM exist?
			if (!RuntimeSingleton.IsVelocimacro(vmName, logTag))
			{
				RuntimeSingleton.Error("Velocity.invokeVelocimacro() : VM '" + vmName + "' not registered.");
				return false;
			}

			// now just create the VM call, and use evaluate
			StringBuilder construct = new StringBuilder("#");

			construct.Append(vmName);
			construct.Append("(");

			for (int i = 0; i < parameters.Length; i++)
			{
				construct.Append(" $");
				construct.Append(parameters[i]);
			}

			construct.Append(" )");

			try
			{
				bool retval = Evaluate(context, writer, logTag, construct.ToString());
				return retval;
			}
			catch (Exception e)
			{
				RuntimeSingleton.Error("Velocity.invokeVelocimacro() : error " + e);
			}

			return false;
		}

		/// <summary>
		/// merges a template and puts the rendered stream into the writer
		/// </summary>
		/// <param name="templateName">name of template to be used in merge</param>
		/// <param name="context"> filled context to be used in merge</param>
		/// <param name="writer"> writer to write template into</param>
		/// <returns>true if successful, false otherwise.  Errors logged to velocity log.</returns>
		[Obsolete("Use the overload that takes an encoding")]
		public static bool MergeTemplate(String templateName, IContext context, TextWriter writer)
		{
			return MergeTemplate(templateName, RuntimeSingleton.getString(RuntimeConstants.INPUT_ENCODING, RuntimeConstants.ENCODING_DEFAULT), context, writer);
		}

		/// <summary>
		/// merges a template and puts the rendered stream into the writer
		/// </summary>
		/// <param name="templateName">name of template to be used in merge</param>
		/// <param name="encoding">encoding used in template</param>
		/// <param name="context"> filled context to be used in merge</param>
		/// <param name="writer"> writer to write template into</param>
		/// <returns>true if successful, false otherwise.  Errors logged to velocity log</returns>
		public static bool MergeTemplate(String templateName, String encoding, IContext context, TextWriter writer)
		{
			Template template = RuntimeSingleton.GetTemplate(templateName, encoding);

			if (template == null)
			{
				RuntimeSingleton.Error("Velocity.parseTemplate() failed loading template '" + templateName + "'");
				return false;
			}
			else
			{
				template.Merge(context, writer);
				return true;
			}
		}

		/// <summary>
		/// Returns a <code>Template</code> from the Velocity
		/// resource management system.
		/// </summary>
		/// <param name="name">The file name of the desired template.
		/// </param>
		/// <returns>    The template.
		/// @throws ResourceNotFoundException if template not found
		/// from any available source.
		/// @throws ParseErrorException if template cannot be parsed due
		/// to syntax (or other) error.
		/// @throws Exception if an error occurs in template initialization
		/// </returns>
		public static Template GetTemplate(String name)
		{
			return RuntimeSingleton.GetTemplate(name);
		}

		/// <summary>
		/// Returns a <code>Template</code> from the Velocity
		/// resource management system.
		/// </summary>
		/// <param name="name">The file name of the desired template.</param>
		/// <param name="encoding">The character encoding to use for the template.</param>
		/// <returns>The <see cref="Template"/> instance.</returns>
		/// <exception cref="ResourceNotFoundException">
		/// If template is not found from any available source.
		/// </exception>
		/// <exception cref="ParseErrorException">
		/// If template cannot be parsed due to syntax (or other) error.
		/// </exception>
		/// <exception cref="Exception">
		/// If an error occurs in template initialization.
		/// </exception>
		public static Template GetTemplate(String name, String encoding)
		{
			return RuntimeSingleton.GetTemplate(name, encoding);
		}

		/// <summary>
		/// <p>Determines whether a resource is accessable via the
		/// currently configured resource loaders. <see cref="Resource"/>
		/// is the generic description of templates, static content, etc.</p>
		/// 
		/// <p>Note that the current implementation will <b>not</b> change
		/// the state of the system in any real way - so this cannot be
		/// used to pre-load the resource cache, as the previous
		/// implementation did as a side-effect.</p>
		/// </summary>
		/// <param name="templateName"> name of the template to search for</param>
		/// <returns>Whether the resource was located.</returns>
		public static bool ResourceExists(String templateName)
		{
			return (RuntimeSingleton.GetLoaderNameForResource(templateName) != null);
		}

		/// <summary>
		/// Log a warning message.
		/// </summary>
		/// <param name="message">message to log
		/// </param>
		public static void Warn(Object message)
		{
			RuntimeSingleton.Warn(message);
		}

		/// <summary>
		/// Log an info message.
		/// </summary>
		/// <param name="message">message to log</param>
		public static void Info(Object message)
		{
			RuntimeSingleton.Info(message);
		}

		/// <summary>
		/// Log an error message.
		/// </summary>
		/// <param name="message">message to log</param>
		public static void Error(Object message)
		{
			RuntimeSingleton.Error(message);
		}

		/// <summary>
		/// Log a debug message.
		/// </summary>
		/// <param name="message">message to log</param>
		public static void Debug(Object message)
		{
			RuntimeSingleton.Debug(message);
		}

		/// <summary>
		/// <p>
		/// Set the an ApplicationAttribue, which is an Object
		/// set by the application which is accessable from
		/// any component of the system that gets a RuntimeServices.
		/// This allows communication between the application
		/// environment and custom pluggable components of the
		/// Velocity engine, such as loaders and loggers.
		/// </p>
		/// <p>
		/// Note that there is no enfocement or rules for the key
		/// used - it is up to the application developer.  However, to
		/// help make the intermixing of components possible, using
		/// the target Class name (e.g.  com.foo.bar ) as the key
		/// might help avoid collision.
		/// </p>
		/// </summary>
		/// <param name="key">object 'name' under which the object is stored
		/// </param>
		/// <param name="value">object to store under this key
		/// </param>
		public static void SetApplicationAttribute(Object key, Object value)
		{
			RuntimeSingleton.RuntimeServices.SetApplicationAttribute(key, value);
		}

		/// <summary></summary>
		/// <see>#ResourceExists(String)</see>
		/// </summary>
		[Obsolete("Use ResourceExists(String) instead")]
		public static Boolean TemplateExists(String resourceName)
		{
			return ResourceExists(resourceName);
		}
	}
}