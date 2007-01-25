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
using System.Collections.Specialized;
using System.Configuration;
using Castle.Core;
using Castle.Igloo.LifestyleManager;
using Castle.Igloo.Navigation;
using Castle.Igloo.Util;
using Castle.MicroKernel;

namespace Castle.Igloo.Scopes.Web
{
    /// <summary>
    /// <see cref="IPageScope"/> implementation that scopes a single component model to the lifecycle of a aspx page.
    /// </summary>
    public sealed class WebPageScope : IPageScope
    {
        public const string PAGE_SCOPE_SUFFIX = "page.";

        private IRequestScope _requestScope = null;
        private ISessionScope _sessionScope = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebPageScope"/> class.
        /// </summary>
        /// <param name="sessionContest">The session contest.</param>
        /// <param name="requestScope">The request context.</param>
        public WebPageScope(ISessionScope sessionContest, IRequestScope requestScope)
        {
            AssertUtils.ArgumentNotNull(sessionContest, "sessionContest");
            AssertUtils.ArgumentNotNull(requestScope, "requestScope");

            _sessionScope = sessionContest;
            _requestScope = requestScope;
        }

        /// <summary>
        /// Gets the navigation context.
        /// </summary>
        /// <value>The navigation context.</value>
        private NavigationState NavigationState
        {
            get { return (NavigationState)_requestScope[NavigationState.NAVIGATION_STATE]; }
        }
        
        #region IPageContext Members

        /// <summary>
        /// Gets a value indicating whether this context is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return NavigationState != null; }
        }
               
 
        /// <summary>
        /// Gets or sets the <see cref="Object"/> with the specified name.
        /// </summary>
        /// <value></value>
        public object this[string name]
        {
            get
            {
                TraceUtil.Log("Get from page scope : " + name);

                return _sessionScope[PAGE_SCOPE_SUFFIX + NavigationState.CurrentView + "." + name];
            }
            set
            {
                TraceUtil.Log("Set to page scope : " + name);

                _sessionScope[PAGE_SCOPE_SUFFIX + NavigationState.CurrentView + "." + name] = value;
            }
        }

        /// <summary>
        /// Removes the element with the specified name from the IScope object.
        /// </summary>
        /// <param name="name">The name of the element to remove.</param>
        public void Remove(string name)
        {
            TraceUtil.Log("Remove from page scope : " + name);

            _sessionScope.Remove(PAGE_SCOPE_SUFFIX + NavigationState.CurrentView + "." + name);
        }

        /// <summary>
        /// Determines whether the IDictionary object contains an element with the specified name.
        /// </summary>
        /// <param name="name">The name to locate in the IScope object.</param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return _sessionScope.Contains(PAGE_SCOPE_SUFFIX + NavigationState.CurrentView + "." + name);
        }

        /// <summary>
        /// Gets All the objects names contain in the IScope object.
        /// </summary>
        /// <value>The names.</value>
        public ICollection Names
        {
            get 
            { 
                StringCollection names = new StringCollection();
                foreach(string name in _sessionScope.Names)
                {
                    if (name.StartsWith(PAGE_SCOPE_SUFFIX + NavigationState.CurrentView))
                    {
                        names.Add(name);
                    }
                }
                return names;
            }
        }

        /// <summary>
        /// Removes all the element from the IScope object.
        /// </summary>
        public void Flush()
        {
            TraceUtil.Log("Flush page scope.");

            IEnumerator enumerator = _sessionScope.Names.GetEnumerator();
            IList<string> toRemove = new List<string>();
            
            while ( enumerator.MoveNext() )
            {
                string name = (string) enumerator.Current;
                if (name.StartsWith(PAGE_SCOPE_SUFFIX + NavigationState.PreviousView))
                {
                    toRemove.Add(name);
                }
            }
            foreach(string name in toRemove)
            {
                TraceUtil.Log("Remove from page scope : " + name);
                _sessionScope.Remove(name);
            }
        }

        /// <summary>
        /// Registers for eviction.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="model">The ComponentModel.</param>
        /// <param name="instance">The instance.</param>
        public void RegisterForEviction(ILifestyleManager manager, ComponentModel model, object instance)
        {
        }

        /// <summary>
        /// Checks the initialisation.
        /// </summary>
        public void CheckInitialisation()
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
        }

        /// <summary>
        /// Gets the type of the scope.
        /// </summary>
        /// <value>The type of the scope.</value>
        public string ScopeType
        {
            get { return Igloo.ScopeType.Page; }
        }

        #endregion
    }
}

