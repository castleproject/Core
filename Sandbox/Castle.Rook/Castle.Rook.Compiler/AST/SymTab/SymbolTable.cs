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

	using Castle.Rook.Compiler.TypeGraph;


	public class SymbolTable : ISymbolTable
	{
		protected ScopeType nstype;
		protected ISymbolTable parent;
		protected TypeGraphSpace graphSpace;
		protected TransientType curType;
		protected NamespaceGraph curNamespace;
		protected HybridDictionary scope = new HybridDictionary();

		protected SymbolTable(ScopeType nstype)
		{
			this.nstype = nstype;
		}

		public SymbolTable(ScopeType nstype, ISymbolTable parent) : this(nstype, parent.TypeGraphView)
		{
			this.parent = parent;
		}

		public SymbolTable(ScopeType nstype, TypeGraphSpace parentGraphSpace) : this(nstype)
		{
			this.graphSpace = new TypeGraphSpace(parentGraphSpace);
		}

		public ScopeType ScopeType
		{
			get { return nstype; }
		}

		public TypeGraphSpace TypeGraphView
		{
			get { return graphSpace; }
		}

		public bool IsDefined(String name)
		{
			return scope.Contains(name);
		}

		public bool IsDefinedInParent(String name)
		{
			ISymbolTable scopeRef = parent;
			
			while(scopeRef != null)
			{
				if (scopeRef.IsDefined(name)) return true;

				scopeRef = scopeRef.Parent;
			}

			return false;
		}

		public ISymbolTable Parent
		{
			get { return parent; }
		}

		public NamespaceGraph CurrentNamespace
		{
			get { return curNamespace != null ? curNamespace : (parent != null ? parent.CurrentNamespace : null) ; }
			set { curNamespace = value; }
		}

		public TransientType CurrentTypeDefinition
		{
			get { return curType != null ? curType : (parent != null ? parent.CurrentTypeDefinition : null) ; }
			set { curType = value; }
		}

		public void AddVariable(Identifier ident)
		{
			EnsureUniqueKey(ident.Name);

			scope.Add(ident.Name, new VarScopeItem(ident) );
		}

		private void EnsureUniqueKey(String name)
		{
			if (scope.Contains(name))
			{
				throw new CompilerException("Scope " + ToString() + " already has a definition for " + name);
			}
		}
	}

	public enum ScopeItemType
	{
		Variable,
//		Namespace,
//		Method,
//		Type
	}

	public abstract class AbstractScopeItem
	{
		private readonly ScopeItemType type;

		public AbstractScopeItem(ScopeItemType type)
		{
			this.type = type;
		}

		public ScopeItemType Type
		{
			get { return type; }
		}
	}

	public class VarScopeItem : AbstractScopeItem
	{
		private readonly Identifier identifier;

		public VarScopeItem(Identifier identifier) : base(ScopeItemType.Variable)
		{
			this.identifier = identifier;
		}

		public Identifier Identifier
		{
			get { return identifier; }
		}
	}

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
