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

using System.Collections;
using System.Collections.Specialized;
using Castle.MicroKernel;
using NUnit.Framework;

namespace Castle.Windsor.Tests.Bugs
{
    [TestFixture]
    public class IoC_138
    {
        [Test]
        public void TestResolveSubComponentInConstructorWithParameters()
        {
            IWindsorContainer container = new WindsorContainer();
            container.AddComponent("A", typeof(A));
            container.AddComponent("B", typeof(B));

            IDictionary parameters = new ListDictionary();
            parameters.Add("test", "bla");

            A a = container.Resolve<A>(parameters);
            Assert.IsNotNull(a);
        }


        public class A
        {
            private B b;

            public A(IKernel kernel, string test)
            {
                IDictionary parameters = new ListDictionary();
                parameters.Add("test2", "bla");
                b = kernel.Resolve<B>(parameters);
            }
        }

        public class B
        {
            public B(string test2)
            {

            }
        }

    }
}