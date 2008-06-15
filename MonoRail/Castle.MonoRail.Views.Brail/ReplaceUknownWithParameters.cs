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
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.Steps;
	using Boo.Lang.Compiler.TypeSystem;

	///<summary>
	/// Replace any uknown identifier with a call to GetParameter('unknown')
	/// this mean that unknonwn identifier in scripts will only fail in run time if they
	/// were not defined by the controller.
	/// </summary>
	public class ReplaceUknownWithParameters : ProcessMethodBodiesWithDuckTyping
	{
		private IMethod getParam;
		private IMethod tryGetParam;

		public override void OnReferenceExpression(ReferenceExpression node)
		{
			IEntity entity = NameResolutionService.Resolve(node.Name);
			if (entity != null)
			{
				base.OnReferenceExpression(node);
			}
			else
			{
				MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
					CodeBuilder.CreateSelfReference(_currentMethod.DeclaringType),
					GetMethod(node.Name));
				mie.Arguments.Add(GetNameLiteral(node.Name));
				node.ParentNode.Replace(node, mie);
			}
		}

		protected override void InitializeMemberCache()
		{
			base.InitializeMemberCache();
			getParam = TypeSystemServices.Map(typeof(BrailBase).GetMethod("GetParameter"));
			tryGetParam = TypeSystemServices.Map(typeof(BrailBase).GetMethod("TryGetParameter"));
		}

		public IMethod GetMethod(string name)
		{
			if (name[0] == '?')
				return tryGetParam;
			else
				return getParam;
		}

		public StringLiteralExpression GetNameLiteral(string name)
		{
			if (name[0] == '?')
				return CodeBuilder.CreateStringLiteral(name.Substring(1));
			else
				return CodeBuilder.CreateStringLiteral(name);
		}
	}
}