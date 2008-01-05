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
	using System;
	using System.Collections;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Framework;
	using System.Collections.Specialized;
	using System.Collections.Generic;
	using System.Reflection;

	public class DslMacro : AbstractAstMacro
	{
		private readonly IDictionary languages = new HybridDictionary(true);

		public DslMacro()
		{
			languages.Add("html", typeof(HtmlExtension));
			languages.Add("xml", typeof(XmlExtension));
		}

		public override Statement Expand(MacroStatement macro)
		{
			Block codeBlock = new Block();

			// Castle.MonoRail.Views.Brail.DslProvider(BrailBase)
			MethodInvocationExpression newDslWrapper = new MethodInvocationExpression();
			newDslWrapper.Target = AstUtil.CreateReferenceExpression("Castle.MonoRail.Views.Brail.DslProvider");
			newDslWrapper.Arguments.Add(new SelfLiteralExpression());

			// dsl = Castle.MonoRail.Views.Brail.DslPRovider(BrailBase)
			ReferenceExpression dslReference = AstUtil.CreateReferenceExpression("dsl");
			codeBlock.Add(new BinaryExpression(BinaryOperatorType.Assign, dslReference, newDslWrapper));

			if (macro.Arguments.Count == 1)
			{
				string language = LookupLanguageExtension(macro.Arguments[0].ToString());
				// LanguageExtension(OutputStream)
				MethodInvocationExpression newLanguage = new MethodInvocationExpression();
				newLanguage.Target = AstUtil.CreateReferenceExpression(language);
				newLanguage.Arguments.Add(AstUtil.CreateReferenceExpression("OutputStream"));

				MethodInvocationExpression registerLanguage = new MethodInvocationExpression();
				registerLanguage.Target = AstUtil.CreateReferenceExpression("dsl.Register");
				registerLanguage.Arguments.Add(newLanguage);

				// dsl.Register(LanguageExtension(OutputStream))
				codeBlock.Add(registerLanguage);
			}

			// rewrite the remaining code to invoke methods on
			// the dsl reference
			Block macroBlock = macro.Block;
			(new NameExpander(dslReference)).Visit(macroBlock);
			codeBlock.Add(macroBlock);
			// dsl.Flush();
			codeBlock.Add(new MethodInvocationExpression(AstUtil.CreateReferenceExpression("dsl.Flush")));
			return codeBlock;
		}

		private string LookupLanguageExtension(string language)
		{
			if (!languages.Contains(language))
			{
				throw new MonoRailException(string.Format("Language '{0}' is not implemented", language));
			}

			Type languageExtension = (Type) languages[language];
			return string.Format("{0}{1}{2}", languageExtension.Namespace, Type.Delimiter, languageExtension.Name);
		}

		#region Nested type: NameExpander

		private class NameExpander : DepthFirstTransformer
		{
			private readonly ReferenceExpression _reference = null;
			private readonly IDictionary<string, Node> _skippedReferences = null;
			private readonly IDictionary<string, Type> _splitNamespaces = null;

			public NameExpander(ReferenceExpression reference)
			{
				_reference = reference;
				_skippedReferences = new Dictionary<string, Node>();
				_splitNamespaces = new Dictionary<string, Type>();

				RecordReferenceTypesToSkip(typeof(BrailBase).Assembly);

				foreach(AssemblyName asn in typeof(BrailBase).Assembly.GetReferencedAssemblies())
				{
					RecordReferenceTypesToSkip(Assembly.Load(asn));
				}

				foreach(MethodInfo method in typeof(BrailBase).GetMethods())
				{
					if (!_skippedReferences.ContainsKey(method.Name))
					{
						_skippedReferences.Add(method.Name, new MemberReferenceExpression(new SelfLiteralExpression(), method.Name));
					}
				}
			}

			private void RecordReferenceTypesToSkip(Assembly asm)
			{
				foreach(Type type in asm.GetExportedTypes())
				{
					SplitTypeNameAndRecordReferenceToSkip(type);

					string keyName = string.Format("{0}{1}{2}", type.Namespace, Type.Delimiter, type.Name);

					if (!_skippedReferences.ContainsKey(keyName))
					{
						_skippedReferences.Add(keyName, new ReferenceExpression(keyName));
					}

					if (!_skippedReferences.ContainsKey(type.Name))
					{
						_skippedReferences.Add(type.Name, new ReferenceExpression(type.Name));
					}
				}
			}

			private void SplitTypeNameAndRecordReferenceToSkip(Type type)
			{
				List<string> nsPieces = new List<string>();

				if (type.Namespace != null && !_splitNamespaces.ContainsKey(type.Namespace))
				{
					nsPieces.Clear();

					foreach(string nsPart in type.Namespace.Split(Type.Delimiter))
					{
						nsPieces.Add(nsPart);
						string nsItem = string.Join(new string(Type.Delimiter, 1), nsPieces.ToArray());

						if (!_skippedReferences.ContainsKey(nsPart))
						{
							_skippedReferences.Add(nsPart, new ReferenceExpression(nsPart));
						}

						if (!_skippedReferences.ContainsKey(nsItem))
						{
							_skippedReferences.Add(nsItem, new ReferenceExpression(nsItem));
						}
					}

					_splitNamespaces.Add(type.Namespace, type);
				}
			}

			public override void OnDeclaration(Declaration node)
			{
				if (!_skippedReferences.ContainsKey(node.Name))
				{
					_skippedReferences.Add(node.Name, node);
				}

				base.OnDeclaration(node);
			}

			public override void OnReferenceExpression(ReferenceExpression node)
			{
				if (node.ParentNode is BinaryExpression || _skippedReferences.ContainsKey(node.Name))
				{
					base.OnReferenceExpression(node);
					if (!_skippedReferences.ContainsKey(node.Name))
					{
						_skippedReferences.Add(node.Name, node);
					}
					return;
				}

				MemberReferenceExpression mre = new MemberReferenceExpression(node.LexicalInfo);
				mre.Name = node.Name;
				mre.Target = _reference.CloneNode();

				ReplaceCurrentNode(mre);
			}
		}

		#endregion
	}
}