
using System;
using System.Threading;
using Castle.Core;
using Castle.Core.Interceptor;
using Castle.Core.Resource;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;
using Castle.Igloo.Attributes;
using Castle.Igloo.LifestyleManager;
using Castle.Igloo.Scopes;
using Castle.Igloo.Scopes.Web;
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

        /// <summary>
        /// Test ScopeRegistry
        /// </summary>
        [Test]
        public void TestScopeRegistry()
        {
            IScopeRegistry registry = _container.Resolve<IScopeRegistry>();
            Assert.IsNotNull(registry);
            
            ISessionScope scope = _container.Resolve<ISessionScope>();
            Assert.IsNotNull(scope);
            
            scope.Add("test", new object());

            Assert.IsTrue(registry.IsInScopes("test"));
            
            InjectAttribute attribute = new InjectAttribute();
            attribute.Name = "test";
            attribute.Scope = ScopeType.Session;

            Assert.IsNotNull(registry.GetFromScopes(attribute));
        }

        [Test]
        public void ScopeSetThroughAttribute()
        {
            _container.AddComponent("b", typeof(TransientScopeComponent));
            IHandler handler = _container.Kernel.GetHandler("b");
            Assert.AreEqual(LifestyleType.Custom, handler.ComponentModel.LifestyleType);
            Assert.AreEqual(typeof(ScopeLifestyleManager), handler.ComponentModel.CustomLifestyle);

            _container.AddComponent("c", typeof(PerScopeThreadComponent));
            handler = _container.Kernel.GetHandler("c");
            Assert.AreEqual(LifestyleType.Custom, handler.ComponentModel.LifestyleType);
            Assert.AreEqual(typeof(ScopeLifestyleManager), handler.ComponentModel.CustomLifestyle);

            //_container.AddComponent("d", typeof(PerScopeWebRequestComponent));
            //handler = _container.Kernel.GetHandler("d");
            //Assert.AreEqual(LifestyleType.Custom, handler.ComponentModel.LifestyleType);
            //Assert.AreEqual(typeof(ScopeLifestyleManager), handler.ComponentModel.CustomLifestyle);
        }

        [Test]
        public void ScopeSetThroughConfig()
        {
            IHandler handler = _container.Kernel.GetHandler("Singleton.Scope.Component");
            Assert.AreEqual(LifestyleType.Custom, handler.ComponentModel.LifestyleType);
            Assert.AreEqual(typeof(ScopeLifestyleManager), handler.ComponentModel.CustomLifestyle);

        }

        [Test]
        public void TestSingletonScope()
        {
            IHandler handler = _container.Kernel.GetHandler("Singleton.Scope.Component");

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
        public void TestTransientScope()
        {
            _container.AddComponent("a", typeof(IComponent), typeof(TransientScopeComponent));

            IHandler handler = _container.Kernel.GetHandler("a");

            IComponent instance1 = handler.Resolve(CreationContext.Empty) as IComponent;
            IComponent instance2 = handler.Resolve(CreationContext.Empty) as IComponent;

            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);

            Assert.IsTrue(!instance1.Equals(instance2));
            Assert.IsTrue(instance1.ID != instance2.ID);

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

        [Test]
        public void TestProxyScopeIdea()
        {
            _container.AddComponent("Simple.Component", typeof(IComponent), typeof(SimpleComponent));

            IHandler handler = _container.Kernel.GetHandler("Simple.Component");

            ProxyGenerator generator = new ProxyGenerator();
            Type[] interfaces = new Type[1];
            interfaces[0] = typeof (IScopedObject);

            IInterceptor interceptor = new ProxyScopeInterceptor(handler.ComponentModel, _container.Kernel);
            IComponent service = (IComponent)generator.CreateInterfaceProxyWithoutTarget(typeof(IComponent), interfaces, interceptor);

            Assert.IsNotNull(service);
            Assert.IsTrue(typeof(IScopedObject).IsAssignableFrom(service.GetType()));

            int result = service.ID;
            Assert.AreEqual(99, result);
        }

        public class ProxyScopeInterceptor : IInterceptor
        {
            private ComponentModel _model = null;
            private IKernel _kernel = null;

            public ProxyScopeInterceptor(ComponentModel model, IKernel kernel)
            {
                _model = model;
                _kernel = kernel;
            }

            public void Intercept(IInvocation invocation)
            {
                // We need to retrieve the componet instance from the Kernel, it will put it in the scope
                // then we must call the invocation method on the component instance

                object scopedObject = _kernel[_model.Name];
                invocation.ReturnValue = invocation.MethodInvocationTarget.Invoke(scopedObject, invocation.Arguments);
            }
        }

        public interface IScopedObject
        {
        }

        /// <summary>
        /// Summary description for SimpleMixin.
        /// </summary>
        [Serializable]
        public class ScopedObject : IScopedObject
        {
        }
    }
}
