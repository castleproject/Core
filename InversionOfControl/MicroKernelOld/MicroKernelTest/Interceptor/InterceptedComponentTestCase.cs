using System.Reflection;
// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel.Test.Interceptor
{
	using NUnit.Framework;

	using Castle.MicroKernel.Interceptor;
	using Castle.MicroKernel.Interceptor.Default;

	/// <summary>
	/// Summary description for InterceptedComponentTestCase.
	/// </summary>
	[TestFixture]
	public class InterceptedComponentTestCase : Assertion
	{
		private IInterceptedComponent m_component;
		private IService m_service;
		private IService m_proxiedService;

		[SetUp]
		public void Init()
		{
			IInterceptedComponentBuilder interceptorManager = new DefaultInterceptedComponentBuilder();

			m_service = new ServiceImpl();

			m_component = interceptorManager.CreateInterceptedComponent(
				m_service, typeof (IService));

			m_proxiedService = m_component.ProxiedInstance as IService;
		}

		[Test]
		public void TailInvocationOnly()
		{
			Assert(!m_service.Done);
			m_proxiedService.DoSomething();
			Assert(m_service.Done);
		}

		[Test]
		public void NewInterceptor()
		{
			DummyInterceptor interceptor = new DummyInterceptor();

			m_component.Add(interceptor);

			Assert(!interceptor.Invoked);

			Assert(!m_service.Done);
			m_proxiedService.DoSomething();
			Assert(m_service.Done);

			Assert(interceptor.Invoked);
		}

		public class DummyInterceptor : IInterceptor
		{
			private bool m_invoked = false;
			private IInterceptor m_next;

			public bool Invoked
			{
				get { return m_invoked; }
			}

			#region IInterceptor Members

			public object Process(object instance, MethodInfo method, params object[] arguments)
			{
				m_invoked = true;
				return Next.Process(instance, method, arguments);
			}

			public IInterceptor Next
			{
				get { return m_next; }
				set { m_next = value; }
			}

			#endregion
		}

		public interface IService
		{
			void DoSomething();

			bool Done { get; }
		}

		public class ServiceImpl : IService
		{
			private bool m_done = false;

			#region IService Members

			public void DoSomething()
			{
				m_done = true;
			}

			public bool Done
			{
				get { return m_done; }
			}

			#endregion
		}
	}
}