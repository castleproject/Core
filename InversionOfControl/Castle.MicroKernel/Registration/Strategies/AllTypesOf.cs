// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Registration
{
	using System;
	using System.IO;
	using System.Reflection;
	using System.Collections.Generic;
	
	/// <summary>
	/// Describes a set of components to register in the kernel.
	/// </summary>
	/// <typeparam name="T">The base type to match against.</typeparam>
	public class AllTypesOf<T>
	{
		protected AllTypesOf()
		{
		}
		
		/// <summary>
		/// Prepares to register types from an assembly.
		/// </summary>
		/// <param name="assemblyName">The assembly name.</param>
		/// <returns>The corresponding <see cref="TypesDescriptor{T}"/></returns>
		public static TypesDescriptor<T> FromAssembly(string assemblyName)
		{
			Assembly assembly;
			String extension = Path.GetExtension(assemblyName);
			
			if (extension == ".dll" || extension == ".exe")
			{
				if (Path.GetDirectoryName(assemblyName) == AppDomain.CurrentDomain.BaseDirectory)
				{
					assembly = Assembly.Load(Path.GetFileNameWithoutExtension(assemblyName));
				}
				else
				{
					assembly = Assembly.LoadFile(assemblyName);
				}
			}
			else
			{
				assembly = Assembly.Load(assemblyName);
			}
			
			return FromAssembly(assembly);
		}

		/// <summary>
		/// Prepares to register types from an assembly.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <returns>The corresponding <see cref="TypesDescriptor{T}"/></returns>
		public static TypesDescriptor<T> FromAssembly(Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			return From(assembly.GetExportedTypes());
		}

		/// <summary>
		/// Prepares to register types from a list of types.
		/// </summary>
		/// <param name="types">The list of types.</param>
		/// <returns>The corresponding <see cref="TypesDescriptor{T}"/></returns>
		public static TypesDescriptor<T> From(IEnumerable<Type> types)
		{
			return new TypesDescriptor<T>(types);
		}

		/// <summary>
		/// Prepares to register types from a list of types.
		/// </summary>
		/// <param name="types">The list of types.</param>
		/// <returns>The corresponding <see cref="TypesDescriptor{T}"/></returns>
		public static TypesDescriptor<T> Pick(IEnumerable<Type> types)
		{
			return new TypesDescriptor<T>(types);
		}
		
		/// <summary>
		/// Prepares to register types from a list of types.
		/// </summary>
		/// <param name="types">The list of types.</param>
		/// <returns>The corresponding <see cref="TypesDescriptor{T}"/></returns>
		public static TypesDescriptor<T> From(params Type[] types)
		{
			return new TypesDescriptor<T>(types);
		}
	}

	/// <summary>
	/// Describes a set of components to register in the kernel.
	/// </summary>
	public class AllTypes : AllTypesOf<object>
	{
	}
}
