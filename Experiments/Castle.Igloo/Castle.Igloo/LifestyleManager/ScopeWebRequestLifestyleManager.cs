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
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;
using Castle.MicroKernel.Lifestyle;

namespace Castle.Igloo.LifestyleManager
{
    /// <summary>
    /// Implements a Lifestyle Manager for Web Apps that
    /// create at most one object per web request.
    /// </summary>
    [Serializable]
    public sealed class ScopeWebRequestLifestyleManager : AbstractLifestyleManager
    {
        
        #region ILifestyleManager Members

        /// <summary>
        /// Resolves the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override object Resolve(CreationContext context)
        {
            HttpContext current = HttpContext.Current;

            if (current == null)
            {
                throw new InvalidOperationException("HttpContext.Current is null.  PerWebRequestLifestyle can only be used in ASP.Net");
            }
            
            string name = (ComponentActivator as AbstractComponentActivator).Model.Name;
            
            if (current.Items[name] == null)
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
                current.Items[name] = instance;
                ScopeLifestyleModule.RegisterForRequestEviction(this, instance);
            }

            return current.Items[name];
        }

        /// <summary>
        /// Releases the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public override void Release(object instance)
        {
            // Since this method is called by the kernel when an external
            // request to release the component is made, it must do nothing
            // to ensure the component is available during the duration of 
            // the web request.  An internal Evict method is provided to
            // allow the actual releasing of the component at the end of
            // the web request.
        }

        /// <summary>
        /// Evicts the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        internal void Evict(object instance)
        {
            base.Release(instance);
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public override void Dispose()
        {
        }

        #endregion
    }
}

