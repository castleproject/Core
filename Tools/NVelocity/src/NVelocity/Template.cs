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

namespace NVelocity
{
	using System;
	using System.IO;
	using Context;
	using Exception;
	using NVelocity.Runtime.Parser;
	using NVelocity.Runtime.Parser.Node;
	using NVelocity.Runtime.Resource;

	/// <summary>
	/// This class is used for controlling all template
	/// operations. This class uses a parser created
	/// by JavaCC to create an AST that is subsequently
	/// traversed by a Visitor.
	/// 
	/// <code>
	/// Template template = Velocity.getTemplate("test.wm");
	/// IContext context = new VelocityContext();
	/// 
	/// context.Put("foo", "bar");
	/// context.Put("customer", new Customer());
	/// 
	/// template.Merge(context, writer);
	/// </code>
	/// </summary>
	public class Template : Resource
	{
		private System.Exception errorCondition = null;


		/// <summary>
		/// Gets the named resource as a stream, parses and inits.
		/// </summary>
		/// <returns>true if successful</returns>
		/// <exception cref="ResourceNotFoundException">
		/// if template not found from any available source.
		/// </exception>
		/// <exception cref="ParseErrorException">
		/// if template cannot be parsed due to syntax (or other) error.
		/// </exception>
		/// <exception cref="System.Exception">
		/// some other problem, should only be from initialization of the template AST.
		/// </exception>
		public override bool Process()
		{
			data = null;
			errorCondition = null;

			// first, try to get the stream from the loader
			Stream s = ObtainStream();

			// if that worked, lets protect in case a loader impl
			// forgets to throw a proper exception

			if (s != null)
			{
				// now parse the template
				try
				{
					StreamReader br = new StreamReader(s, System.Text.Encoding.GetEncoding(encoding));

					data = runtimeServices.Parse(br, name);
					InitDocument();
					return true;
				}
				catch(IOException ioException)
				{
					String msg = string.Format("Template.process : Unsupported input encoding : {0} for template {1}", encoding, name);

					throw errorCondition = new ParseErrorException(msg, ioException);
				}
				catch(ParseException parseException)
				{
					// remember the error and convert
					throw errorCondition = new ParseErrorException(parseException.Message, parseException);
				}
				catch(System.Exception e)
				{
					// who knows?  Something from initDocument()
					errorCondition = e;
					throw;
				}
				finally
				{
					// Make sure to close the inputstream when we are done.
					s.Close();
				}
			}
			else
			{
				// is == null, therefore we have some kind of file issue
				throw errorCondition = new ResourceNotFoundException(string.Format("Unknown resource error for resource {0}", name));
			}
		}

		/// <summary> 
		/// initializes the document.  init() is not longer
		/// dependant upon context, but we need to let the
		/// init() carry the template name down through for VM
		/// namespace features
		/// </summary>
		public void InitDocument()
		{
			// send an empty InternalContextAdapter down into the AST to initialize it
			InternalContextAdapterImpl internalContextAdapterImpl = new InternalContextAdapterImpl(new VelocityContext());

			try
			{
				// put the current template name on the stack
				internalContextAdapterImpl.PushCurrentTemplateName(name);

				// init the AST
				((SimpleNode) data).Init(internalContextAdapterImpl, runtimeServices);
			}
			finally
			{
				// in case something blows up...
				// pull it off for completeness
				internalContextAdapterImpl.PopCurrentTemplateName();
			}
		}

		/// <summary>
		/// The AST node structure is merged with the
		/// context to produce the final output.
		/// 
		/// Throws IOException if failure is due to a file related
		/// issue, and Exception otherwise
		/// </summary>
		/// <param name="context">Context with data elements accessed by template</param>
		/// <param name="writer">writer for rendered template</param>
		/// <exception cref="ResourceNotFoundException">
		/// if template not found from any available source.
		/// </exception>
		/// <exception cref="ParseErrorException">
		/// if template cannot be parsed due to syntax (or other) error.
		/// </exception>
		/// <exception cref="System.Exception">
		/// anything else.
		/// </exception>
		public void Merge(IContext context, TextWriter writer)
		{
			// we shouldn't have to do this, as if there is an error condition, 
			// the application code should never get a reference to the 
			// Template
			if (errorCondition != null)
			{
				throw errorCondition;
			}

			if (data != null)
			{
				// create an InternalContextAdapter to carry the user Context down
				// into the rendering engine.  Set the template name and render()
				InternalContextAdapterImpl internalContextAdapterImpl = new InternalContextAdapterImpl(context);

				try
				{
					internalContextAdapterImpl.PushCurrentTemplateName(name);
					internalContextAdapterImpl.CurrentResource = this;

					((SimpleNode) data).Render(internalContextAdapterImpl, writer);
				}
				finally
				{
					// lets make sure that we always clean up the context 
					internalContextAdapterImpl.PopCurrentTemplateName();
					internalContextAdapterImpl.CurrentResource = null;
				}
			}
			else
			{
				// this shouldn't happen either, but just in case.
				String msg = "Template.merge() failure. The document is null, most likely due to parsing error.";

				runtimeServices.Error(msg);
				throw new System.Exception(msg);
			}
		}

		protected virtual Stream ObtainStream()
		{
			try
			{
				return resourceLoader.GetResourceStream(name);
			}
			catch(ResourceNotFoundException resourceNotFoundException)
			{
				//  remember and re-throw
				errorCondition = resourceNotFoundException;
				throw;
			}
		}
	}
}