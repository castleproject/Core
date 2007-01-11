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
using System.Configuration;
using System.Web;
using Castle.Igloo.Scopes;
using Castle.Igloo.Scopes.Web;
using Castle.Igloo.LifestyleManager;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;
using Castle.MicroKernel.Lifestyle;

namespace Castle.Igloo.LifestyleManager
{
    /// <summary>
    /// Implements a Lifestyle Manager for Web Apps that
    /// create at most one object per web session.
    /// 
    /// ScopeWebSessionLifestyleManager tries to lookup component by name 
    /// in http session. 
    /// If not found, it tries to instantiate new bean and attaches it to said session.
    /// </summary>
    [Serializable]
    public sealed class ScopeWebSessionLifestyleManager : AbstractLifestyleManager
    {
        private ISessionScope _sessionScope = null;

        /// <summary>
        /// Inits the specified component activator.
        /// </summary>
        /// <param name="componentActivator">The component activator.</param>
        /// <param name="kernel">The kernel.</param>
        public override void Init(IComponentActivator componentActivator, IKernel kernel)
        {
            base.Init(componentActivator, kernel);

            _sessionScope = Kernel[typeof(ISessionScope)] as ISessionScope;
        }

        #region ILifestyleManager Members

        /// <summary>
        /// Find a component by name in session scoped storage implementation. 
        /// If not found, try to instantiate new one by the <see cref="IKernel"/>.
        /// Then found component will be attached to session store implementation.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The component</returns>
        public override object Resolve(CreationContext context)
        {
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("HttpContext.Current is null. ScopeWebSessionLifestyleManager can only be used in ASP.NET.");
            }
            
            string name = (ComponentActivator as AbstractComponentActivator).Model.Name;

            if (_sessionScope[name] == null)
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
                _sessionScope.Add(name, instance);
                ScopeLifestyleModule.RegisterForSessionEviction(this, name, instance);
            }

            return _sessionScope[name];
        }

        public override void Release(object instance)
        {
            // Since this method is called by the kernel when an external
            // request to release the component is made, it must do nothing
            // to ensure the component is available during the duration of 
            // the web session.  An internal Evict method is provided to
            // allow the actual releasing of the component at the end of
            // the web session.
        }

        public void Evict(Candidate candidate)
        {
            base.Release(candidate.Component);
            _sessionScope.Remove(candidate.Name);
        }

        public override void Dispose()
        {
        }

        #endregion
    }
}
