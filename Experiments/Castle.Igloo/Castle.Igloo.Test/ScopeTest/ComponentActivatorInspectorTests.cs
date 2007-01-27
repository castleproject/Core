
using System;
using Castle.Core;
using Castle.Core.Internal;
using Castle.Igloo.Attributes;
using Castle.Igloo.ComponentActivator;
using Castle.Igloo.Inspectors;
using Castle.Igloo.Interceptors;
using Castle.Igloo.LifestyleManager;
using Castle.Igloo.Test.ScopeTest.Components;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;
using Castle.Windsor;
using NUnit.Framework;

namespace Castle.Igloo.Test.ScopeTest
{  
    [TestFixture]
    public class ComponentActivatorTests
    {
        private IContributeComponentModelConstruction _inspector = null;
        private IKernel _kernel = null;
        private IWindsorContainer _container = null;
        
        [SetUp]
        public void Setup()
        {
            _kernel = new DefaultKernel();
            _inspector = new IglooInspector();
            _container = new WindsorContainer();
            _container.AddFacility("Igloo.Facility", new IglooFacility());
        }        
        
        [Test]
        public void ProcessModel_NoScopeAttribute()
        {
            ComponentModel model = new ComponentModel("test", typeof(IComponent), typeof(SingletonScopeComponent));
            _inspector.ProcessModel(_kernel, model);
            Assert.IsNull(model.CustomComponentActivator);
        }

        [Test]
        public void ProcessModel_ScopeAttributeWithProxy()
        {
            ComponentModel model = new ComponentModel("test", typeof(IComponent), typeof(SimpleComponent));
            _inspector.ProcessModel(_kernel, model);
            
            Assert.IsNotNull(model.CustomComponentActivator);
            Assert.AreEqual(typeof(ScopeComponentActivator), model.CustomComponentActivator);
            
            ScopeAttribute scopeAttribute = (ScopeAttribute)model.ExtendedProperties[ScopeInspector.SCOPE_ATTRIBUTE];

            Assert.IsTrue(scopeAttribute.UseProxy);
            Assert.AreEqual(ScopeType.Singleton ,scopeAttribute.Scope);
        }

        [Test]
        public void ProcessModel_ScopeAttributeWithoutProxy()
        {
            ComponentModel model = new ComponentModel("test", typeof(IComponent), typeof(TransientScopeComponent));
            _inspector.ProcessModel(_kernel, model);

            Assert.IsNull(model.CustomComponentActivator);

            ScopeAttribute scopeAttribute = (ScopeAttribute)model.ExtendedProperties[ScopeInspector.SCOPE_ATTRIBUTE];

            Assert.IsFalse(scopeAttribute.UseProxy);
            Assert.AreEqual(ScopeType.Transient, scopeAttribute.Scope);

            Assert.AreEqual(LifestyleType.Custom, model.LifestyleType);
            Assert.AreEqual(typeof(ScopeLifestyleManager), model.CustomLifestyle);
        }

        [Test]
        public void ProcessModel_ScopeConfigWithProxy()
        {
            _container.AddComponent("Simple.Component", typeof(IComponent), typeof(SimpleComponent));

            IVertex[] vertices = TopologicalSortAlgo.Sort(_container.Kernel.GraphNodes);

			for (int i = 0; i < vertices.Length; i++)
			{
			    ComponentModel model = (ComponentModel)vertices[i];
			    if (model.Name == "Simple.Component")
			    {
                    Assert.IsNotNull(model.CustomComponentActivator);
                    Assert.AreEqual(typeof(ScopeComponentActivator), model.CustomComponentActivator);

                    ScopeAttribute scopeAttribute = (ScopeAttribute)model.ExtendedProperties[ScopeInspector.SCOPE_ATTRIBUTE];

                    Assert.IsTrue(scopeAttribute.UseProxy);
                    Assert.AreEqual(ScopeType.Singleton, scopeAttribute.Scope);
			    }
			}
        }

