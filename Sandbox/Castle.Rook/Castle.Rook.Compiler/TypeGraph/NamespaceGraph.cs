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
	using System.Collections;
	using System.Collections.Specialized;


	public class NamespaceGraph
	{
		protected readonly String name;
		protected readonly IDictionary namespaces = new HybridDictionary();
		protected readonly IDictionary types = new HybridDictionary();

		public NamespaceGraph(String name)
		{
			this.name = name;
		}

		public string Name
		{
			get { return name; }
		}

		public NamespaceGraph AddNamespace(NamespaceGraph graph)
		{
			if (namespaces.Contains(graph.name))
			{
				throw new ArgumentNullException("Namespace " + graph.name + " already exists.");
			}

			namespaces[graph.name] = graph;

			return graph;
		}

		public ExternalType AddExternalType(Type type)
		{
			if (types.Contains(type.Name))
			{
				if (type.IsNestedPublic) return null;

				throw new ArgumentNullException("Type " + type.Name + " already exists.");
			}

			ExternalType et = new ExternalType(type);

			types[type.Name] = et;

			return et;
		}

		public virtual AbstractType GetType(String path)
		{
			int index = path.LastIndexOf("::");
			
			if (index == -1)
			{
				return types[path] as AbstractType;
			}

			String ns = path.Substring(0, index);

			NamespaceGraph ng = GetNamespace(ns);

			if (ng == null)
			{
				// throw new ArgumentException("Namespace "+ ns + " could not be found");
				return null;
			}

			return ng.GetType( path.Substring(index + 2) );
		}

		public virtual NamespaceGraph GetNamespace(String path)
		{
			if (path.Length == 0) return this;

			int index = path.IndexOf("::");

			if (index == -1)
			{
				return namespaces[path] as NamespaceGraph;
			}
			else
			{
				String partial = path.Substring(0, index);
				
				NamespaceGraph ng = namespaces[ partial ] as NamespaceGraph;

				if (ng == null)
				{
					// ng = AddNamespace(new NamespaceGraph(partial));
					// throw new ArgumentException("Namespace part " + partial + " wasn't found for " + path);
					return null;
				}

				return ng.GetNamespace(path.Substring( index + 2 ));
			}
		}

		protected NamespaceGraph GetOrCreateNamespace(String namespaceName)
		{
			NamespaceGraph ng = namespaces[namespaceName] as NamespaceGraph;
			
			if (ng != null) return ng;

			if (namespaceName.IndexOf('.') == -1)
			{
				return AddNamespace(new NamespaceGraph(namespaceName));
			}

			String[] parts = namespaceName.Split('.');

			foreach(String ns in parts)
			{
				if (ng != null) 
				{
					ng = ng.GetOrCreateNamespace(ns);
				}
				else
					ng = GetOrCreateNamespace(ns);
			}

			return ng;
		}

		public IDictionary Types
		{
			get { return types; }
		}

		public ICollection Namespaces
		{
			get { return namespaces.Values; }
		}
	}
}
