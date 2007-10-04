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
	public class FixTryGetParameterConditionalChecks : AbstractNamespaceSensitiveVisitorCompilerStep
	{
		public override void Run()
		{
			Visit(CompileUnit);
		}

		public override void OnIfStatement(IfStatement node)
		{
			node.Condition = FixCondition(node.Condition);
		}

		public override void OnUnlessStatement(UnlessStatement node)
		{
			node.Condition = FixCondition(node.Condition);
		}

		private Expression FixCondition(Expression condition)
		{
			MethodInvocationExpression mie = condition as MethodInvocationExpression;
			if (mie == null)
				return condition;

			ReferenceExpression expression = mie.Target as ReferenceExpression;
			if (expression == null || expression.Name != "TryGetParameter")
				return condition;


			TypeofExpression typeofExpression = new TypeofExpression();
			typeofExpression.Type = CodeBuilder.CreateTypeReference(typeof(IgnoreNull));

			BinaryExpression be = new BinaryExpression(BinaryOperatorType.TypeTest, condition,
			                                           typeofExpression);

			return new UnaryExpression(UnaryOperatorType.LogicalNot, be);
		}
	}
}
