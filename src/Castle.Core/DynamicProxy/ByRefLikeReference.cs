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
#pragma warning disable CS8500

namespace Castle.DynamicProxy
{
	using System;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Threading;

	using Castle.DynamicProxy.Internal;

	// This file contains a set of `unsafe` types used at runtime by DynamicProxy proxies to represent byref-like values
	// in an `IInvocation`. Such values live exclusively on the evaluation stack and therefore cannot be boxed. Thus they are
	// in principle incompatible with `IInvocation` and we need to replace them with something else... namely these types here.
	//
	// What follows are the safety considerations that went into the design of these types.
	//
	// *) These types use unmanaged pointers (`void*`) to reference storage locations (of byref-like method parameters).
	//
	// *) Unmanaged pointers are generally unsafe when used to reference unpinned heap-allocated objects.
	//    These types here should NEVER reference heap-allocated objects. We attempt to enforce this by asking for the
	//    `type` of the storage location, and throw for anything other than byref-like types (which by definition cannot
	//    live on the heap).
	//
	// *) Unmanaged pointers can be safe when used to reference stack-allocated objects. However, that is only true
	//    when they point into "live" stack frames. That is, they MUST NOT reference parameters or local variables
	//    of methods that have already finished executing. This is why we have the `ByRefLikeReference.Dispose` method:
	//    DynamicProxy (or whatever else instantiated a `ByRefLikeReference` object to point at a method parameter or local
	//    variable) must invoke this method before said method returns (or tail-calls).
	//
	// *) The `checkType` / `checkPtr` arguments of `GetPtr` or `Dispose`, respectively, have two purposes:
	//
	//     1. DynamicProxy, or whatever else instantiated a `ByRefLikeReference`, is expected to know at all times what
	//        exactly each instance references. These parameters make it harder for anyone to use the type directly
	//        if they didn't also instantiate it themselves.
	//
	//     2. `checkPtr` of `Dispose` attempts to prevent re-use of a referenced storage location for another
	//        similarly-typed local variable by the JIT. DynamicProxy typically instantiates `ByRefLikeReference` instances
	//        at the start of intercepted method bodies, and it invokes `Dispose` at the very end, meaning that
	//        the address of the local/parameter is taken at each method boundary, meaning that static analysis should
	//        never during the whole method see the local/parameter as "no longer in use". (This may be a little
	//        paranoid, since the CoreCLR JIT probably exempts so-called "address-exposed" locals from reuse anyway.)
	//
	// *) We track if each reference represents scoped value. Scoped values usually represent data living on stack only,
	//    so we should apply more strict rules on how we expose the value. Otherwise, we could have use-after-free scenario,
	//    which could lead to stack corruption and weird behavior.
	//    Dynamic generator analyzes method signature and provides the correct value.
	//
	// *) We always return a copy of the original value and never expose the value by reference.
	//    This is important to make sure that we prevent possibility `ref struct` interior mutability (as copy will be changed)
	//    and close a possibility to leak scoped values out of their lifetime scope by storing them inside another `ref struct`.
	//
	// *) The SetValue() function uses delegate to get the value. This is required to make sure that local variables
	//    or variables with scoped visibility could never be promoted outside their lifetime.
	//    When we have delegate, we could only use heap-backed values and compiler will enforce the safety for us.
	//
	// *) Finally, we allow accessing reference data from the owning thread only to avoid all possible concurrency-related issues.
	//    Otherwise, it's very easy to have use-after-free scenario, as other thread could access value after function exit.
	//
	// As far as I can reason, `ByRefLikeReference` et al. should be safe to use IFF they are never copied out from an
	// `IInvocation`, and IFF DynamicProxy succeeds in destructing them and erasing them from the `IInvocation` right
	// before the intercepted method finishes executing.

	public unsafe class ByRefLikeReference
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Type type;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private void* ptr;

		private Thread ownerThread;

		internal bool ValueIsScoped { get; }

		internal ByRefLikeReference(Type type, void* ptr, bool valueIsScoped)
		{
			if (type.IsByRefLikeSafe() == false)
			{
				throw new ArgumentOutOfRangeException(nameof(type));
			}

			if (ptr == null)
			{
				throw new ArgumentNullException(nameof(ptr));
			}

			this.type = type;
			this.ptr = ptr;
			this.ValueIsScoped = valueIsScoped;
			this.ownerThread = Thread.CurrentThread;
		}

