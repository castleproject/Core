// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
    using System;
    using System.Collections;
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using Castle.MonoRail.Framework;

    public class SectionMacro : AbstractAstMacro
    {
        public override Statement Expand(MacroStatement macro)
        {
            if (macro.Arguments.Count == 0)
                throw new MonoRailException("Section must be called with a name");

            MacroStatement component = GetParentComponent(macro);

            string sectionName = macro.Arguments[0].ToString();
            Block block = new Block();
            //if (!Component.SupportsSection(section.Name))
            //   throw new ViewComponentException( String.Format("The section '{0}' is not supported by the ViewComponent '{1}'", section.Name, ComponentName));
            MethodInvocationExpression supportsSection = new MethodInvocationExpression(
                AstUtil.CreateReferenceExpression("component.SupportsSection"),
                 new StringLiteralExpression(sectionName));
            //create the new exception
            RaiseStatement raiseSectionNotSupportted = new RaiseStatement(
                new MethodInvocationExpression(
                    AstUtil.CreateReferenceExpression(typeof(ViewComponentException).FullName),
                new StringLiteralExpression(
                    String.Format("The section '{0}' is not supported by the ViewComponent '{1}'", sectionName,
                                  component.Arguments[0].ToString())
                    )
                ));

            Block trueBlock = new Block();
            trueBlock.Add(raiseSectionNotSupportted);
            IfStatement ifSectionNotSupported = new IfStatement(new UnaryExpression(UnaryOperatorType.LogicalNot, supportsSection),
                                                                trueBlock, null);
            block.Add(ifSectionNotSupported);
            //componentContext.RegisterSection(sectionName);
            MethodInvocationExpression mie = new MethodInvocationExpression(
                new MemberReferenceExpression(AstUtil.CreateReferenceExpression("componentContext"), "RegisterSection"),
                new StringLiteralExpression(sectionName),
                CodeBuilderHelper.CreateCallableFromMacroBody(CodeBuilder, macro));
            block.Add(mie);
            
            IDictionary sections = (IDictionary)component["sections"];
            if(sections==null)
            {
                component["sections"] = sections = new Hashtable();
            }
            sections.Add(sectionName, block);
            return null;
        }

        private MacroStatement GetParentComponent(MacroStatement macro)
        {
            Node parent = macro.ParentNode;
            while( !(parent is MacroStatement) )
            {
                parent = parent.ParentNode;
            }
            MacroStatement parentComponent = (MacroStatement) parent;
            if(parentComponent == null ||
               parentComponent.Name.ToLowerInvariant() != "component" )
            {
                throw new MonoRailException("A section must be contained in a component");
            }
            return parentComponent;
        }
    }
}
