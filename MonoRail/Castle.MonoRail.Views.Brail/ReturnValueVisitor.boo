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
namespace Castle.MonoRail.Views.Brail

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps

class ReturnValueVisitor(DepthFirstVisitor):

	normalizer as NormalizeStatementModifiers
	mie as MethodInvocationExpression
	
	[Property(Found)]
	found = false
	
	def constructor():
	
		normalizer = NormalizeStatementModifiers()
		mie = MethodInvocationExpression(
			Target: AstUtil.CreateReferenceExpression("transform") )
		super()

	override def OnReturnStatement(stmt as ReturnStatement):
		# First normalize the statement
		normalizer.Visit(stmt)
		#empty return, so error
		if stmt.Expression is null:
			raise Exception("An empty return statement on a method with output attribute")
			
		found = true
		block as Block = stmt.ParentNode 
		index = 0
		while block.Statements[index] != stmt:
			index ++
		
		invocation = mie.CloneNode()
		invocation.Arguments.Add(stmt.Expression)
		
		stmt.Expression = invocation