		internal void* GetPtr(Type checkType)
		{
			AssertCurrentThread();
			
			if (checkType != type)
			{
				throw new ArgumentException($"The reference type {type.FullName} does not match the expected type {checkType.FullName}");
			}

			if (this.ptr == null)
			{
				throw new ObjectDisposedException("This reference was already disposed");
			}
			
			return this.ptr;
		}

		internal void Dispose(void* checkPtr)
		{
			AssertCurrentThread();
			
			if (this.ptr == null || this.ptr != checkPtr)
			{
				throw new InvalidOperationException($"BUG: Pointer mismatch on reference disposal. Expected: {(nint)checkPtr:X16}, Actual: {(nint)this.ptr:X16}");
			}

			this.ptr = null;
		}

		private void AssertCurrentThread()
		{
			if (this.ownerThread != Thread.CurrentThread)
			{
				throw new InvalidOperationException("This reference cannot be used from another thread");
			}
		}
	}

#if NET9_0_OR_GREATER
	/// <summary>
	///   Permits indirect access to byref-like argument values during method interception.
	/// </summary>
	/// <remarks>
	///   Instances of byref-like (<c>ref struct</c>) types live exclusively on the evaluation stack.
	///   Therefore, they cannot be boxed and put into the <see langword="object"/>-typed <see cref="IInvocation.Arguments"/> array.
	///   DynamicProxy replaces these unboxable values with <see cref="ByRefLikeReference{TByRefLike}"/> references
	///   (or, in the case of spans, with <see cref="SpanReference{T}"/> or <see cref="ReadOnlySpanReference{T}"/>),
	///   which grant you indirect read/write access to the actual values.
	///   <para>
	///     These references are only valid for the duration of the intercepted method call.
	///     Any attempt to use it beyond that will result in a <see cref="AccessViolationException"/>.
	///   </para>
	/// </remarks>
	/// <typeparam name="TByRefLike">A byref-like (<c>ref struct</c>) type.</typeparam>
	public unsafe class ByRefLikeReference<TByRefLike> : ByRefLikeReference
		where TByRefLike : struct, allows ref struct
	{
		internal ByRefLikeReference(Type type, void* ptr, bool valueIsScoped)
			: base(type, ptr, valueIsScoped)
		{
			if (type != typeof(TByRefLike))
			{
				throw new ArgumentOutOfRangeException(nameof(type));
			}
		}

		private ref TByRefLike Value
		{
			get
			{
				return ref *(TByRefLike*)GetPtr(typeof(TByRefLike));
			}
		}
		
		public TByRefLike GetValue()
		{
			if (ValueIsScoped)
			{
				throw new InvalidOperationException($"Use {nameof(UseValue)} method for scoped arguments");
			}
			
			return Value;
		}

		public void SetValue(ValueGetter valueGetter)
		{
			Value = valueGetter.Invoke();
		}

		public void UseValue(ValueConsumer valueConsumer)
		{
			valueConsumer.Invoke(Value);
		}
		
		public TResult UseValue<TResult>(ValueConsumerWithResult<TResult> valueConsumer) where TResult : allows ref struct
		{
			return valueConsumer.Invoke(Value);
		}
		
		public delegate TResult ValueConsumerWithResult<TResult>(scoped TByRefLike value) where TResult : allows ref struct;
		
		public delegate void ValueConsumer(scoped TByRefLike value);
		
		public delegate TByRefLike ValueGetter();
	}
#endif

	/// <summary>
	///   Permits indirect access to <see cref="ReadOnlySpan{T}"/>-typed argument values during method interception.
	/// </summary>
	/// <remarks>
	///   <see cref="ReadOnlySpan{T}"/> is a byref-like (<c>ref struct</c>) type, which means that
	///   instances of it live exclusively on the evaluation stack. Therefore, they cannot be boxed
	///   and put into the <see langword="object"/>-typed <see cref="IInvocation.Arguments"/> array.
	///   DynamicProxy replaces these unboxable values with instances of <see cref="ReadOnlySpanReference{T}"/>,
	///   which grant you indirect read/write access to the actual value.
	///   <para>
	///     These references are only valid for the duration of the intercepted method call.
	///     Any attempt to use it beyond that will result in a <see cref="AccessViolationException"/>.
	///   </para>
	/// </remarks>
	public unsafe class ReadOnlySpanReference<T>
