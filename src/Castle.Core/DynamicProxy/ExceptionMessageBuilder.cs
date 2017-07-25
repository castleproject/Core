// Copyright 2004-2017 Castle Project - http://www.castleproject.org/
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
	using System;
	using System.Linq;
	using System.Reflection;

	using Castle.Core.Internal;
	using Castle.DynamicProxy.Generators.Emitters;

	internal static class ExceptionMessageBuilder
	{
		/// <summary>
		/// Provides instructions that a user could follow to make a type or method in <paramref name="targetAssembly"/>
		/// visible to DynamicProxy.</summary>
		/// <param name="targetAssembly">The assembly containing the type or method.</param>
		/// <returns>Instructions that a user could follow to make a type or method visible to DynamicProxy.</returns>
		internal static string CreateInstructionsToMakeVisible(Assembly targetAssembly)
		{
			string strongNamedOrNotIndicator = " not"; // assume not strong-named
			string assemblyToBeVisibleTo = "\"DynamicProxyGenAssembly2\""; // appropriate for non-strong-named

			if (targetAssembly.IsAssemblySigned())
			{
				strongNamedOrNotIndicator = "";
				assemblyToBeVisibleTo = ReferencesCastleCore(targetAssembly)
					? "InternalsVisible.ToDynamicProxyGenAssembly2"
					: '"' + InternalsVisible.ToDynamicProxyGenAssembly2 + '"';
			}

			var instructionsFormat =
				"Make it public, or internal and mark your assembly with " +
				"[assembly: InternalsVisibleTo({0})] attribute, because assembly {1} " +
				"is{2} strong-named.";

			var instructions = String.Format(instructionsFormat,
				assemblyToBeVisibleTo,
				targetAssembly.GetName().Name,
				strongNamedOrNotIndicator);
			return instructions;

			bool ReferencesCastleCore(Assembly ia)
			{
#if FEATURE_GET_REFERENCED_ASSEMBLIES
				return ia.GetReferencedAssemblies()
					.Any(r => r.FullName == Assembly.GetExecutingAssembly().FullName);
#else
				// .NET Core does not provide an API to do this, so we just fall back to the solution that will definitely work.
				// After all it is just an exception message.
				return false;
#endif
			}
		}

		/// <summary>
		/// Creates a message to inform clients that a proxy couldn't be created due to reliance on an
		/// inaccessible type (perhaps itself).
		/// </summary>
		/// <param name="inaccessibleType">the inaccessible type that prevents proxy creation</param>
		/// <param name="typeToProxy">the type that couldn't be proxied</param>
		public static string CreateMessageForInaccessibleType(Type inaccessibleType, Type typeToProxy)
		{
			var targetAssembly = typeToProxy.GetTypeInfo().Assembly;

			string inaccessibleTypeDescription = inaccessibleType == typeToProxy
				? "it"
				: "type " + inaccessibleType.GetBestName();

			var messageFormat = "Can not create proxy for type {0} because {1} is not accessible. ";

			var message = string.Format(messageFormat,
				typeToProxy.GetBestName(),
				inaccessibleTypeDescription);

			var instructions = CreateInstructionsToMakeVisible(targetAssembly);

			return message + instructions;
		}
	}
}