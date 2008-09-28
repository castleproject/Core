// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.Steps;

	/// <summary>
	/// We need to handle the following code:
	/// if ?Error:
	/// end
	/// 
	/// However, because ?Op will return an IgnoreNull object, we can't just
	/// use the default boo behavior, but need to fix it up
	/// </summary>
	public class FixTryGetParameterConditionalChecks : AbstractNamespaceSensitiveTransformerCompilerStep
	{
		public override void Run()
		{
			Visit(CompileUnit);
		}

		public override void OnBinaryExpression(BinaryExpression node)
		{
			if (node.Operator != BinaryOperatorType.Equality)
				return;

			if (IsTryGetParameterInvocation(node.Left) == false &&
				IsTryGetParameterInvocation(node.Right) == false)
				return;

			MethodInvocationExpression mie = new MethodInvocationExpression();
			ReferenceExpression expression = AstUtil.CreateReferenceExpression("Castle.MonoRail.Views.Brail.IgnoreNull.AreEqual");
			mie.Target = expression;
			mie.Arguments.Add(node.Left);
			mie.Arguments.Add(node.Right);

			ReplaceCurrentNode(mie);
		}


		public override void OnIfStatement(IfStatement node)
		{
			base.OnIfStatement(node);
			node.Condition = FixCondition(node.Condition);
		}

		public override void OnUnlessStatement(UnlessStatement node)
		{
			base.OnUnlessStatement(node);
			node.Condition = FixCondition(node.Condition);
		}

		private Expression FixCondition(Expression condition)
		{
			if (IsTryGetParameterInvocation(condition) == false)
				return condition;

			MemberReferenceExpression isNull =
				new MemberReferenceExpression(condition, "_IsIgnoreNullReferencingNotNullObject_");
			return isNull;
		}

		private static bool IsTryGetParameterInvocation(Expression condition)
		{
			MethodInvocationExpression mie = condition as MethodInvocationExpression;
			if (mie == null)
				return false;
			ReferenceExpression expression = mie.Target as ReferenceExpression;
			if (expression == null)
				return false;
			return expression.Name == "TryGetParameter";
		}
	}
}