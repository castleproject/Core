namespace NVelocity.Util.Introspection
{
	using System;

	/// <summary>  
	/// Little class to carry in info such as template name, line and column
	/// for information error reporting from the uberspector implementations
	/// *
	/// </summary>
	/// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version>  $Id: Info.cs,v 1.1 2004/12/27 05:55:08 corts Exp $
	/// 
	/// </version>
	public class Info
	{
		private int line;
		private int column;
		private String templateName;

		/// <param name="source">Usually a template name.
		/// </param>
		/// <param name="line">The line number from <code>source</code>.
		/// </param>
		/// <param name="column">The column number from <code>source</code>.
		/// 
		/// </param>
		public Info(String source, int line, int column)
		{
			this.templateName = source;
			this.line = line;
			this.column = column;
		}

		public String TemplateName
		{
			get { return templateName; }
		}

		public int Line
		{
			get { return line; }
		}

		public int Column
		{
			get { return column; }
		}

		/// <summary> Formats a textual representation of this object as <code>SOURCE
		/// [line X, column Y]</code>.
		/// </summary>
		public override String ToString()
		{
			return TemplateName + " [line " + Line + ", column " + Column + ']';
		}
	}
}