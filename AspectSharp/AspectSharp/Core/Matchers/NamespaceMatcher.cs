 // Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace AspectSharp.Core.Matchers
{
	using System;
	using AspectSharp.Lang.AST;

	/// <summary>
	/// Summary description for NamespaceMatcher.
	/// </summary>
	public class NamespaceMatcher : IClassMatcher
	{
		private static readonly NamespaceMatcher _instance = new NamespaceMatcher();

		protected NamespaceMatcher()
		{
		}

		public static NamespaceMatcher Instance
		{
			get { return _instance; }
		}

		public bool Match(Type targetType, AspectDefinition aspect)
		{
			String namespaceRoot = aspect.TargetType.NamespaceRoot;
			String typeNamespace = targetType.Namespace;

			if (typeNamespace.Equals(namespaceRoot))
			{
				foreach (TypeReference typeRef in aspect.TargetType.Excludes)
				{
					if (typeRef.ResolvedType.Equals(targetType))
					{
						return false;
					}
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		protected virtual Type GetTypeToCompare(AspectDefinition aspect)
		{
			return aspect.TargetType.SingleType.ResolvedType;
		}
	}
}