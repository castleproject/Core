namespace NVelocity.Runtime.Directive
{
	using System;
	using System.IO;
	using NVelocity.Context;
	using NVelocity.Runtime.Parser.Node;

	/// <summary>
	/// Directive Types
	/// </summary>
	public enum DirectiveType
	{
		BLOCK = 1,
		LINE = 2,
	}

	/// <summary> Base class for all directives used in Velocity.</summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a> </author>
	/// <version> $Id: Directive.cs,v 1.3 2003/10/27 13:54:10 corts Exp $ </version>
	public abstract class Directive
	{
		/// <summary>
		/// Return the name of this directive
		/// </summary>
		public abstract String Name { get; set; }

		/// <summary>
		/// Get the directive type BLOCK/LINE
		/// </summary>
		public abstract DirectiveType Type { get; }

		/// <summary>
		/// for log msg purposes
		/// </summary>
		public int Line
		{
			get { return line; }
		}

		/// <summary>
		/// for log msg purposes
		/// </summary>
		public int Column
		{
			get { return column; }
		}

		private int line = 0;
		private int column = 0;

		protected internal IRuntimeServices rsvc = null;

		/// <summary>
		/// Allows the template location to be set
		/// </summary>
		public void SetLocation(int line, int column)
		{
			this.line = line;
			this.column = column;
		}

		public virtual bool AcceptParams
		{
			get { return true; }
		}

		/// <summary>
		/// How this directive is to be initialized.
		/// </summary>
		public virtual void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
		{
			rsvc = rs;

			//        int i, k = node.jjtGetNumChildren();

			//for (i = 0; i < k; i++)
			//    node.jjtGetChild(i).init(context, rs);
		}

		/// <summary>
		/// How this directive is to be rendered
		/// </summary>
		public abstract bool Render(IInternalContextAdapter context, TextWriter writer, INode node);
	}
}
