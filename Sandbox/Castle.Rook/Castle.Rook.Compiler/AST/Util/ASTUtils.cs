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

namespace Castle.Rook.Compiler.AST.Util
{
	using System;

	using Castle.Rook.Compiler.Visitors;


	public abstract class ASTUtils
	{
		public static void CollectMethodInformation(string fullname, out string boundTo, out string name, out bool isStatic)
		{
			isStatic = false;
			boundTo = null;

			int point = fullname.IndexOf('.');
			
			if (point == -1)
			{
				name = fullname;
			}
			else
			{
				String firstPart = fullname.Substring(0, point);

				if ("self".Equals(firstPart))
				{
					// TODO: If fullname contains addition '.' when signalize the error

					isStatic = true;

					name = fullname.Substring(++point);
				}
				else
				{
					point = fullname.LastIndexOf('.');
					boundTo = fullname.Substring(0, point);
					name = fullname.Substring(++point);
				}
			}
		}

		public static bool IsCycled(IASTNode node)
		{
			CyclicCheckVisitor vis = new CyclicCheckVisitor();

			try
			{
				vis.VisitNode(node);
			}
			catch(Exception)
			{
				return false;
			}

			return true;
		}

		public static IStatement GetParentStatement(IASTNode node)
		{
			if (node == null)
			{
				return null;
			}
			else if (node is IStatement)
			{
				return node as IStatement;
			}

			return GetParentStatement(node.Parent);
		}
	}
}
