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

namespace Castle.Rook.Compiler.AST
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;


	public sealed class SymbolTable : ISymbolTable
	{
		private readonly ISymbolTable parent;

		private ScopeType scopeType;

		private IDictionary identifierMap = new HybridDictionary();
		private IDictionary methodMap = new HybridDictionary();


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

			return IsIdentifierDefined(name) || IsMethodDefined(name);
		}

		public bool IsDefinedRecursive(String name)
		{
			if (name == null) throw new ArgumentNullException("name");

			return (!IsDefined(name) && parent != null) ? 
				parent.IsDefinedRecursive(name) : false;
		}

		public void AddMethod(MethodDefinitionStatement methodDef)
		{
			if (methodDef == null) throw new ArgumentNullException("methodDef");

			String name = methodDef.Name;

			IList list = null;

			if (!methodMap.Contains(name))
			{
				list = new ArrayList();
				methodMap[name] = list;
			}
			else
			{
				list = (IList) methodMap[name];
			}

			list.Add(methodDef);
		}

		public void AddIdentifier(Identifier identifier)
		{
			if (identifier == null) throw new ArgumentNullException("identifier");

			String name = identifier.Name;

			if (name == null) throw new ArgumentNullException("identifier.name");

			if (identifierMap.Contains(name))
			{
				throw new Exception(name + " is already defined within this scope");
			}

			identifierMap[name] = identifier;
		}

		public Identifier GetIdentifier(String name)
		{
			if (IsIdentifierDefined(name))
			{
				return (Identifier) identifierMap[name];
			}

			throw new Exception(name + " is not defined on this scope");
		}

		public MethodDefinitionStatement GetMethod(String name)
		{
			if (name == null) throw new ArgumentNullException("name");

			if (IsMethodDefined(name))
			{
				IList list = (IList) methodMap[name];
				return list[0] as MethodDefinitionStatement;
			}

			throw new Exception(name + " is not defined on this scope");
		}

		public MethodDefinitionStatement[] GetMethods(String name)
		{
			if (name == null) throw new ArgumentNullException("name");

			if (IsMethodDefined(name))
			{
				IList list = (IList) methodMap[name];
				Array array = Array.CreateInstance( typeof(MethodDefinitionStatement), list.Count );
				list.CopyTo( array, 0 );
				return (MethodDefinitionStatement[]) array;
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

		private bool IsIdentifierDefined(String name)
		{
			return identifierMap.Contains(name);
		}

		private bool IsMethodDefined(String name)
		{
			return methodMap.Contains(name);
		}
	}
}