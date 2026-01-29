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

#if FEATURE_BYREFLIKE

#nullable enable 

namespace Castle.DynamicProxy.Tests.ByRefLikeSupport
{
	using System;

	using NUnit.Framework;

	/// <summary>
	///   Tests whether types involving byref-like parameters and return types can be proxied.
	/// </summary>
	[TestFixture]
	public class ProxyableTestCase : BasePEVerifyTestCase
	{
		// `ByRefLike` types:
		[TestCase(typeof(IPassByRefLikeByValue))]
		[TestCase(typeof(IPassByRefLikeByRefIn))]
		[TestCase(typeof(IPassByRefLikeByRefRef))]
		[TestCase(typeof(IPassByRefLikeByRefOut))]
		[TestCase(typeof(IReturnByRefLikeByValue))]
		// `ReadOnlySpan<T>` types:
		[TestCase(typeof(IPassReadOnlySpanByValue))]
		[TestCase(typeof(IPassReadOnlySpanByRefIn))]
		[TestCase(typeof(IPassReadOnlySpanByRefRef))]
		[TestCase(typeof(IPassReadOnlySpanByRefOut))]
		[TestCase(typeof(IReturnReadOnlySpanByValue))]
		// `Span<T>` types:
		[TestCase(typeof(IPassSpanByValue))]
		[TestCase(typeof(IPassSpanByRefIn))]
		[TestCase(typeof(IPassSpanByRefRef))]
		[TestCase(typeof(IPassSpanByRefOut))]
		[TestCase(typeof(IReturnSpanByValue))]
		public void Can_proxy_type_with_byref_like_parameters(Type interfaceType)
		{
			_ = generator.CreateInterfaceProxyWithoutTarget(interfaceType);
		}
	}
}

#endif
