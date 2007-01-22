
using System;
using Castle.Igloo.Scopes;
using Castle.Igloo.Test.ScopeTest.Components;
using Castle.Windsor;
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
            _container = new WindsorContainer();
            _container.AddFacility("Igloo.Facility", new IglooFacility());
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

        [Test]
        public void TestGraphOfScopeComponent()
        {
            _container.AddComponent("Thread.Scope.Proxy.Component", typeof(IComponent), typeof(SimpleComponent));
            _container.AddComponent("Singleton.Component", typeof(IComponent0), typeof(SimpleComponent0));
            _container.AddComponent("Thread.Scope.Proxy.Component1", typeof(IComponent1), typeof(SimpleComponent1));
            _container.AddComponent("Thread.Scope.Proxy.Component2", typeof(IComponent2), typeof(SimpleComponent2));

            IComponent0 service00 = _container.Resolve<IComponent0>();
            IComponent0 service01 = _container.Resolve<IComponent0>();

            Assert.IsNotNull(service00);
            Assert.IsNotNull(service01);

            Assert.IsTrue(service00.Equals(service01));
            Assert.IsTrue(service01.ID == service00.ID);

            Assert.IsTrue(typeof(IScopedObject).IsAssignableFrom(service00.SimpleComponent1.GetType()));
            Assert.IsTrue(typeof(IScopedObject).IsAssignableFrom(service01.SimpleComponent1.GetType()));
            Assert.IsTrue(service00.SimpleComponent1.Equals(service01.SimpleComponent1));

            IComponent1 service1 = service00.SimpleComponent1;

            Type[] interfaces = service1.SimpleComponent2.GetType().GetInterfaces();

            Assert.IsTrue(typeof(IScopedObject).IsAssignableFrom( service1.SimpleComponent2.GetType()));
            Assert.AreEqual(101, service1.ID);

            IComponent2 service2 = service1.SimpleComponent2;

            interfaces = service2.SimpleComponent.GetType().GetInterfaces();

            Assert.IsTrue(typeof(IScopedObject).IsAssignableFrom(service2.SimpleComponent.GetType()));
            Assert.AreEqual("SimpleComponent2", service2.Name);

            IComponent service = service2.SimpleComponent;

            Assert.AreEqual(99, service.ID);
        }
    }
}
