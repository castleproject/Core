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