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
import Boo.Lang.Compiler.Ast.Visitors
import Castle.MonoRail.Framework
import Castle.MonoRail.Framework.Internal

class ComponentMacro(AbstractAstMacro):

	override def Expand(macro as MacroStatement):
		if macro.Arguments.Count == 0:
			raise RailsException("component must be called with a component name")
			
		method as Ast.Method
		parent = macro.ParentNode
		while not parent isa Ast.Method:
			parent = parent.ParentNode
		method = parent
		
		componentName = StringLiteralExpression(macro.Arguments[0].ToString())

		# Make sure that hash table is an case insensitive one.
		dictionary = MethodInvocationExpression( Target: AstUtil.CreateReferenceExpression("System.Collections.Hashtable") )
			
		if macro.Arguments.Count == 2:
			dictionary.Arguments.Add( macro.Arguments[1] )
		dictionary.Arguments.Add( AstUtil.CreateReferenceExpression("System.Collections.CaseInsensitiveHashCodeProvider.Default") )
		dictionary.Arguments.Add( AstUtil.CreateReferenceExpression("System.Collections.CaseInsensitiveComparer.Default") )
		
		block = Block()
		
		# create closure for macro's body or null
		macroBody as Expression = NullLiteralExpression()
		if macro.Block.Statements.Count > 0:
			callableExpr = CallableBlockExpression(Body: macro.Block)
			callableExpr .Parameters.Add( ParameterDeclaration("outputStream", CodeBuilder.CreateTypeReference( TextWriter ) ) )
			macroBody = callableExpr 
			
		initContext =  MethodInvocationExpression( 
				Target: AstUtil.CreateReferenceExpression("Castle.MonoRail.Views.Brail.BrailViewComponentContext") )
		initContext.Arguments.Extend( (macroBody, componentName, AstUtil.CreateReferenceExpression("OutputStream") , dictionary) )
		
		# compilerContext = BrailViewComponentContext(macroBodyClosure, "componentName", OutputStream, dictionary)
		block.Add(BinaryExpression(Operator: BinaryOperatorType.Assign,
			Left: ReferenceExpression("componentContext"),
			Right: initContext ))
		
		# AddProperties( compilerContext.ContextVars )
		mie = MethodInvocationExpression( Target: AstUtil.CreateReferenceExpression("AddProperties") )
		mie.Arguments.Add( AstUtil.CreateReferenceExpression("componentContext.ContextVars") )
		block.Add( mie )
		
		viewComponentFactoryLocal = CodeBuilder.DeclareLocal(method,"viewComponentFactory", 
			TypeSystemServices.Map( IViewComponentFactory) )
		
		# viewComponentFactory = MonoRailHttpHandler.CurrentContext.GetService(IViewComponentFactory)
		callService = MethodInvocationExpression(
			Target: AstUtil.CreateReferenceExpression("MonoRailHttpHandler.CurrentContext.GetService") )
		callService.Arguments.Add( CodeBuilder.CreateTypeofExpression( IViewComponentFactory ) )
		
		block.Add(BinaryExpression(Operator: BinaryOperatorType.Assign,
			Left:  CodeBuilder.CreateLocalReference("viewComponentFactory",viewComponentFactoryLocal),  
			Right: callService ) )
		
		# component = viewComponentFactory.Create( componentName)
		createComponent = MethodInvocationExpression(
				Target: MemberReferenceExpression( CodeBuilder.CreateLocalReference("viewComponentFactory",viewComponentFactoryLocal),
					"Create") )
		createComponent.Arguments.Add( componentName )
		block.Add(BinaryExpression(Operator: BinaryOperatorType.Assign,
			Left: ReferenceExpression("component"),
			Right: createComponent ) )
		
		# component.Init(context, componentContext)
		initComponent = MethodInvocationExpression( 
			Target: AstUtil.CreateReferenceExpression("component.Init"))
		initComponent.Arguments.Extend( (AstUtil.CreateReferenceExpression("context"), ReferenceExpression("componentContext")) )
		
		block.Add(initComponent )
		
		# component.Render()
		block.Add(MethodInvocationExpression( 
			Target: AstUtil.CreateReferenceExpression("component.Render") ) )
		
		# if component.ViewToRender is not null:
		#	OutputSubView("/"+component.ViewToRender, context.CompnentParameters)
		renderView = Block() 
		outputSubView = MethodInvocationExpression( 
			Target: AstUtil.CreateReferenceExpression("OutputSubView"))
		outputSubView.Arguments.Add(BinaryExpression(Operator: BinaryOperatorType.Addition,
			Left: StringLiteralExpression('/'),
			Right: AstUtil.CreateReferenceExpression("componentContext.ViewToRender") ) )
		outputSubView.Arguments.Add(AstUtil.CreateReferenceExpression("componentContext.ComponentParameters") )
		renderView.Add( outputSubView )
		
		block.Add( IfStatement(Condition: AstUtil.CreateReferenceExpression("componentContext.ViewToRender"),
			TrueBlock: renderView) )
		
		return block
