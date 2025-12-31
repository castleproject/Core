// Copyright 2004-2026 Castle Project - http://www.castleproject.org/
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

#if NET9_0_OR_GREATER

#nullable enable

namespace Castle.DynamicProxy
{
	using System;
	using System.IO;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Runtime.Loader;

	/// <summary>
	///   A <see cref="ModuleScope"/> specialization used by <see cref="PersistentProxyBuilder"/>,
	///   based on <see cref="PersistedAssemblyBuilder"/>.
	/// </summary>
	/// <remarks>
	///   Dynamic types created with <see cref="PersistedAssemblyBuilder"/> cannot be activated.
	///   In order for them to be usable, the dynamic assembly has to first be written out
	///   and then loaded by the runtime as a regular assembly. This implies that either all proxy types
	///   must be generated ahead of time (if they are to be placed in a single assembly); or, that
	///   every proxy type gets its own assembly (if they should be immediately activatable).
	///   This class opts for the latter approach.
	/// </remarks>
	internal sealed class PersistentProxyBuilderModuleScope : ModuleScope
	{
		private bool usingStrongNamedModule;

		public PersistentProxyBuilderModuleScope()
			: base(savePhysicalAssembly: false, disableSignedModule: false)
		{
			usingStrongNamedModule = false;
		}

		public event Action<Assembly, byte[]>? AssemblyCreated;

		internal override AssemblyBuilder CreateAssembly(bool signStrongName)
		{
			var assemblyName = GetAssemblyName(signStrongName);
			return new PersistedAssemblyBuilder(assemblyName, coreAssembly: typeof(object).Assembly);
		}

		internal override TypeBuilder DefineType(bool inSignedModulePreferably, string name, TypeAttributes flags)
		{
			TypeBuilder typeBuilder;

			if (IsAuxiliaryType(name) == false)
			{
				// The requested `TypeBuilder` is for a main proxy type.
				// Each of those gets placed in its own assembly.
				ResetModules();
				typeBuilder = base.DefineType(inSignedModulePreferably, name, flags);
				usingStrongNamedModule = typeBuilder.Module == StrongNamedModule;
			}
			else
			{
				// The requested `TypeBuilder` is for an extra type needed for
				// the main proxy type. (Currently, those are always invocation types.)
				// They need to be placed in the same assembly as the main proxy type,
				// otherwise the runtime won't find them during actual use.
				//
				// TODO: Currently, strong-named modules seem to be preferred for
				// invocation types even when the main proxy type is in a non-strong-named
				// module. Should find out why that is. Also, ignoring that preference
				// may cause other problems.
				typeBuilder = base.DefineType(usingStrongNamedModule, name, flags);
			}

			return typeBuilder;
		}

		internal override Type BuildType(TypeBuilder typeBuilder)
		{
			Type type = typeBuilder.CreateTypeInfo();
			var typeName = type.FullName!;

			if (IsAuxiliaryType(typeName) == false)
			{
				var assemblyBuilder = (PersistedAssemblyBuilder)typeBuilder.Assembly;

				using var stream = new MemoryStream();
				assemblyBuilder.Save(stream);

				stream.Seek(0, SeekOrigin.Begin);
				var alc = new AssemblyLoadContext(name: null);
				var assembly = alc.LoadFromStream(stream);

				stream.Seek(0, SeekOrigin.Begin);
				AssemblyCreated?.Invoke(assembly, stream.GetBuffer());

				type = assembly.GetType(typeName)!;
			}

			return type;
		}

		private bool IsAuxiliaryType(string name)
		{
			return name.StartsWith("Castle.Proxies.Invocations.");
		}
	}
}

#endif
