// Copyright 2004-2014 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Internal
{
	using System;
	using System.Linq;
	using System.Reflection;
	using Castle.DynamicProxy.Generators.Emitters;

	internal static class ExceptionMessageBuilder
	{
		/// <summary>
		/// Creates a message to inform clients that a proxy couldn't be created due to reliance on an
		/// inaccessible type (perhaps itself).
		/// </summary>
		/// <param name="inaccessibleType">the inaccessible type that prevents proxy creation</param>
		/// <param name="typeToProxy">the type that couldn't be proxied</param>
		public static string CreateMessageForInaccessibleType(Type inaccessibleType, Type typeToProxy)
		{
			var targetAssembly = typeToProxy.Assembly;

			string strongNamedOrNotIndicator = " not"; // assume not strong-named
			string assemblyToBeVisibleTo = "\"DynamicProxyGenAssembly2\""; // appropriate for non-strong-named
	
			if (targetAssembly.IsAssemblySigned())
			{
				strongNamedOrNotIndicator = "";
				assemblyToBeVisibleTo = ReferencesCastleCore(targetAssembly)
					? assemblyToBeVisibleTo = "InternalsVisible.ToDynamicProxyGenAssembly2"
					: assemblyToBeVisibleTo = '"' + InternalsVisible.ToDynamicProxyGenAssembly2 + '"';
			}

			string inaccessibleTypeDescription = inaccessibleType == typeToProxy
				? "it"
				: "type " + inaccessibleType.GetBestName();
			
			var messageFormat =
				"Can not create proxy for type {0} because {1} is not accessible. " +
				"Make it public, or internal and mark your assembly with " +
				"[assembly: InternalsVisibleTo({2})] attribute, because assembly {3} " +
				"is{4} strong-named.";

			return string.Format(messageFormat,
				typeToProxy.GetBestName(),
				inaccessibleTypeDescription,
				assemblyToBeVisibleTo,
				GetAssemblyName(targetAssembly),
				strongNamedOrNotIndicator);
		}

		private static string GetAssemblyName(Assembly targetAssembly)
		{
#if SILVERLIGHT
			// SILVERLIGHT doesn't allow us to call assembly.GetName()
			var fullName = targetAssembly.FullName;
			if (string.IsNullOrEmpty(fullName))
			{
				return fullName;
			}
			var index = fullName.IndexOf(", Version=", StringComparison.OrdinalIgnoreCase);
			if (index > 0)
			{
				return fullName.Substring(0, index);
			}
			return fullName;
#else
			return targetAssembly.GetName().Name;
#endif
		}

		private static bool ReferencesCastleCore(Assembly inspectedAssembly)
		{
#if SILVERLIGHT
			// no way to check that in SILVERLIGHT, so we just fall back to the solution that will definitely work
			return false;
#else
			return inspectedAssembly.GetReferencedAssemblies()
				.Any(r => r.FullName == Assembly.GetExecutingAssembly().FullName);
#endif
		}
	}
}