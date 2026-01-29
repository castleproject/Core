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

	#region Types for `ByRefLike` tests (this type represents all byref-like types that aren't spans)

	public ref struct ByRefLike
	{
		private readonly object? value;

		public ByRefLike(object? value)
		{
			this.value = value;
		}

		public object? Value
		{
			get => value;
		}
	}

	public interface IPassByRefLikeByValue
	{
		void PassByValue(ByRefLike arg);
	}

	public interface IPassByRefLikeByRefIn
	{
		void PassByRefIn(in ByRefLike arg);
	}

	public interface IPassByRefLikeByRefRef
	{
		void PassByRefRef(ref ByRefLike arg);
	}

	public interface IPassByRefLikeByRefOut
	{
		void PassByRefOut(out ByRefLike arg);
	}

	public interface IReturnByRefLikeByValue
	{
		ByRefLike ReturnByValue();
	}

	#endregion

	#region Types for `ReadOnlySpan<T>` tests

	public interface IPassReadOnlySpanByValue
	{
		void PassByValue(ReadOnlySpan<char> arg);
	}

	public interface IPassReadOnlySpanByRefIn
	{
		void PassByRefIn(in ReadOnlySpan<char> arg);
	}

	public interface IPassReadOnlySpanByRefRef
	{
		void PassByRefRef(ref ReadOnlySpan<char> arg);
	}

	public interface IPassReadOnlySpanByRefOut
	{
		void PassByRefOut(out ReadOnlySpan<char> arg);
	}

	public interface IReturnReadOnlySpanByValue
	{
		ReadOnlySpan<char> ReturnByValue();
	}

	#endregion

	#region Types for `Span<T>` tests

	public interface IPassSpanByValue
	{
		void PassByValue(Span<char> arg);
	}

	public interface IPassSpanByRefIn
	{
		void PassByRefIn(in Span<char> arg);
	}

	public interface IPassSpanByRefRef
	{
		void PassByRefRef(ref Span<char> arg);
	}

	public interface IPassSpanByRefOut
	{
		void PassByRefOut(out Span<char> arg);
	}

	public interface IReturnSpanByValue
	{
		Span<char> ReturnByValue();
	}

	#endregion
}

#endif
