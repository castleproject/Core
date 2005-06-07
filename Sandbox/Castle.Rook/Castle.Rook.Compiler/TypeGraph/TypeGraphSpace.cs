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

	using Castle.Rook.Compiler.AST;


	public class TypeGraphSpace : NamespaceGraph
	{
		private TypeGraphSpace parent;
		private NamespaceGraph definedNamespace;
		private IDictionary ambiguities = new HybridDictionary();

		public TypeGraphSpace() : base(String.Empty)
		{
		}

		public TypeGraphSpace(TypeGraphSpace graph) : this()
		{
			parent = graph;
		}

		public NamespaceGraph DefinedNamespace
		{
			get { return definedNamespace; }
		}

		public InternalType DefineType(TypeDefinitionStatement typeDefinition)
		{
			if (definedNamespace == null && parent != null)
			{
				return parent.DefineType(typeDefinition);
			}
			else if (definedNamespace != null)
			{
				return definedNamespace.AddUserType(typeDefinition);
			}
			else
			{
				return AddUserType(typeDefinition);
			}
		}

		public void DefineConstructorMethod(MethodDefinitionStatement methodDefinition)
		{
			throw new NotImplementedException();
		}

		public void DefineStaticMethod(MethodDefinitionStatement methodDefinition)
		{
			throw new NotImplementedException();
		}

		public void DefineMethod(MethodDefinitionStatement methodDefinition)
		{
			throw new NotImplementedException();
		}

		public NamespaceGraph DefineNamespace(String namespaceName)
		{
			if (parent == null)
			{
				return GetOrCreateNamespace( namespaceName.Replace("::", ".") );
			}
			else
			{
				definedNamespace = parent.DefineNamespace(namespaceName);

				return definedNamespace;
			}
		}

		public void AddAssemblyReference(String assemblyName)
		{
			Assembly assembly = Assembly.Load(assemblyName);

			AddAssemblyExportedTypes(assembly);
		}

		public void AddAssemblyFileReference(String assemblyName)
		{
		}

		private void AddAssemblyExportedTypes(Assembly assembly)
		{
			foreach(Type type in assembly.GetExportedTypes())
			{
				AddType(type);
			}
		}

		private void AddType(Type type)
		{
			NamespaceGraph ng = GetOrCreateNamespace(type.Namespace);

			ng.AddExternalType(type);
		}

		/// <summary>
		/// Make the types and subnamespaces available on the 
		/// root of this space.
		/// </summary>
		/// <param name="namespaceName"></param>
		public void AddRequire(String namespaceName)
		{
			if (parent == null)
			{
				throw new ApplicationException("You can use require on a root type space");
			}

			NamespaceGraph ng = parent.GetNamespace(namespaceName);

			if (ng == null)
			{
				throw new ArgumentException("Could not found namespace " + namespaceName);
			}

			foreach(NamespaceGraph sub in ng.Namespaces)
			{
				if (namespaces.Contains(sub.Name))
				{
					RegisterAmbiguity(sub);
					continue;
				}

				namespaces.Add( sub.Name, sub );
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

		public override AbstractType GetType(String path)
		{
			AbstractType type = base.GetType(path);

			if (type == null && parent != null)
			{
				type = parent.GetType(path);
			}

			return type;
		}

		public override NamespaceGraph GetNamespace(String path)
		{
			NamespaceGraph ng = base.GetNamespace(path);

			if (ng == null && parent != null)
			{
				ng = parent.GetNamespace(path);
			}

			return ng;
		}

		public bool HasAmbiguity(String path)
		{
			return (ambiguities.Contains(path));
		}

		private void RegisterAmbiguity(AbstractType type)
		{
			ambiguities.Add(type.Name, type);
		}

		private void RegisterAmbiguity(NamespaceGraph ns)
		{
			ambiguities.Add(ns.Name, ns);
		}
	}
}
