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

            if (!scopeAttribute.UseProxy)
            {
                return GetKernelInstance(context, scope, component);
                 
            }
            else
            {
                // If the Kernel want to inject a component that, for the sake of argument is scoped at the HTTP request scope, 
                // into another singleton component scope, the Kernel will need to inject a proxy in place 
                // of the request scoped compoenent. 
                // That is to say, the Kernel need to inject a proxy object that exposes the same public interface as the scoped object, 
                // but that is smart enough to be able to retrieve the real,target object from the relevant scope 
                // (for example a HTTP request) and delegate method calls onto the real object.

                // To do

                /// When using a proxy scoped component, a proxy will be created for every reference to the scoped component. 
                /// The proxy will determine the actual instance it will point to based on the context in which the component is called.

                // Created proxies are singletons and may be injected, with transparent scoping behavior.

                // Must return a proxy generated as in TestProxyScopeIdea if the call not come from the poxiefied component
                // else
                // must delegate to the Kernel
                // Need to find a way to distinct which I must return

                return GetKernelInstance(context, scope, component);

            }
        }

        /// <summary>
        /// Gets an instance from the kernel.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="component">The component.</param>
        /// <returns></returns>
        private object GetKernelInstance(CreationContext context, IScope scope, ComponentModel component)
        {
            object instance = scope[component.Name];

            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            //for (int i = 0; i < st.FrameCount;i++ )
            //{
            //    Console.WriteLine(		st.GetFrame(i).GetMethod() );
            //}

            if (instance == null)
            {
                scope.CheckInitialisation();

                instance = base.Resolve(context);

                scope.Add(component.Name, instance);
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
