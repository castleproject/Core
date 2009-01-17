using Castle.MicroKernel.Registration;
using NUnit.Framework;

namespace Castle.Windsor.Tests.Bugs
{
    [TestFixture]
    public class IoC_92
    {
        [Test]
        public void Can_mix_hashtable_parameters_and_configuration_parameters()
        {
            var container = new WindsorContainer();
            container.Register(
                Component.For<Foo>()
                    .Parameters(Parameter.ForKey("x").Eq("abc"))
                );

            container.Resolve<Foo>(new {y=1});
        }

        public class Foo
        {
            public Foo(string x, int y)
            {
                
            }
        }
    }
}