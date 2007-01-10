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

using System.Collections.Generic;
using Castle.Igloo.Attributes;

namespace Castle.Igloo
{
    /// <summary>
    /// Holds context request messages.
    /// </summary>
    [Scope(Scope = ScopeType.Request)]
    public sealed class FlashMessages : Dictionary<string, string>
    {
        public const string FLASH_MESSAGES = "_FLASH_MESSAGES_";

    }
}
