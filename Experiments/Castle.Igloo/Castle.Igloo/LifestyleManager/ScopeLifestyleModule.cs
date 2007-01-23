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
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Castle.Core;
using Castle.Igloo.Scopes.Web;
using Castle.Igloo.LifestyleManager;
using Castle.Igloo.Util;

namespace Castle.Igloo.LifestyleManager
{
    /// <summary>
    /// 
    /// </summary>
    public class ScopeLifestyleModule : IHttpModule
    {
        private static bool _initialized = false;
        
        private const string PER_REQUEST_EVICT = "_REQUEST_COMPONENTS_TO_EVICT_";
        public const string PER_SESSION_EVICT = "_SESSION_COMPONENTS_TO_ECVICT_";

        #region IHttpModule Members

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"></see>.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication"></see> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            _initialized = true;
            //HttpModuleCollection modules = context.Modules;
            //IHttpModule module = modules["Session"];
            
            //// Attach to session end event
            //if (module.GetType() == typeof(SessionStateModule))
            //{
            //    SessionStateModule stateModule = (SessionStateModule)module;
            //    stateModule.End += new EventHandler(ReleaseSessionScopeComponent);
            //}
            
            // Attach to events
            context.EndRequest += new EventHandler(ReleaseRequestScopeComponent);
            context.PostRequestHandlerExecute += new EventHandler(ReleasePageScopeComponent);
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether this <see cref="ScopeLifestyleModule"/> is initialized.
        /// </summary>
        /// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
        internal static bool Initialized
        {
            get { return _initialized; }
        }

        #region Request scope garbage
        /// <summary>
        /// Registers for request eviction.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="component">The component.</param>
        /// <param name="instance">The instance.</param>
        internal static void RegisterForRequestEviction(
            ScopeLifestyleManager manager,
            ComponentModel component, object instance)
        {
            HttpContext httpContext = HttpContext.Current;

            IDictionary<ScopeLifestyleManager, Candidate> candidates = (Dictionary<ScopeLifestyleManager, Candidate>)httpContext.Items[PER_REQUEST_EVICT];

            if (candidates == null)
            {
                candidates = new Dictionary<ScopeLifestyleManager, Candidate>();
                httpContext.Items[PER_REQUEST_EVICT] = candidates;
            }
            Candidate candidate = new Candidate(component, instance);

            candidates.Add(manager, candidate);
        }

        /// <summary>
        /// Called when request end.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        internal void ReleaseRequestScopeComponent(Object sender, EventArgs e)
        {
            TraceUtil.Log("<<< End web request");
            
            HttpApplication application = (HttpApplication)sender;
            HttpContext httpContext = application.Context;

            IDictionary<ScopeLifestyleManager, Candidate> candidates = (Dictionary<ScopeLifestyleManager, Candidate>)httpContext.Items[PER_REQUEST_EVICT];

            if (candidates != null)
            {
                foreach (KeyValuePair<ScopeLifestyleManager, Candidate> kvp in candidates)
                {
                    ScopeLifestyleManager manager = kvp.Key;
                    manager.Evict(kvp.Value);
                }

                httpContext.Items.Remove(PER_REQUEST_EVICT);
            }

        }
        #endregion

        #region Session scope garbage
        /// <summary>
        /// Registers for session eviction.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="component">The component.</param>
        /// <param name="instance">The instance.</param>
        internal static void RegisterForSessionEviction(
            ScopeLifestyleManager manager,
            ComponentModel component,
            object instance)
        {
            HttpContext context = HttpContext.Current;

            IDictionary<ScopeLifestyleManager, Candidate> candidates = (Dictionary<ScopeLifestyleManager, Candidate>)context.Session[PER_SESSION_EVICT];

            if (candidates == null)
            {
                candidates = new Dictionary<ScopeLifestyleManager, Candidate>();
                context.Session[PER_SESSION_EVICT] = candidates;
            }
            Candidate candidate = new Candidate(component, instance);
            candidates.Add(manager, candidate);
        }
        
        /// <summary>
        /// Called when session end.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        public static void ReleaseSessionScopeComponent(Object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;

            IDictionary<ScopeLifestyleManager, Candidate> candidates = (Dictionary<ScopeLifestyleManager, Candidate>)application.Session[PER_SESSION_EVICT];

            if (candidates != null)
            {
                foreach (KeyValuePair<ScopeLifestyleManager, Candidate> kvp in candidates) 
                {
                    ScopeLifestyleManager manager = kvp.Key;
                    manager.Evict(kvp.Value);
                }

                application.Session.Remove(PER_SESSION_EVICT);
            }
        }
        #endregion

        #region Page scope garbage
        
        /// <summary>
        /// Handles the PostRequestHandlerExecute event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ReleasePageScopeComponent(Object sender, EventArgs e)
        {
            if (IsAspxPage)
            {
                if (!IsSamePage)
                {
                    IPageScope pageScope = ContainerWebAccessorUtil.Container.Resolve<IPageScope>();
                    pageScope.Flush();
                }
                
                // Process end of session
                HttpContext httpContext = HttpContext.Current;
                bool? isSessionInvalid = (bool?)httpContext.Session[WebSessionScope.SESSION_INVALID];

                if ((isSessionInvalid != null) && (isSessionInvalid == true))
                {
                    ReleaseSessionScopeComponent(sender, null);
                    httpContext.Session.Abandon();
                }                
                
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is aspx page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is aspx page; otherwise, <c>false</c>.
        /// </value>
        private bool IsAspxPage
        {
            get
            {
                HttpContext httpContext = WebUtil.GetCurrentHttpContext();
                return httpContext.Handler is Page;
            }
        }

        /// <summary>
        /// Determines if the current Action resulted from a new web page.
        /// </summary>
        private bool IsSamePage
        {
            get
            {
                HttpContext httpContext = WebUtil.GetCurrentHttpContext();
                return ((httpContext.Request.UrlReferrer == null) ||
                         (httpContext.Request.UrlReferrer == httpContext.Request.Url));
            }
        }
        #endregion
        
        
    }
}
