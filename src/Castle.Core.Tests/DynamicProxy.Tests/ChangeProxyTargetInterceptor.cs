// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests
{
	using NUnit.Framework;

	public class ChangeProxyTargetInterceptor : IInterceptor
	{
		private readonly object target;

		public ChangeProxyTargetInterceptor(object target)
		{
			this.target = target;
		}

		public void Intercept(IInvocation invocation)
		{
			var targetAccessor = invocation.Proxy as IProxyTargetAccessor;
			Assert.IsNotNull(targetAccessor);
			targetAccessor.DynProxySetTarget(target);
			invocation.Proceed();
		}
	}
}