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
using System.Reflection;
using Castle.Core;
using Castle.Core.Interceptor;
using Castle.MicroKernel.Registration;
using NUnit.Framework;

namespace Castle.Windsor.Tests
{
    [TestFixture]
    public class InterceptorsSelectorTestCase
    {
        public interface ICatalog
        {
			void AddItem(object item);

			void RemoveItem(object item);
        }

        public class SimpleCatalog : ICatalog
        {
			public void AddItem(object item)
			{
			}

			public void RemoveItem(object item)
			{	
			}
        }

        public class DummyInterceptor : StandardInterceptor
        {
            public static bool WasCalled;

            protected override void PreProceed(IInvocation invocation)
            {
                WasCalled = true;
            }
        }

        public class DummyInterceptorSelector : IInterceptorSelector
        {  
			public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
			{
				if (typeof(ICatalog).IsAssignableFrom(type))
				{
					if (method.Name == "AddItem")
						return interceptors;
				}
				return null;
			}
		}

        [Test]
        public void CanApplyInterceptorsToSelectedMethods()
        {
            IWindsorContainer container = new WindsorContainer();
			container.Register(
				Component.For<ICatalog>()
					.ImplementedBy<SimpleCatalog>()
					.Interceptors(InterceptorReference.ForType<DummyInterceptor>())
						.SelectedWith(new DummyInterceptorSelector()).Anywhere,
				Component.For<DummyInterceptor>()
					);

			Assert.IsFalse(DummyInterceptor.WasCalled);

			ICatalog catalog = container.Resolve<ICatalog>();
			catalog.AddItem("hot dogs");
			Assert.IsTrue(DummyInterceptor.WasCalled);

			DummyInterceptor.WasCalled = false;
			catalog.RemoveItem("hot dogs");
			Assert.IsFalse(DummyInterceptor.WasCalled);
        }
    }
}

#endif
