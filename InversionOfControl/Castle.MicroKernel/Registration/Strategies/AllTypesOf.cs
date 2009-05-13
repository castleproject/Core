// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;
	using System.Collections.Generic;
	
	/// <summary>
	/// Describes a related group of components to register in the kernel.
	/// </summary>
	public class AllTypesOf
	{
		private readonly Type basedOn;

		internal AllTypesOf(Type basedOn)
		{
			this.basedOn = basedOn;
		}
		
		/// <summary>
		/// Prepares to register types from an assembly.
		/// </summary>
		/// <param name="assemblyName">The assembly name.</param>
		/// <returns>The corresponding <see cref="BasedOnDescriptor"/></returns>
		public BasedOnDescriptor FromAssemblyNamed(string assemblyName)
		{
			return AllTypes.FromAssemblyNamed(assemblyName).BasedOn(basedOn);
		}

		/// <summary>
		/// Prepares to register types from an assembly.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <returns>The corresponding <see cref="BasedOnDescriptor"/></returns>
		public BasedOnDescriptor FromAssembly(Assembly assembly)
		{
			return AllTypes.FromAssembly(assembly).BasedOn(basedOn);
		}

		/// <summary>
		/// Prepares to register types from a list of types.
		/// </summary>
		/// <param name="types">The list of types.</param>
		/// <returns>The corresponding <see cref="BasedOnDescriptor"/></returns>
		public BasedOnDescriptor From(IEnumerable<Type> types)
		{
			return AllTypes.From(types).BasedOn(basedOn);
		}

		/// <summary>
		/// Prepares to register types from a list of types.
		/// </summary>
		/// <param name="types">The list of types.</param>
		/// <returns>The corresponding <see cref="BasedOnDescriptor"/></returns>
		public BasedOnDescriptor Pick(IEnumerable<Type> types)
		{
			return AllTypes.Pick(types).BasedOn(basedOn);
		}
		
		/// <summary>
		/// Prepares to register types from a list of types.
		/// </summary>
		/// <param name="types">The list of types.</param>
		/// <returns>The corresponding <see cref="BasedOnDescriptor"/></returns>
		public BasedOnDescriptor From(params Type[] types)
		{
			return AllTypes.From(types).BasedOn(basedOn);
		}
	}
}
