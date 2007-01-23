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

using System.Diagnostics;

namespace Castle.Igloo.Util
{
    /// <summary>
    /// Utils for diganostic
    /// </summary>
    public class TraceUtil
    {
        /// <summary>
        /// Logs the specified message to trace log in debug mode or if trave is enabled
        /// </summary>
        /// <param name="message">The message.</param>
        [Conditional("DEBUG"), Conditional("TRACE")]
        public static void Log(string message)
        {
            Trace.WriteLine(message);
        }
    }
}
