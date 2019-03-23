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

namespace CastleTests.Interceptors
{
	using System.Threading.Tasks;
	using Castle.DynamicProxy;

	public class AsyncInterceptor : IInterceptor
	{
		public void Intercept(IInvocation invocation)
		{
			invocation.ReturnValue = InterceptAsyncMethod(invocation);
		}

		private static async Task InterceptAsyncMethod(IInvocation invocation)
		{
			var interceptionProgress = invocation.GetMemento(InvocationMementoOptions.InterceptionProgress);

			// It all falls down when executing async before calling Proceed().
			await Task.Delay(10).ConfigureAwait(false);

			interceptionProgress.Restore();

			invocation.Proceed();

			// Hmmmmm, now with it simplified down to this, I see the glaring hole that is the return value being set
			// in two situations.
			Task returnValue = (Task)invocation.ReturnValue;

			await returnValue.ConfigureAwait(false);
		}
	}
}
