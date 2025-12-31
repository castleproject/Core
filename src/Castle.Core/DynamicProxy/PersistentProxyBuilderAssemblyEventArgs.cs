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
	using System.Reflection;

	/// <summary>
	///   Provides data for the <see cref="PersistentProxyBuilder.AssemblyCreated"/> event.
	/// </summary>
	public sealed class PersistentProxyBuilderAssemblyEventArgs : EventArgs
	{
		/// <summary>
		///   Initializes a new instance of the <see cref="PersistentProxyBuilderAssemblyEventArgs"/> class.
		/// </summary>
		/// <param name="assembly">The assembly that has been created and loaded into the runtime.</param>
		/// <param name="assemblyBytes">The raw bytes of the created assembly (can be saved as a DLL file).</param>
		internal PersistentProxyBuilderAssemblyEventArgs(Assembly assembly, byte[] assemblyBytes)
		{
			Assembly = assembly;
			AssemblyBytes = assemblyBytes;
		}

		/// <summary>
		///   The assembly that has been created and loaded into the runtime.
		/// </summary>
		public Assembly Assembly { get; }

		/// <summary>
		///   The raw bytes of the created assembly (can be saved as a DLL file).
		/// </summary>
		/// <remarks></remarks>
		public byte[] AssemblyBytes { get; }
	}
}

#endif
