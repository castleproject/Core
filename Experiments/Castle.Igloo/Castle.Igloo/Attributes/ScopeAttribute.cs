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
using Castle.Igloo.Scopes;

namespace Castle.Igloo.Attributes
{
    /// <summary>
    /// Specifies the scope (context) of a component.
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ScopeAttribute : Attribute 
    {
        private string _scope = ScopeType.UnSpecified;
        private bool _proxy = false;

        /// <summary>
        /// The component scope.
        /// </summary>
        public string Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }

        /// <summary>
        /// Some scope like <see cref="ISessionScope"/> are not always available.
        /// When using such a scoped comoposent, a proxy will be created for every reference to the scoped component. 
        /// (The proxy will determine the actual instance it will point to based on the scope in which the component is called.)
        /// This property specifies if the container must create a proxy for the component.
        /// </summary>
        /// <remarks>
        /// You do not need to set to true with components that are not web scoped as <see cref="ISessionScope"/>....
        /// </remarks>
        public bool Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }
    }
}
