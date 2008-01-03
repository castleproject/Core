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
	using System.Web;
	using anrControls;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;

	//base class for the [Html], [Markdown], etc attributes
	public abstract class OutputMethodAttribute : AbstractAstAttribute
	{
		public abstract string TransformMethodName { get; }

		public override void Apply(Node targetNode)
		{
			Method method = targetNode as Method;
			if (method == null)
			{
				InvalidNodeForAttribute("method");
				return;
			}

			method.Body.Accept(new ReturnValueVisitor());
			ReplaceReferences(method.Body, "transform", TransformMethodName);
		}

		private void ReplaceReferences(Node node, string what, string value)
		{
			node.ReplaceNodes(
				new ReferenceExpression(what),
				AstUtil.CreateReferenceExpression(value));
		}
	}
}