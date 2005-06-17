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


	public abstract class AbstractDeclarationContainer : AbstractASTNode
	{
		private IList namespaces = new ArrayList();
		private IList mixintypes = new ArrayList();
		private IList interfaces = new ArrayList();
		private IList classes = new ArrayList();
		private IList structs = new ArrayList();

		public IList Namespaces
		{
			get { return namespaces; }
		}

		public IList MixinTypes
		{
			get { return mixintypes; }
		}

		public IList ClassesTypes
		{
			get { return classes; }
		}

		public IList InterfaceTypes
		{
			get { return interfaces; }
		}

		public IList StructTypes
		{
			get { return structs; }
		}
	}
}
