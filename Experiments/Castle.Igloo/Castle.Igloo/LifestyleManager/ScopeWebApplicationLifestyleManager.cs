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
using System.Collections;
using System.Configuration;
using System.Web;
using Castle.Igloo.Scopes.Web;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;
using Castle.MicroKernel.Lifestyle;

namespace Castle.Igloo.LifestyleManager
{
    /// <summary>
    /// Implements a Lifestyle Manager for Web Apps that
    /// create object and store them in applictaion context.
    /// </summary>
    [Serializable]
    public class ScopeWebApplicationLifestyleManager : AbstractLifestyleManager
    {
        private IApplicationScope _applicationScope = null;

        /// <summary>
        /// Inits the specified component activator.
        /// </summary>
        /// <param name="componentActivator">The component activator.</param>
        /// <param name="kernel">The kernel.</param>
        public override void Init(IComponentActivator componentActivator, IKernel kernel)
        {
            base.Init(componentActivator, kernel);

            _applicationScope = Kernel[typeof(IApplicationScope)] as IApplicationScope;
        }

        #region ILifestyleManager Members

        /// <summary>
        /// Resolves the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override object Resolve(CreationContext context)
        {
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("HttpContext.Current is null. ScopeWebApplicationLifestyleManager can only be used in ASP.NET");
            }

            string name = (ComponentActivator as AbstractComponentActivator).Model.Name;

            if (_applicationScope[name] == null)
            {
                if (!ScopeLifestyleModule.Initialized)
                {
                    string message = "Looks like you forgot to register the http module " +
                        typeof(ScopeLifestyleModule).FullName +
                        "\r\nAdd '<add name=\"ScopeLifestyleModule\" type=\"Castle.Igloo.LifestyleManager.ScopeLifestyleModule, Castle.Igloo\" />' " +
                        "to the <httpModules> section on your web.config";
                    {
                        throw new ConfigurationErrorsException(message);
                    }

                }

                object instance = base.Resolve(context);

                _applicationScope.Add(name, instance);
            }

            return _applicationScope[name];
        }

        /// <summary>
        /// Releases the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public override void Release(object instance)
        {
           //do nothing
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public override void Dispose()
        {
            IEnumerator enumerator = _applicationScope.Names.GetEnumerator();
            while ( enumerator.MoveNext() )
            {
                string name = (string) enumerator.Current;
                base.Release(_applicationScope[name]);
            }
        }

        #endregion
    }
}
