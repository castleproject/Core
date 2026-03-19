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
	using System.Threading.Tasks;
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
				_ = new ByRefLikeReference(typeof(bool), &local, false);
			});
		}

		[Test]
		public unsafe void Ctor_succeeds_if_by_ref_like_type()
		{
			ReadOnlySpan<char> local = default;
			_ = new ByRefLikeReference(typeof(ReadOnlySpan<char>), &local, false);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public unsafe void Ctor_preserves_value_is_scoped_value(bool valueIsScoped)
		{
			ReadOnlySpan<char> local = default;
			var result = new ByRefLikeReference(typeof(ReadOnlySpan<char>), &local, valueIsScoped: valueIsScoped);
			Assert.AreEqual(valueIsScoped, result.ValueIsScoped);
		}

		[Test]
		public unsafe void Dispose_throws_if_address_mismatch()
		{
			ReadOnlySpan<char> local = default;
			var reference = new ByRefLikeReference(typeof(ReadOnlySpan<char>), &local, false);
			Assert.Throws<InvalidOperationException>(() =>
			{
				ReadOnlySpan<char> otherLocal = default;
				reference.Dispose(&otherLocal);
			});
		}

		[Test]
		public unsafe void Dispose_succeeds_if_address_match()
		{
			ReadOnlySpan<char> local = default;
			var reference = new ByRefLikeReference(typeof(ReadOnlySpan<char>), &local, false);
			reference.Dispose(&local);
		}
		
		[Test]
		public unsafe void Dispose_throws_when_access_from_other_thread()
		{
			ReadOnlySpan<char> local = default;
			var reference = new ByRefLikeReference(typeof(ReadOnlySpan<char>), &local, false);
			var address = reference.GetPtr(typeof(ReadOnlySpan<char>));
			var task = Task.Run(() => reference.Dispose(address));
			var msg = Assert.Throws<InvalidOperationException>(() => task.GetAwaiter().GetResult()).Message;
			StringAssert.Contains("thread", msg);
		}

		[Test]
		public unsafe void GetPtr_throws_if_type_mismatch()
		{
			ReadOnlySpan<char> local = default;
			var reference = new ByRefLikeReference(typeof(ReadOnlySpan<char>), &local, false);
			Assert.Throws<ArgumentException>(() => reference.GetPtr(typeof(bool)));
		}

		[Test]
		public unsafe void GetPtr_returns_ctor_address_if_type_match()
		{
			ReadOnlySpan<char> local = default;
			var reference = new ByRefLikeReference(typeof(ReadOnlySpan<char>), &local, false);
			var ptr = reference.GetPtr(typeof(ReadOnlySpan<char>));
			Assert.True(ptr == &local);
		}

		[Test]
		public unsafe void GetPtr_throws_after_Dispose()
		{
			ReadOnlySpan<char> local = default;
			var reference = new ByRefLikeReference(typeof(ReadOnlySpan<char>), &local, false);
			reference.Dispose(&local);
			Assert.Throws<ObjectDisposedException>(() => reference.GetPtr(typeof(ReadOnlySpan<char>)));
		}
		
		[Test]
		public unsafe void GetPtr_throws_when_access_from_other_thread()
		{
			ReadOnlySpan<char> local = default;
			var reference = new ByRefLikeReference(typeof(ReadOnlySpan<char>), &local, false);
			var task = Task.Run(() => reference.GetPtr(typeof(ReadOnlySpan<char>)));
			var msg = Assert.Throws<InvalidOperationException>(() => task.GetAwaiter().GetResult()).Message;
			StringAssert.Contains("thread", msg);
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
				_ = new ReadOnlySpanReference<char>(typeof(ReadOnlySpan<bool>), &local, false);
			});
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public unsafe void ReadOnlySpanReference_ctor_preserves_value_is_scoped_value(bool valueIsScoped)
		{
			ReadOnlySpan<char> local = default;
			var result = new ReadOnlySpanReference<char>(typeof(ReadOnlySpan<char>), &local, valueIsScoped: valueIsScoped);
			Assert.AreEqual(valueIsScoped, result.ValueIsScoped);
		}

		public unsafe void ReadOnlySpanReference_GetValue_returns_equal_span()
		{
			ReadOnlySpan<char> local = "foo".AsSpan();
			var reference = new ReadOnlySpanReference<char>(typeof(ReadOnlySpan<char>), &local, false);
			Assert.True(reference.GetValue() == "foo".AsSpan());
		}
		
		public unsafe void ReadOnlySpanReference_UseValue_returns_equal_span()
		{
			ReadOnlySpan<char> local = "foo".AsSpan();
			var reference = new ReadOnlySpanReference<char>(typeof(ReadOnlySpan<char>), &local, false);
			var returnedValue = reference.UseValue((scoped x) => x.ToString());
			Assert.True(returnedValue == "foo");
		}

		[Test]
		public unsafe void ReadOnlySpanReference_SetValue_can_update_original()
		{
			ReadOnlySpan<char> local = "foo".AsSpan();
			var reference = new ReadOnlySpanReference<char>(typeof(ReadOnlySpan<char>), &local, false);
			reference.SetValue(() => "bar".AsSpan());
			Assert.True(local == "bar".AsSpan());
		}
		
		[Test]
		public unsafe void ReadOnlySpanReference_GetValue_throws_when_access_from_other_thread()
		{
			ReadOnlySpan<char> local = "foo".AsSpan();
			var reference = new ReadOnlySpanReference<char>(typeof(ReadOnlySpan<char>), &local, false);
			var task = Task.Run(() => reference.GetValue().ToString());
			var msg = Assert.Throws<InvalidOperationException>(() => task.GetAwaiter().GetResult()).Message;
			StringAssert.Contains("thread", msg);
		}
		
		[Test]
		public unsafe void ReadOnlySpanReference_GetValue_throws_when_called_for_scoped()
		{
			ReadOnlySpan<char> local = "foo".AsSpan();
			var reference = new ReadOnlySpanReference<char>(typeof(ReadOnlySpan<char>), &local, valueIsScoped: true);
			var msg = Assert.Throws<InvalidOperationException>(() => reference.GetValue()).Message;
			StringAssert.Contains("scoped", msg);
		}
		
		[Test]
		public unsafe void ReadOnlySpanReference_UseValue_returns_for_scoped()
		{
			ReadOnlySpan<char> local = "foo".AsSpan();
			var reference = new ReadOnlySpanReference<char>(typeof(ReadOnlySpan<char>), &local, valueIsScoped: true);
			var returnedValue = reference.UseValue((scoped x) => x.ToString());
			Assert.True(returnedValue == "foo");
		}

		#endregion

		// We do not test `ByRefLikeReference<TByRefLike>` and `SpanReference<T>`
		// since these two types are practically identical to `ReadOnlySpanReference<T>`.
	}
}

#pragma warning restore CS8500

#endif
