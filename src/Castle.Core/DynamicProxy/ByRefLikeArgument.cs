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
	using System.ComponentModel;
	using System.Runtime.CompilerServices;

	/// <summary>
	///   Wraps a byref-like (<c>ref struct</c>) method argument
	///   such that it can be placed in the <see cref="IInvocation.Arguments"/> array during interception.
	/// </summary>
	public unsafe class ByRefLikeArgument
	{
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
		[CLSCompliant(false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void* GetPointer()
		{
			return ptr;
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
		public ref TByRefLike Get()
		{
			return ref Unsafe.AsRef<TByRefLike>(ptr);
		}
	}

#endif

}

#endif
