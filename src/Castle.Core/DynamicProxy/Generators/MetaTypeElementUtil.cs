// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using System.Text;

	internal static class MetaTypeElementUtil
	{
		public static string CreateNameForExplicitImplementation(Type sourceType, string name)
		{
			var ns = sourceType.Namespace;
			Debug.Assert(ns == null || ns != "");

			if (sourceType.GetTypeInfo().IsGenericType)
			{
				var nameBuilder = new StringBuilder();
				if (ns != null)
				{
					nameBuilder.Append(ns);
					nameBuilder.Append('.');
				}
				nameBuilder.AppendNameOf(sourceType);
				nameBuilder.Append('.');
				nameBuilder.Append(name);
				return nameBuilder.ToString();
			}
			else if (ns != null)
			{
				return string.Concat(ns, ".", sourceType.Name, ".", name);
			}
			else
			{
				return string.Concat(sourceType.Name, ".", name);
			}
		}

		private static void AppendNameOf(this StringBuilder nameBuilder, Type type)
		{
			nameBuilder.Append(type.Name);
			if (type.GetTypeInfo().IsGenericType)
			{
				nameBuilder.Append('[');
				var genericTypeArguments = type.GetGenericArguments();
				for (int i = 0, n = genericTypeArguments.Length; i < n; ++i)
				{
					if (i > 0)
					{
						nameBuilder.Append(',');
					}
					nameBuilder.AppendNameOf(genericTypeArguments[i]);
				}
				nameBuilder.Append(']');
			}
		}
	}
}
