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
using Castle.Igloo.Util;

namespace Castle.Igloo.Contexts
{
    public sealed class ApplicationContext : IContext
    {
        private IApplicationState _applicationState = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationContext"/> class.
        /// </summary>
        /// <param name="applicationState">State of the application.</param>
        public ApplicationContext(IApplicationState applicationState)
        {
            AssertUtils.ArgumentNotNull(applicationState, "applicationState");

            _applicationState = applicationState;
        }
        
        #region IContext Members

        /// <summary>
        /// Gets the <see cref="Object"/> with the specified name.
        /// </summary>
        /// <value></value>
        public object this[string name]
        {
            get { return _applicationState[name]; }
        }

        /// <summary>
        /// Gets the <see cref="Object"/> with the specified clazz.
        /// </summary>
        /// <value></value>
        public object this[Type clazz]
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// Add the specified object under the name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void Add(string name, object value)
        {
            _applicationState.Add(name, value);
        }

        /// <summary>
        /// Removes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Remove(string name)
        {
            _applicationState.Remove(name);
        }


        /// <summary>
        /// Determines whether [contains] [the specified name].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified name]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string name)
        {
            return (_applicationState[name] != null);
        }

        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <value>The names.</value>
        public ICollection Names
        {
            get { return _applicationState.Keys; }
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public void Flush()
        {
            _applicationState.RemoveAll();
        }

        /// <summary>
        /// Abandons the current session.
        /// </summary>
        public void Abandon()
        {
            Flush();
        }
        
        /// <summary>
        /// Gets the type of the scope.
        /// </summary>
        /// <value>The type of the scope.</value>
        public ScopeType ScopeType
        {
            get { return ScopeType.Application; }
        }

        #endregion
    }
}
