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

namespace Castle.Rook.AST
{
	using System;


	public interface IVisitor
	{
		void OnCompilationUnit(CompilationUnitNode node);

		void OnNamespace(NamespaceNode namespaceNode);
		
		void OnType(TypeNode typeNode);

		void OnType(ClassNode typeNode);

		void OnType(InterfaceNode typeNode);

//		void OnType(StructNode typeNode);

		void OnIdentifier(String identifier);

		void OnIdentifier(Identifier identifier);
		
		void OnIdentifier(QualifiedIdentifier qualifiedIdentifier);
		
		void OnStatement(AbstractStatement statement);
		
		void OnExpression(Expression expression);
	}
}
