// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Generators
{
	/// <summary>
	///   Represents the scope of uniquenes of names for types and their members
	/// </summary>
	public interface INamingScope
	{
		INamingScope ParentScope { get; }

		/// <summary>
		///   Gets a unique name based on <paramref name = "suggestedName" />
		/// </summary>
		/// <param name = "suggestedName">Name suggested by the caller</param>
		/// <returns>Unique name based on <paramref name = "suggestedName" />.</returns>
		/// <remarks>
		///   Implementers should provide name as closely resembling <paramref name = "suggestedName" /> as possible.
		///   Generally if no collision occurs it is suggested to return suggested name, otherwise append sequential suffix.
		///   Implementers must return deterministic names, that is when <see cref = "GetUniqueName" /> is called twice 
		///   with the same suggested name, the same returned name should be provided each time. Non-deterministic return
		///   values, like appending random suffices will break serialization of proxies.
		/// </remarks>
		string GetUniqueName(string suggestedName);

		/// <summary>
		///   Returns new, disposable naming scope. It is responsibilty of the caller to make sure that no naming collision
		///   with enclosing scope, or other subscopes is possible.
		/// </summary>
		/// <returns>New naming scope.</returns>
		INamingScope SafeSubScope();
	}
}