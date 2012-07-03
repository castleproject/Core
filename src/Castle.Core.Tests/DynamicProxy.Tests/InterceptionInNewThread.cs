// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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
	using System.Threading;
	using System.Threading.Tasks;

	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class InterceptionInNewThread : BasePEVerifyTestCase
	{
		[Test(Description = "DYNPROXY-173: Interceptor calling AbstractInvocation.Proceed() in new thread can cause never-ending loop")]
		public void Interceptor_should_intercept_invocation_only_once()
		{
			var interceptor = new ProceedInNewThread();
			var target = new Simple();
			var proxy = generator.CreateInterfaceProxyWithTarget<ISimple>(target, interceptor);

			proxy.Method();

			interceptor.task.Wait();
			Assert.AreEqual(1, target.Count);
		}
	}

	public class ProceedInNewThread : IInterceptor
	{
		public Task task;
		public ProceedInNewThread()
		{
		}

		public void Intercept(IInvocation invocation)
		{
			Assert.IsNull(task, "This interceptor should be executed just once!");
			var invocationCopy = invocation.ShallowCopy();
			task = Task.Factory.StartNew(t =>
			{
				Thread.Sleep(2000); // give AbstractInvocation.Proceed() chance to finish finaly() before we call Proceed again
				var i = (IInvocation)t;
				// This call can cause never-ending loop, AbstractInvocation is incrementing currentInterceptorIndex before each Intercept call
				// but then decrements it in finally (this finally was added in https://github.com/castleproject/Core/commit/ea8fa09)
				// so if finally finishes before our Proceed() then currentInterceptorIndex points to our interceptor which called again and again
				// instead of InvokeMethodOnTarget();
				i.Proceed();
			}, invocationCopy);
		}
	}

}