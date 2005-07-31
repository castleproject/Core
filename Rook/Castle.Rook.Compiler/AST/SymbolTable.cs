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


	public sealed class SymbolTable : ISymbolTable
	{
		private readonly ISymbolTable parent;

		private ScopeType scopeType;

		private IDictionary table = new HybridDictionary();


		public SymbolTable()
		{
		}

		public SymbolTable(ISymbolTable parent, ScopeType scopeType)
		{
			this.parent = parent;
			this.scopeType = scopeType;
		}

		public bool IsDefined(String name)
		{
			if (name == null) throw new ArgumentNullException("name");

			return table.Contains(name);
		}

		public bool IsDefinedRecursive(String name)
		{
			if (name == null) throw new ArgumentNullException("name");

			return (!IsDefined(name) && parent != null) ? 
				parent.IsDefinedRecursive(name) : false;
		}

		public void AddIdentifier(Identifier identifier)
		{
			if (identifier == null) throw new ArgumentNullException("identifier");

			String name = identifier.Name;

			if (name == null) throw new ArgumentNullException("identifier.name");

			if (table.Contains(name))
			{
				throw new Exception(name + " is already defined within this scope");
			}

			table[name] = identifier;
		}

		public Identifier GetIdentifier(String name)
		{
			if (IsDefined(name))
			{
				return (Identifier) table[name];
			}

			throw new Exception(name + " is not defined on this scope");
		}

		public ISymbolTable Parent
		{
			get { return parent; }
		}

		public ScopeType ScopeType
		{
			get { return scopeType; }
		}
	}
}