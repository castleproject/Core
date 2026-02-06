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

	public interface ITarget
	{
		bool Executed { get; }
	}

	#region Target type for `ByRefLike` test types (this type represents all byref-like types that aren't spans)

#if NET9_0_OR_GREATER

	public delegate void ByRefLikeInspector(ByRefLike arg);

	public delegate void ByRefLikeSetter(out ByRefLike arg);

	public class PassByRefLikeTarget
		: ITarget
		, IPassByRefLikeByValue
		, IPassByRefLikeByRefIn
		, IPassByRefLikeByRefRef
		, IPassByRefLikeByRefOut
		, IReturnByRefLikeByValue
	{
		public PassByRefLikeTarget(ByRefLikeInspector inspect)
		{
			Inspect = inspect;
		}

		public PassByRefLikeTarget(ByRefLikeSetter set)
		{
			Set = set;
		}

		public ByRefLikeInspector? Inspect { get; private set; }

		public ByRefLikeSetter? Set { get; private set; }

		public bool Executed { get; private set; }

		public void PassByValue(ByRefLike arg)
		{
			try
			{
				Inspect?.Invoke(arg);
			}
			finally
			{
				Executed = true;
			}
		}

		public void PassByRefIn(in ByRefLike arg)
		{
			try
			{
				Inspect?.Invoke(arg);
			}
			finally
			{
				Executed = true;
			}
		}

		public void PassByRefRef(ref ByRefLike arg)
		{
			try
			{
				Inspect?.Invoke(arg);
				Set?.Invoke(out arg);
			}
			finally
			{
				Executed = true;
			}
		}

		public void PassByRefOut(out ByRefLike arg)
		{
			try
			{
				arg = default;
				Set?.Invoke(out arg);
			}
			finally
			{
				Executed = true;
			}
		}

		public ByRefLike ReturnByValue()
		{
			try
			{
				ByRefLike result = default;
				Set?.Invoke(out result);
				return result;
			}
			finally
			{
				Executed = true;
			}
		}
	}

#endif

	#endregion

	#region Target type for `ReadOnlySpan<T>` test types

	public delegate void ReadOnlySpanInspector(ReadOnlySpan<char> arg);

	public delegate void ReadOnlySpanSetter(out ReadOnlySpan<char> arg);

	public class PassReadOnlySpanTarget
		: ITarget
		, IPassReadOnlySpanByValue
		, IPassReadOnlySpanByRefIn
		, IPassReadOnlySpanByRefRef
		, IPassReadOnlySpanByRefOut
		, IReturnReadOnlySpanByValue
	{
		public PassReadOnlySpanTarget(ReadOnlySpanInspector inspect)
		{
			Inspect = inspect;
		}

		public PassReadOnlySpanTarget(ReadOnlySpanSetter set)
		{
			Set = set;
		}

		public ReadOnlySpanInspector? Inspect { get; private set; }

		public ReadOnlySpanSetter? Set { get; private set; }

		public bool Executed { get; private set; }

		public void PassByValue(ReadOnlySpan<char> arg)
		{
			try
			{
				Inspect?.Invoke(arg);
			}
			finally
			{
				Executed = true;
			}
		}

		public void PassByRefIn(in ReadOnlySpan<char> arg)
		{
			try
			{
				Inspect?.Invoke(arg);
			}
			finally
			{
				Executed = true;
			}
		}

		public void PassByRefRef(ref ReadOnlySpan<char> arg)
		{
			try
			{
				Inspect?.Invoke(arg);
				Set?.Invoke(out arg);
			}
			finally
			{
				Executed = true;
			}
		}

		public void PassByRefOut(out ReadOnlySpan<char> arg)
		{
			try
			{
				arg = default;
				Set?.Invoke(out arg);
			}
			finally
			{
				Executed = true;
			}
		}

		public ReadOnlySpan<char> ReturnByValue()
		{
			try
			{
				ReadOnlySpan<char> result = default;
				Set?.Invoke(out result);
				return result;
			}
			finally
			{
				Executed = true;
			}
		}
	}

	#endregion

	#region Target type for `ReadOnlySpan<T>` test types

	public delegate void SpanInspector(Span<char> arg);

	public delegate void SpanSetter(out Span<char> arg);

	public class PassSpanTarget
		: ITarget
		, IPassSpanByValue
		, IPassSpanByRefIn
		, IPassSpanByRefRef
		, IPassSpanByRefOut
		, IReturnSpanByValue
	{
		public PassSpanTarget(SpanInspector inspect)
		{
			Inspect = inspect;
		}

		public PassSpanTarget(SpanSetter set)
		{
			Set = set;
		}

		public SpanInspector? Inspect { get; private set; }

		public SpanSetter? Set { get; private set; }

		public bool Executed { get; private set; }

		public void PassByValue(Span<char> arg)
		{
			try
			{
				Inspect?.Invoke(arg);
			}
			finally
			{
				Executed = true;
			}
		}

		public void PassByRefIn(in Span<char> arg)
		{
			try
			{
				Inspect?.Invoke(arg);
			}
			finally
			{
				Executed = true;
			}
		}

		public void PassByRefRef(ref Span<char> arg)
		{
			try
			{
				Inspect?.Invoke(arg);
				Set?.Invoke(out arg);
			}
			finally
			{
				Executed = true;
			}
		}

		public void PassByRefOut(out Span<char> arg)
		{
			try
			{
				arg = default;
				Set?.Invoke(out arg);
			}
			finally
			{
				Executed = true;
			}
		}

		public Span<char> ReturnByValue()
		{
			try
			{
				Span<char> result = default;
				Set?.Invoke(out result);
				return result;
			}
			finally
			{
				Executed = true;
			}
		}
	}

	#endregion

}

#endif
