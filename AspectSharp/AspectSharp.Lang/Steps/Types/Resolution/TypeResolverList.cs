using AspectSharp.Lang.AST.Types;
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

namespace AspectSharp.Lang.Steps.Types.Resolution
{
	using System;
	using System.Collections;

	/// <summary>
	/// Summary description for TypeResolverList.
	/// </summary>
	public class TypeResolverList : ITypeResolver
	{
		private IDictionary _namespaces = new Hashtable();

		public NamespaceTypeMap DefineNamespace(String fullName)
		{
			NamespaceTypeMap map = _namespaces[ fullName ] as NamespaceTypeMap;

			if (map == null)
			{
				map = new NamespaceTypeMap( fullName );
				_namespaces[ fullName ] = map;
			}

			return map;
		}

		public Type ResolveType(String name)
		{
			if (HasNamespace(name))
			{
				String typeName = ExtractTypeName(name);
				return TypeResolverFromName(name).ResolveType( typeName );
			}
			else
			{
				foreach(ITypeResolver resolver in _namespaces.Values)
				{
					if (resolver.Contains(name))
					{
						return resolver.ResolveType(name);
					}
				}
			}
			return null;
		}

		public bool Contains(String name)
		{
			if (HasNamespace(name))
			{
				String typeName = ExtractTypeName(name);
				return TypeResolverFromName(name).Contains( typeName );
			}
			else
			{
				foreach(ITypeResolver resolver in _namespaces.Values)
				{
					if (resolver.Contains(name))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool HasNamespace( String name )
		{
			return name.IndexOf('.') != -1;
		}

		private ITypeResolver TypeResolverFromName( String fullTypeName )
		{
			String nameSpace = ExtractNamespace( fullTypeName );
			return DefineNamespace( nameSpace );
		}

		private String ExtractNamespace( String name )
		{
			int index = name.LastIndexOf('.');
			return index == -1 ? name : name.Substring(0, index);
		}

		private String ExtractTypeName( String name )
		{
			int index = name.LastIndexOf('.');
			return index == -1 ? name : name.Substring(index+1);
		}
	}
}
