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

namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using Context;

	/// <summary>
    /// Handles integer division of nodes
	/// 
	/// Please look at the Parser.jjt file which is
	/// what controls the generation of this class.
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a> </author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a> </author>
	/// <version> $Id: ASTDivNode.cs,v 1.3 2003/10/27 13:54:10 corts Exp $ </version>
	public class ASTDivNode : SimpleNode
	{
		public ASTDivNode(int id) : base(id)
		{
		}

		public ASTDivNode(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>
		/// Accept the visitor.
		/// </summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		/// <summary>
		/// Computes the result of the division. Currently limited to Integers.
		/// </summary>
		/// <returns>Integer(value) or null</returns>
		public override Object Value(IInternalContextAdapter context)
		{
			// get the two args
			Object left = GetChild(0).Value(context);
			Object right = GetChild(1).Value(context);

			// if either is null, lets log and bail
			if (left == null || right == null)
			{
				runtimeServices.Error(
					string.Format(
						"{0} side ({1}) of division operation has null value. Operation not possible. {2} [line {3}, column {4}]",
						(left == null ? "Left" : "Right"), GetChild((left == null ? 0 : 1)).Literal, context.CurrentTemplateName, Line,
						Column));
				return null;
			}

			Type maxType = MathUtil.ToMaxType(left.GetType(), right.GetType());
			if (maxType == null)
			{
				return null;
			}

			try
			{
				return MathUtil.Div(maxType, left, right);
			}
			catch (DivideByZeroException)
			{
				runtimeServices.Error("Right side of division operation is zero. Must be non-zero. " + context.CurrentTemplateName + " [line " + Line + ", column " + Column + "]");
			}
			return null;
		}
	}
}