
using Castle.Core.Resource;
using Castle.Igloo.Scopes;
using Castle.Igloo.Test.ScopeTest.Components;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using NUnit.Framework;

namespace Castle.Igloo.Test.ScopeTest
{

    /// <summary>
    /// Summary description for LifestyleManagerTestCase.
    /// </summary>
    [TestFixture]
    public class ProxyScopeTest
    {
        private IWindsorContainer _container = null;

        [SetUp]
        public void CreateContainer()
        {
            DefaultConfigurationStore store = new DefaultConfigurationStore();
            XmlInterpreter interpreter = new XmlInterpreter(new ConfigResource());
            interpreter.ProcessResource(interpreter.Source, store);

            _container = new WindsorContainer(interpreter);
        }

        [TearDown]
        public void DisposeContainer()
        {
            _container.Dispose();
        }

        [Test]
        public void TestScopedObjectOnProxyScope()
        {
            _container.AddComponent("Simple.Component", typeof(IComponent), typeof(SimpleComponent));

            IComponent service1 = _container.Resolve<IComponent>("Simple.Component");

            Assert.IsNotNull(service1);
            Assert.IsTrue(typeof(IScopedObject).IsAssignableFrom(service1.GetType()));
        }

        [Test]
        public void TestProxyScope()
        {
            _container.AddComponent("Simple.Component", typeof(IComponent), typeof(SimpleComponent));

            IComponent service1 = _container.Resolve<IComponent>("Simple.Component");

            int result = service1.ID;
            Assert.AreEqual(99, result);

            IComponent service2 = _container.Resolve<IComponent>("Simple.Component");

            Assert.IsTrue(service1.Equals(service2));

            result = service2.ID;
            Assert.AreEqual(99, result);
        }
    }
}
