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
	using System.Collections;


	public class MethodDefinitionStatement : AbstractStatement
	{
		private QualifiedIdentifier returnType;
		private String name;
		private String boundTo;
		private IList parameters = new ArrayList();
		private IList statements = new ArrayList();

		public MethodDefinitionStatement(String[] nameParts)
		{
			if (nameParts[1] == null)
			{
				name = nameParts[0];
			}
			else
			{
				name = nameParts[0];
				boundTo = nameParts[1]; // self, interface and so on
			}
		}

		public IList Parameters
		{
			get { return parameters; }
		}

		public IList Statements
		{
			get { return statements; }
		}

		public QualifiedIdentifier ReturnType
		{
			get { return returnType; }
			set { returnType = value; }
		}

		public string Name
		{
			get { return name; }
		}

		public string BoundTo
		{
			get { return boundTo; }
		}
	}
}
