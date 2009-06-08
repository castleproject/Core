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

using System;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Tests.ClassComponents;
using NUnit.Framework;

namespace Castle.MicroKernel.Tests
{
    [TestFixture]
    public class DecoratorsTestCase
    {
        [Test]
        public void Should_ignore_reference_to_itself()
        {
            DefaultKernel kernel = new DefaultKernel();
            kernel.Register(
                Component.For<IRepository>().ImplementedBy<Repository1>(),
                Component.For<IRepository>().ImplementedBy<DecoratedRepository>()
                );
            Repository1 repos = (Repository1)kernel.Resolve<IRepository>();
            Assert.IsInstanceOf(typeof(DecoratedRepository), repos.InnerRepository);
        }

        [Test]
        [ExpectedException(typeof(HandlerException))]
        public void Will_give_good_error_message_if_cannot_resolve_service_that_is_likely_decorated_when_there_are_multiple_service()
        {
            DefaultKernel kernel = new DefaultKernel();
            kernel.Register(
                Component.For<IRepository>().ImplementedBy<Repository1>(),
                Component.For<IRepository>().ImplementedBy<DecoratedRepository2>().Named("foo"),
                Component.For<IRepository>().ImplementedBy<Repository1>().Named("bar")
                );
            kernel.Resolve<IRepository>();
        }

        [Test]
        public void Will_give_good_error_message_if_cannot_resolve_service_that_is_likely_decorated()
        {
            DefaultKernel kernel = new DefaultKernel();
            kernel.Register(
                Component.For<IRepository>().ImplementedBy<Repository1>(),
                Component.For<IRepository>().ImplementedBy<DecoratedRepository2>()
                );
            try
            {
                kernel.Resolve<IRepository>();
            }
            catch (HandlerException e)
            {
                const string expectedMessage = @"Can't create component 'Castle.MicroKernel.Tests.ClassComponents.Repository1' as it has dependencies to be satisfied. 
Castle.MicroKernel.Tests.ClassComponents.Repository1 is waiting for the following dependencies: 

Services: 
- Castle.MicroKernel.Tests.ClassComponents.IRepository. 
  A dependency cannot be satisfied by itself, did you forget to add a parameter name to differentiate between the two dependencies? 
 
Castle.MicroKernel.Tests.ClassComponents.DecoratedRepository2 is registered and is matching the required service, but cannot be resolved.

Castle.MicroKernel.Tests.ClassComponents.DecoratedRepository2 is waiting for the following dependencies: 

Keys (components with specific keys)
- name which was not registered. 
";
                Assert.AreEqual(expectedMessage, e.Message);
            }
        }
    }
}