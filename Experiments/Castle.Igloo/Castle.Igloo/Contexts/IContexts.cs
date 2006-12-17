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

using Castle.Igloo.Attributes;

namespace Castle.Igloo.Contexts
{
    public interface IContexts
    {
        /// <summary>
        /// Gets the request context.
        /// </summary>
        /// <value>The request context.</value>
        IContext RequestContext { get; }

        /// <summary>
        /// Gets the application context.
        /// </summary>
        /// <value>The application context.</value>
        IContext ApplicationContext { get; }

        /// <summary>
        /// Gets the session context.
        /// </summary>
        /// <value>The session context.</value>
        IContext SessionContext { get; }
        
        /// <summary>
        /// Gets the page context.
        /// </summary>
        /// <value>The page context.</value>
        IContext PageContext { get; }
        
        /// <summary>
        /// Gets the conversation context.
        /// </summary>
        /// <value>The conversation context.</value>
        IContext ConversationContext { get; }

        /// <summary>
        /// Determines whether the specified component key is in contexts.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the specified key is in contexts; otherwise, <c>false</c>.
        /// </returns>
        bool IsInContexts(string key);

        /// <summary>
        /// Gets object from contexts.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns>The find object or null</returns>
        object GetFromContexts(InjectAttribute attribute);
    }
}
