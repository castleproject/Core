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
using System.Collections;
using Castle.MicroKernel;

namespace Castle.Igloo.Scopes
{
    /// <summary>
    /// A set of named components and items of data that
    /// is associated with a particular scope.
    /// 
    /// Used by the by the <see cref="IKernel"/> to hold components in.
    /// <p>Provides the ability to add and get objects from whatever underlying
    /// storage mechanism, such as HTTP session or request. 
    /// 
    /// <p><code>Scope</code> implementations are expected to be thread-safe.</p>
    /// 
    /// </summary>
    public interface IScope
    {
        /// <summary>
        /// Gets a value indicating whether this context is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        bool IsActive { get; }
        
        /// <summary>
        /// Gets the <see cref="Object"/> with the specified name.
        /// </summary>
        /// <value></value>
        object this[string name] { get; }
        
        /// <summary>
        /// Adds an element with the provided key and value to the IScope object. 
        /// </summary>
        /// <param name="name">The name of the element to add.</param>
        /// <param name="value">The Object to use as the value of the element to add.</param>
        void Add(string name, Object value);
        
        /// <summary>
        /// Removes the element with the specified name from the IScope object. 
        /// </summary>
        /// <param name="name">The name of the element to remove. </param>
        void Remove(string name);
        
        /// <summary>
        /// Determines whether the IDictionary object contains an element with the specified name. 
        /// </summary>
        /// <param name="name">The name to locate in the IScope object.</param>
        /// <returns></returns>
        bool Contains(string name);
        
        /// <summary>
        /// Gets All the objects names contain in the IScope object.
        /// </summary>
        /// <value>The names.</value>
        ICollection Names { get; }
        
        /// <summary>
        /// Removes all the elements from the IScope object. 
        /// </summary>
        void Flush();
        
        /// <summary>
        /// Gets the type of the scope.
        /// </summary>
        /// <value>The type of the scope.</value>
        string ScopeType { get; }
    }
}
