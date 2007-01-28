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

using Castle.Core;
using Castle.Core.Interceptor;
using Castle.Igloo.Util;
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
        private string _componentName = string.Empty;
        private IKernel _kernel = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyScopeInterceptor"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="kernel">The kernel.</param>
        public ProxyScopeInterceptor(ComponentModel model, IKernel kernel)
        {
            _componentName = TARGET_NAME_PREFIX + model.Name;
            _kernel = kernel;

            if (!kernel.HasComponent(_componentName))
            {
                kernel.AddComponent(_componentName, model.Service, model.Implementation);      
            }
        }

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            // We need to retrieve the component instance from the Kernel, it will put it in the scope if needed
            // then we must call the invocation method on the component instance

            object scopedObject = _kernel[_componentName];
            invocation.ReturnValue = invocation.MethodInvocationTarget.Invoke(scopedObject, invocation.Arguments);

            TraceUtil.Log("Intercepted call to proxy scope component : " + _componentName + " on method " + invocation.MethodInvocationTarget.Name);

        }
    }
}
