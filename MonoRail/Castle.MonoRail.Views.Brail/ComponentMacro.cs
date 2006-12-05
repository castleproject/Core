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
{
	using System.IO;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	using Castle.MonoRail.Framework;

	public class ComponentMacro : AbstractAstMacro
	{
		public override Statement Expand(MacroStatement macro)
		{
			if (macro.Arguments.Count == 0)
				throw new RailsException("Component must be called with a name");

			Method method;
			Node parent = macro.ParentNode;
			while((parent is Method) == false)
			{
				parent = parent.ParentNode;
			}
			method = (Method) parent;

			StringLiteralExpression componentName = new StringLiteralExpression(macro.Arguments[0].ToString());

			MethodInvocationExpression dictionary = CreateParametersDictionary(macro);

			Block block = new Block();

			Expression macroBody = CreateMacroBody(macro);

			MethodInvocationExpression initContext = new MethodInvocationExpression();
			initContext.Target = AstUtil.CreateReferenceExpression("Castle.MonoRail.Views.Brail.BrailViewComponentContext");
			initContext.Arguments.Extend(
				new object[]
					{
						macroBody, componentName,
						AstUtil.CreateReferenceExpression("OutputStream"),
						dictionary
					});

			// compilerContext = BrailViewComponentContext(macroBodyClosure, "componentName", OutputStream, dictionary)
			block.Add(new BinaryExpression(BinaryOperatorType.Assign,
			                               new ReferenceExpression("componentContext"), initContext));

			// AddProperties( compilerContext.ContextVars )
			MethodInvocationExpression mie = new MethodInvocationExpression(AstUtil.CreateReferenceExpression("AddProperties"));
			mie.Arguments.Add(AstUtil.CreateReferenceExpression("componentContext.ContextVars"));
			block.Add(mie);

			InternalLocal viewComponentFactoryLocal = CodeBuilder.DeclareLocal(method, "viewComponentFactory",
			                                                                   TypeSystemServices.Map(
			                                                                   	typeof(IViewComponentFactory)));

			// viewComponentFactory = MonoRailHttpHandler.CurrentContext.GetService(IViewComponentFactory)
			MethodInvocationExpression callService = new MethodInvocationExpression(
				AstUtil.CreateReferenceExpression("MonoRailHttpHandler.CurrentContext.GetService"));
			callService.Arguments.Add(CodeBuilder.CreateTypeofExpression(typeof(IViewComponentFactory)));

			block.Add(new BinaryExpression(BinaryOperatorType.Assign,
			                               CodeBuilder.CreateLocalReference("viewComponentFactory", viewComponentFactoryLocal),
			                               callService));

			// component = viewComponentFactory.Create( componentName)
			MethodInvocationExpression createComponent = new MethodInvocationExpression(
				new MemberReferenceExpression(CodeBuilder.CreateLocalReference("viewComponentFactory", viewComponentFactoryLocal),
				                              "Create"));
			createComponent.Arguments.Add(componentName);
			block.Add(new BinaryExpression(BinaryOperatorType.Assign,
			                               new ReferenceExpression("component"),
			                               createComponent));

			// component.Init(context, componentContext)
			MethodInvocationExpression initComponent = new MethodInvocationExpression(
				AstUtil.CreateReferenceExpression("component.Init"));
			initComponent.Arguments.Extend(
				new object[] {AstUtil.CreateReferenceExpression("context"), new ReferenceExpression("componentContext")});

			block.Add(initComponent);

			// component.Render()
			block.Add(new MethodInvocationExpression(
			          	AstUtil.CreateReferenceExpression("component.Render")));

			// if component.ViewToRender is not null:
			//	OutputSubView("/"+component.ViewToRender, context.CompnentParameters)
			Block renderView = new Block();
			MethodInvocationExpression outputSubView = new MethodInvocationExpression(
				AstUtil.CreateReferenceExpression("OutputSubView"));
			outputSubView.Arguments.Add(new BinaryExpression(BinaryOperatorType.Addition,
			                                                 new StringLiteralExpression("/"),
			                                                 AstUtil.CreateReferenceExpression("componentContext.ViewToRender")));

			outputSubView.Arguments.Add(AstUtil.CreateReferenceExpression("componentContext.ComponentParameters"));
			renderView.Add(outputSubView);

			block.Add(new IfStatement(AstUtil.CreateReferenceExpression("componentContext.ViewToRender"),
			                          renderView, new Block()));

			return block;
		}

		private Expression CreateMacroBody(MacroStatement macro)
		{
			// create closure for macro's body or null
			Expression macroBody = new NullLiteralExpression();
			if (macro.Block.Statements.Count > 0)
			{
				CallableBlockExpression callableExpr = new CallableBlockExpression();
				callableExpr.Body = macro.Block;
				callableExpr.Parameters.Add(
					new ParameterDeclaration("outputStream", CodeBuilder.CreateTypeReference(typeof(TextWriter))));

				macroBody = callableExpr;
			}
			return macroBody;
		}

		private static MethodInvocationExpression CreateParametersDictionary(MacroStatement macro)
		{
			// Make sure that hash table is an case insensitive one.
			MethodInvocationExpression dictionary = new MethodInvocationExpression();
			dictionary.Target = AstUtil.CreateReferenceExpression("System.Collections.Hashtable");

			//If component has parameters, add them
			if (macro.Arguments.Count == 2)
				dictionary.Arguments.Add(macro.Arguments[1]);

			dictionary.Arguments.Add(
				AstUtil.CreateReferenceExpression("System.Collections.CaseInsensitiveHashCodeProvider.Default"));
			dictionary.Arguments.Add(AstUtil.CreateReferenceExpression("System.Collections.CaseInsensitiveComparer.Default"));
			return dictionary;
		}
	}
}