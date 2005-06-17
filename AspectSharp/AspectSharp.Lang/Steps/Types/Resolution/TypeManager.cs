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

namespace AspectSharp.Lang.Steps.Types.Resolution
{
	using System;
	using System.Reflection;
	using System.Collections;

	using AspectSharp.Lang.AST.Types;

	/// <summary>
	/// Summary description for TypeManager.
	/// </summary>
	public class TypeManager
	{
		private TypeResolverList _tree = new TypeResolverList();

		public void InspectAppDomainAssemblies()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach( Assembly assembly in assemblies )
			{
				DiscoverTypes(assembly);
			}
		}

		public void AddAssembly(Assembly assembly)
		{
			DiscoverTypes(assembly);
		}

		public Type ResolveType( String name )
		{
			return Tree.ResolveType(name);
		}

		protected void DiscoverTypes( Assembly assembly )
		{
			Type[] types = null;

			try
			{
				types = assembly.GetExportedTypes();
			}
			catch(Exception)
			{
				// Ok, Dynamic assemblies don't support GetExportedTypes
				types = new Type[0];
			}
			
			foreach( Type type in types )
			{
				String nameSpace = type.Namespace;

				if (nameSpace == null)
				{
					continue;
				}
				
				Tree.DefineNamespace( nameSpace ).AddType( type );
			}
		}

		private TypeResolverList Tree
		{
			get { return _tree; }
		}
	}
}
