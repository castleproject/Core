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
using System.Collections.Generic;
using Castle.Igloo.Util;
using Castle.Igloo.Navigation;

namespace Castle.Igloo.Contexts
{
    /// <summary>
    /// The page context allows you to store state during a request that
    /// renders a page, and access that state from any postback request
    /// that originates from that page.
    /// The state is destroyed at the begining of the second request.
    /// </summary>
    public sealed class PageContext : IContext
    {
        public const string COMPONENT_SUFFIX = "page.context.";

        private IContext _sessionContext = null;
        private NavigationContext _navigationContext = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageContext"/> class.
        /// </summary>
        /// <param name="sessionContest">The session contest.</param>
        /// <param name="navigationContext">The navigationContext context.</param>
        public PageContext(IContext sessionContest, NavigationContext navigationContext)
        {
            AssertUtils.ArgumentNotNull(sessionContest, "sessionContest");
            AssertUtils.ArgumentNotNull(navigationContext, "navigationContext");

            _sessionContext = sessionContest;
            _navigationContext = navigationContext;
        }
        
        #region IContext Members

        /// <summary>
        /// Gets the <see cref="Object"/> with the specified name.
        /// </summary>
        /// <value></value>
        public object this[string name]
        {
            get { return _sessionContext[COMPONENT_SUFFIX+_navigationContext.CurrentView + "." + name]; }
        }

        /// <summary>
        /// Gets the <see cref="Object"/> with the specified type.
        /// </summary>
        /// <value></value>
        public object this[Type clazz]
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the IContext object.
        /// </summary>
        /// <param name="name">The name of the element to add.</param>
        /// <param name="value">The Object to use as the value of the element to add.</param>
        public void Add(string name, object value)
        {
            _sessionContext.Add(COMPONENT_SUFFIX + _navigationContext.CurrentView + "." + name, value);
        }

        /// <summary>
        /// Removes the element with the specified name from the IContext object.
        /// </summary>
        /// <param name="name">The name of the element to remove.</param>
        public void Remove(string name)
        {
            _sessionContext.Remove(COMPONENT_SUFFIX + _navigationContext.CurrentView + "." + name);
        }

        /// <summary>
        /// Determines whether the IDictionary object contains an element with the specified name.
        /// </summary>
        /// <param name="name">The name to locate in the IContext object.</param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return _sessionContext.Contains(COMPONENT_SUFFIX + _navigationContext.CurrentView + "." + name);
        }

        /// <summary>
        /// Gets All the objects names contain in the IContext object.
        /// </summary>
        /// <value>The names.</value>
        public ICollection Names
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// Removes all the element from the IContext object.
        /// </summary>
        public void Flush()
        {
            IEnumerator enumerator = _sessionContext.Names.GetEnumerator();
            IList<string> toRemove = new List<string>();
            
            while ( enumerator.MoveNext() )
            {
                string name = (string) enumerator.Current;
                if (name.StartsWith(COMPONENT_SUFFIX + _navigationContext.PreviousView))
                {
                    toRemove.Add(name);
                }
            }
            foreach(string name in toRemove)
            {
                _sessionContext.Remove(name);
            }
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
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }
}

