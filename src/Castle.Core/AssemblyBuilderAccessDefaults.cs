using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Castle
{
	public static class AssemblyBuilderAccessDefaults
	{
#if NET
		// If Castle.Core is loaded in a collectible AssemblyLoadContext, the generated assembly should be collectible as well by default.
		static readonly AssemblyBuilderAccess _automatic = System.Runtime.Loader.AssemblyLoadContext.GetLoadContext(typeof(AssemblyBuilderAccessDefaults).Assembly).IsCollectible ? AssemblyBuilderAccess.RunAndCollect : AssemblyBuilderAccess.Run;
#else
		static readonly AssemblyBuilderAccess _automatic = AssemblyBuilderAccess.Run;
#endif
		static AssemblyBuilderAccess? _override;

		/// <summary>
		/// Get or set the default <see cref="System.Reflection.Emit.AssemblyBuilderAccess"/> to use when creating dynamic assemblies.
		/// On .Net Core, the default is <see cref="System.Reflection.Emit.AssemblyBuilderAccess.RunAndCollect"/> if Castle.Core is loaded in a collectible AssemblyLoadContext.
		/// </summary>
		public static AssemblyBuilderAccess Default
		{
			get => _override ?? _automatic;
			set => _override = value;
		}
	}
}
