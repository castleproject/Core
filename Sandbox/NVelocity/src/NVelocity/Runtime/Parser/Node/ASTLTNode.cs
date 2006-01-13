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

	public class ASTLTNode : SimpleNode
	{
		public ASTLTNode(int id) : base(id)
		{
		}

		public ASTLTNode(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>
		/// Accept the visitor.
		/// </summary>
		public override Object jjtAccept(ParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		public override bool Evaluate(InternalContextAdapter context)
		{
			// get the two args
			Object left = jjtGetChild(0).Value(context);
			Object right = jjtGetChild(1).Value(context);

			// if either is null, lets log and bail
			if (left == null || right == null)
			{
				rsvc.Error((left == null ? "Left" : "Right") + " side (" + jjtGetChild((left == null ? 0 : 1)).Literal + ") of '<' operation has null value." + " Operation not possible. " + context.CurrentTemplateName + " [line " + Line + ", column " + Column + "]");
				return false;
			}

			try
			{
				return ObjectComparer.CompareObjects( left, right ) == ObjectComparer.Smaller;
			}
			catch ( ArgumentException ae )
			{
				rsvc.Error( ae.Message );

				return false;
			}
		}
	}
}
