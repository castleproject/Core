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
using Castle.Igloo.Attributes;

namespace Castle.Igloo.Scopes.Web
{
    /// <summary>
    /// A conversation context is a logical context that lasts longer than 
    /// a request but shorter than a login session.
    /// </summary>
    //[Scope(Scope = ScopeType.Application)]
    public sealed class WebConversationScope : IScope
    {
        private IConversationManager _conversationManager = null;
        private ISessionScope _sessionScope = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebConversationScope"/> class.
        /// </summary>
        /// <param name="conversationManager">The conversation manager.</param>
        /// <param name="sessionScope">The session context.</param>
        public WebConversationScope(IConversationManager conversationManager, ISessionScope sessionScope)
        {
            _conversationManager = conversationManager;
            _sessionScope = sessionScope;
        }

        #region IScope Members

        /// <summary>
        /// Gets a value indicating whether this context is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return false; }
        }
        
        /// <summary>
        /// Gets the <see cref="Object"/> with the specified name.
        /// </summary>
        /// <value></value>
        public object this[string name]
        {
            get { return _sessionScope[_conversationManager.CurrentConversationId + "." + name]; }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the IScope object.
        /// </summary>
        /// <param name="name">The name of the element to add.</param>
        /// <param name="value">The Object to use as the value of the element to add.</param>
        public void Add(string name, object value)
        {
            _sessionScope.Add(_conversationManager.CurrentConversationId + "." + name, value);
        }

        /// <summary>
        /// Removes the element with the specified name from the IScope object.
        /// </summary>
        /// <param name="name">The name of the element to remove.</param>
        public void Remove(string name)
        {
            _sessionScope.Remove(_conversationManager.CurrentConversationId + "." + name);
        }

        /// <summary>
        /// Determines whether the IDictionary object contains an element with the specified name.
        /// </summary>
        /// <param name="name">The name to locate in the IScope object.</param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return _sessionScope.Contains(_conversationManager.CurrentConversationId + "." + name);
        }

        /// <summary>
        /// Gets All the objects names contain in the IScope object.
        /// </summary>
        /// <value>The names.</value>
        public ICollection Names
        {
            get { throw new Exception("TO DO."); }
        }

        /// <summary>
        /// Removes all the elementfrom the IScope object.
        /// </summary>
        public void Flush()
        {
            throw new Exception("TO DO.");
        }



        /// <summary>
        /// Gets the type of the scope.
        /// </summary>
        /// <value>The type of the scope.</value>
        public string ScopeType
        {
            get { return Igloo.ScopeType.Conversation; }
        }

        #endregion
    }
}
