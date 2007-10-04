namespace Castle.MonoRail.Views.Brail
{
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
			if(node.Operator!=BinaryOperatorType.Equality)
				return;

			if(IsTryGetParameterInvocation(node.Left) ==false && 
				IsTryGetParameterInvocation(node.Right)  == false )
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


			TypeofExpression typeofExpression = new TypeofExpression();
			typeofExpression.Type = CodeBuilder.CreateTypeReference(typeof(IgnoreNull));

			BinaryExpression be = new BinaryExpression(BinaryOperatorType.TypeTest, condition,
													   typeofExpression);

			return new UnaryExpression(UnaryOperatorType.LogicalNot, be);
		}

		private static bool IsTryGetParameterInvocation(Expression condition)
		{
			ReferenceExpression expression = condition as ReferenceExpression;
			if (expression == null)
				return false;

			return expression.Name.StartsWith("?");
		}
	}
}
