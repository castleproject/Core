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
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Castle.Core;
using Castle.Core.Interceptor;
using Castle.Igloo.Attributes;
using Castle.Igloo.Scopes;
using Castle.MicroKernel;

namespace Castle.Igloo.Interceptors
{
    /// <summary>
    /// Before invoking the component, inject all dependencies. 
    /// After invoking, outject dependencies back into their context. 
    /// </summary>
    [Transient]
    public class BijectionInterceptor : IInterceptor, IOnBehalfAware 
    {
        private IKernel _kernel = null;
        private ComponentModel _model = null;
        private IDictionary<InjectAttribute, PropertyInfo> _inMembers = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BijectionInterceptor"/> class.
        /// </summary>
        public BijectionInterceptor(IKernel kernel)
		{
            _kernel = kernel;
		}
        
        #region IMethodInterceptor Members

        /// <summary>
        /// Method invoked by the proxy in order to allow
        /// the interceptor to do its work before and after
        /// the actual invocation.
        /// </summary>
        /// <param name="invocation">The invocation holds the details of this interception</param>
        /// <returns>The return value of this invocation</returns>
        public void Intercept(IInvocation invocation)
        {
            if (NeedsInjection)
            {
                //if (log.isTraceEnabled())
                //{
                Trace.WriteLine("injecting dependencies of : " + _model.Name);
                //}
                IScopeRegistry scopeRegistry = _kernel[typeof(IScopeRegistry)] as IScopeRegistry;

                foreach (KeyValuePair<InjectAttribute, PropertyInfo> kvp in _inMembers)
                {                   
                    PropertyInfo propertyInfo = kvp.Value;
                    object instanceToInject = scopeRegistry.GetFromScopes(kvp.Key);

                    if (instanceToInject == null)
                    {
                        if (kvp.Key.Create)
                        {
                            instanceToInject = Activator.CreateInstance(propertyInfo.PropertyType);

                            IScope scope = scopeRegistry[kvp.Key.Scope];
                            scope.Add(kvp.Key.Name, instanceToInject);
                        }
                        else
                        {
                            // do log / use  kvp.Key.Required
                        }
                    }
                    propertyInfo.SetValue(invocation.InvocationTarget, instanceToInject, null);
                }
            }

            invocation.Proceed();
        }

        #endregion

        #region IOnBehalfAware Members

        /// <summary>
        /// Sets the intercepted component model.
        /// </summary>
        /// <param name="target">The target.</param>
        public void SetInterceptedComponentModel(ComponentModel target)
        {
            _model = target;
            _inMembers =
                (IDictionary<InjectAttribute, PropertyInfo>)target.ExtendedProperties[BijectionInspector.IN_MEMBERS];
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether the component needs injection.
        /// </summary>
        /// <value><c>true</c> if [needs injection]; otherwise, <c>false</c>.</value>
        public bool NeedsInjection
        {
            get { return _inMembers.Count > 0; }
        }
    }
}
