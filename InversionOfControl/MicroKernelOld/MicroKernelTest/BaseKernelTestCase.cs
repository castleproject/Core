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

	using Apache.Avalon.Framework;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Test.Components;

	/// <summary>
	/// Summary description for BaseKernelTestCase.
	/// </summary>
	[TestFixture]
	public class BaseKernelTestCase : Assertion
	{
		IKernel m_container;

		[SetUp]
		public void Init()
		{
			m_container = new BaseKernel();
		}

		[TearDown]
		public void Terminate()
		{
			m_container.Dispose();
		}

		/// <summary>
		/// Just a simple Service resolution.
		/// No concerns or aspects involved.
		/// </summary>
		[Test]
		public void SimpleUsage()
		{
			m_container.AddComponent( "a", typeof(IMailService), typeof(SimpleMailService) );

			IHandler handler = m_container[ "a" ];

			IMailService service = handler.Resolve() as IMailService;

			AssertNotNull( service );

			service.Send("hammett at apache dot org", "johndoe at yahoo dot org", "Aloha!", "What's up?");

			handler.Release( service );
		}

		[Test]
		public void InvalidComponent()
		{
			try
			{
				m_container.AddComponent( "a", typeof(IMailService), typeof(String) );
				Fail("Should not allow a type which not implements the specified service.");
			}
			catch(ArgumentException)
			{
				// Expected
			}
		}

        [Test]
        public void AddAndRemove()
        {
            m_container.AddComponent( "a", typeof(IMailService), typeof(SimpleMailService) );

            AssertNotNull( m_container.GetHandlerForService( typeof(IMailService) ) );
            AssertNotNull( m_container[ "a" ] );

            AssertNull( m_container.GetHandlerForService( typeof(IMailMarketingService) ) );
            AssertNull( m_container[ "b" ] );

            m_container.RemoveComponent( "a" );

            AssertNull( m_container.GetHandlerForService( typeof(IMailService) ) );
            AssertNull( m_container[ "a" ] );
        }

		[Test]
		public void StartableComponent()
		{
			SimpleStartableComponent.Constructed = false;

			m_container.AddComponent( "a", typeof(IStartableService), typeof(SimpleStartableComponent) );

			Assert( SimpleStartableComponent.Constructed );
		}

		[Test]
		public void StartableComponentWithDependencies()
		{
			SimpleStartableComponent.Constructed = false;

			m_container.AddComponent( "a", typeof(IStartableService), typeof(SimpleStartableComponent2) );

			Assert( !SimpleStartableComponent.Constructed );

			m_container.AddComponent( "b", typeof(IMailService), typeof(SimpleMailService) );

			Assert( SimpleStartableComponent.Constructed );
		}

		[Test]
		public void ComponentDependingOnLogger()
		{
			m_container.AddComponent( "a", typeof(IMailService), typeof(SimpleMailServiceWithLogger) );

			IHandler handler = m_container[ "a" ];

			IMailService service = handler.Resolve() as IMailService;

			AssertNotNull( service );

			service.Send("hammett at apache dot org", 
				"johndoe at yahoo dot org", "Aloha!", "What's up?");

			handler.Release( service );
		}

        [Test]
		public void FacilityLifecycle()
		{
			MockFacility facility = new MockFacility();
            Assert(!facility.InitCalled);
            Assert(!facility.TerminateCalled);

            m_container.AddFacility( "mock", facility );
            Assert(facility.InitCalled);
            Assert(!facility.TerminateCalled);

            m_container.RemoveFacility("mock");

            Assert(facility.TerminateCalled);
        }

        public class MockFacility : IKernelFacility
		{
            private bool m_initCalled = false;
            private bool m_terminateCalled = false;

            public void Init(IKernel kernel)
            {
                m_initCalled = true;
            }

            public void Terminate(IKernel kernel)
            {
                m_terminateCalled = true;
            }

            public bool InitCalled
            {
                get { return m_initCalled; }
            }

            public bool TerminateCalled
            {
                get { return m_terminateCalled; }
            }
        }
    }
}
