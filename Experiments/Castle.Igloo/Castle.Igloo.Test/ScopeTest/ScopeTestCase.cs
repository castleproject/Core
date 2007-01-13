
using System.Threading;

using Castle.Core.Resource;
using Castle.Igloo.Test.ScopeTest.Components;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using NUnit.Framework;

using Castle.MicroKernel;

namespace Castle.Igloo.Test.ScopeTest
{

    /// <summary>
    /// Summary description for LifestyleManagerTestCase.
    /// </summary>
    [TestFixture]
    public class ScopeTestCase
    {
        private static IWindsorContainer _container = null;

        private IComponent instance3;

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
        public void TestSingletonScope()
        {
            _container.AddComponent("a", typeof(IComponent), typeof(SingletonScopeComponent));

            IHandler handler = _container.Kernel.GetHandler("a");

            IComponent instance1 = handler.Resolve(CreationContext.Empty) as IComponent;
            IComponent instance2 = handler.Resolve(CreationContext.Empty) as IComponent;

            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);

            Assert.IsTrue(instance1.Equals(instance2));
            Assert.IsTrue(instance1.ID == instance2.ID);

            handler.Release(instance1);
            handler.Release(instance2);
        }
        
        [Test]
        public void TestThreadScope()
        {
            _container.AddComponent("a", typeof(IComponent), typeof(PerScopeThreadComponent));

            IHandler handler = _container.Kernel.GetHandler("a");

            IComponent instance1 = handler.Resolve(CreationContext.Empty) as IComponent;
            IComponent instance2 = handler.Resolve(CreationContext.Empty) as IComponent;

            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);

            Assert.IsTrue(instance1.Equals(instance2));
            Assert.IsTrue(instance1.ID == instance2.ID);

            Thread thread = new Thread(new ThreadStart(OtherThread));
            thread.Start();
            thread.Join();

            Assert.IsNotNull(this.instance3);
            Assert.IsTrue(!instance1.Equals(this.instance3));
            Assert.IsTrue(instance1.ID != this.instance3.ID);

            handler.Release(instance1);
            handler.Release(instance2);
        }

        private void OtherThread()
        {
            IHandler handler = _container.Kernel.GetHandler("a");
            this.instance3 = handler.Resolve(CreationContext.Empty) as IComponent;
        }
    }
}
