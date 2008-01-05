// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System.Collections;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	using Framework;
	using Macros;

	public class ComponentMacro : AbstractAstMacro
	{
		private string componentContextName;
		private string componentFactoryName;
		private string componentVariableName;

		public override Statement Expand(MacroStatement macro)
		{
			componentContextName = ComponentNaming.GetComponentContextName(macro);
			componentFactoryName = ComponentNaming.GetComponentFactoryName(macro);
			componentVariableName = ComponentNaming.GetComponentNameFor(macro);

			if (macro.Arguments.Count == 0) throw new MonoRailException("Component must be called with a name");

			Block block = new Block();

			Method method;

			method = (Method) macro.GetAncestor(NodeType.Method);

			StringLiteralExpression componentName = new StringLiteralExpression(macro.Arguments[0].ToString());

			MethodInvocationExpression dictionary = CreateParametersDictionary(macro);

			Expression macroBody = CodeBuilderHelper.CreateCallableFromMacroBody(CodeBuilder, macro);

			MethodInvocationExpression initContext = new MethodInvocationExpression();
			initContext.Target = AstUtil.CreateReferenceExpression("Castle.MonoRail.Views.Brail.BrailViewComponentContext");
			initContext.Arguments.Extend(
				new Expression[]
					{
						new SelfLiteralExpression(),
						macroBody, componentName,
						AstUtil.CreateReferenceExpression("OutputStream"),
						dictionary
					});

			// compilerContext = BrailViewComponentContext(macroBodyClosure, "componentName", OutputStream, dictionary)
			block.Add(new BinaryExpression(BinaryOperatorType.Assign,
			                               new ReferenceExpression(componentContextName), initContext));

			// AddViewComponentProperties( compilerContext.ComponentParams )
			MethodInvocationExpression addProperties =
				new MethodInvocationExpression(AstUtil.CreateReferenceExpression("AddViewComponentProperties"));
			addProperties.Arguments.Add(AstUtil.CreateReferenceExpression(componentContextName + ".ComponentParameters"));
			block.Add(addProperties);

			InternalLocal viewComponentFactoryLocal = CodeBuilder.DeclareLocal(method, componentFactoryName,
			                                                                   TypeSystemServices.Map(
			                                                                   	typeof(IViewComponentFactory)));

			// viewComponentFactory = context.GetService(IViewComponentFactory)
			MethodInvocationExpression callService = new MethodInvocationExpression(
				AstUtil.CreateReferenceExpression("context.GetService"));
			callService.Arguments.Add(CodeBuilder.CreateTypeofExpression(typeof(IViewComponentFactory)));

			block.Add(new BinaryExpression(BinaryOperatorType.Assign,
			                               CodeBuilder.CreateLocalReference(componentFactoryName, viewComponentFactoryLocal),
			                               callService));

			// component = viewComponentFactory.Create( componentName)
			MethodInvocationExpression createComponent = new MethodInvocationExpression(
				new MemberReferenceExpression(CodeBuilder.CreateLocalReference(componentFactoryName, viewComponentFactoryLocal),
				                              "Create"));
			createComponent.Arguments.Add(componentName);
			block.Add(new BinaryExpression(BinaryOperatorType.Assign,
			                               new ReferenceExpression(componentVariableName),
			                               createComponent));
			AddSections(block, macro);

			// component.Init(context, componentContext)
			MethodInvocationExpression initComponent = new MethodInvocationExpression(
				AstUtil.CreateReferenceExpression(componentVariableName + ".Init"));
			initComponent.Arguments.Extend(
				new Expression[]
					{
						new ReferenceExpression("context"),
						new ReferenceExpression(componentContextName)
					});

			block.Add(initComponent);

			// component.Render()
			block.Add(new MethodInvocationExpression(
			          	AstUtil.CreateReferenceExpression(componentVariableName + ".Render")));

			// if component.ViewToRender is not null:
			//	OutputSubView("/"+component.ViewToRender, context.CompnentParameters)
			Block renderView = new Block();
			MethodInvocationExpression outputSubView = new MethodInvocationExpression(
				AstUtil.CreateReferenceExpression("OutputSubView"));
			outputSubView.Arguments.Add(new BinaryExpression(BinaryOperatorType.Addition,
			                                                 new StringLiteralExpression("/"),
			                                                 AstUtil.CreateReferenceExpression(componentContextName +
			                                                                                   ".ViewToRender")));

			outputSubView.Arguments.Add(AstUtil.CreateReferenceExpression(componentContextName + ".ComponentParameters"));
			renderView.Add(outputSubView);

			block.Add(new IfStatement(AstUtil.CreateReferenceExpression(componentContextName + ".ViewToRender"),
			                          renderView, new Block()));

			// RemoveViewComponentProperties( compilerContext.ComponentParams )
			MethodInvocationExpression removeProperties =
				new MethodInvocationExpression(AstUtil.CreateReferenceExpression("RemoveViewComponentProperties"));
			removeProperties.Arguments.Add(AstUtil.CreateReferenceExpression(componentContextName + ".ComponentParameters"));
			block.Add(removeProperties);

			return block;
		}

		private static void AddSections(Block block, Node macro)
		{
			IDictionary sections = (IDictionary) macro["sections"];
			
			if (sections == null) return;

			foreach(DictionaryEntry entry in sections)
			{
				block.Add((Block) entry.Value);
			}
		}

		private static MethodInvocationExpression CreateParametersDictionary(MacroStatement macro)
		{
			// Make sure that hash table is an case insensitive one.
			MethodInvocationExpression dictionary = new MethodInvocationExpression();
			dictionary.Target = AstUtil.CreateReferenceExpression("System.Collections.Hashtable");

			//If component has parameters, add them
			if (macro.Arguments.Count == 2)
			{
				dictionary.Arguments.Add(macro.Arguments[1]);
			}

			dictionary.Arguments.Add(
				AstUtil.CreateReferenceExpression("System.Collections.CaseInsensitiveHashCodeProvider.Default"));
			dictionary.Arguments.Add(AstUtil.CreateReferenceExpression("System.Collections.CaseInsensitiveComparer.Default"));
			return dictionary;
		}
	}
}