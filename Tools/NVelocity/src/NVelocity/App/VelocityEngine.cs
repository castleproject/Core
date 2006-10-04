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
	/// This class provides a separate new-able instance of the
	/// Velocity template engine.  The alternative model for use
	/// is using the Velocity class which employs the singleton
	/// model.
	/// 
	/// Please ensure that you call one of the init() variants.
	/// This is critical for proper behavior.
	/// 
	/// Coming soon : Velocity will call
	/// the parameter-less init() at the first use of this class
	/// if the init() wasn't explicitly called.  While this will
	/// ensure that Velocity functions, it almost certainly won't
	/// function in the way you intend, so please make sure to
	/// call init().
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	public class VelocityEngine
	{
		private RuntimeInstance ri = new RuntimeInstance();

		/// <summary>
		/// Init-less CTOR
		/// </summary>
		public VelocityEngine()
		{
			// do nothing
		}

		/// <summary>
		/// CTOR that invokes an init(String), initializing
		/// the engine using the properties file specified
		/// </summary>
		/// <exception cref="Exception"></exception>
		/// <param name="propsFilename">name of properties file to init with</param>
		public VelocityEngine(String propsFilename)
		{
			ri.Init(propsFilename);
		}

		/// <summary>
		/// CTOR that invokes an init(String), initializing
		/// the engine using the Properties specified
		/// </summary>
		/// <param name="p">name of properties to init with</param>
		public VelocityEngine(ExtendedProperties p)
		{
			ri.Init(p);
		}

		/// <summary>
		/// Set an entire configuration at once. This is
		/// useful in cases where the parent application uses
		/// the ExtendedProperties class and the velocity configuration
		/// is a subset of the parent application's configuration.
		/// </summary>
		public void SetExtendedProperties(ExtendedProperties value)
		{
			ri.Configuration = value;
		}

		/// <summary>
		/// initialize the Velocity runtime engine, using the default
		/// properties of the Velocity distribution
		/// </summary>
		public void Init()
		{
			ri.Init();
		}

		/// <summary>
		/// initialize the Velocity runtime engine, using default properties
		/// plus the properties in the properties file passed in as the arg
		/// </summary>
		/// <param name="propsFilename">file containing properties to use to initialize
		/// the Velocity runtime</param>
		public void Init(String propsFilename)
		{
			ri.Init(propsFilename);
		}

		/// <summary>
		/// initialize the Velocity runtime engine, using default properties
		/// plus the properties in the passed in java.util.Properties object
		/// </summary>
		/// <param name="p"> Proprties object containing initialization properties</param>
		public void Init(ExtendedProperties p)
		{
			ri.Init(p);
		}

		/// <summary>
		/// Set a Velocity Runtime property.
		/// </summary>
		public void SetProperty(String key, Object value)
		{
			ri.SetProperty(key, value);
		}

		/// <summary>
		/// Add a Velocity Runtime property.
		/// </summary>
		public void AddProperty(String key, Object value)
		{
			ri.AddProperty(key, value);
		}

		/// <summary>
		/// Clear a Velocity Runtime property.
		/// </summary>
		/// <param name="key">ley of property to clear</param>
		public void ClearProperty(String key)
		{
			ri.ClearProperty(key);
		}

		/// <summary>
		/// Get a Velocity Runtime property.
		/// </summary>
		/// <param name="key">property to retrieve</param>
		/// <returns>
		/// property value or null if the property not currently set
		/// </returns>
		public Object GetProperty(String key)
		{
			return ri.GetProperty(key);
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
		public bool Evaluate(IContext context, TextWriter writer, String logTag, String instring)
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
		[Obsolete("Use the overload that takes an TextReader")]
		public bool Evaluate(IContext context, TextWriter writer, String logTag, Stream instream)
		{
			// first, parse - convert ParseException if thrown
			TextReader br = null;
			String encoding = null;

			try
			{
				encoding = ri.GetString(RuntimeConstants.INPUT_ENCODING, RuntimeConstants.ENCODING_DEFAULT);
				br = new StreamReader(new StreamReader(instream, Encoding.GetEncoding(encoding)).BaseStream);
			}
			catch (IOException uce)
			{
				String msg = "Unsupported input encoding : " + encoding + " for template " + logTag;
				throw new ParseErrorException(msg, uce);
			}

			return Evaluate(context, writer, logTag, br);
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
		public bool Evaluate(IContext context, TextWriter writer, String logTag, TextReader reader)
		{
			SimpleNode nodeTree = null;

			try
			{
				nodeTree = ri.Parse(reader, logTag);
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
						nodeTree.Init(ica, ri);
					}
					catch (Exception e)
					{
						ri.Error("Velocity.evaluate() : init exception for tag = " + logTag + " : " + e);
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
		public bool InvokeVelocimacro(String vmName, String logTag, String[] parameters, IContext context, TextWriter writer)
		{
			// check parms
			if (vmName == null || parameters == null || context == null || writer == null || logTag == null)
			{
				ri.Error("VelocityEngine.invokeVelocimacro() : invalid parameter");
				return false;
			}

			// does the VM exist?
			if (!ri.IsVelocimacro(vmName, logTag))
			{
				ri.Error("VelocityEngine.invokeVelocimacro() : VM '" + vmName + "' not registered.");
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
				ri.Error("VelocityEngine.invokeVelocimacro() : error " + e);
				throw;
			}
		}

		/// <summary>
		/// merges a template and puts the rendered stream into the writer
		/// </summary>
		/// <param name="templateName">name of template to be used in merge</param>
		/// <param name="context"> filled context to be used in merge</param>
		/// <param name="writer"> writer to write template into</param>
		/// <returns>true if successful, false otherwise.  Errors logged to velocity log.</returns>
		[Obsolete("Use the overload that takes the encoding as parameter")]
		public bool MergeTemplate(String templateName, IContext context, TextWriter writer)
		{
			return MergeTemplate(templateName, ri.GetString(RuntimeConstants.INPUT_ENCODING, RuntimeConstants.ENCODING_DEFAULT), context, writer);
		}

		/// <summary>
		/// merges a template and puts the rendered stream into the writer
		/// </summary>
		/// <param name="templateName">name of template to be used in merge</param>
		/// <param name="encoding">encoding used in template</param>
		/// <param name="context"> filled context to be used in merge</param>
		/// <param name="writer"> writer to write template into</param>
		/// <returns>true if successful, false otherwise.  Errors logged to velocity log</returns>
		public bool MergeTemplate(String templateName, String encoding, IContext context, TextWriter writer)
		{
			Template template = ri.GetTemplate(templateName, encoding);

			if (template == null)
			{
				ri.Error("Velocity.parseTemplate() failed loading template '" + templateName + "'");
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
		/// <param name="name">The file name of the desired template.</param>
		/// <returns>The template.</returns>
		/// <exception cref="ResourceNotFoundException">
		/// if template not found from any available source.
		/// </exception>
		/// <exception cref="ParseErrorException">
		/// if template cannot be parsed due
		/// to syntax (or other) error.
		/// </exception>
		/// <exception cref="Exception">
		/// if an error occurs in template initialization
		/// </exception>
		public Template GetTemplate(String name)
		{
			return ri.GetTemplate(name);
		}

		/// <summary>
		/// Returns a <code>Template</code> from the Velocity
		/// resource management system.
		/// </summary>
		/// <param name="name">The file name of the desired template.</param>
		/// <param name="encoding">The character encoding to use for the template.</param>
		/// <returns>The template.</returns>
		/// <exception cref="ResourceNotFoundException">
		/// if template not found from any available source.
		/// </exception>
		/// <exception cref="ParseErrorException">
		/// if template cannot be parsed due
		/// to syntax (or other) error.
		/// </exception>
		/// <exception cref="Exception">
		/// if an error occurs in template initialization
		/// </exception>
		public Template GetTemplate(String name, String encoding)
		{
			return ri.GetTemplate(name, encoding);
		}

		/// <summary>
		/// Determines if a template is accessable via the currently
		/// configured resource loaders.
		/// <br/><br/>
		/// Note that the current implementation will <b>not</b>
		/// change the state of the system in any real way - so this
		/// cannot be used to pre-load the resource cache, as the
		/// previous implementation did as a side-effect.
		/// <br/><br/>
		/// The previous implementation exhibited extreme lazyness and
		/// sloth, and the author has been flogged.
		/// </summary>
		/// <param name="templateName"> name of the temlpate to search for
		/// </param>
		/// <returns>true if found, false otherwise
		/// </returns>
		public bool TemplateExists(String templateName)
		{
			return (ri.GetLoaderNameForResource(templateName) != null);
		}

		/// <summary>
		/// Log a warning message.
		/// </summary>
		/// <param name="message">message to log</param>
		public void Warn(Object message)
		{
			ri.Warn(message);
		}

		///
		/// <summary>
		/// Log an info message.
		/// </summary>
		/// <param name="message">message to log</param>
		public void Info(Object message)
		{
			ri.Info(message);
		}

		/// <summary>
		/// Log an error message.
		/// </summary>
		/// <param name="message">message to log</param>
		public void Error(Object message)
		{
			ri.Error(message);
		}

		/// <summary>
		/// Log a debug message.
		/// </summary>
		/// <param name="message">message to log</param>
		public void Debug(Object message)
		{
			ri.Debug(message);
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
		/// <param name="key">object 'name' under which the object is stored</param>
		/// <param name="value">object to store under this key</param>
		public void SetApplicationAttribute(Object key, Object value)
		{
			ri.SetApplicationAttribute(key, value);
		}
	}
}