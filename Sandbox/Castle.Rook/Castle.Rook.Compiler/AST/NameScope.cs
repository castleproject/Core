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

namespace Castle.Rook.Compiler.AST
{
	using System;
	using System.Collections.Specialized;

	public enum NameScopeType
	{
		Global,
		Namespace,
		Type,
		Method,
		Block,
		Compound
	}

	public class NameScope : INameScope
	{
		private HybridDictionary scope = new HybridDictionary();
		private NameScopeType nstype;
		private INameScope parent;

		public NameScope(NameScopeType nstype)
		{
			this.nstype = nstype;
		}

		public NameScope(NameScopeType nstype, INameScope parent) : this(nstype)
		{
			this.parent = parent;
		}

		public NameScopeType NameScopeType
		{
			get { return nstype; }
		}

		public bool IsDefined(String name)
		{
			return scope.Contains(name);
		}

		public bool IsDefinedInParent(String name)
		{
			INameScope scopeRef = parent;
			
			while(scopeRef != null)
			{
				if (scopeRef.IsDefined(name)) return true;

				scopeRef = scopeRef.Parent;
			}

			return false;
		}

		public INameScope Parent
		{
			get { return parent; }
		}

		public void AddVariable(Identifier ident)
		{
			EnsureUniqueKey(ident.Name);

//			scope.Add(ident.Name, new VarScopeItem(ident) );
		}

		public bool HasMethod(String name)
		{
			// TODO: Fix that
			return IsDefined(name);
		}

		public void AddMethodDefintion(MethodDefinitionStatement def)
		{
			EnsureUniqueKey(def.Name);

//			scope.Add(def.Name, new MethodScopeItem(def) );
		}

		public bool HasNamespace(String name)
		{
			// TODO: Fix that
			return IsDefined(name);
		}

		public void AddNamespace(NamespaceDeclaration ns)
		{
			EnsureUniqueKey(ns.Name);

//			scope.Add(ns.Name, new NamespaceScopeItem(ns) );
		}

		public bool HasTypeDefinition(String name)
		{
			// TODO: Fix that
			return IsDefined(name);
		}

		public void AddTypeDefinition(TypeDefinitionStatement def)
		{
			EnsureUniqueKey(def.Name);

//			scope.Add(def.Name, new TypeScopeItem(def) );
		}

		public void AddRequire(String qualifiedName)
		{
			
		}

		private void EnsureUniqueKey(String name)
		{
			if (scope.Contains(name))
			{
				throw new CompilerException("Scope " + ToString() + " already has a definition for " + name);
			}
		}
	}

//	public enum ScopeItemType
//	{
//		Variable,
//		Namespace,
//		Method,
//		Type
//	}
//
//	public abstract class AbstractScopeItem
//	{
//		private readonly ScopeItemType type;
//
//		public AbstractScopeItem(ScopeItemType type)
//		{
//			this.type = type;
//		}
//
//		public ScopeItemType Type
//		{
//			get { return type; }
//		}
//	}
//
//	public class VarScopeItem : AbstractScopeItem
//	{
//		private readonly Identifier identifier;
//
//		public VarScopeItem(Identifier identifier) : base(ScopeItemType.Variable)
//		{
//			this.identifier = identifier;
//		}
//
//		public Identifier Identifier
//		{
//			get { return identifier; }
//		}
//	}
//
//	public class NamespaceScopeItem : AbstractScopeItem
//	{
//		private readonly NamespaceDeclaration ns;
//
//		public NamespaceScopeItem(NamespaceDeclaration ns) : base(ScopeItemType.Namespace)
//		{
//			this.ns = ns;
//		}
//
//		public NamespaceDeclaration Ns
//		{
//			get { return ns; }
//		}
//	}
//
//	public class TypeScopeItem : AbstractScopeItem
//	{
//		private readonly TypeDefinitionStatement typeDecl;
//
//		public TypeScopeItem(TypeDefinitionStatement typeDecl) : base(ScopeItemType.Type)
//		{
//			this.typeDecl = typeDecl;
//		}
//
//		public TypeDefinitionStatement TypeDecl
//		{
//			get { return typeDecl; }
//		}
//	}
//
//	public class MethodScopeItem : AbstractScopeItem
//	{
//		private readonly MethodDefinitionStatement methodDecl;
//
//		public MethodScopeItem(MethodDefinitionStatement methodDecl) : base(ScopeItemType.Method)
//		{
//			this.methodDecl = methodDecl;
//		}
//
//		public MethodDefinitionStatement MethodDecl
//		{
//			get { return methodDecl; }
//		}
//	}
}
