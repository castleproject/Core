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

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Ast.Visitors


abstract class OutputMethodAttribute(AbstractAstAttribute):
	
	def Apply(node as Node):
		method = node as Method
		if method is null:
			InvalidNodeForAttribute("Method")
			return
		
		method.Body.Accept(ReturnValueVisitor())
		ReplaceReferences(method.Body, "transform", TransformMethodName)
		
	
	abstract TransformMethodName:
		get:
			pass
	
	def ReplaceReferences(node as Node, what as string, value as string):
		node.ReplaceNodes(
			ReferenceExpression(what),
			AstUtil.CreateReferenceExpression(value))
	
class HtmlAttribute(OutputMethodAttribute):
	
	override TransformMethodName:
		get:
			return "Castle.MonoRail.Views.Brail.HtmlAttribute.Transform"
			
	static def Transform(original as string) as string:
		return System.Web.HttpUtility.HtmlEncode(original)		
	
	static def Transform(original) as string:
		return Transform(original.ToString)
	
class RawAttribute(OutputMethodAttribute):
	override TransformMethodName:
		get:
			return "Castle.MonoRail.Views.Brail.RawAttribute.Transform"
			
	static def Transform(original as string) as string:
		return original
	
	static def Transform(original) as string:
		return Transform(original.ToString)
	
class MarkDownAttribute(OutputMethodAttribute):
	override TransformMethodName:
		get:
			return "Castle.MonoRail.Views.Brail.MarkDownAttribute.Transform"
			
	static def Transform(original as string) as string:
		markdown = anrControls.Markdown()
		return markdown.Transform(original)
		
	static def Transform(original) as string:
		return Transform(original.ToString)
