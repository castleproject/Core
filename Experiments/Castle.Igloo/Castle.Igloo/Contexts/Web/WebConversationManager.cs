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
using Castle.Igloo.Attributes;
using Castle.Igloo.Util;

namespace Castle.Igloo.Scopes.Web
{
    /// <summary>
    /// Manages conversation context in web context
    /// </summary>
    [Scope(Scope = ScopeType.Request)]
    public class WebConversationManager : IConversationManager
    {
        private const string CONVERSATION_ID = "_CONVERSATION_ID_";
        private Guid _currentConversationId = Guid.Empty;

        //  To Do : ClientScript.RegisterHiddenField("_CONVERSATION_ID_", _currentConversationId.ToString());
        // on page end

        // To Do : Maintains a stack of ConversationId
        
        /// <summary>
        /// Initializes a new instance of the <see cref="WebConversationManager"/> class.
        /// </summary>
        public WebConversationManager()
        {
            if (WebUtil.GetCurrentHttpContext().Request.Params[CONVERSATION_ID]!=null)
            {
                _currentConversationId = new Guid(WebUtil.GetCurrentHttpContext().Request.Params[CONVERSATION_ID]);
            }
        }
        
        #region IConversationManager Members

        /// <summary>
        /// Gets the current conversation id.
        /// </summary>
        /// <value>The current conversation id.</value>
        public Guid CurrentConversationId
        {
            get { return _currentConversationId; }
        }

        /// <summary>
        /// Begins a new conversation.
        /// </summary>
        public void Begin()
        {}

        /// <summary>
        /// Ends the current conversation.
        /// </summary>
        public void End()
        {}
        #endregion
    }
}
