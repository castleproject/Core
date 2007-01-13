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
    public class ScopeType 
    {
        /// <summary>
        /// Constant identifying the singleton scope.
        /// Scopes a single component model to a single object instance per Castle IoC container.
        /// </summary>
        public const string Singleton = "Singleton";

        /// <summary>
        /// Constant identifying the transient scope.
        /// Scopes a single component model to any number of object instances.
        /// i.e. : For each demand the container will create a new instance 
        /// </summary>
        public const string Transient = "Transient";

        /// <summary>
        /// Constant identifying the thread scope.
        /// Scopes a single component model to the lifecycle of a thread; 
        /// </summary>
        public const string Thread = "Thread";

        /// <summary>
        /// Constant identifying the application scope.
        /// Scopes a single component definition to the lifecycle of an ASP.NET application; 
        /// </summary>
        /// <remarks>
        /// Only valid in web context.
        /// </remarks>
        public const string Application = "Application";
        
        /// <summary>
        /// Constant identifying the conversation scope. Spans multiple requests from
        /// the same browser window, demarcated by Begin and End methods.
        /// A conversation scope is propagated by any web request,
        /// or by any request that specifies a conversation id as a request parameter. 
        /// </summary>
        public const string Conversation = "Conversation";
        
        /// <summary>
        /// Constant identifying the page scope. 
        /// Scopes a single component model to the lifecycle of a aspx page.
        /// 
        /// Begins during the first invocation of a page, and lasts 
        /// until the beginning of any invocation of an http request originating from that page.
        /// </summary>
        /// <remarks>
        /// Only valid in web context.
        /// </remarks>        
        public const string Page = "Page";
        
        /// <summary>
        /// Constant identifying the request scope. 
        /// Scopes a single component model to the lifecycle of a single HTTP request; 
        /// i.e. : Each and every HTTP request will have its own instance of a component created by the container. 
        /// The request scope is not propagate.
        /// </summary>    
        /// <remarks>
        /// Only valid in web context.
        /// </remarks>
        public const string Request = "Request";
        
        /// <summary>
        /// Constant identifying the session scope.
        /// Scopes a single component model to the lifecycle of a HTTP Session.
        /// i.e. : each and every HTTP session will have its own instance of a component created by the container. 
        /// </summary>
        /// <remarks>
        /// Only valid in web context.
        /// </remarks>
        public const string Session = "Session";

        /// <summary>
        /// Constant indicates that the scope is implied.
        /// </summary>
        public const string UnSpecified = "UnSpecified";

    }
}
