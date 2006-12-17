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

using System.Collections.Generic;
using Castle.Core;
using Castle.Core.Interceptor;
using Castle.Igloo.Attributes;
using Castle.Igloo.Controllers;
using Castle.Igloo.Navigation;

namespace Castle.Igloo.Interceptors
{
    /// <summary>
    /// Intercepts call for <see cref="IController"/> components, do navigation if needed.
    /// </summary>
    [Transient]
    public class NavigationInterceptor : IMethodInterceptor, IOnBehalfAware 
    {
        private readonly INavigator _navigator = null;
        private IDictionary<string, NoNavigationAttribute> _noNavigationMembers = null;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationInterceptor"/> class.
        /// </summary>
        /// <param name="navigator">The navigator.</param>
        public NavigationInterceptor(INavigator navigator)
		{
            _navigator = navigator;
		}

        
        #region Membres de IMethodInterceptor

        /// <summary>
        /// Method invoked by the proxy in order to allow
        /// the interceptor to do its work before and after
        /// the actual invocation.
        /// </summary>
        /// <param name="invocation">The invocation holds the details of this interception</param>
        /// <param name="args">The original method arguments</param>
        /// <returns>The return value of this invocation</returns>
        public object Intercept(IMethodInvocation invocation, params object[] args)
        {
            object result = invocation.Proceed(args);
            if (!_noNavigationMembers.Keys.Contains(invocation.Method.Name))
            {
                _navigator.Navigate();
            }

            return result;
        }

        #endregion

        #region IOnBehalfAware Members

        /// <summary>
        /// Sets the intercepted component model.
        /// </summary>
        /// <param name="target">The target.</param>
        public void SetInterceptedComponentModel(ComponentModel target)
        {
            _noNavigationMembers = (IDictionary<string, NoNavigationAttribute>)target.ExtendedProperties[ControllerInspector.NO_NAVIGATION];
        }

        #endregion
    }
}
