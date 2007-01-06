// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.Brail
{
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.Steps;
	using Castle.MonoRail.Framework;

	//This will search for all return statements in a method
	//and will replace them with a method call (only a marker)
	//called transform, later, the output attribute will replace it 
	//with the correct method call.
	public class ReturnValueVisitor : DepthFirstVisitor
	{
		private NormalizeStatementModifiers normalizer;
		private MethodInvocationExpression mie;
		private bool found = false;

		public bool Found
		{
			get { return found; }
		}

		public ReturnValueVisitor()
		{
			normalizer = new NormalizeStatementModifiers();
			mie = new MethodInvocationExpression();
			mie.Target = AstUtil.CreateReferenceExpression("transform");
		}

		public override void OnReturnStatement(ReturnStatement stmt)
		{
			// First normalize the statement
			normalizer.Visit(stmt);
			//empty return, so error
			if (stmt.Expression == null)
				throw new RailsException("An empty return statement on a method with output attribute");
			found = true;
			Block block = (Block) stmt.ParentNode;
			int index = 0;
			while(block.Statements[index] != stmt)
			{
				index ++;
			}

			MethodInvocationExpression invocation = mie.CloneNode();
			invocation.Arguments.Add(stmt.Expression);

			stmt.Expression = invocation;
		}
	}
}
