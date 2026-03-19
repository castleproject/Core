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

namespace Castle.DynamicProxy;

using System;

/// <summary>
/// Collection of methods to perform unsafe manipulations on <see cref="ByRefLikeReference"/> instances.
/// Usage could lead to memory safe issues, so be careful when using it
/// </summary>
public static class ByRefLikeReferenceUnsafe
{
	[CLSCompliant(false)]
	public static unsafe ByRefLikeReference CreateByRefLikeReferenceUntyped(Type type, void* ptr, bool valueIsScoped)
	{
		return new ByRefLikeReference(type, ptr, valueIsScoped);
	}
	
#if NET9_0_OR_GREATER
	
	[CLSCompliant(false)]
	public static unsafe ByRefLikeReference<TByRefLike> CreateByRefLikeReference<TByRefLike>(Type type, void* ptr, bool valueIsScoped)
		where TByRefLike : struct, allows ref struct
	{
		return new ByRefLikeReference<TByRefLike>(type, ptr, valueIsScoped);
	}
#endif
	
	[CLSCompliant(false)]
	public static unsafe ReadOnlySpanReference<T> CreateReadOnlySpanReference<T>(Type type, void* ptr, bool valueIsScoped)
	{
		return new ReadOnlySpanReference<T>(type, ptr, valueIsScoped);
	}

	[CLSCompliant(false)]
	public static unsafe SpanReference<T> CreateSpanReference<T>(Type type, void* ptr, bool valueIsScoped)
	{
		return new SpanReference<T>(type, ptr, valueIsScoped);
	}
	
	/// <summary>
	/// Returns raw pointer held by the <paramref name="reference"/>.
	/// </summary>
	[CLSCompliant(false)]
	public static unsafe void* GetRawPtr(ByRefLikeReference reference, Type expectedType)
	{
		return reference.GetPtr(expectedType);
	}
	
	/// <summary>
	/// Disposes <paramref name="reference"/>, so that underlying value can no longer be used
	/// </summary>
	[CLSCompliant(false)]
	public static unsafe void DisposeReference(ByRefLikeReference reference, void* expectedPtr)
	{
		reference.Dispose(expectedPtr);
	}

	/// <summary>
	/// Returns information if argument value has `scoped` lifetime 
	/// </summary>
	public static bool IsReferenceValueScoped(ByRefLikeReference reference)
	{
		return reference.ValueIsScoped;
	}
}

#endif