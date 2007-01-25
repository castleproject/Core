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
using Castle.Igloo.Attributes;
using Castle.Igloo.LifestyleManager;
using Castle.Igloo.Scopes;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;
using Castle.MicroKernel.Lifestyle;

namespace Castle.Igloo.LifestyleManager
{
    /// <summary>
    /// ScopeLifestyleManager tries to lookup component by name in the scope. 
    /// If not found, it tries to instantiate new component and register it in its scope.
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
            ComponentModel component = ((AbstractComponentActivator)ComponentActivator).Model;
            ScopeAttribute scopeAttribute = (ScopeAttribute)component.ExtendedProperties[ScopeInspector.SCOPE_ATTRIBUTE];

            IScope scope = (IScope)Kernel[scopeAttribute.Scope];

            object instance = scope[component.Name];

            if (instance == null)
            {
                scope.CheckInitialisation();

                instance = base.Resolve(context);

                scope[component.Name] = instance;
                scope.RegisterForEviction(this, component, instance);
            }

            return instance; 
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
            // the web session.  An internal Evict method is provided to
            // allow the actual releasing of the component at the end of
            // the web session.
        }

        /// <summary>
        /// Evicts the specified candidate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        public void Evict(Candidate candidate)
        {
            base.Release(candidate.Intance);

            ScopeAttribute scopeAttribute = (ScopeAttribute)candidate.ComponentModel.ExtendedProperties[ScopeInspector.SCOPE_ATTRIBUTE];

            IScope scope = (IScope)Kernel[scopeAttribute.Scope];

            scope.Remove(candidate.ComponentModel.Name);
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
