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

namespace Castle.Rook.Compiler.TypeGraph
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	using Castle.Rook.Compiler.AST;


	public class TransientType : TypeDefinition
	{
		private readonly TypeDefinitionStatement typeDef;
		private ListValueDictionary methods = new ListValueDictionary();
		private ListValueDictionary smethods = new ListValueDictionary();
		private ArrayList constructors = new ArrayList();
		private HybridDictionary fields = new HybridDictionary();
		private HybridDictionary sfields = new HybridDictionary();

		public TransientType(TypeDefinitionStatement typeDef)
		{
			this.typeDef = typeDef;
		}

		public TypeDefinitionStatement TypeDef
		{
			get { return typeDef; }
		}

		public override String Name
		{
			get { return typeDef.Name; }
		}

		public override String FullName
		{
			get { return typeDef.FullName; }
		}

		public override bool HasInstanceMember(String name)
		{
			return methods.Contains(name) || fields.Contains(name);
		}

		public override bool HasStaticMember(String name)
		{
			return smethods.Contains(name) || sfields.Contains(name);
		}

		public void AddMethod(MethodDefinitionStatement methodDef)
		{
			if (methodDef.IsConstructor)
			{
				AddConstructor(methodDef);
			}
			else if (methodDef.IsStatic)
			{
				AddStaticMethod(methodDef);
			}
			else 
			{
				AddInstanceMethod(methodDef);
			}
		}

		public void AddField(SingleVariableDeclarationStatement singleVarDecl)
		{
			HybridDictionary dict = null;

			if (singleVarDecl.Identifier.Type == IdentifierType.Local)
			{
				// Pardon?
				throw new ArgumentException("A local var declaration should get here");
			}
			else if (singleVarDecl.Identifier.Type == IdentifierType.InstanceField)
			{
				dict = fields;
			}
			else if (singleVarDecl.Identifier.Type == IdentifierType.StaticField)
			{
				dict = sfields;
			}

			dict.Add(singleVarDecl.Identifier.Name, singleVarDecl);
		}

		private void AddInstanceMethod(MethodDefinitionStatement def)
		{
			methods.Add(def.Name, def);
		}

		private void AddStaticMethod(MethodDefinitionStatement def)
		{
			smethods.Add(def.Name, def);
		}

		private void AddConstructor(MethodDefinitionStatement def)
		{
			constructors.Add(def);
		}

	}
}
