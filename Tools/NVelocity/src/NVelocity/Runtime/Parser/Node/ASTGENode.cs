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

	public class ASTGENode : SimpleNode
	{
		public ASTGENode(int id) : base(id)
		{
		}

		public ASTGENode(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>
		/// Accept the visitor.
		/// </summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		public override bool Evaluate(IInternalContextAdapter context)
		{
			// get the two args
			Object left = GetChild(0).Value(context);
			Object right = GetChild(1).Value(context);

			// if either is null, lets log and bail
			if (left == null || right == null)
			{
				runtimeServices.Error(
					string.Format(
						"{0} side ({1}) of '>=' operation has null value. Operation not possible. {2} [line {3}, column {4}]",
						(left == null ? "Left" : "Right"), GetChild((left == null ? 0 : 1)).Literal, context.CurrentTemplateName, Line,
						Column));
				return false;
			}

			try
			{
				return ObjectComparer.CompareObjects(left, right) >= 0;
			}
			catch(ArgumentException ae)
			{
				runtimeServices.Error(ae.Message);

				return false;
			}
		}
	}
}