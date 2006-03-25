namespace NVelocity
{
	using System;
	using System.IO;
	using System.Text;

	using NVelocity.Context;
	using NVelocity.Exception;
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
		/// Default constructor.
		/// </summary>
		public Template()
		{
		}

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
			Stream s = null;
			errorCondition = null;

			// first, try to get the stream from the loader
			s = ObtainStream();

			// if that worked, lets protect in case a loader impl
			// forgets to throw a proper exception

			if (s != null)
			{
				// now parse the template
				try
				{
					StreamReader br = new StreamReader(s, System.Text.Encoding.GetEncoding(encoding));

					data = rsvc.Parse(br, name);
					InitDocument();
					return true;
				}
				catch (IOException uce)
				{
					String msg = "Template.process : Unsupported input encoding : " + encoding + " for template " + name;

					throw errorCondition = new ParseErrorException(msg, uce);
				}
				catch (ParseException pex)
				{
					// remember the error and convert
					throw errorCondition = new ParseErrorException(pex.Message, pex);
				}
				catch (System.Exception e)
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
				throw errorCondition = new ResourceNotFoundException("Unknown resource error for resource " + name);
			}
		}

		/// <summary> 
		/// initializes the document.  init() is not longer
		/// dependant upon context, but we need to let the
		/// init() carry the template name down throught for VM
		/// namespace features
		/// </summary>
		public void InitDocument()
		{
			// send an empty InternalContextAdapter down into the AST to initialize it
			InternalContextAdapterImpl ica = new InternalContextAdapterImpl(new VelocityContext());

			try
			{
				// put the current template name on the stack
				ica.PushCurrentTemplateName(name);

				// init the AST
				((SimpleNode) data).Init(ica, rsvc);
			}
			finally
			{
				// in case something blows up...
				// pull it off for completeness
				ica.PopCurrentTemplateName();
			}
		}

		/// <summary>
		/// The AST node structure is merged with the
		/// context to produce the final output.
		/// 
		/// Throws IOException if failure is due to a file related
		/// issue, and Exception otherwise
		/// </summary>
		/// <param name="context">Conext with data elements accessed by template</param>
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
				InternalContextAdapterImpl ica = new InternalContextAdapterImpl(context);

				try
				{
					ica.PushCurrentTemplateName(name);
					ica.CurrentResource = this;

					((SimpleNode) data).Render(ica, writer);
				}
				finally
				{
					// lets make sure that we always clean up the context 
					ica.PopCurrentTemplateName();
					ica.CurrentResource = null;
				}
			}
			else
			{
				// this shouldn't happen either, but just in case.
				String msg = "Template.merge() failure. The document is null, most likely due to parsing error.";

				rsvc.Error(msg);
				throw new System.Exception(msg);
			}
		}

		protected virtual Stream ObtainStream()
		{
			try
			{
				return resourceLoader.GetResourceStream(name);
			}
			catch (ResourceNotFoundException rnfe)
			{
				//  remember and re-throw
				errorCondition = rnfe;
				throw;
			}
		}
	}
}