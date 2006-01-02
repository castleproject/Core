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

namespace Castle.Rook.AST
{
	using System;
	using System.Collections.Specialized;


	public class NamingScope : INamingScope
	{
		private INamingScope parent;
		private HybridDictionary dict = new HybridDictionary();

		public NamingScope()
		{
		}

		public bool HasName(String name)
		{
			if (dict.Contains(name))
			{
				return true;
			}

			if (Parent != null) return Parent.HasName(name);

			return false;
		}

		public void Register(String name)
		{
			
		}

		public INamingScope Parent
		{
			get { return parent; }
			set { parent = value; }
		}
	}
}