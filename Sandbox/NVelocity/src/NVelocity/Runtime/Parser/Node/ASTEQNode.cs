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

	/// <summary>  Handles the equivalence operator
	/// *
	/// <arg1>  == <arg2>
	/// *
	/// This operator requires that the LHS and RHS are both of the
	/// same Class.
	/// *
	/// </summary>
	/// <version> $Id: ASTEQNode.cs,v 1.3 2003/10/27 13:54:10 corts Exp $
	///
	/// </version>
	public class ASTEQNode : SimpleNode
	{
		public ASTEQNode(int id) : base(id)
		{
		}

		public ASTEQNode(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>Accept the visitor. *
		/// </summary>
		public override Object jjtAccept(ParserVisitor visitor, Object data)
		{
			return visitor.visit(this, data);
		}

		/// <summary>   Calculates the value of the logical expression
		/// *
		/// arg1 == arg2
		/// *
		/// All class types are supported.   Uses equals() to
		/// determine equivalence.  This should work as we represent
		/// with the types we already support, and anything else that
		/// implements equals() to mean more than identical references.
		/// *
		/// *
		/// </summary>
		/// <param name="context"> internal context used to evaluate the LHS and RHS
		/// </param>
		/// <returns>true if equivalent, false if not equivalent,
		/// false if not compatible arguments, or false
		/// if either LHS or RHS is null
		///
		/// </returns>
		public override bool evaluate(InternalContextAdapter context)
		{
			Object left = jjtGetChild(0).Value(context);
			Object right = jjtGetChild(1).Value(context);

			/*
			 * for equality, they are allowed to be null references 
			 */
			try
			{
				if ( ObjectComparer.CompareObjects( left, right ) == 0 )
					return true;
			}
			catch
			{
				// Ignore, we can't compare decently by value, but we honestly don't give a sh*t
			}

			// They are not equal by value, try a reference comparison
			// reference equal => definitely equal objects ;)
			// For operator overloaded types, this will not really be a reference comp, but that's ok.
			return left == right;
			
		}
	}
}
