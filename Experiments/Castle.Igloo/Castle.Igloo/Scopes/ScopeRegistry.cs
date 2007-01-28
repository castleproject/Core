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
using System.Collections.Generic;
using Castle.Igloo.Attributes;
using Castle.Igloo.Util;

namespace Castle.Igloo.Scopes
{
    /// <summary>
    /// Register all <see cref="IScope"/> component
    /// </summary>
    public sealed class ScopeRegistry : IScopeRegistry
    {
        //Map from scope identifier String to corresponding context
        private IDictionary<string, IScope> _scopes = new Dictionary<string, IScope>();

        /// <summary>
        ///  Register the given scope with hisgiven Scope implementation.
        /// </summary>
        /// <param name="scopeName">The scope identifier, must be unique</param>
        /// <param name="scope">The <see cref="IScope"/> implementation</param>
        public void RegisterScope(string scopeName, IScope scope)
        {
            AssertUtils.ArgumentNotNull(scopeName, "Scope identifier must not be null");
            AssertUtils.ArgumentNotNull(scope, "scope must not be null");

            if (_scopes.ContainsKey(scopeName) || ScopeType.UnSpecified.Equals(scopeName))
            {
                throw new ArgumentException("Cannot replace existing scopes : " + scopeName);
            }
            _scopes.Add(scopeName, scope);
        }

        /// <summary>
        /// Determines whether the specified component key is in a <see cref="IScope"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the specified key is in a <see cref="IScope"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInScopes(string key)
        {
            foreach (KeyValuePair<string, IScope> kvp in _scopes)
            {
                if (kvp.Value.Contains(key))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets object from the <see cref="IScope"/>.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns>The find object or null</returns>
        public object GetFromScopes(InjectAttribute attribute)
        {
            if (attribute.Scope != ScopeType.UnSpecified)
            {
                IScope scope = _scopes[attribute.Scope];
                if (scope.IsActive)
                {
                    object instance = scope[attribute.Name];
                    if (instance != null)
                    {
                        TraceUtil.Log("found in scope : " + attribute.Name);
                        return instance;
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, IScope> kvp in _scopes)
                {
                    IScope scope = kvp.Value;
                    if (scope.IsActive)
                    {
                        object instance = scope[attribute.Name];
                        if (instance != null)
                        {
                            TraceUtil.Log("found in scope : " + attribute.Name);
                            return instance;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the <see cref="IScope"/> with the specified name.
        /// </summary>
        /// <value>A <see cref="IScope"/></value>
        public IScope this[string scopeName]
        {
            get { return _scopes[scopeName]; }
        }
    }
}
