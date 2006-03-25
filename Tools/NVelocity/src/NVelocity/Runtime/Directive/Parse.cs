namespace NVelocity.Runtime.Directive
{
	using System;
	using System.IO;
	using System.Text;
	using NVelocity.Context;
	using NVelocity.Exception;
	using NVelocity.Runtime.Parser.Node;
	using NVelocity.Runtime.Resource;
	using Node = Parser.Node.INode;

	/// <summary>
	/// Pluggable directive that handles the #parse() statement in VTL.
	/// 
	/// Notes:
	/// -----
	/// 1) The parsed source material can only come from somewhere in
	/// the TemplateRoot tree for security reasons. There is no way
	/// around this.  If you want to include content from elsewhere on
	/// your disk, use a link from somwhere under Template Root to that
	/// content.
	/// 
	/// 2) There is a limited parse depth.  It is set as a property
	/// "parse_directive.maxdepth = 10"  for example.  There is a 20 iteration
	/// safety in the event that the parameter isn't set.
	/// 
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a> </author>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a> </author>
	/// <author> <a href="mailto:Christoph.Reck@dlr.de">Christoph Reck<a> </author>
	/// <version> $Id: Parse.cs,v 1.4 2003/10/27 13:54:10 corts Exp $ </version>
	public class Parse : Directive
	{
		/// <summary>
		/// Return name of this directive.
		/// </summary>
		public override String Name
		{
			get { return "parse"; }
			set { throw new NotSupportedException(); }
		}

		/// <summary> Return type of this directive. </summary>
		public override DirectiveType Type
		{
			get { return DirectiveType.LINE; }
		}

		/// <summary>
		/// iterates through the argument list and renders every
		/// argument that is appropriate.  Any non appropriate
		/// arguments are logged, but render() continues.
		/// </summary>
		public override bool Render(IInternalContextAdapter context, TextWriter writer, Node node)
		{
			// did we get an argument?
            if(!this.AssertArgument(node)) return false;

			// does it have a value?  If you have a null reference, then no.
            Object value_;
            if(!this.AssertNodeHasValue(node, context, out value_)) return false;

			// get the path
			String arg = value_.ToString();

            AssertTemplateStack(context);

			Resource current = context.CurrentResource;

			// get the resource, and assume that we use the encoding of the current template
	        // the 'current resource' can be null if we are processing a stream....
			String encoding = null;

			if (current != null)
				encoding = current.Encoding;
			else
				encoding = (String) rsvc.GetProperty(RuntimeConstants.INPUT_ENCODING);

			// now use the Runtime resource loader to get the template
			Template t = null;

            t = this.GetTemplate(arg, encoding, context);
            if(t == null) return false;

			// and render it
            if(!this.RenderTemplate(t, arg, writer, context)) return false;

			return true;
		}

        private bool AssertArgument(Node node)
        {
            bool result = true;
            if(node.GetChild(0) == null)
            {
                rsvc.Error("#parse() error :  null argument");
                result = false;
            }
            return result;
        }
        private bool AssertNodeHasValue(Node node, IInternalContextAdapter context, out Object value)
        {
            bool result = true;

            value = node.GetChild(0).Value(context);
            
            if(value == null)
            {
                rsvc.Error("#parse() error :  null argument");
                result = false;
            }
            return result;
        }
        
        /// <summary>
        /// See if we have exceeded the configured depth.
        /// If it isn't configured, put a stop at 20 just in case.
        /// </summary>
        private bool AssertTemplateStack(IInternalContextAdapter context)
        {
            bool result = true;
            Object[] templateStack = context.TemplateNameStack;

            if(templateStack.Length >= rsvc.GetInt(RuntimeConstants.PARSE_DIRECTIVE_MAXDEPTH, 20))
            {
                StringBuilder path = new StringBuilder();

                for(int i = 0; i < templateStack.Length; ++i)
                    path.Append(" > " + templateStack[i]);

                rsvc.Error("Max recursion depth reached (" + templateStack.Length + ")" + " File stack:" + path);
                result = false;
            }
            return result;
        }


        private Template GetTemplate(String arg, String encoding, IInternalContextAdapter context)
        {
            Template result = null;
            try
            {
                result = rsvc.GetTemplate(arg, encoding);
            }
            catch(ResourceNotFoundException rnfe)
            {
                // the arg wasn't found.  Note it and throw
                rsvc.Error("#parse(): cannot find template '" + arg + "', called from template " + context.CurrentTemplateName + " at (" + Line + ", " + Column + ")");
                throw;
            }
            catch(ParseErrorException pee)
            {
                // the arg was found, but didn't parse - syntax error
                // note it and throw
                rsvc.Error("#parse(): syntax error in #parse()-ed template '" + arg + "', called from template " + context.CurrentTemplateName + " at (" + Line + ", " + Column + ")");

                throw;
            }
            catch(Exception e)
            {
                rsvc.Error("#parse() : arg = " + arg + ".  Exception : " + e);
                result = null;
            }
            return result;
        }

        private bool RenderTemplate(Template template, String arg, TextWriter writer, IInternalContextAdapter context)
        {
            bool result = true;
            try
            {
                context.PushCurrentTemplateName(arg);
                ((SimpleNode)template.Data).Render(context, writer);
            }
            catch(Exception e)
            {
                // if it's a MIE, it came from the render.... throw it...
                if(e is MethodInvocationException)
                    throw;

                rsvc.Error("Exception rendering #parse( " + arg + " )  : " + e);
                result = false;
            }
            finally
            {
                context.PopCurrentTemplateName();
            }

            return result;
        }
	}
}
