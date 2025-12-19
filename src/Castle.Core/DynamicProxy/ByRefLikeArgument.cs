// Copyright 2004-2025 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy
{
	using System;
	using System.Collections.Concurrent;
	using System.ComponentModel;
	using System.Reflection;
	using System.Runtime.CompilerServices;

	/// <summary>
	///   Wraps a byref-like (<c>ref struct</c>) method argument
	///   such that it can be placed in the <see cref="IInvocation.Arguments"/> array during interception.
	/// </summary>
	public unsafe class ByRefLikeArgument : IDisposable
	{
		private static readonly ConcurrentDictionary<Type, ConstructorInfo> constructorMap = new();

		internal static ConstructorInfo GetConstructorFor(Type byRefLikeType)
		{
			return constructorMap.GetOrAdd(byRefLikeType, static byRefLikeType =>
			{
				Type? type = null;

				if  (byRefLikeType.IsConstructedGenericType)
				{
					var typeDef = byRefLikeType.GetGenericTypeDefinition();
					if (typeDef == typeof(Span<>))
					{
						var typeArg = byRefLikeType.GetGenericArguments()[0];
						type = typeof(SpanArgument<>).MakeGenericType(typeArg);
					}
					else if (typeDef == typeof(ReadOnlySpan<>))
					{
						var typeArg = byRefLikeType.GetGenericArguments()[0];
						type = typeof(ReadOnlySpanArgument<>).MakeGenericType(typeArg);
					}
				}

#if FEATURE_ALLOWS_REF_STRUCT_ANTI_CONSTRAINT
				type ??= typeof(ByRefLikeArgument<>).MakeGenericType(byRefLikeType);
#else
				type ??= typeof(ByRefLikeArgument);
#endif

				return type.GetConstructor([ typeof(void*) ])!;
			});
		}

		protected void* ptr;

		/// <summary>
		///   Do not use this! Only generated proxies should construct instances this type.
		/// </summary>
		[CLSCompliant(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ByRefLikeArgument(void* ptr)
		{
			this.ptr = ptr;
		}

		/// <summary>
		///   Gets an unmanaged pointer to the byref-like (<c>ref struct</c>) argument.
		/// </summary>
		/// <remarks>
		///   <para>
		///     You may only use the returned pointer during the first run through the interception pipeline.
		///     After that, it will be invalid, because the argument that it referred to will be gone
		///     from the evaluation stack.
		///   </para>
		///   <para>
		///     In particular, if you intercept an <see langword="async"/> method and make use of
		///     <see cref="IInvocationProceedInfo"/> to proceed through the pipeline again
		///     after an <see langword="await"/>, you may no longer access any byref-like arguments.
		///     (.NET compilers would forbid any such attempts, too.)
		///   </para>
		///   <para>
		///     Using the returned pointer beyond the lifetime of the byref-like argument
		///     will cause undefined behavior, or an <see cref="AccessViolationException"/> at best.
		///   </para>
		/// </remarks>
		/// <exception cref="ObjectDisposedException" />
		[CLSCompliant(false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void* GetPointer()
		{
			EnsureNotDisposed();

			return ptr;
		}

		public void Dispose()
		{
			ptr = null;
		}

		protected void EnsureNotDisposed()
		{
			if (ptr == null)
			{
				throw new ObjectDisposedException(
					message: "Byref-like method arguments are only available during the method call. "
					       + "This reference has been invalidated to prevent potentially unsafe access.",
					objectName: null);
			}
		}
	}

#if FEATURE_ALLOWS_REF_STRUCT_ANTI_CONSTRAINT

	/// <summary>
	///   Wraps a byref-like (<c>ref struct</c>) method argument
	///   such that it can be placed in the <see cref="IInvocation.Arguments"/> array during interception.
	/// </summary>
	public unsafe class ByRefLikeArgument<TByRefLike> : ByRefLikeArgument where TByRefLike : allows ref struct
	{
		/// <summary>
		///   Do not use this! Only generated proxies should construct instances this type.
		/// </summary>
		[CLSCompliant(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ByRefLikeArgument(void* ptr)
			: base(ptr)
		{
		}

		/// <summary>
		///   Gets the byref-like (<c>ref struct</c>) argument.
		/// </summary>
		/// <exception cref="ObjectDisposedException" />
		public ref TByRefLike Get()
		{
			EnsureNotDisposed();

			return ref Unsafe.AsRef<TByRefLike>(ptr);
		}
	}

#endif

	// The following two specializations for `Span<T>` and `ReadOnlySpan<T>` are provided
	// because those two types have become so common in the Framework Class Library, and
	// dealing with them through unmanaged pointers all the time would be cumbersome.
	// We can provide a type-safe wrapper for them even on .NET 8. And we keep the types
	// for .NET 9 (even though they're redundant) so downstream code can expect to always
	// encounter a `[ReadOnly]SpanArgument<>` for `[ReadOnly]Span<>` regardless of
	// whether they target .NET 8 or 9.

	/// <summary>
	///   Wraps a <see cref="ReadOnlySpan{T}"/> method argument
	///   such that it can be placed in the <see cref="IInvocation.Arguments"/> array during interception.
	/// </summary>
	public unsafe class ReadOnlySpanArgument<T>
#if FEATURE_ALLOWS_REF_STRUCT_ANTI_CONSTRAINT
		: ByRefLikeArgument<ReadOnlySpan<T>>
#else
		: ByRefLikeArgument
#endif
	{
		/// <summary>
		///   Do not use this! Only generated proxies should construct instances this type.
		/// </summary>
		[CLSCompliant(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ReadOnlySpanArgument(void* ptr)
			: base(ptr)
		{
		}

#if !FEATURE_ALLOWS_REF_STRUCT_ANTI_CONSTRAINT
		/// <summary>
		///   Gets the byref-like (<c>ref struct</c>) argument.
		/// </summary>
		/// <exception cref="ObjectDisposedException" />
		public ref ReadOnlySpan<T> Get()
		{
			EnsureNotDisposed();

#pragma warning disable CS8500
			return ref *(ReadOnlySpan<T>*)ptr;
#pragma warning restore CS8500
		}
#endif
	}

	/// <summary>
	///   Wraps a <see cref="Span{T}"/> method argument
	///   such that it can be placed in the <see cref="IInvocation.Arguments"/> array during interception.
	/// </summary>
	public unsafe class SpanArgument<T>
#if FEATURE_ALLOWS_REF_STRUCT_ANTI_CONSTRAINT
		: ByRefLikeArgument<Span<T>>
#else
		: ByRefLikeArgument
#endif
	{
		/// <summary>
		///   Do not use this! Only generated proxies should construct instances this type.
		/// </summary>
		[CLSCompliant(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public SpanArgument(void* ptr)
			: base(ptr)
		{
		}

#if !FEATURE_ALLOWS_REF_STRUCT_ANTI_CONSTRAINT
		/// <summary>
		///   Gets the byref-like (<c>ref struct</c>) argument.
		/// </summary>
		/// <exception cref="ObjectDisposedException" />
		public ref Span<T> Get()
		{
			EnsureNotDisposed();

#pragma warning disable CS8500
			return ref *(Span<T>*)ptr;
#pragma warning restore CS8500
		}
#endif
	}
}

#endif
