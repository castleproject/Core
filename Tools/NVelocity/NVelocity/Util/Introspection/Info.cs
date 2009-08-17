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
		private readonly int line;
		private readonly int column;
		private readonly String templateName;

		/// <param name="source">Usually a template name.
		/// </param>
		/// <param name="line">The line number from <code>source</code>.
		/// </param>
		/// <param name="column">The column number from <code>source</code>.
		/// 
		/// </param>
		public Info(String source, int line, int column)
		{
			templateName = source;
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
			return string.Format("{0} [line {1}, column {2}{3}", TemplateName, Line, Column, ']');
		}
	}
}