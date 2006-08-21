// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Windsor.Tests
{
    using Castle.MicroKernel.Exceptions;
    using Castle.Windsor.Tests.Components;

    using NUnit.Framework;
	using Castle.MicroKernel.Handlers;

    [TestFixture]
    public class CircularDependencyTests
    {
        [Test]
        [ExpectedException(typeof(CircularDependecyException), @"A cycle was detected when trying to create a service. The dependency graph that resulted in a cycle is:
 - Service dependency 'view' type 'Castle.Windsor.Tests.Components.IView' for Void .ctor(Castle.Windsor.Tests.Components.IView) in type Castle.Windsor.Tests.Components.Controller
 - Service dependency 'Controller' type 'Castle.Windsor.Tests.Components.IController' for Castle.Windsor.Tests.Components.IController Controller in type Castle.Windsor.Tests.Components.View
 + Service dependency 'view' type 'Castle.Windsor.Tests.Components.IView' for Void .ctor(Castle.Windsor.Tests.Components.IView) in Castle.Windsor.Tests.Components.Controller
")]
        public void ThrowsACircularDependencyException()
        {
            IWindsorContainer container = new WindsorContainer();
            container.AddComponent("controller", typeof(IController), typeof(Controller));
            container.AddComponent("view", typeof(IView), typeof(View));

            container.Resolve("controller");
        }
    	
		[Test]
		[ExpectedException(typeof(HandlerException), @"Can't create component 'compA' as it has dependencies to be satisfied. 
compA is waiting for the following dependencies: 

Services: 
- Castle.Windsor.Tests.Components.CompB which was registered but is also waiting for dependencies. 

compB is waiting for the following dependencies: 

Services: 
- Castle.Windsor.Tests.Components.CompC which was registered but is also waiting for dependencies. 

compC is waiting for the following dependencies: 

Services: 
- Castle.Windsor.Tests.Components.CompD which was registered but is also waiting for dependencies. 

compD is waiting for the following dependencies: 

Services: 
- Castle.Windsor.Tests.Components.CompA which was registered but is also waiting for dependencies. 
")]
		public void ThrowsACircularDependencyException2()
		{
			IWindsorContainer container = new WindsorContainer();
			container.AddComponent("compA", typeof(CompA));
			container.AddComponent("compB", typeof(CompB));
			container.AddComponent("compC", typeof(CompC));
			container.AddComponent("compD", typeof(CompD));

			container.Resolve("compA");
		}
    }
}
