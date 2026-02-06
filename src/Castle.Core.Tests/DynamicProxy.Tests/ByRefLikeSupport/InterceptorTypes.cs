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

namespace Castle.DynamicProxy.Tests.ByRefLikeSupport
{
	using System;

	public sealed class AdHoc : IInterceptor
	{
		public AdHoc(Action<IInvocation> intercept)
		{
			Intercept = intercept;
		}

		public Action<IInvocation> Intercept { get; private set; }

		public bool Executed { get; private set; }

		void IInterceptor.Intercept(IInvocation invocation)
		{
			try
			{
				Intercept(invocation);
			}
			finally
			{
				Executed = true;
			}

		}
	}

	public sealed class Proceed : IInterceptor
	{
		public bool Executed { get; private set; }

		void IInterceptor.Intercept(IInvocation invocation)
		{
			try
			{
				invocation.Proceed();
			}
			finally
			{
				Executed = true;
			}
		}
	}
}
