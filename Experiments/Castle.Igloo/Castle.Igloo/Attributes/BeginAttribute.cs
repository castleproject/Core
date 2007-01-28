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

namespace Castle.Igloo.Attributes
{
    /// <summary>
    /// Marks a method as beginning a conversation. 
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Method)]
    public class BeginAttribute : Attribute
    {
        private bool _nested = false;

        /// <summary>
        /// If enabled, and if a conversation is already active,
        /// begin a nested conversation, instead of continuing
        /// in the context of the existing conversation.
        /// </summary>
        /// <value><c>true</c> if nested; otherwise, <c>false</c>.</value>
        public bool Nested
        {
            get { return _nested; }
            set { _nested = value; }
        }
    }
}
