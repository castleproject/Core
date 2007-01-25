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
using System.Configuration;
using Castle.Core;
using Castle.Igloo.LifestyleManager;
using Castle.Igloo.Util;
using Castle.MicroKernel;

namespace Castle.Igloo.Scopes.Web
{
    /// <summary>
    /// Represents a session context, acts as a proxy to the <see cref="ISessionScope"/>.
    /// </summary>
    //[Scope(Scope = ScopeType.Application)]
    public sealed class WebSessionScope : ISessionScope
    {
        public const string SESSION_SCOPE_SUFFIX = "session.scope.";
        public const string SESSION_INVALID = "_SESSION_INVALID_";
        private const string COMPONENT_NAMES = "_COMPONENT_NAMES_";



        #region ISessionContext Members

        /// <summary>
        /// Gets a value indicating whether this context is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return WebUtil.GetCurrentHttpContext().Session != null; }
        }
        
        /// <summary>
        /// Gets or sets the <see cref="Object"/> with the specified name.
        /// </summary>
        /// <value></value>
        public object this[string name]
        {
            get
            {
                TraceUtil.Log("Gets to session scope : " + name);
                return WebUtil.GetCurrentHttpContext().Session[ SESSION_SCOPE_SUFFIX + name];
            }
            set
            {
                TraceUtil.Log("Sets to session scope : " + name);
                if (!ComponentNames.Contains(name))
                {
                    ComponentNames.Add(name);
                }
                WebUtil.GetCurrentHttpContext().Session[SESSION_SCOPE_SUFFIX + name] = value;
            }
        }

        /// <summary>
        /// Removes the element with the specified name from the IScope object.
        /// </summary>
        /// <param name="name">The name of the element to remove.</param>
        public void Remove(string name)
        {
            TraceUtil.Log("Remove from session scope : " + name);
            ComponentNames.Remove(name);
            WebUtil.GetCurrentHttpContext().Session.Remove(SESSION_SCOPE_SUFFIX + name);
        }

        /// <summary>
        /// Determines whether the IDictionary object contains an element with the specified name.
        /// </summary>
        /// <param name="name">The name to locate in the IScope object.</param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return ComponentNames.Contains(name);
        }

        /// <summary>
        /// Gets All the objects names contain in the IScope object.
        /// </summary>
        /// <value>The names.</value>
        public ICollection Names
        {
            get { return ComponentNames; }
        }

        /// <summary>
        /// Removes all the element from the IScope object.
        /// </summary>
        public void Flush()
        {
            TraceUtil.Log("Flush session scope.");

            StringCollection toRemove = new StringCollection();
            StringCollection names = (StringCollection)WebUtil.GetCurrentHttpContext().Items[COMPONENT_NAMES];
            foreach (string name in names)
            {
                WebUtil.GetCurrentHttpContext().Session.Remove(SESSION_SCOPE_SUFFIX + name);
                toRemove.Remove(name);
            }
            names.Clear();
        }

        /// <summary>
        /// Registers for eviction.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="model">The ComponentModel.</param>
        /// <param name="instance">The instance.</param>
        public void RegisterForEviction(ILifestyleManager manager, ComponentModel model, object instance)
        {
            ScopeLifestyleModule.RegisterForSessionEviction((ScopeLifestyleManager)manager, model, instance);
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
        /// Abandons the current session.
        /// </summary>
        public void Abandon()
        {
            // Mark Session as invalid
            WebUtil.GetCurrentHttpContext().Session.Add(SESSION_INVALID, true);
        }

        /// <summary>
        /// Gets the type of the scope.
        /// </summary>
        /// <value>The type of the scope.</value>
        public string ScopeType
        {
            get { return Igloo.ScopeType.Session; }
        }

        #endregion

        private StringCollection ComponentNames
        {
            get
            {
                StringCollection names = (StringCollection)WebUtil.GetCurrentHttpContext().Session[COMPONENT_NAMES];
                if (names == null)
                {
                    names = new StringCollection();
                    WebUtil.GetCurrentHttpContext().Session.Add(COMPONENT_NAMES, names);
                }
                return names;
            }
        }
    }
}