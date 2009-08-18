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

namespace Castle.Core.Interceptor
{
	using System;
	using System.Reflection.Emit;

	/// <summary>
	/// Encapsulates process of custom attribute dissassembly back to <see cref="CustomAttributeBuilder"/>.
	/// </summary>
	/// <remarks>
	/// This interface is used for custom attribute replication in Dynamic Proxy and to put additional custom attributes on proxy type.
	/// Implement it if default implementatio does not meet your needs or does not support your scenario.
	/// </remarks>
	public interface IAttributeDisassembler
	{
		/// <summary>
		/// Dissassembles given <paramref name="attribute"/> to <see cref="CustomAttributeBuilder"/>.
		/// </summary>
		/// <param name="attribute">Custom attribute instance to disassemble.</param>
		/// <returns>Builder that represents <paramref name="attribute"/>.</returns>
		CustomAttributeBuilder Disassemble(Attribute attribute);
	}
}