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

    using Castle.MicroKernel;
    using Castle.MicroKernel.Model;
	using Castle.MicroKernel.Interceptor;
	using Castle.MicroKernel.Test.Components;

	/// <summary>
	/// Summary description for KernelEventsTestCase.
	/// </summary>
	[TestFixture]
    public class KernelEventsTestCase : Assertion
	{
        private bool m_registered = false;
        private bool m_unregistered = false;
        private bool m_modelConstructed = false;
        private bool m_wrap = false;
        private bool m_unwrap= false;
        private bool m_ready = false;
        private bool m_released = false;

        [Test]
        public void RegisteredAndUnregistered()
        {
            BaseKernel container = new BaseKernel();

            container.ComponentRegistered += new ComponentDataDelegate(Registered);
            container.ComponentUnregistered += new ComponentDataDelegate(Unregistered);

            Assert(!m_registered);
            Assert(!m_unregistered);

            container.AddComponent( "a", typeof(IMailService), typeof(SimpleMailService) );

            Assert(m_registered);
            Assert(!m_unregistered);

            m_registered = false;

            container.RemoveComponent( "a" );

            Assert(!m_registered);
            Assert(m_unregistered);
        }

        [Test]
        public void ModelConstructed()
        {
            BaseKernel container = new BaseKernel();

            container.ComponentModelConstructed += new ComponentModelDelegate(ComponentModelConstructed);

            Assert(!m_modelConstructed);

            container.AddComponent( "a", typeof(IMailService), typeof(SimpleMailService) );

            Assert(m_modelConstructed);

            m_modelConstructed = false;

            container.RemoveComponent( "a" );

            Assert(!m_modelConstructed);
        }

        [Test]
        public void WrapAndUnwrap()
        {
            BaseKernel container = new BaseKernel();

            container.ComponentWrap += new WrapDelegate(ComponentWrap);
            container.ComponentUnWrap += new WrapDelegate(ComponentUnWrap);

            container.AddComponent( "a", typeof(IMailService), typeof(SimpleMailService) );

            Assert(!m_wrap);
            Assert(!m_unwrap);

            IHandler handler = container[ "a" ];

            IMailService service = handler.Resolve() as IMailService;

            Assert(m_wrap);
            Assert(!m_unwrap);
            m_wrap = false;

            service.Send("hammett at apache dot org", "johndoe at yahoo dot org", "Aloha!", "What's up?");

            handler.Release( service );

            Assert(!m_wrap);
            Assert(m_unwrap);
        }

        [Test]
        public void ReadyAndRelease()
        {
            BaseKernel container = new BaseKernel();

            container.ComponentReady += new ComponentInstanceDelegate(ComponentReady);
            container.ComponentReleased += new ComponentInstanceDelegate(ComponentReleased);

            container.AddComponent( "a", typeof(IMailService), typeof(SimpleMailService) );

            Assert(!m_ready);
            Assert(!m_released);

            IHandler handler = container[ "a" ];

            IMailService service = handler.Resolve() as IMailService;

            Assert(m_ready);
            Assert(!m_released);
            m_ready = false;

            service.Send("hammett at apache dot org", "johndoe at yahoo dot org", "Aloha!", "What's up?");

            handler.Release( service );

            Assert(!m_ready);
            Assert(m_released);
        }

        private void Registered(IComponentModel model, String key, IHandler handler)
        {
            AssertNotNull( model );
            AssertNotNull( key );
            AssertNotNull( handler );
            AssertEquals( "a", key );

            m_registered = true;
        }

        private void Unregistered(IComponentModel model, String key, IHandler handler)
        {
            AssertNotNull( model );
            AssertNotNull( key );
            AssertNotNull( handler );
            AssertEquals( "a", key );

            m_unregistered = true;
        }

        private void ComponentModelConstructed(IComponentModel model, String key)
        {
            AssertNotNull( model );
            AssertNotNull( key );

            m_modelConstructed = true;
        }

        private void ComponentWrap(IComponentModel model, String key, IHandler handler, Castle.MicroKernel.Interceptor.IInterceptedComponent interceptedComponent)
        {
            AssertNotNull( model );
            AssertNotNull( key );
            AssertNotNull( handler );

            m_wrap = true;
        }

        private void ComponentUnWrap(IComponentModel model, String key, IHandler handler, IInterceptedComponent interceptedComponent)
        {
            AssertNotNull( model );
            AssertNotNull( key );
            AssertNotNull( handler );
            AssertNull( interceptedComponent );

            m_unwrap = true;
        }

        private void ComponentReady(IComponentModel model, String key, IHandler handler, object instance)
        {
            AssertNotNull( model );
            AssertNotNull( key );
            AssertNotNull( handler );
            AssertNotNull( instance );

            m_ready = true;
        }

        private void ComponentReleased(IComponentModel model, String key, IHandler handler, object instance)
        {
            AssertNotNull( model );
            AssertNotNull( key );
            AssertNotNull( handler );
            AssertNotNull( instance );

            m_released = true;
        }
    }
}
