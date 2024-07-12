// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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


namespace Castle.DynamicProxy
{
	using System.Reflection.Emit;

	public static class AssemblyBuilderAccessDefaults
	{
#if NET
		// If Castle.Core is loaded in a collectible AssemblyLoadContext, the generated assembly should be collectible as well by default.
		static readonly AssemblyBuilderAccess automatic = System.Runtime.Loader.AssemblyLoadContext.GetLoadContext(typeof(AssemblyBuilderAccessDefaults).Assembly).IsCollectible ? AssemblyBuilderAccess.RunAndCollect : AssemblyBuilderAccess.Run;
#else
		static readonly AssemblyBuilderAccess automatic = AssemblyBuilderAccess.Run;
#endif
		static AssemblyBuilderAccess? userSpecified;

		/// <summary>
		/// Get or set the default <see cref="System.Reflection.Emit.AssemblyBuilderAccess"/> to use when creating dynamic assemblies.
		/// On .Net Core, the default is <see cref="System.Reflection.Emit.AssemblyBuilderAccess.RunAndCollect"/> if Castle.Core is loaded in a collectible AssemblyLoadContext.
		/// </summary>
		public static AssemblyBuilderAccess Default
		{
			get => userSpecified ?? automatic;
			set => userSpecified = value;
		}
	}
}
