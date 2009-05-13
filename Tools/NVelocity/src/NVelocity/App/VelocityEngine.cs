// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace NVelocity.App
{
	using System;
	using System.IO;
	using System.Text;
	using Commons.Collections;
	using Context;
	using Exception;
	using NVelocity.Runtime.Parser;
	using NVelocity.Runtime.Parser.Node;
	using Runtime;

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
		private RuntimeInstance runtimeInstance = new RuntimeInstance();

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
			runtimeInstance.Init(propsFilename);
		}

		/// <summary>
		/// CTOR that invokes an init(String), initializing
		/// the engine using the Properties specified
		/// </summary>
		/// <param name="p">name of properties to init with</param>
		public VelocityEngine(ExtendedProperties p)
		{
			runtimeInstance.Init(p);
		}

		/// <summary>
		/// Set an entire configuration at once. This is
		/// useful in cases where the parent application uses
		/// the ExtendedProperties class and the velocity configuration
		/// is a subset of the parent application's configuration.
		/// </summary>
		public void SetExtendedProperties(ExtendedProperties value)
		{
			runtimeInstance.Configuration = value;
		}

		/// <summary>
		/// initialize the Velocity runtime engine, using the default
		/// properties of the Velocity distribution
		/// </summary>
		public void Init()
		{
			runtimeInstance.Init();
		}

		/// <summary>
		/// initialize the Velocity runtime engine, using default properties
		/// plus the properties in the properties file passed in as the arg
		/// </summary>
		/// <param name="propsFilename">file containing properties to use to initialize
		/// the Velocity runtime</param>
		public void Init(String propsFilename)
		{
			runtimeInstance.Init(propsFilename);
		}

		/// <summary>
		/// initialize the Velocity runtime engine, using default properties
		/// plus the properties in the passed in java.util.Properties object
		/// </summary>
		/// <param name="p"> Properties object containing initialization properties</param>
		public void Init(ExtendedProperties p)
		{
			runtimeInstance.Init(p);
		}

		/// <summary>
		/// Set a Velocity Runtime property.
		/// </summary>
		public void SetProperty(String key, Object value)
		{
			runtimeInstance.SetProperty(key, value);
		}

		/// <summary>
		/// Add a Velocity Runtime property.
		/// </summary>
		public void AddProperty(String key, Object value)
		{
			runtimeInstance.AddProperty(key, value);
		}

		/// <summary>
		/// Clear a Velocity Runtime property.
		/// </summary>
		/// <param name="key">key of property to clear</param>
		public void ClearProperty(String key)
		{
			runtimeInstance.ClearProperty(key);
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
			return runtimeInstance.GetProperty(key);
		}

		/// <summary>
		/// renders the input string using the context into the output writer.
		/// To be used when a template is dynamically constructed, or want to use
		/// Velocity as a token replacer.
		/// </summary>
		/// <param name="context">context to use in rendering input string</param>
		/// <param name="writer"> Writer in which to render the output</param>
		/// <param name="logTag"> string to be used as the template name for log messages in case of error</param>
		/// <param name="inString">input string containing the VTL to be rendered</param>
		/// <returns>true if successful, false otherwise.  If false, see Velocity runtime log</returns>
		public bool Evaluate(IContext context, TextWriter writer, String logTag, String inString)
		{
			return Evaluate(context, writer, logTag, new StringReader(inString));
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
				encoding = runtimeInstance.GetString(RuntimeConstants.INPUT_ENCODING, RuntimeConstants.ENCODING_DEFAULT);
				br = new StreamReader(new StreamReader(instream, Encoding.GetEncoding(encoding)).BaseStream);
			}
			catch(IOException ioException)
			{
				String msg = string.Format("Unsupported input encoding : {0} for template {1}", encoding, logTag);
				throw new ParseErrorException(msg, ioException);
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
				nodeTree = runtimeInstance.Parse(reader, logTag);
			}
			catch(ParseException parseException)
			{
				throw new ParseErrorException(parseException.Message, parseException);
			}

			// now we want to init and render
			if (nodeTree != null)
			{
				InternalContextAdapterImpl internalContextAdapterImpl = new InternalContextAdapterImpl(context);

				internalContextAdapterImpl.PushCurrentTemplateName(logTag);

				try
				{
					try
					{
						nodeTree.Init(internalContextAdapterImpl, runtimeInstance);
					}
					catch(Exception e)
					{
						runtimeInstance.Error(string.Format("Velocity.evaluate() : init exception for tag = {0} : {1}", logTag, e));
					}

					// now render, and let any exceptions fly
					nodeTree.Render(internalContextAdapterImpl, writer);
				}
				finally
				{
					internalContextAdapterImpl.PopCurrentTemplateName();
				}

				return true;
			}

			return false;
		}


		/// <summary>
		/// Invokes a currently registered Velocimacro with the parameters provided
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
			// check parameters
			if (vmName == null || parameters == null || context == null || writer == null || logTag == null)
			{
				runtimeInstance.Error("VelocityEngine.invokeVelocimacro() : invalid parameter");
				return false;
			}

			// does the VM exist?
			if (!runtimeInstance.IsVelocimacro(vmName, logTag))
			{
				runtimeInstance.Error(string.Format("VelocityEngine.invokeVelocimacro() : VM '{0}' not registered.", vmName));
				return false;
			}

			// now just create the VM call, and use evaluate
			StringBuilder construct = new StringBuilder("#");

			construct.Append(vmName);
			construct.Append("(");

			for(int i = 0; i < parameters.Length; i++)
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
			catch(Exception e)
			{
				runtimeInstance.Error(string.Format("VelocityEngine.invokeVelocimacro() : error {0}", e));
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
			return
				MergeTemplate(templateName,
				              runtimeInstance.GetString(RuntimeConstants.INPUT_ENCODING, RuntimeConstants.ENCODING_DEFAULT),
				              context, writer);
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
			Template template = runtimeInstance.GetTemplate(templateName, encoding);

			if (template == null)
			{
				runtimeInstance.Error(string.Format("Velocity.parseTemplate() failed loading template '{0}'", templateName));
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
			return runtimeInstance.GetTemplate(name);
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
			return runtimeInstance.GetTemplate(name, encoding);
		}

		/// <summary>
		/// Determines if a template is accessible via the currently
		/// configured resource loaders.
		/// <br/><br/>
		/// Note that the current implementation will <b>not</b>
		/// change the state of the system in any real way - so this
		/// cannot be used to pre-load the resource cache, as the
		/// previous implementation did as a side-effect.
		/// <br/><br/>
		/// The previous implementation exhibited extreme laziness and
		/// sloth, and the author has been flogged.
		/// </summary>
		/// <param name="templateName"> name of the template to search for
		/// </param>
		/// <returns>true if found, false otherwise
		/// </returns>
		public bool TemplateExists(String templateName)
		{
			return (runtimeInstance.GetLoaderNameForResource(templateName) != null);
		}

		/// <summary>
		/// Log a warning message.
		/// </summary>
		/// <param name="message">message to log</param>
		public void Warn(Object message)
		{
			runtimeInstance.Warn(message);
		}

		///
		/// <summary>
		/// Log an info message.
		/// </summary>
		/// <param name="message">message to log</param>
		public void Info(Object message)
		{
			runtimeInstance.Info(message);
		}

		/// <summary>
		/// Log an error message.
		/// </summary>
		/// <param name="message">message to log</param>
		public void Error(Object message)
		{
			runtimeInstance.Error(message);
		}

		/// <summary>
		/// Log a debug message.
		/// </summary>
		/// <param name="message">message to log</param>
		public void Debug(Object message)
		{
			runtimeInstance.Debug(message);
		}

		/// <summary>
		/// <p>
		/// Set the an ApplicationAttribute, which is an Object
		/// set by the application which is accessible from
		/// any component of the system that gets a RuntimeServices.
		/// This allows communication between the application
		/// environment and custom pluggable components of the
		/// Velocity engine, such as loaders and loggers.
		/// </p>
		/// <p>
		/// Note that there is no enforcement or rules for the key
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
			runtimeInstance.SetApplicationAttribute(key, value);
		}
	}
}