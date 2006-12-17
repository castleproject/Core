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

namespace Castle.Igloo.Attributes
{
    /// <summary>
    /// Specifies the scope (context) of a component.
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ScopeAttribute : Attribute 
    {
        private ScopeType _scope = ScopeType.UnSpecified;

        /// <summary>S
        /// The component scope.
        /// </summary>
        public ScopeType Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }
    }
}
