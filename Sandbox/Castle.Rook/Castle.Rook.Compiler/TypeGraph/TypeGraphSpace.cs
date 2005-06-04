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

namespace Castle.Rook.Compiler.TypeGraph
{
	using System;
	using System.Reflection;
	using System.Collections;
	using System.Collections.Specialized;


	public class TypeGraphSpace : NamespaceGraph
	{
		private readonly TypeGraphSpace parent;
		private IDictionary ambiguities = new HybridDictionary();

		public TypeGraphSpace() : base(String.Empty)
		{
		}

		public TypeGraphSpace(TypeGraphSpace graph) : this()
		{
			parent = graph;
		}

		public void AddAssemblyReference(String assemblyName)
		{
			Assembly assembly = Assembly.Load(assemblyName);
			
			foreach(Type type in assembly.GetExportedTypes())
			{
				if (type.IsNestedPublic) continue;

				AddType(type);
			}
		}

		public void AddAssemblyFileReference(String assemblyName)
		{
			
		}

		private void AddType(Type type)
		{
			NamespaceGraph ng = GetOrCreateNamespace(type.Namespace);

			ng.AddExternalType(type);
		}

		public void AddRequire(String namespaceName)
		{
			NamespaceGraph ng = GetNamespace(namespaceName);

			if (ng == null)
			{
				throw new ArgumentException("Could not found namespace " + namespaceName);
			}

			foreach(AbstractType type in ng.Types.Values)
			{
				if (types.Contains(type.Name))
				{
					RegisterAmbiguity(type);
					continue;
				}

				types[type.Name] = type;
			}
		}

		private void RegisterAmbiguity(AbstractType type)
		{
			ambiguities.Add(type.Name, type);
		}
	}
}
