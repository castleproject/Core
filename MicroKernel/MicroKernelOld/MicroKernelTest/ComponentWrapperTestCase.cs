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

namespace Castle.MicroKernel.Test
{
	using System;

	using NUnit.Framework;

	using Castle.MicroKernel.Interceptor;
	using Castle.MicroKernel.Model;
	using Castle.MicroKernel.Test.Components;

	/// <summary>
	/// Summary description for ComponentWrapperTestCase.
	/// </summary>
	[TestFixture]
	public class ComponentWrapperTestCase : Assertion
	{
		private MyInterceptor m_interceptor = new MyInterceptor();

		[Test]
		public void WrappingComponent()
		{
			IKernel container = new BaseKernel();

			container.ComponentWrap += new WrapDelegate(ComponentWrap);
            container.ComponentUnWrap += new WrapDelegate(ComponentUnWrap);

            container.AddComponent( "a", typeof(IMailService), typeof(SimpleMailService) );

			IHandler handler = container[ "a" ];

			IMailService service = handler.Resolve() as IMailService;

			service.Send("hammett at apache dot org", "johndoe at yahoo dot org", "Aloha!", "What's up?");
			Assert( m_interceptor.InterceptorInvoked );

			handler.Release( service );
		}

		private void ComponentWrap(IComponentModel model, string key, IHandler handler, IInterceptedComponent interceptedComponent)
		{
			interceptedComponent.Add( m_interceptor );

			AssertNotNull( interceptedComponent.Instance );
			AssertNotNull( interceptedComponent.ProxiedInstance );
			AssertNotNull( interceptedComponent.InterceptorChain );
			Assert( interceptedComponent.Instance != interceptedComponent.ProxiedInstance );
		}

		private void ComponentUnWrap(IComponentModel model, string key, IHandler handler, IInterceptedComponent interceptedComponent)
		{
			AssertNotNull( interceptedComponent );
		}

		internal class MyInterceptor : AbstractInterceptor
		{
			private bool m_interceptorInvoked = false;

			public bool InterceptorInvoked
			{
				get { return m_interceptorInvoked; }
			}

			public override object Process(object instance, System.Reflection.MethodInfo method, params object[] arguments)
			{
				m_interceptorInvoked = true;

				return base.Process (instance, method, arguments);
			}
		}
	}
}
