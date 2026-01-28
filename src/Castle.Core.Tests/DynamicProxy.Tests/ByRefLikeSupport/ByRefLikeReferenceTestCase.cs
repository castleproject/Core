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

namespace Castle.DynamicProxy.Tests.ByRefLikeSupport
{
	using System;
#if NET9_0_OR_GREATER
	using System.Runtime.CompilerServices;
#endif

	using NUnit.Framework;

	/// <summary>
	///   Tests for the substitute types used by DynamicProxy to implement byref-like parameter and return type support.
	/// </summary>
	[TestFixture]
	public class ByRefLikeReferenceTestCase
	{
		#region `ByRefLikeReference`

		[Test]
		public unsafe void Ctor_throws_if_non_by_ref_like_type()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				bool local = default;
				_ = new ByRefLikeReference(typeof(bool), &local);
			});
		}

		[Test]
		public unsafe void Ctor_succeeds_if_by_ref_like_type()
		{
			ReadOnlySpan<char> local = default;
			_ = new ByRefLikeReference(typeof(ReadOnlySpan<char>), &local);
		}

		[Test]
		public unsafe void Invalidate_throws_if_address_mismatch()
		{
			ReadOnlySpan<char> local = default;
			var reference = new ByRefLikeReference(typeof(ReadOnlySpan<char>), &local);
			Assert.Throws<AccessViolationException>(() =>
			{
				ReadOnlySpan<char> otherLocal = default;
				reference.Invalidate(&otherLocal);
			});
		}

		[Test]
		public unsafe void Invalidate_succeeds_if_address_match()
		{
			ReadOnlySpan<char> local = default;
			var reference = new ByRefLikeReference(typeof(ReadOnlySpan<char>), &local);
			reference.Invalidate(&local);
		}

		[Test]
		public unsafe void GetPtr_throws_if_type_mismatch()
		{
			ReadOnlySpan<char> local = default;
			var reference = new ByRefLikeReference(typeof(ReadOnlySpan<char>), &local);
			Assert.Throws<AccessViolationException>(() => reference.GetPtr(typeof(bool)));
		}

		[Test]
		public unsafe void GetPtr_returns_ctor_address_if_type_match()
		{
			ReadOnlySpan<char> local = default;
			var reference = new ByRefLikeReference(typeof(ReadOnlySpan<char>), &local);
			var ptr = reference.GetPtr(typeof(ReadOnlySpan<char>));
			Assert.True(ptr == &local);
		}

		[Test]
		public unsafe void GetPtr_throws_after_Invalidate()
		{
			ReadOnlySpan<char> local = default;
			var reference = new ByRefLikeReference(typeof(ReadOnlySpan<char>), &local);
			reference.Invalidate(&local);
			Assert.Throws<AccessViolationException>(() => reference.GetPtr(typeof(ReadOnlySpan<char>)));
		}

		#endregion

		#region `ReadOnlySpanReference<T>`

		// We do not repeat the above tests for `ReadOnlySpanReference<T>`
		// since it inherits the tested methods from `ByRefLikeReference`.

		public unsafe void ReadOnlySpanReference_ctor_throws_if_type_mismatch()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				ReadOnlySpan<bool> local = default;
				_ = new ReadOnlySpanReference<char>(typeof(ReadOnlySpan<bool>), &local);
			});
		}

		public unsafe void ReadOnlySpanReference_Value_returns_equal_span()
		{
			ReadOnlySpan<char> local = "foo".AsSpan();
			var reference = new ReadOnlySpanReference<char>(typeof(ReadOnlySpan<char>), &local);
			Assert.True(reference.Value == "foo".AsSpan());
		}

#if NET9_0_OR_GREATER
		[Test]
		public unsafe void ReadOnlySpanReference_Value_returns_same_span()
		{
			ReadOnlySpan<char> local = "foo".AsSpan();
			var reference = new ReadOnlySpanReference<char>(typeof(ReadOnlySpan<char>), &local);
			Assert.True(Unsafe.AreSame(ref reference.Value, ref local));
		}
#endif

		[Test]
		public unsafe void ReadOnlySpanReference_Value_can_update_original()
		{
			ReadOnlySpan<char> local = "foo".AsSpan();
			var reference = new ReadOnlySpanReference<char>(typeof(ReadOnlySpan<char>), &local);
			reference.Value = "bar".AsSpan();
			Assert.True(local == "bar".AsSpan());
		}

		#endregion

		// We do not test `ByRefLikeReference<TByRefLike>` and `SpanReference<T>`
		// since these two types are practically identical to `ReadOnlySpanReference<T>`.
	}
}

#pragma warning restore CS8500

#endif
