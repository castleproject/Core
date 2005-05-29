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
	using System.Collections;
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
		private readonly NameScopeType nstype;
		private HybridDictionary scope = new HybridDictionary();
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

		public void AddVariable(string name, TypeReference reference)
		{
			if (scope.Contains(name))
			{
				throw new CompilerException("Scope " + ToString() + " already has a definition for " + name);
			}

			scope.Add(name, new ScopeItem(ScopeItemType.Variable, reference) );
		}
	}

	public enum ScopeItemType
	{
		Variable,
		Method,
		Type
	}

	public class ScopeItem
	{
		private readonly ScopeItemType type;
		private readonly TypeReference reference;

		public ScopeItem(ScopeItemType type, TypeReference reference)
		{
			this.type = type;
			this.reference = reference;
		}

		public ScopeItemType Type
		{
			get { return type; }
		}

		public TypeReference Reference
		{
			get { return reference; }
		}
	}
}
