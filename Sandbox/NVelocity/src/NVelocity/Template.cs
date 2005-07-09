namespace NVelocity
{
	using System;
	using System.IO;

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
	/// <pre>
	/// Template template = Velocity.getTemplate("test.wm");
	/// IContext context = new VelocityContext();
	/// 
	/// context.Put("foo", "bar");
	/// context.Put("customer", new Customer());
	/// 
	/// template.Merge(context, writer);
	/// </pre>
	/// </summary>
	public class Template : Resource
	{
		/// <summary>
		/// To keep track of whether this template has been
		/// initialized. We use the document.init(context)
		/// to perform this.
		/// </summary>
		// private bool initialized = false;

		private System.Exception errorCondition = null;

		/// <summary>
		/// Default constructor
		/// </summary>
		public Template()
		{
		}

		/// <summary>
		/// gets the named resource as a stream, parses and inits
		/// </summary>
		/// <returns>true if successful
		/// @throws ResourceNotFoundException if template not found
		/// from any available source.
		/// @throws ParseErrorException if template cannot be parsed due
		/// to syntax (or other) error.
		/// @throws Exception some other problem, should only be from
		/// initialization of the template AST.
		/// </returns>
		public override bool Process()
		{
			data = null;
			Stream is_Renamed = null;
			errorCondition = null;

			// first, try to get the stream from the loader
			try
			{
				is_Renamed = resourceLoader.getResourceStream(name);
			}
			catch (ResourceNotFoundException rnfe)
			{
				//  remember and re-throw
				errorCondition = rnfe;
				throw rnfe;
			}

			// if that worked, lets protect in case a loader impl
	        // forgets to throw a proper exception

			if (is_Renamed != null)
			{
				// now parse the template
				try
				{
					StreamReader br = new StreamReader(is_Renamed, System.Text.Encoding.GetEncoding(encoding));

					data = rsvc.parse(br, name);
					InitDocument();
					return true;
				}
				catch (IOException uce)
				{
					String msg = "Template.process : Unsupported input encoding : " + encoding + " for template " + name;

					errorCondition = new ParseErrorException(msg);
					throw errorCondition;
				}
				catch (ParseException pex)
				{
					// remember the error and convert
					errorCondition = new ParseErrorException(pex.Message);
					throw errorCondition;
				}
				catch (System.Exception e)
				{
					// who knows?  Something from initDocument()
					errorCondition = e;
					throw e;
				}
				finally
				{
					// Make sure to close the inputstream when we are done.
					is_Renamed.Close();
				}
			}
			else
			{
		        // is == null, therefore we have some kind of file issue
				errorCondition = new ResourceNotFoundException("Unknown resource error for resource " + name);
				throw errorCondition;
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
			/*
	    *  send an empty InternalContextAdapter down into the AST to initialize it
	    */
			InternalContextAdapterImpl ica = new InternalContextAdapterImpl(new VelocityContext());

			try
			{
				/*
		*  put the current template name on the stack
		*/
				ica.PushCurrentTemplateName(name);

				/*
		*  init the AST
		*/
				((SimpleNode) data).init(ica, rsvc);
			}
			finally
			{
				/*
		*  in case something blows up...
		*  pull it off for completeness
		*/
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
		/// <param name="writer">
		/// output writer for rendered template
		/// @throws ResourceNotFoundException if template not found
		/// from any available source.
		/// @throws ParseErrorException if template cannot be parsed due
		/// to syntax (or other) error.
		/// @throws  Exception  anything else.
		/// </param>
		public void Merge(IContext context, TextWriter writer)
		{
			/*
	    *  we shouldn't have to do this, as if there is an error condition, 
	    *  the application code should never get a reference to the 
	    *  Template
	    */

			if (errorCondition != null)
			{
				throw errorCondition;
			}

			if (data != null)
			{
				/*
		*  create an InternalContextAdapter to carry the user Context down
		*  into the rendering engine.  Set the template name and render()
		*/
				InternalContextAdapterImpl ica = new InternalContextAdapterImpl(context);

				try
				{
					ica.PushCurrentTemplateName(name);
					ica.CurrentResource = this;

					((SimpleNode) data).render(ica, writer);
				}
				finally
				{
					/*
		    *  lets make sure that we always clean up the context 
		    */
					ica.PopCurrentTemplateName();
					ica.CurrentResource = null;
				}
			}
			else
			{
				/*
		* this shouldn't happen either, but just in case.
		*/
				String msg = "Template.merge() failure. The document is null, " + "most likely due to parsing error.";

				rsvc.error(msg);
				throw new System.Exception(msg);
			}
		}
	}
}