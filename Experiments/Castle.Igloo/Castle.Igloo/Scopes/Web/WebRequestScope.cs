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
using System.Collections.Specialized;
using System.Diagnostics;
using Castle.Igloo.Navigation;
using Castle.Igloo.Util;


namespace Castle.Igloo.Scopes.Web
{
    /// <summary>
    /// Implementation of <see cref="IScope"/> whisch spans an HTTP request; 
    /// </summary>
    public sealed class WebRequestScope : IRequestScope
    {
        public const string REQUEST_SCOPE_SUFFIX = "request.scope.";
        private const string INIT_REQUEST_CONTEXT = "_INIT_REQUEST_CONTEXT_";
        private const string COMPONENT_NAMES = "_COMPONENT_NAMEs_";

        private void InitRequestContext()
        {
            if (!WebUtil.GetCurrentHttpContext().Items.Contains(INIT_REQUEST_CONTEXT))
            {
                NavigationState navigationState = new NavigationState();
                WebUtil.GetCurrentHttpContext().Items.Add(REQUEST_SCOPE_SUFFIX+NavigationState.NAVIGATION_STATE, navigationState);

                WebUtil.GetCurrentHttpContext().Items.Add(COMPONENT_NAMES, new StringCollection());

                WebUtil.GetCurrentHttpContext().Items.Add(INIT_REQUEST_CONTEXT, INIT_REQUEST_CONTEXT);
            }
        }
        
        
        #region IRequestContext Members

        /// <summary>
        /// Gets a value indicating whether this context is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return WebUtil.GetCurrentHttpContext() != null; }
        }
        
        /// <summary>
        /// Gets the <see cref="Object"/> with the specified name.
        /// </summary>
        /// <value></value>
        public object this[string name]
        {
            get
            {
                InitRequestContext();
                return WebUtil.GetCurrentHttpContext().Items[REQUEST_SCOPE_SUFFIX + name];
            }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the IScope object.
        /// </summary>
        /// <param name="name">The name of the element to add.</param>
        /// <param name="value">The Object to use as the value of the element to add.</param>
        public void Add(string name, object value)
        {
            Trace.WriteLine("Add to request scope : " + name);

            InitRequestContext();
            ComponentNames.Add(name);
            WebUtil.GetCurrentHttpContext().Items.Add(REQUEST_SCOPE_SUFFIX + name, value);
        }

        /// <summary>
        /// Removes the element with the specified name from the IScope object.
        /// </summary>
        /// <param name="name">The name of the element to remove.</param>
        public void Remove(string name)
        {
            Trace.WriteLine("Remove from request scope : " + name);

            InitRequestContext();
            ComponentNames.Remove(name);
            WebUtil.GetCurrentHttpContext().Items.Remove(REQUEST_SCOPE_SUFFIX + name);
        }

        /// <summary>
        /// Determines whether the IDictionary object contains an element with the specified name.
        /// </summary>
        /// <param name="name">The name to locate in the IScope object.</param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            InitRequestContext();
            return ComponentNames.Contains(name);
        }

        /// <summary>
        /// Gets All the objects names contain in the IScope object.
        /// </summary>
        /// <value>The names.</value>
        public ICollection Names
        {
            get
            {
                InitRequestContext();
                return ComponentNames;
            }
        }

        /// <summary>
        /// Removes all the elements from the IScope object.
        /// </summary>
        public void Flush()
        {
            Trace.WriteLine("Flush request scope.");
            StringCollection toRemove = new StringCollection();
            StringCollection names = (StringCollection)WebUtil.GetCurrentHttpContext().Items[COMPONENT_NAMES];
            foreach (string name in names)
            {
                WebUtil.GetCurrentHttpContext().Items.Remove(REQUEST_SCOPE_SUFFIX + name);
                toRemove.Remove(name);
            }
            names.Clear();
        }

        /// <summary>
        /// Gets the type of the scope.
        /// </summary>
        /// <value>The type of the scope.</value>
        public string ScopeType
        {
            get
            {
                InitRequestContext();
                return Igloo.ScopeType.Request;
            }
        }

        #endregion

        private StringCollection ComponentNames
        {
            get
            {
                StringCollection names = (StringCollection)WebUtil.GetCurrentHttpContext().Items[COMPONENT_NAMES];
                if (names == null)
                {
                    names = new StringCollection();
                    WebUtil.GetCurrentHttpContext().Items.Add(COMPONENT_NAMES, names);
                }
                return names;
            }
        }
    }
}
