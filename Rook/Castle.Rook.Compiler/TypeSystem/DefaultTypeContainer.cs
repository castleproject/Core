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

namespace Castle.Rook.Compiler.TypeSystem
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Reflection;
	using System.Reflection.Emit;
	
	using Castle.Model;
	using Castle.Rook.Compiler.Services;


	public class DefaultTypeContainer : ITypeContainer, IInitializable
	{
		private static readonly INamespace defaultNs = new DefaultTypeContainer();

		private readonly ICompilerOptionsSet compilerOptions;

		private IDictionary ns2Container = new HybridDictionary();
		private IDictionary name2Type = new HybridDictionary();

		public DefaultTypeContainer(ICompilerOptionsSet compilerOptions)
		{
			this.compilerOptions = compilerOptions;
		}

		protected DefaultTypeContainer()
		{
		}

		public void Initialize()
		{
			if (compilerOptions.SkipStdLib)
			{
				return;
			}
			
			AddAssembly( "mscorlib" );
			AddAssembly( "System, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" );

			// graph.AddAssemblyReference( "mscorlib" );
			// graph.AddAssemblyReference( "System, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" );
		}

		public void AddAssembly(string assemblyFileName)
		{
			AddAssembly( Assembly.Load( assemblyFileName ) );
		}

		public void AddAssembly(Assembly assembly)
		{
			Type[] types = assembly.GetExportedTypes();

			foreach(Type type in types)
			{
				if (type.FullName.IndexOf('+') != -1)
				{
					// For now we ignore Nested Types
					continue;
				}

				String ns = type.Namespace;
				
				INamespace container = GetNamespace(ns);

				container.AddType( type );
			}
		}

		public INamespace GetNamespace(String ns)
		{
			if (!(IsQualified(ns)))
			{
				// Flat hierarchy

				INamespace nsNode = ns2Container[ns] as INamespace;

				if (nsNode == null)
				{
					nsNode = new DefaultTypeContainer();
					ns2Container[ns] = nsNode;
				}

				return nsNode;
			}
			else
			{
				String stripedRoot = ExtractRootNamespace(ns, out ns);
				
				return GetNamespace(stripedRoot).GetNamespace( ns );
			}
		}

		public bool AddType(Type type)
		{
			if (name2Type.Contains( type.Name ))
			{
				throw new CompilerException( type.FullName + " already in" );
			}

			name2Type.Add( type.Name, type );

			return true;
		}

		public bool AddUserType(TypeBuilder type)
		{
			return AddType( type );
		}

		public Type LookupType(String typename)
		{
			if (IsQualified(typename))
			{
				String stripedRoot = ExtractRootNamespace(typename, out typename);
				
				return GetNamespace(stripedRoot).LookupType( typename );
			}
			else
			{
				return (Type) name2Type[typename];
			}
		}

		private bool IsQualified(string typename)
		{
			return typename.IndexOf('.') != -1 || typename.IndexOf("::") != -1;
		}

		private String ExtractRootNamespace(string typename, out string rest)
		{
			int index1, index2;

			index1 = typename.IndexOf('.');
			index2 = typename.IndexOf("::");
		
			int offset = 1;

			String stripedRoot = null;
				
			if (index2 != -1 && (index2 < index1 || index1 == -1) )
			{
				offset = 2;
			}
			else if (index2 != -1)
			{
				// Ugh! Something like System.Diagnostics::Debug was found
				// which is wrong, but we are good guys and accept it

				// TODO: Issue a warning!
			}

			if (index1 == -1) index1 = Int32.MaxValue;
			if (index2 == -1) index2 = Int32.MaxValue;

			stripedRoot = typename.Substring(0, Math.Min( index1, index2 ));

			rest = typename.Substring( stripedRoot.Length + offset );

			return stripedRoot;
		}

		public static INamespace DefaultNamespace
		{
			get { return defaultNs; }
		}
	}
}
