using System;
using Castle.MicroKernel.ComponentActivator;
using NUnit.Framework;

namespace Castle.Windsor.Tests.Bugs
{
    [TestFixture]
    public class IoC_120
    {
        [Test]
        public void Can_resolve_component_with_internal_ctor()
        {
            var container = new WindsorContainer();
            container.AddComponent<Foo>();
            container.AddComponent<Bar>();

            try
            {
                container.Resolve<Bar>();
                Assert.Fail();
            }
            catch (ComponentActivatorException e)
            {
                Assert.AreEqual("Could not find a public constructor for the type Castle.Windsor.Tests.Bugs.Bar",
                    e.InnerException.Message);
            }
        }
    }

    public class Foo
    {
        
    }

    public class Bar
    {
        internal Bar(Foo f)
        {
            
        }
    }
}