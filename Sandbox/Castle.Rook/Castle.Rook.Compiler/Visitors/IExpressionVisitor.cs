// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Rook.Compiler.Visitors
{
	using System;

	using Castle.Rook.Compiler.AST;


	public interface IExpressionAttrVisitor
	{
		IExpression VisitAssignmentExpression(AssignmentExpression assignExp);
		IExpression VisitAugAssignmentExpression(AugAssignmentExpression auAssignExp);
		IExpression VisitYieldExpression(YieldExpression yieldExpression);
		IExpression VisitVariableReferenceExpression(VariableReferenceExpression variableReferenceExpression);
		IExpression VisitUnaryExpression(UnaryExpression unaryExpression);
		IExpression VisitRetryExpression(RetryExpression expression);
		IExpression VisitNextExpression(NextExpression expression);
		IExpression VisitRedoExpression(RedoExpression expression);
		IExpression VisitRangeExpression(RangeExpression rangeExpression);
		IExpression VisitRaiseExpression(RaiseExpression expression);
		IExpression VisitMethodInvocationExpression(MethodInvocationExpression invocationExpression);
		IExpression VisitMemberAccessExpression(MemberAccessExpression accessExpression);
		IExpression VisitLiteralReferenceExpression(LiteralReferenceExpression expression);
		IExpression VisitListExpression(ListExpression expression);
		IExpression VisitLambdaExpression(LambdaExpression expression);
		IExpression VisitIfStatement(IfStatement ifStatement);
		IExpression VisitForStatement(ForStatement statement);
		IExpression VisitExpressionStatement(ExpressionStatement statement);
		IExpression VisitDictExpression(DictExpression expression);
		IExpression VisitCompoundExpression(CompoundExpression expression);
		IExpression VisitBreakExpression(BreakExpression breakExpression);
		IExpression VisitBlockExpression(BlockExpression expression);
		IExpression VisitBinaryExpression(BinaryExpression expression);
		IExpression VisitBaseReferenceExpression(BaseReferenceExpression expression);
		IExpression VisitSelfReferenceExpression(SelfReferenceExpression expression);
		IExpression VisitNullCheckExpression(NullCheckExpression expression);
	}
}
