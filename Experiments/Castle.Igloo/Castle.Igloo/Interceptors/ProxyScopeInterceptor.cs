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
using System.Diagnostics;
using Castle.Core;
using Castle.Core.Interceptor;
using Castle.Igloo.LifestyleManager;
using Castle.MicroKernel;

namespace Castle.Igloo.Interceptors
{
    /// <summary>
    /// The proxy will retrieve the current scoped instance it must point and
    /// delegate method call on this instance.
    /// </summary>
    public class ProxyScopeInterceptor : IInterceptor
    {
        public const string TARGET_NAME_PREFIX = "scopedTarget.";
        private ILifestyleManager _manager = null;
        private CreationContext _context = null;
        private string _componentName = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyScopeInterceptor"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="kernel">The kernel.</param>
        /// <param name="context">The context.</param>
        public ProxyScopeInterceptor(ComponentModel model, IKernel kernel, CreationContext context )
        {
            _context = context;
            _componentName = model.Name;

            ComponentModel proxyModel = kernel.ComponentModelBuilder.BuildModel(
                TARGET_NAME_PREFIX + model.Name, model.Service, model.Implementation, model.ExtendedProperties);

            proxyModel.CustomComponentActivator = null;
            proxyModel.LifestyleType = LifestyleType.Custom;
            proxyModel.CustomLifestyle = typeof(ScopeLifestyleManager);

            // Add the bijection interceptor
            proxyModel.Interceptors.AddFirst(new InterceptorReference(typeof(BijectionInterceptor)));

            IComponentActivator defaultActivator = kernel.CreateComponentActivator(proxyModel);

            _manager = (ILifestyleManager)Activator.CreateInstance(proxyModel.CustomLifestyle);
            _manager.Init(defaultActivator, kernel);
        }

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            // We need to retrieve the component instance from the Kernel, it will put it in the scope if needed
            // then we must call the invocation method on the component instance

            object scopedObject = _manager.Resolve(_context);
            invocation.ReturnValue = invocation.MethodInvocationTarget.Invoke(scopedObject, invocation.Arguments);

            Trace.WriteLine("Intercepted call to proxy scope component : " + _componentName + " on method " + invocation.MethodInvocationTarget.Name);

        }
    }
}
