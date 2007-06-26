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
    using System.Collections.Specialized;

    public class DslMacro : AbstractAstMacro
    {
        IDictionary languages = new HybridDictionary(true);

        public DslMacro()
        {
            languages.Add("html", typeof(HtmlExtension));
        }

        public override Statement Expand(MacroStatement macro)
        {
            //if (macro.Arguments.Count == 0)
            //{
            //    throw new RailsException("Dsl must be invoked with a language");
            //}

            Block codeBlock = new Block();

            // Castle.MonoRail.Views.Brail.DslProvider(BrailBase)
            //MethodInvocationExpression newDslWrapper = new MethodInvocationExpression();
            //newDslWrapper.Target = AstUtil.CreateReferenceExpression("Castle.MonoRail.Views.Brail.DslProvider");
            //newDslWrapper.Arguments.Add(new SelfLiteralExpression());

            // dsl = Castle.MonoRail.Views.Brail.DslPRovider(BrailBase)
            ReferenceExpression dslReference = new ReferenceExpression("Dsl");
            //codeBlock.Add(new BinaryExpression(BinaryOperatorType.Assign, dslReference, newDslWrapper));

            if (macro.Arguments.Count == 1)
            {
                string language = LookupLanguageExtension(macro.Arguments[0].ToString());
                // LanguageExtension(OutputStream)
                MethodInvocationExpression newLanguage = new MethodInvocationExpression();
                newLanguage.Target = AstUtil.CreateReferenceExpression(language);
                newLanguage.Arguments.Add(AstUtil.CreateReferenceExpression("OutputStream"));

                MethodInvocationExpression registerLanguage = new MethodInvocationExpression();
                registerLanguage.Target = AstUtil.CreateReferenceExpression("Dsl.Register");
                registerLanguage.Arguments.Add(newLanguage);

                // dsl.Register(LanguageExtension(OutputStream))
                codeBlock.Add(registerLanguage);
            }

            // rewrite the remaining code to invoke methods on
            // the dsl reference
            Block macroBlock = macro.Block;
            (new NameExpander(dslReference)).Visit(macroBlock);
            codeBlock.Add(macroBlock);
            return codeBlock;
        }

        private string LookupLanguageExtension(string language)
        {
            if (!languages.Contains(language))
            {
                throw new RailsException(string.Format("Language '{0}' is not implemented", language));
            }

            Type languageExtension = (Type)languages[language];
            return string.Format("{0}{1}{2}", languageExtension.Namespace, Type.Delimiter, languageExtension.Name);
        }

        private class NameExpander : DepthFirstTransformer
        {
            ReferenceExpression _instance = null;

            public NameExpander(ReferenceExpression instance)
            {
                _instance = instance;
            }

            public override void OnReferenceExpression(ReferenceExpression node)
            {
                MemberReferenceExpression mre = new MemberReferenceExpression(node.LexicalInfo);
                mre.Name = node.Name;
                mre.Target = _instance.CloneNode();

                ReplaceCurrentNode(mre);
            }

            public override void OnHashLiteralExpression(HashLiteralExpression node)
            {
                base.OnHashLiteralExpression(node);
            }
        }

    }
}
