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
import System.IO
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps

#This class is responsible for taking a view script and transforming it to a legible
#code. It does so by doing the following transformations:
# * Add imports to Brail and Castle.MonoRail.Framework
# * Create an overriding method call Run and trasnfer all the global code in the script ot it and then
#   empty all the code in the global scope
# * Create a class that inherit from BrailBase with the same name as the file 
# * Add the Run method to this class
# * Add any higher level elements (classes, methods, etc) to the newly created class
#   and then remove them from the global scope
# * Create a constructor that delegate to BrailBase constructor
class TransformToBrailStep(AbstractCompilerStep):
	override def Run():
		for module in CompileUnit.Modules:
		
			module.Imports.Add(Import(module.LexicalInfo,"Castle.MonoRail.Views.Brail"))
			module.Imports.Add(Import(module.LexicalInfo,"Castle.MonoRail.Framework"))
			macro = ClassDefinition(Name: "${module.FullName}")
			macro.BaseTypes.Add(SimpleTypeReference("Castle.MonoRail.Views.Brail.BrailBase"))
			
			AddConstructor(macro)
			ScriptDirectoryProperty(macro, module)
			AddRunMethod(macro, module)
			
			for member in module.Members:
				macro.Members.Add(member)
			
			module.Members.Clear()
			module.Members.Add(macro)
			
			
	# get the directory name where this script reside and create a property
	# that return this value.
	# this is used to calculate relative paths when loading subviews.
	def ScriptDirectoryProperty(macro as ClassDefinition,  module as Module):
		p = Property("ScriptDirectory",
								Modifiers: TypeMemberModifiers.Override,
								Getter: Method(Name: "get"))
		p.Getter.Body.Add( 
			ReturnStatement(
				StringLiteralExpression( 
					Path.GetDirectoryName(module.LexicalInfo.FileName) )))
		
		macro.Members.Add(p)
	
	# create the Run method override for this class
	# this is where all the global code from the script goes
	def AddRunMethod(macro as ClassDefinition, module as Module):
		method = Method(Name: "Run",
							Modifiers: TypeMemberModifiers.Override,
							Body: module.Globals)
		module.Globals = Block()
		macro.Members.Add(method)
		

	# create a constructor that delegate to the base class
	def AddConstructor(macro as ClassDefinition):
		ctor = Constructor (macro.LexicalInfo)
		ctor.Parameters.Add(
			ParameterDeclaration("viewEngine", 
				SimpleTypeReference("Castle.MonoRail.Views.Brail.BooViewEngine")))
		
		ctor.Parameters.Add(
			ParameterDeclaration("output", 
				SimpleTypeReference("System.IO.TextWriter")))
		ctor.Parameters.Add(
			ParameterDeclaration("context",
				SimpleTypeReference("Castle.MonoRail.Framework.IRailsEngineContext")))
		
		ctor.Parameters.Add(
			ParameterDeclaration("controller",
				SimpleTypeReference("Castle.MonoRail.Framework.Controller")))
		
		
		mie = MethodInvocationExpression(SuperLiteralExpression())
		mie.Arguments.Add(AstUtil.CreateReferenceExpression("viewEngine"))
		mie.Arguments.Add(AstUtil.CreateReferenceExpression("output"))
		mie.Arguments.Add(AstUtil.CreateReferenceExpression("context"))
		mie.Arguments.Add(AstUtil.CreateReferenceExpression("controller"))
		
		ctor.Body.Add(mie)
		
		macro.Members.Add(ctor)
