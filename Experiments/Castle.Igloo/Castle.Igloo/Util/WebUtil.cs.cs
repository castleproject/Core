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
using System.Web;

namespace Castle.Igloo.Util
{
    /// <summary>
    /// Web utils
    /// </summary>
    public static class WebUtil
    {
        /// <summary>
        /// Gets the current HTTP context.
        /// </summary>
        /// <returns></returns>
        public static HttpContext GetCurrentHttpContext()
        {
            HttpContext httpContext = HttpContext.Current;

            if (httpContext == null)
            {
                throw new InvalidOperationException("HttpContext.Current is null.");
            }
            return httpContext;
        }
    }
}