#if NET9_0_OR_GREATER
		: ByRefLikeReference<ReadOnlySpan<T>>
#else
		: ByRefLikeReference
#endif
	{
		internal ReadOnlySpanReference(Type type, void* ptr, bool valueIsScoped)
			: base(type, ptr, valueIsScoped)
		{
			if (type != typeof(ReadOnlySpan<T>))
			{
				throw new ArgumentOutOfRangeException(nameof(type));
			}
		}

#if !NET9_0_OR_GREATER
		private ref ReadOnlySpan<T> Value
		{
			get
			{
				return ref *(ReadOnlySpan<T>*)GetPtr(typeof(ReadOnlySpan<T>));
			}
		}
		
		public ReadOnlySpan<T> GetValue()
		{
			if (ValueIsScoped)
			{
				throw new InvalidOperationException($"Use {nameof(UseValue)} method for scoped arguments");
			}
			
			return Value;
		}

		public void SetValue(ValueGetter valueGetter)
		{
			Value = valueGetter.Invoke();
		}

		public void UseValue(ValueConsumer valueConsumer)
		{
			valueConsumer.Invoke(Value);
		}
		
		public TResult UseValue<TResult>(ValueConsumerWithResult<TResult> valueConsumer)
		{
			return valueConsumer.Invoke(Value);
		}

		public delegate TResult ValueConsumerWithResult<TResult>(scoped ReadOnlySpan<T> value);
		
		public delegate void ValueConsumer(scoped ReadOnlySpan<T> value);
		
		public delegate ReadOnlySpan<T> ValueGetter();
#endif
	}

	/// <summary>
	///   Permits indirect access to <see cref="Span{T}"/>-typed argument values during method interception.
	/// </summary>
	/// <remarks>
	///   <see cref="Span{T}"/> is a byref-like (<c>ref struct</c>) type, which means that
	///   instances of it live exclusively on the evaluation stack. Therefore, they cannot be boxed
	///   and put into the <see langword="object"/>-typed <see cref="IInvocation.Arguments"/> array.
	///   DynamicProxy replaces these unboxable values with instances of <see cref="SpanReference{T}"/>,
	///   which grant you indirect read/write access to the actual value.
	///   <para>
	///     These references are only valid for the duration of the intercepted method call.
	///     Any attempt to use it beyond that will result in a <see cref="AccessViolationException"/>.
	///   </para>
	/// </remarks>
	public unsafe class SpanReference<T>
#if NET9_0_OR_GREATER
		: ByRefLikeReference<Span<T>>
#else
		: ByRefLikeReference
#endif
	{
		internal SpanReference(Type type, void* ptr, bool valueIsScoped)
			: base(type, ptr, valueIsScoped)
		{
			if (type != typeof(Span<T>))
			{
				throw new ArgumentOutOfRangeException(nameof(type));
			}
		}

#if !NET9_0_OR_GREATER
		private ref Span<T> Value
		{
			get
			{
				return ref *(Span<T>*)GetPtr(typeof(Span<T>));
			}
		}
		
		public Span<T> GetValue()
		{
			if (ValueIsScoped)
			{
				throw new InvalidOperationException($"Use {nameof(UseValue)} method for scoped arguments");
			}
			
			return Value;
		}

		public void SetValue(ValueGetter valueGetter)
		{
			Value = valueGetter.Invoke();
		}

		public void UseValue(ValueConsumer valueConsumer)
		{
			valueConsumer.Invoke(Value);
		}
		
		public TResult UseValue<TResult>(ValueConsumerWithResult<TResult> valueConsumer)
		{
			return valueConsumer.Invoke(Value);
		}

		public delegate TResult ValueConsumerWithResult<TResult>(scoped Span<T> value);
		
		public delegate void ValueConsumer(scoped Span<T> value);
		
		public delegate Span<T> ValueGetter();
#endif
	}
}

#pragma warning restore CS8500

#endif
