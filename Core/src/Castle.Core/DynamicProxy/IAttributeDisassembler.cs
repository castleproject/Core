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

namespace Castle.DynamicProxy
{
	using System;
	using System.Reflection.Emit;

	/// <summary>
	///   Provides functionality for disassembling instances of attributes to CustomAttributeBuilder form, during the process of emiting new types by Dynamic Proxy.
	/// </summary>
	public interface IAttributeDisassembler
	{
		/// <summary>
		///   Disassembles given attribute instance back to corresponding CustomAttributeBuilder.
		/// </summary>
		/// <param name = "attribute">An instance of attribute to disassemble</param>
		/// <returns><see cref = "CustomAttributeBuilder" /> corresponding 1 to 1 to given attribute instance, or null reference.</returns>
		/// <remarks>
		///   Implementers should return <see cref = "CustomAttributeBuilder" /> that corresponds to given attribute instance 1 to 1,
		///   that is after calling specified constructor with specified arguments, and setting specified properties and fields with values specified
		///   we should be able to get an attribute instance identical to the one passed in <paramref name = "attribute" />. Implementer can return null
		///   if it wishes to opt out of replicating the attribute. Notice however, that for some cases, like attributes passed explicitly by the user
		///   it is illegal to return null, and doing so will result in exception.
		/// </remarks>
		CustomAttributeBuilder Disassemble(Attribute attribute);
	}
}