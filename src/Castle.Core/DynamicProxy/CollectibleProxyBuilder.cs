// Copyright 2004-2024 Castle Project - http://www.castleproject.org/
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

#nullable enable

namespace Castle.DynamicProxy
{
	using System.Reflection.Emit;

	/// <summary>
	///   Proxy builder that causes generated assemblies to be collectible.
	/// </summary>
	public class CollectibleProxyBuilder : DefaultProxyBuilder
	{
		/// <summary>
		///   Initializes a new instance of the <see cref="CollectibleProxyBuilder" /> class.
		/// </summary>
		public CollectibleProxyBuilder() : base(new ModuleScope(AssemblyBuilderAccess.RunAndCollect))
		{
		}
	}
}
