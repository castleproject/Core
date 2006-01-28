// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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


using NVelocity.Runtime.Directive;
using NVelocity.Context;
using NVelocity.Runtime.Parser.Node;

namespace Castle.MonoRail.Framework.Views.NVelocity.CustomDirectives
{
	using System;
	using System.IO;
	using System.Text;

	public class CaptureForDirective : Directive
	{
		/// <summary>
		/// Render's the contents of the directive and store them in the context
		/// variable so it can be referenced later on the template
		///
		/// #capturefor(someId)
		///		Some content goes here
		/// #end
		///
		/// $someId
		/// </summary>
		public CaptureForDirective()
		{
		}

		public override bool Render( IInternalContextAdapter context, TextWriter writer, INode node )
		{
			if (node.ChildrenCount != 2)
			{
				throw new RailsException("#capturefor directive expects an id attribute and a template block");
			}

			ASTWord idNode = node.GetChild(0) as ASTWord;
			ASTBlock bodyNode = node.GetChild(1) as ASTBlock;

			string id = idNode.Literal;

			StringWriter buffer = new StringWriter();
			StringBuilder sb = buffer.GetStringBuilder();

			bodyNode.Render(context, buffer);

			String currentContent = context[id] as string;

			if( currentContent != null )
			{
				sb.Append(currentContent);
			}

			context[id] = sb.ToString();

			return true;
		}

		public override String Name
		{
			get { return "capturefor"; }
			set {  }
		}

		public override DirectiveType Type
		{
			get { return DirectiveType.BLOCK; }
		}
	}
}
