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
	using Context;
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
		protected internal IRuntimeServices runtimeServices = null;

		private int line = 0;
		private int column = 0;

		/// <summary>
		/// How this directive is to be initialized.
		/// </summary>
		public virtual void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
		{
			runtimeServices = rs;

//			int i, k = node.jjtGetNumChildren();
//			for (i = 0; i < k; i++)
//				node.jjtGetChild(i).init(context, rs);
		}

		public virtual bool SupportsNestedDirective(String name)
		{
			return false;
		}

		public virtual Directive CreateNestedDirective(String name)
		{
			return null;
		}

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
		/// How this directive is to be rendered
		/// </summary>
		public abstract bool Render(IInternalContextAdapter context, TextWriter writer, INode node);
	}
}