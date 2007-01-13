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
using Castle.Igloo.LifestyleManager;
using Castle.Igloo.Scopes;
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
    public class ScopeLifestyleManager : AbstractLifestyleManager
    {

        #region ILifestyleManager Members

        /// <summary>
        /// Find a component by name in scoped storage implementation. 
        /// If not found, try to instantiate new one by the <see cref="IKernel"/>.
        /// Then found component will be attached to scope store implementation.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The component</returns>
        public override object Resolve(CreationContext context)
        {
            ComponentModel component = (ComponentActivator as AbstractComponentActivator).Model;
            string scopeName = (string)component.ExtendedProperties[ScopeInspector.SCOPE_TOKEN];

            IScope scope = (IScope)Kernel[scopeName];

            object instance = scope[component.Name];

            if (instance == null)
            {
                scope.CheckInitialisation();

                instance = base.Resolve(context);
                
                scope.Add(component.Name, instance);
                scope.RegisterForEviction(this, component, instance);
            }
            
            return instance;
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
            base.Release(candidate.Intance);

            string scopeName = (string)candidate.ComponentModel.ExtendedProperties[ScopeInspector.SCOPE_TOKEN];

            IScope scope = (IScope)Kernel[scopeName];

            scope.Remove(candidate.ComponentModel.Name);
        }

        public override void Dispose()
        {
        }

        #endregion
    }
}
