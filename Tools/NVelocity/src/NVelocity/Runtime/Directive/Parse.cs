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

namespace NVelocity.Runtime.Directive
{
	using System;
	using System.IO;
	using System.Text;
	using Context;
	using NVelocity.Exception;
	using NVelocity.Runtime.Parser.Node;
	using Resource;

	/// <summary>
	/// Pluggable directive that handles the #parse() statement in VTL.
	/// 
	/// Notes:
	/// -----
	/// 1) The parsed source material can only come from somewhere in
	/// the TemplateRoot tree for security reasons. There is no way
	/// around this.  If you want to include content from elsewhere on
	/// your disk, use a link from somewhere under Template Root to that
	/// content.
	/// 
	/// 2) There is a limited parse depth.  It is set as a property
	/// "parse_directive.maxdepth = 10"  for example.  There is a 20 iteration
	/// safety in the event that the parameter isn't set.
	/// 
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a> </author>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a> </author>
	/// <author> <a href="mailto:Christoph.Reck@dlr.de">Christoph Reck</a> </author>
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
		public override bool Render(IInternalContextAdapter context, TextWriter writer, INode node)
		{
			// did we get an argument?
			if (!AssertArgument(node))
			{
				return false;
			}

			// does it have a value?  If you have a null reference, then no.
			Object value;
			if (!AssertNodeHasValue(node, context, out value))
			{
				return false;
			}

			// get the path
			String arg = value.ToString();

			AssertTemplateStack(context);

			Resource current = context.CurrentResource;

			// get the resource, and assume that we use the encoding of the current template
			// the 'current resource' can be null if we are processing a stream....
			String encoding;

			if (current == null)
			{
				encoding = (String) runtimeServices.GetProperty(RuntimeConstants.INPUT_ENCODING);
			}
			else
			{
				encoding = current.Encoding;
			}

			// now use the Runtime resource loader to get the template
			Template t = null;

			t = GetTemplate(arg, encoding, context);
			if (t == null)
			{
				return false;
			}

			// and render it
			if (!RenderTemplate(t, arg, writer, context))
			{
				return false;
			}

			return true;
		}

		private bool AssertArgument(INode node)
		{
			bool result = true;
			if (node.GetChild(0) == null)
			{
				runtimeServices.Error("#parse() error :  null argument");
				result = false;
			}
			return result;
		}

		private bool AssertNodeHasValue(INode node, IInternalContextAdapter context, out Object value)
		{
			bool result = true;

			value = node.GetChild(0).Value(context);

			if (value == null)
			{
				runtimeServices.Error("#parse() error :  null argument");
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

			if (templateStack.Length >= runtimeServices.GetInt(RuntimeConstants.PARSE_DIRECTIVE_MAXDEPTH, 20))
			{
				StringBuilder path = new StringBuilder();

				for(int i = 0; i < templateStack.Length; ++i)
				{
					path.AppendFormat(" > {0}", (object[]) templateStack[i]);
				}

				runtimeServices.Error(string.Format("Max recursion depth reached ({0}) File stack:{1}", templateStack.Length, path));
				result = false;
			}
			return result;
		}


		private Template GetTemplate(String arg, String encoding, IInternalContextAdapter context)
		{
			Template result;
			try
			{
				result = runtimeServices.GetTemplate(arg, encoding);
			}
			catch(ResourceNotFoundException)
			{
				// the arg wasn't found.  Note it and throw
				runtimeServices.Error(
					string.Format("#parse(): cannot find template '{0}', called from template {1} at ({2}, {3})", arg,
					              context.CurrentTemplateName, Line, Column));
				throw;
			}
			catch(ParseErrorException)
			{
				// the arg was found, but didn't parse - syntax error
				// note it and throw
				runtimeServices.Error(
					string.Format("#parse(): syntax error in #parse()-ed template '{0}', called from template {1} at ({2}, {3})", arg,
					              context.CurrentTemplateName, Line, Column));

				throw;
			}
			catch(Exception e)
			{
				runtimeServices.Error(string.Format("#parse() : arg = {0}.  Exception : {1}", arg, e));
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
				((SimpleNode) template.Data).Render(context, writer);
			}
			catch(Exception)
			{
				// if it's a MIE, it came from the render.... throw it...
				// if (e is MethodInvocationException)
				throw;

				// runtimeServices.Error("Exception rendering #parse( " + arg + " )  : " + e);
				// result = false;
			}
			finally
			{
				context.PopCurrentTemplateName();
			}

			return result;
		}
	}
}