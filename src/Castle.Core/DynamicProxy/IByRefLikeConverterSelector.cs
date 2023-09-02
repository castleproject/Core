// Copyright 2004-2023 Castle Project - http://www.castleproject.org/
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

#if FEATURE_BYREFLIKE

namespace Castle.DynamicProxy
{
	using System;
	using System.Reflection;

	/// <summary>
	///   Provides an extension point that allows proxies to convert by-ref-like argument values
	///   on a per method parameter basis.
	/// </summary>
	public interface IByRefLikeConverterSelector
	{
		/// <summary>
		///   Selects the converter that should be used to convert by-ref-like argument values of the given method parameter.
		/// </summary>
		/// <param name="method">The method that will be intercepted.</param>
		/// <param name="parameterPosition">The zero-based index of the method parameter for which an argument value is to be converted.</param>
		/// <param name="parameterType">The type of the method parameter for which an argument value is to be converted.</param>
		/// <returns>The type of converter that should be used to convert argument values of the given method parameter.</returns>
		Type SelectConverterType(MethodInfo method, int parameterPosition, Type parameterType);
	}
}

#endif
