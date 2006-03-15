namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.IO;
	using NVelocity.Context;
	using NVelocity.Runtime.Directive;

	/// <summary>
	/// This class is responsible for handling the pluggable
	/// directives in VTL. ex.  #foreach()
	///
	/// Please look at the Parser.jjt file which is
	/// what controls the generation of this class.
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	/// <author> <a href="mailto:kav@kav.dk">Kasper Nielsen</a></author>
	/// <version> $Id: ASTDirective.cs,v 1.3 2003/10/27 13:54:10 corts Exp $</version>
	public class ASTDirective : SimpleNode
	{
		/// <summary>
		/// Gets or sets the directive name.
		/// Used by the parser.  
		/// This keeps us from having to
		/// dig it out of the token stream and gives the parse 
		/// the change to override.
		/// </summary>
		public String DirectiveName
		{
			get { return directiveName; }
			set { directiveName = value; }
		}

		private Directive directive;
		private String directiveName = "";
		private bool isDirective;

		public ASTDirective(int id) : base(id)
		{
		}

		public ASTDirective(Parser p, int id) : base(p, id)
		{
		}


		/// <summary>
		/// Accept the visitor.
		/// </summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		public override Object Init(IInternalContextAdapter context, Object data)
		{
			base.Init(context, data);

			// only do things that are not context dependant
			if (parser.IsDirective(directiveName))
			{
				isDirective = true;

				// create a new instance of the directive
				directive = parser.GetDirective(directiveName);
				directive.Init(rsvc, context, this);
				directive.SetLocation(Line, Column);
			}
			else if (rsvc.IsVelocimacro(directiveName, context.CurrentTemplateName))
			{
				// we seem to be a Velocimacro.
				isDirective = true;
				directive = rsvc.GetVelocimacro(directiveName, context.CurrentTemplateName);

				directive.Init(rsvc, context, this);
				directive.SetLocation(Line, Column);
			}
			else
			{
				isDirective = false;
			}

			return data;
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer)
		{
			// normal processing
			if (isDirective)
			{
				directive.Render(context, writer, this);
			}
			else
			{
				writer.Write("#");
				writer.Write(directiveName);
			}

			return true;
		}
	}
}
