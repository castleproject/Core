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

namespace Castle.DynamicProxy.Tests
{
	using System;

	using Castle.DynamicProxy.Tests.Interceptors;

	using NUnit.Framework;

	/// <summary>
	///   Tests for by-ref-like (<see langword="ref"/> <see langword="struct"/>) method parameter and return types.
	/// </summary>
	[TestFixture]
	public class ByRefLikeTestCase : BasePEVerifyTestCase
	{
		[TestCase(typeof(IHaveMethodWithByRefLikeParameter))]
		[TestCase(typeof(IHaveMethodWithByRefLikeInParameter))]
		[TestCase(typeof(IHaveMethodWithByRefLikeRefParameter))]
		[TestCase(typeof(IHaveMethodWithByRefLikeOutParameter))]
		[TestCase(typeof(IHaveMethodWithByRefLikeReturnType))]
		public void Can_proxy_type(Type interfaceType)
		{
			_ = generator.CreateInterfaceProxyWithoutTarget(interfaceType);
		}

		[Test]
		public void Can_invoke_method_with_by_ref_like_parameter()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHaveMethodWithByRefLikeParameter>(new DoNothingInterceptor());
			var arg = default(ByRefLike);
			proxy.Method(arg);
		}

		[Test]
		public void Can_invoke_method_with_by_ref_like_in_parameter()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHaveMethodWithByRefLikeInParameter>(new DoNothingInterceptor());
			ByRefLike arg = default;
			proxy.Method(in arg);
		}

		[Test]
		public void Can_invoke_method_with_by_ref_like_ref_parameter()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHaveMethodWithByRefLikeRefParameter>(new DoNothingInterceptor());
			ByRefLike arg = default;
			proxy.Method(ref arg);
		}

		[Test]
		public void Can_invoke_method_with_by_ref_like_out_parameter()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHaveMethodWithByRefLikeOutParameter>(new DoNothingInterceptor());
			proxy.Method(out _);
		}

		[Test]
		public void Can_invoke_method_with_by_ref_like_return_type()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHaveMethodWithByRefLikeReturnType>(new DoNothingInterceptor());
			_ = proxy.Method();
		}

		[Test]
		public void Can_proceed_to_target_method_with_by_ref_like_parameter()
		{
			var target = new HasMethodWithByRefLikeParameter();
			var proxy = generator.CreateInterfaceProxyWithTarget<IHaveMethodWithByRefLikeParameter>(target, new StandardInterceptor());
			ByRefLike arg = default;
			proxy.Method(arg);
		}

		[Test]
		public void Can_proceed_to_target_method_with_by_ref_like_in_parameter()
		{
			var target = new HasMethodWithByRefLikeInParameter();
			var proxy = generator.CreateInterfaceProxyWithTarget<IHaveMethodWithByRefLikeInParameter>(target, new StandardInterceptor());
			ByRefLike arg = default;
			proxy.Method(in arg);
		}

		[Test]
		public void Can_proceed_to_target_method_with_by_ref_like_ref_parameter()
		{
			var target = new HasMethodWithByRefLikeRefParameter();
			var proxy = generator.CreateInterfaceProxyWithTarget<IHaveMethodWithByRefLikeRefParameter>(target, new StandardInterceptor());
			ByRefLike arg = default;
			proxy.Method(ref arg);
		}

		[Test]
		public void Can_proceed_to_target_method_with_by_ref_like_out_parameter()
		{
			var target = new HasMethodWithByRefLikeOutParameter();
			var proxy = generator.CreateInterfaceProxyWithTarget<IHaveMethodWithByRefLikeOutParameter>(target, new StandardInterceptor());
			proxy.Method(out _);
		}

		[Test]
		public void Can_proceed_to_target_method_with_by_ref_like_return_type()
		{
			var target = new HasMethodWithByRefLikeReturnType();
			var proxy = generator.CreateInterfaceProxyWithTarget<IHaveMethodWithByRefLikeReturnType>(target, new StandardInterceptor());
			_ = proxy.Method();
		}

		public ref struct ByRefLike
		{
		}

		public interface IHaveMethodWithByRefLikeParameter
		{
			void Method(ByRefLike arg);
		}

		public class HasMethodWithByRefLikeParameter : IHaveMethodWithByRefLikeParameter
		{
			public virtual void Method(ByRefLike arg)
			{
			}
		}

		public interface IHaveMethodWithByRefLikeInParameter
		{
			void Method(in ByRefLike arg);
		}

		public class HasMethodWithByRefLikeInParameter : IHaveMethodWithByRefLikeInParameter
		{
			public virtual void Method(in ByRefLike arg)
			{
			}
		}

		public interface IHaveMethodWithByRefLikeRefParameter
		{
			void Method(ref ByRefLike arg);
		}

		public class HasMethodWithByRefLikeRefParameter : IHaveMethodWithByRefLikeRefParameter
		{
			public virtual void Method(ref ByRefLike arg)
			{
			}
		}

		public interface IHaveMethodWithByRefLikeOutParameter
		{
			void Method(out ByRefLike arg);
		}

		public class HasMethodWithByRefLikeOutParameter : IHaveMethodWithByRefLikeOutParameter
		{
			public virtual void Method(out ByRefLike arg)
			{
				arg = default;
			}
		}

		public interface IHaveMethodWithByRefLikeReturnType
		{
			ByRefLike Method();
		}

		public class HasMethodWithByRefLikeReturnType : IHaveMethodWithByRefLikeReturnType
		{
			public virtual ByRefLike Method()
			{
				return default;
			}
		}
	}
}

#endif
