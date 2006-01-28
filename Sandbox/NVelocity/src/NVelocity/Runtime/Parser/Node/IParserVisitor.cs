namespace NVelocity.Runtime.Parser.Node
{
	using System;

	public interface IParserVisitor
	{
		Object Visit(SimpleNode node, Object data);
		Object Visit(ASTprocess node, Object data);
		Object Visit(ASTComment node, Object data);
		Object Visit(ASTNumberLiteral node, Object data);
		Object Visit(ASTStringLiteral node, Object data);
		Object Visit(ASTIdentifier node, Object data);
		Object Visit(ASTWord node, Object data);
		Object Visit(ASTDirective node, Object data);
		Object Visit(ASTBlock node, Object data);
		Object Visit(ASTObjectArray node, Object data);
		Object Visit(ASTMethod node, Object data);
		Object Visit(ASTReference node, Object data);
		Object Visit(ASTTrue node, Object data);
		Object Visit(ASTFalse node, Object data);
		Object Visit(ASTText node, Object data);
		Object Visit(ASTIfStatement node, Object data);
		Object Visit(ASTElseStatement node, Object data);
		Object Visit(ASTElseIfStatement node, Object data);
		Object Visit(ASTSetDirective node, Object data);
		Object Visit(ASTExpression node, Object data);
		Object Visit(ASTAssignment node, Object data);
		Object Visit(ASTOrNode node, Object data);
		Object Visit(ASTAndNode node, Object data);
		Object Visit(ASTEQNode node, Object data);
		Object Visit(ASTNENode node, Object data);
		Object Visit(ASTLTNode node, Object data);
		Object Visit(ASTGTNode node, Object data);
		Object Visit(ASTLENode node, Object data);
		Object Visit(ASTGENode node, Object data);
		Object Visit(ASTAddNode node, Object data);
		Object Visit(ASTSubtractNode node, Object data);
		Object Visit(ASTMulNode node, Object data);
		Object Visit(ASTDivNode node, Object data);
		Object Visit(ASTModNode node, Object data);
		Object Visit(ASTNotNode node, Object data);
	}
}