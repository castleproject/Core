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
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.TypeSystem

# Replace any uknown identifier with a call to GetParameter('unknown')
# this mean that unknonw identifier in scripts will only fail in run time if they
# were not defined by the controller.
class ReplaceUknownWithParameters(ProcessMethodBodiesWithDuckTyping):
	
	_getParam as IMethod
	
	override def OnReferenceExpression(node as ReferenceExpression):
		entity = self.NameResolutionService.Resolve(node.Name)
		if entity is not null:
			super(node)
		else:
			mie = CodeBuilder.CreateMethodInvocation(
					CodeBuilder.CreateSelfReference(self._currentMethod.DeclaringType),
					_getParam)
			mie.Arguments.Add(CodeBuilder.CreateStringLiteral(node.Name))
			node.ParentNode.Replace(node, mie)
			
	override def InitializeMemberCache():
		super()
		_getParam  = TypeSystemServices.Map(typeof(BrailBase).GetMethod("GetParameter"))
