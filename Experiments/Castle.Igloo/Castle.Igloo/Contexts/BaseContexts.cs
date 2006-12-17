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

using Castle.Igloo.Util;
using Castle.Igloo.Attributes;
using Castle.Igloo.Navigation;

namespace Castle.Igloo.Contexts
{         
    /// <summary>
    /// Base class to IContexts implementation
    /// </summary>
    public abstract class BaseContexts : IContexts
    {       
        private IContext _applicationContext = null;        
        private IContext _requestContext = null;
        private IContext _sessionContext = null;
        private IContext _pageContext = null;
        private IContext _conversationContext = null;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Contexts"/> class.
        /// </summary>
        /// <param name="conversationManager">The conversation manager.</param>
        /// <param name="processContext">The process context.</param>
        public BaseContexts(IConversationManager conversationManager, IProcessContext processContext)
        {
            AssertUtils.ArgumentNotNull(conversationManager, "conversationManager");
            AssertUtils.ArgumentNotNull(processContext, "processContext");

            _requestContext = new RequestContext(processContext);
            NavigationContext navigationContext = new NavigationContext();
            _requestContext.Add(NavigationContext.NAVIGATION_CONTEXT, navigationContext);
            Messages messages = new Messages();
            _requestContext.Add(Messages.REQUEST_MESSAGES, messages);

            _sessionContext = new SessionContext(processContext);
            _pageContext = new PageContext(_sessionContext, navigationContext);

            _applicationContext = new ApplicationContext(processContext.ApplicationState);
            _conversationContext = new ConversationContext(conversationManager, _sessionContext);
        }
        
        #region IContexts Members

        /// <summary>
        /// Gets the application context.
        /// </summary>
        /// <value>The application context.</value>
        public IContext ApplicationContext
        {
            get { return _applicationContext; }
        }

        /// <summary>
        /// Gets the request context.
        /// </summary>
        /// <value>The request context.</value>
        public IContext RequestContext
        {
            get { return _requestContext; }
        }

        /// <summary>
        /// Gets the session context.
        /// </summary>
        /// <value>The session context.</value>
        public IContext SessionContext
        {
            get { return _sessionContext; }

        }

        /// <summary>
        /// Gets the page context.
        /// </summary>
        /// <value>The page context.</value>
        public IContext PageContext
        {
            get { return _pageContext; }
        }

        /// <summary>
        /// Gets the conversation context.
        /// </summary>
        /// <value>The conversation context.</value>
        public IContext ConversationContext
        {
            get { return _conversationContext; }
        }

        /// <summary>
        /// Determines whether the specified component key is in contexts.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the specified key is in contexts; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInContexts(string key)
        {
            return ApplicationContext.Contains(key) ||
                RequestContext.Contains(key) ||
                SessionContext.Contains(key) ||
                PageContext.Contains(key);//||
            //ConversationContext.Contains(key);
        }

        /// <summary>
        /// Gets object from contexts.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns>The find object or null</returns>
        public object GetFromContexts(InjectAttribute attribute)
        {
            if (attribute.Scope == ScopeType.Application)
            {
                return ApplicationContext[attribute.Name];
            }
            else if (attribute.Scope == ScopeType.Session)
            {
                return SessionContext[attribute.Name];
            }
            else if (attribute.Scope == ScopeType.Request)
            {
                return RequestContext[attribute.Name];
            }
            else if (attribute.Scope == ScopeType.Page)
            {
                return PageContext[attribute.Name];
            }
            //else if (attribute.Scope == ScopeType.Conversation)
            //{
            //    return ConversationContext[attribute.Name];
            //}
            else if (attribute.Scope == ScopeType.UnSpecified)
            {
                if (SessionContext.Contains(attribute.Name))
                {
                    return SessionContext[attribute.Name];
                }
                else if (RequestContext.Contains(attribute.Name))
                {
                    return RequestContext[attribute.Name];
                }
                else if (PageContext.Contains(attribute.Name))
                {
                    return PageContext[attribute.Name];
                }
                else if (ApplicationContext.Contains(attribute.Name))
                {
                    return ApplicationContext[attribute.Name];
                }
                //else if (ConversationContext.Contains(attribute.Name))
                //{
                //    return ConversationContext[attribute.Name];
                //}
            }

            return null;
        }

        #endregion
    }
}
