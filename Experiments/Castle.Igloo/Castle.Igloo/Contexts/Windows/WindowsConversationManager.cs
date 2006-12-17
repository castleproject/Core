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

namespace Castle.Igloo.Contexts.Windows
{
    /// <summary>
    ///     /// Manages conversation context in windows context
    /// </summary>
    public class WindowsConversationManager : IConversationManager
    {

        #region IConversationManager Members

        /// <summary>
        /// Gets the current conversation id.
        /// </summary>
        /// <value>The current conversation id.</value>
        public Guid CurrentConversationId
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// Begins a new conversation.
        /// </summary>
        public void Begin()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Ends the current conversation.
        /// </summary>
        public void End()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
