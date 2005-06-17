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

namespace AspectSharp.Lang.AST.Types
{
	using System;
	using System.Collections;
	using System.Reflection;

	/// <summary>
	/// Summary description for NamespaceTypeMap.
	/// </summary>
	public class NamespaceTypeMap : ITypeResolver
	{
		private String _namespace;
		private IDictionary _name2Type = new Hashtable();

		public NamespaceTypeMap( String nameSpace )
		{
			_namespace = nameSpace;
		}

		public String Namespace
		{
			get { return _namespace; }
		}

		public void AddType( Type type )
		{
			_name2Type[ type.Name ] = type;
		}

		public Type ResolveType( String name )
		{
			return _name2Type[ name ] as Type;
		}

		public bool Contains( String name )
		{
			return _name2Type.Contains(name);
		}
	}
}
