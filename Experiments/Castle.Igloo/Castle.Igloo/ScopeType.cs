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

namespace Castle.Igloo
{
    /// <summary>
    /// Enumeration of the differents scope
    /// </summary>
    public enum ScopeType : int
    {
        /// <summary>
        /// The request context. Spans a http request.
        /// The request scope is not propagate.
        /// </summary>
        Request,
        /// <summary>
        /// The page context. Begins during the first invocation of a page, and lasts 
        /// until the beginning of any invocation of an http request originating from that page.
        /// </summary>
        Page,
        /// <summary>
        /// The conversation context. Spans multiple requests from
        /// the same browser window, demarcated by Begin and End
        /// methods. A conversation context is propagated by
        /// any web request, or by any request that specifies
        /// a conversation id as a request parameter. 
        /// </summary>
        Conversation,
        /// <summary>
        /// The session context. (An Http session.)
        /// </summary>
        Session,
        /// <summary>
        ///  The application context (HttpApplication context.)
        /// </summary>
        Application,
        /// <summary>
        /// Indicates that the scope is implied.
        /// </summary>
        UnSpecified
    }
}