        [Test]
        public void ProcessModel_ScopeConfigWithoutProxy()
        {
            _container.AddComponent("Transient.Scope.Component", typeof(IComponent), typeof(TransientScopeComponent));

            IVertex[] vertices = TopologicalSortAlgo.Sort(_container.Kernel.GraphNodes);

            for (int i = 0; i < vertices.Length; i++)
            {
                ComponentModel model = (ComponentModel)vertices[i];
                if (model.Name == "Transient.Scope.Component")
                {
                    Assert.IsNull(model.CustomComponentActivator);

                    ScopeAttribute scopeAttribute = (ScopeAttribute)model.ExtendedProperties[ScopeInspector.SCOPE_ATTRIBUTE];

                    Assert.IsFalse(scopeAttribute.UseProxy);
                    Assert.AreEqual(ScopeType.Transient, scopeAttribute.Scope);

                    Assert.AreEqual(LifestyleType.Custom, model.LifestyleType);
                    Assert.AreEqual(typeof(ScopeLifestyleManager), model.CustomLifestyle);
                    Assert.AreEqual(0, model.Interceptors.Count);

                    foreach (InterceptorReference interceptorRef in model.Interceptors)
                    {
                        Assert.AreEqual(interceptorRef.ReferenceType, InterceptorReferenceType.Interface);
                        Assert.AreEqual(interceptorRef.ServiceType, typeof(BijectionInterceptor));
                    }
                }
            }
        }
        
        [Test]
        public void ProcessModel_WithoutInjectProperties()
        {
            ComponentModel model = new ComponentModel("test", typeof(IComponent), typeof(TransientScopeComponent));
            _inspector.ProcessModel(_kernel, model);

            Assert.AreEqual(0, model.Interceptors.Count);
        }

        [Test]
        public void ProcessModel_WithInjectProperties()
        {
            ComponentModel model = new ComponentModel("test", typeof(IComponent), typeof(ScopeComponentWithInject));
            _inspector.ProcessModel(_kernel, model);

            Assert.AreEqual(1, model.Interceptors.Count);

            foreach (InterceptorReference interceptorRef in model.Interceptors)
            {
                Assert.AreEqual(interceptorRef.ReferenceType, InterceptorReferenceType.Interface);
                Assert.AreEqual(interceptorRef.ServiceType, typeof(BijectionInterceptor));
            }
        }

        [Test]
        public void ProcessModel_WithOutjectProperties()
        {
            ComponentModel model = new ComponentModel("test", typeof(IComponent), typeof(TransientScopeComponent));
            _inspector.ProcessModel(_kernel, model);

            Assert.AreEqual(0, model.Interceptors.Count);
        }

        [Test]
        public void ProcessModel_WithoutOutjectProperties()
        {
            ComponentModel model = new ComponentModel("test", typeof(IComponent), typeof(ScopeComponentWithOutject));
            _inspector.ProcessModel(_kernel, model);

            Assert.AreEqual(1, model.Interceptors.Count);

            foreach (InterceptorReference interceptorRef in model.Interceptors)
            {
                Assert.AreEqual(interceptorRef.ReferenceType, InterceptorReferenceType.Interface);
                Assert.AreEqual(interceptorRef.ServiceType, typeof(BijectionInterceptor));
            }
        }

        [Test]
        public void ProcessModel_WithInjectOutjectProperties()
        {
            ComponentModel model = new ComponentModel("test", typeof(IComponent), typeof(ScopeComponentWithInjectOutject));
            _inspector.ProcessModel(_kernel, model);

            Assert.AreEqual(1, model.Interceptors.Count);

            foreach (InterceptorReference interceptorRef in model.Interceptors)
            {
                Assert.AreEqual(interceptorRef.ReferenceType, InterceptorReferenceType.Interface);
                Assert.AreEqual(interceptorRef.ServiceType, typeof(BijectionInterceptor));
            }
        }

        [Scope(Scope = ScopeType.Transient)]
        private class ScopeComponentWithInject : IComponent
        {
            [Inject(Name="Test")]
            public string Name
            {
              set
              {
                  throw new NotImplementedException("The method or operation is not implemented."); 
              }   
            }
            #region IComponent Members

            public int ID
            {
                get { throw new NotImplementedException("The method or operation is not implemented."); }
            }

            #endregion
        }

        [Scope(Scope = ScopeType.Transient)]
        private class ScopeComponentWithOutject : IComponent
        {
            [Outject(Name = "Test")]
            public string Name
            {
                get
                {
                    throw new NotImplementedException("The method or operation is not implemented.");
                }
            }
            #region IComponent Members

            public int ID
            {
                get { throw new NotImplementedException("The method or operation is not implemented."); }
            }

            #endregion
        }

        [Scope(Scope = ScopeType.Transient)]
        private class ScopeComponentWithInjectOutject : IComponent
        {
            [Outject(Name = "Test")]
            [Inject(Name = "Test")]
            public string Name
            {
                get
                {
                    throw new NotImplementedException("The method or operation is not implemented.");
                }
                set
                {
                    throw new NotImplementedException("The method or operation is not implemented.");
                } 
            }
            #region IComponent Members

            public int ID
            {
                get { throw new NotImplementedException("The method or operation is not implemented."); }
            }

            #endregion
        }
    }
}
