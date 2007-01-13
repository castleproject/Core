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

namespace Castle.Igloo.Scopes
{
    /// <summary>
    /// Register all <see cref="IScope"/> component
    /// </summary>
    public interface IScopeRegistry
    {
        /// <summary>
        ///  Register the given scope with hisgiven Scope implementation.
        /// </summary>
        /// <param name="scopeName">The scope identifier, must be unique</param>
        /// <param name="scope">The <see cref="IScope"/> implementation</param>
        void RegisterScope(string scopeName, IScope scope);

        /// <summary>
        /// Determines whether the specified component key is in a <see cref="IScope"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the specified key is in a <see cref="IScope"/>; otherwise, <c>false</c>.
        /// </returns>
        bool IsInScopes(string key);

        /// <summary>
        /// Gets object from the <see cref="IScope"/>.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns>The find object or null</returns>
        object GetFromScopes(InjectAttribute attribute);

        /// <summary>
        /// Gets the <see cref="IScope"/> with the specified name.
        /// </summary>
        /// <value>A <see cref="IScope"/></value>
        IScope this[string scopeName] { get; }
    }
}