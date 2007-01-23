#region Apache Notice
/*****************************************************************************
 * 
 * Castle.Igloo
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 ********************************************************************************/
#endregion

using System;
using Castle.Core;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using Castle.Igloo.Interceptors;
using Castle.Igloo.Scopes;
using Castle.Igloo.Util;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;

namespace Castle.Igloo.ComponentActivator
{
    /// <summary>
    /// Implementation of <see cref="IComponentActivator"/>.
    /// Proxy factory for scoped objects.
    /// Created proxies will be injected as singletons, with transparent scoping behavior.
    /// Proxies returned by this class implement the ScopedObject interface.
    /// </summary>
    [Serializable]
    public class ScopeComponentActivator : DefaultComponentActivator
    {
        private ProxyGenerator _generator = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeComponentActivator"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="kernel">The kernel.</param>
        /// <param name="onCreation">The on creation.</param>
        /// <param name="onDestruction">The on destruction.</param>
        public ScopeComponentActivator(ComponentModel model, IKernel kernel,
            ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
            : base(model, kernel, onCreation, onDestruction)
        {
            _generator = new ProxyGenerator();
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="signature">The signature.</param>
        /// <returns></returns>
        protected override object CreateInstance(CreationContext context, object[] arguments, Type[] signature)
        {
            //ProxyGenerationOptions options = new ProxyGenerationOptions();
            //DefaultScopedObject scopedObject = new DefaultScopedObject(Kernel, ProxyScopeInterceptor.TARGET_NAME_PREFIX+Model.Name);
            //options.AddMixinInstance(scopedObject);

            IInterceptor interceptor = new ProxyScopeInterceptor(Model, Kernel);

            Type[] interfaces = new Type[1];
            interfaces[0] = typeof(IScopedObject);

            if (Model.Service.IsInterface)
            {
                object instance = _generator.CreateInterfaceProxyWithoutTarget(Model.Service, interfaces, interceptor);

                //object instance = _generator.CreateInterfaceProxyWithoutTarget(Model.Service, interfaces, options, interceptor);

                TraceUtil.Log("Return a proxy scope for component : " + Model.Name );

                return instance;  
            }
            else
            {
                //object instance = _generator.CreateClassProxy(Model.Service, interfaces, options, interceptor);

                TraceUtil.Log("Return a proxy scope for component : " + Model.Name);

                return new NotImplementedException();  
            }
        }

    }
}
