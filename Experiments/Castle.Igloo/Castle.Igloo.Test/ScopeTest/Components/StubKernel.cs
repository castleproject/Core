using System;
using Castle.MicroKernel;

namespace Castle.Igloo.Test.ScopeTest.Components
{
    public class StubKernel : IKernel
    {
        #region IKernel Members

        public void AddChildKernel(IKernel kernel)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AddComponent(string key, Type serviceType, Type classType, Castle.Core.LifestyleType lifestyle, bool overwriteLifestyle)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AddComponent(string key, Type serviceType, Type classType, Castle.Core.LifestyleType lifestyle)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AddComponent(string key, Type serviceType, Type classType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AddComponent(string key, Type classType, Castle.Core.LifestyleType lifestyle, bool overwriteLifestyle)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AddComponent(string key, Type classType, Castle.Core.LifestyleType lifestyle)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AddComponent(string key, Type classType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AddComponentInstance(string key, Type serviceType, object instance)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AddComponentInstance(string key, object instance)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AddComponentWithExtendedProperties(string key, Type serviceType, Type classType, System.Collections.IDictionary extendedProperties)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AddComponentWithExtendedProperties(string key, Type classType, System.Collections.IDictionary extendedProperties)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AddCustomComponent(Castle.Core.ComponentModel model)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AddFacility(string key, IFacility facility)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AddSubSystem(string key, ISubSystem subsystem)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IComponentModelBuilder ComponentModelBuilder
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IConfigurationStore ConfigurationStore
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public IComponentActivator CreateComponentActivator(Castle.Core.ComponentModel model)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IHandler[] GetAssignableHandlers(Type service)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IFacility[] GetFacilities()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IHandler GetHandler(Type service)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IHandler GetHandler(string key)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IHandler[] GetHandlers(Type service)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ISubSystem GetSubSystem(string key)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Castle.Core.GraphNode[] GraphNodes
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IHandlerFactory HandlerFactory
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool HasComponent(Type service)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool HasComponent(string key)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IKernel Parent
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public IProxyFactory ProxyFactory
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public void RegisterCustomDependencies(string key, System.Collections.IDictionary dependencies)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RegisterCustomDependencies(Type service, System.Collections.IDictionary dependencies)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ReleaseComponent(object instance)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IReleasePolicy ReleasePolicy
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public void RemoveChildKernel(IKernel kernel)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool RemoveComponent(string key)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object Resolve(string key, Type service, System.Collections.IDictionary arguments)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object Resolve(string key, Type service)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object Resolve(string key, System.Collections.IDictionary arguments)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object Resolve(Type service, System.Collections.IDictionary arguments)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public TService[] ResolveServices<TService>()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IDependencyResolver Resolver
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public object this[Type service]
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public object this[string key]
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IKernelEvents Members

        public event EventHandler AddedAsChildKernel;

        public event ComponentInstanceDelegate ComponentCreated;

        public event ComponentInstanceDelegate ComponentDestroyed;

        public event ComponentModelDelegate ComponentModelCreated;

        public event ComponentDataDelegate ComponentRegistered;

        public event ComponentDataDelegate ComponentUnregistered;

        public event DependencyDelegate DependencyResolving;

        public event HandlerDelegate HandlerRegistered;

        public event EventHandler RemovedAsChildKernel;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
