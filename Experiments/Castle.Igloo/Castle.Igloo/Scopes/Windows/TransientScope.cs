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
using Castle.Core;
using Castle.Igloo.Scopes;
using Castle.MicroKernel;

namespace Castle.Igloo.Scopes.Windows
{
    public class TransientScope :IScope
    {
        #region IScope Members

        /// <summary>
        /// Gets a value indicating whether this context is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the <see cref="Object"/> with the specified name.
        /// </summary>
        /// <value></value>
        public object this[string name]
        {
            get { return null; }
        }

        public void Add(string name, object value)
        {
        }

        public void Remove(string name)
        {
        }

        public bool Contains(string name)
        {
            return false;
        }

        public ICollection Names
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void Flush()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Registers for eviction.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="componentModel">The componentModel.</param>
        /// <param name="instance">The instance.</param>
        public void RegisterForEviction(ILifestyleManager manager, ComponentModel componentModel, object instance)
        {
        }

        /// <summary>
        /// Checks the initialisation.
        /// </summary>
        public void CheckInitialisation()
        {
        }

        /// <summary>
        /// Gets the type of the scope.
        /// </summary>
        /// <value>The type of the scope.</value>
        public string ScopeType
        {
            get { return Igloo.ScopeType.Transient; }
        }

        #endregion
    }
}
