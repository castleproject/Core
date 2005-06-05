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

	using Castle.Rook.Compiler.TypeGraph;


	public interface INameScope
	{
		INameScope Parent { get; }

		NameScopeType NameScopeType { get; }

		TypeGraphSpace CurrentTypeGraph { get; }

		bool IsDefined(String name);

		bool IsDefinedInParent(String name);

//		void AddRequire(String qualifiedName);
//		
//		void AddVariable(Identifier ident);
//
//		bool HasMethod(String name);
//		
//		void AddMethodDefintion(MethodDefinitionStatement def);
//		
//		bool HasNamespace(String name);
//		
//		void AddNamespace(NamespaceDeclaration ns);
//		
//		bool HasTypeDefinition(String name);
//		
//		void AddTypeDefinition(TypeDefinitionStatement def);
	}
}
