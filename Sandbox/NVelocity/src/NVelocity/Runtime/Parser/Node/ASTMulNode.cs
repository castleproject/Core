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

namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using NVelocity.Context;

	/// <summary>
	/// Handles integer multiplication
	/// 
	/// Please look at the Parser.jjt file which is
	/// what controls the generation of this class.
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a> </author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a> </author>
	/// <version> $Id: ASTMulNode.cs,v 1.3 2003/10/27 13:54:10 corts Exp $ </version>
	public class ASTMulNode : SimpleNode
	{
		public ASTMulNode(int id) : base(id)
		{
		}

		public ASTMulNode(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>Accept the visitor. *
		/// </summary>
		public override Object jjtAccept(ParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		/// <summary>
		/// Computes the product of the two args.
		/// Returns null if either arg is null
		/// or if either arg is not an integer
		/// </summary>
		public override Object Value(InternalContextAdapter context)
		{
			// get the two args
			Object left = jjtGetChild(0).Value(context);
			Object right = jjtGetChild(1).Value(context);

			// if either is null, lets log and bail
			if (left == null || right == null)
			{
				rsvc.Error((left == null ? "Left" : "Right") + " side (" + jjtGetChild((left == null ? 0 : 1)).Literal + ") of multiplication operation has null value." + " Operation not possible. " + context.CurrentTemplateName + " [line " + Line + ", column " + Column + "]");
				return null;
			}

			// if not an Integer, not much we can do either
			if (!(left is Int32) || !(right is Int32))
			{
				rsvc.Error((!(left is Int32) ? "Left" : "Right") + " side of multiplication operation is not a valid type. " + "Currently only integers (1,2,3...) and Integer type is supported. " + context.CurrentTemplateName + " [line " + Line + ", column " + Column + "]");

				return null;
			}

			return ((Int32) left)*((Int32) right);
		}
	}
}