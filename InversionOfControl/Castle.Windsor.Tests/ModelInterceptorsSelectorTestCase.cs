// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

#if !SILVERLIGHT

using System;
using Castle.Core;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using Castle.MicroKernel.Proxy;
using Castle.Windsor;
using NUnit.Framework;

namespace Castle.Windsor.Tests
{
    [TestFixture]
    public class ModelInterceptorsSelectorTestCase
    {
        public interface IWatcher
        {
            event Action<string> OnSomethingInterestingToWatch;
        }

        public class BirdWatcher : IWatcher
        {
            public event Action<string> OnSomethingInterestingToWatch = delegate { };
        }

        public class DummyInterceptor : StandardInterceptor
        {
            public static bool WasCalled;

            protected override void PreProceed(IInvocation invocation)
            {

                WasCalled = true;
            }
        }

        public class Person
        {
            public IWatcher Watcher;

            public Person(IWatcher watcher)
            {
                Watcher = watcher;
            }
        }

        public enum Interceptors
        {
            None,
            Dummy,
        }

        public class WatcherInterceptorSelector : IModelInterceptorsSelector
        {
            public Interceptors Interceptors = Interceptors.None;

            public InterceptorReference[] SelectInterceptors(ComponentModel model)
            {
                if(model.Service!=typeof(IWatcher))
                    return null;
                if(Interceptors==Interceptors.None)
                    return null;
                return new InterceptorReference[]{new InterceptorReference(typeof(DummyInterceptor)), };
            }

            public bool HasInterceptors(ComponentModel model)
            {
                return model.Service == typeof (IWatcher) && Interceptors == Interceptors.Dummy;
            }
        }

        [Test]
        public void TurnProxyOnAndOff_DirectSelection()
        {
            IWindsorContainer container = new WindsorContainer();
            container
                .AddComponent<DummyInterceptor>()
                .AddComponentLifeStyle<IWatcher, BirdWatcher>("bird.watcher",LifestyleType.Transient);
            WatcherInterceptorSelector selector = new WatcherInterceptorSelector();
            container.Kernel.ProxyFactory.AddInterceptorSelector(selector);

            Assert.IsFalse(container.Resolve<IWatcher>().GetType().Name.Contains("Proxy"));
            selector.Interceptors = Interceptors.Dummy;
            Assert.IsTrue(container.Resolve<IWatcher>().GetType().Name.Contains("Proxy"));
        }

        [Test]
        public void TurnProxyOnAndOff_SubDependency()
        {
            IWindsorContainer container = new WindsorContainer();
            container
                .AddComponent<DummyInterceptor>()
                .AddComponentLifeStyle<IWatcher, BirdWatcher>("bird.watcher", LifestyleType.Transient)
                .AddComponentLifeStyle<Person>(LifestyleType.Transient);
            WatcherInterceptorSelector selector = new WatcherInterceptorSelector();
            container.Kernel.ProxyFactory.AddInterceptorSelector(selector);

            Assert.IsFalse(container.Resolve<Person>().Watcher.GetType().Name.Contains("Proxy"));
            Assert.IsFalse(container.Resolve<Person>().GetType().Name.Contains("Proxy"));
            
            selector.Interceptors = Interceptors.Dummy;
      
            Assert.IsFalse(container.Resolve<Person>().GetType().Name.Contains("Proxy"));
            Assert.IsTrue(container.Resolve<Person>().Watcher.GetType().Name.Contains("Proxy"));
        }

        [Test]
        public void CanAddInterceptor_DirectSelection()
        {
            IWindsorContainer container = new WindsorContainer();
            container
                .AddComponent<DummyInterceptor>()
                .AddComponentLifeStyle<IWatcher, BirdWatcher>("bird.watcher", LifestyleType.Transient);
          
            WatcherInterceptorSelector selector = new WatcherInterceptorSelector();
            container.Kernel.ProxyFactory.AddInterceptorSelector(selector);

            DummyInterceptor.WasCalled = false;
            IWatcher watcher = container.Resolve<IWatcher>();
            watcher.OnSomethingInterestingToWatch+=delegate {  };
            Assert.IsFalse(DummyInterceptor.WasCalled);
            
            selector.Interceptors = Interceptors.Dummy;

            DummyInterceptor.WasCalled = false;
            watcher = container.Resolve<IWatcher>();
            watcher.OnSomethingInterestingToWatch += delegate { };
            Assert.IsTrue(DummyInterceptor.WasCalled);
            
        }
    }
}

#endif
