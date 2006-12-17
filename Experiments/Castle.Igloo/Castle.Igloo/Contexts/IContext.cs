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

namespace Castle.Igloo.Contexts
{
    /// <summary>
    /// A set of named components and items of data that
    /// is associated with a particular context.
    /// </summary>
    public interface IContext
    {

        /// <summary>
        /// Gets the <see cref="Object"/> with the specified name.
        /// </summary>
        /// <value></value>
        object this[string name] { get; }
        
        /// <summary>
        /// Gets the <see cref="Object"/> with the specified type.
        /// </summary>
        /// <value></value>
        object this[Type clazz] { get; }
        
        /// <summary>
        /// Adds an element with the provided key and value to the IContext object. 
        /// </summary>
        /// <param name="name">The name of the element to add.</param>
        /// <param name="value">The Object to use as the value of the element to add.</param>
        void Add(string name, Object value);
        
        /// <summary>
        /// Removes the element with the specified name from the IContext object. 
        /// </summary>
        /// <param name="name">The name of the element to remove. </param>
        void Remove(string name);
        
        /// <summary>
        /// Determines whether the IDictionary object contains an element with the specified name. 
        /// </summary>
        /// <param name="name">The name to locate in the IContext object.</param>
        /// <returns></returns>
        bool Contains(string name);
        
        /// <summary>
        /// Gets All the objects names contain in the IContext object.
        /// </summary>
        /// <value>The names.</value>
        ICollection Names { get; }
        
        /// <summary>
        /// Removes all the elements from the IContext object. 
        /// </summary>
        void Flush();

        /// <summary>
        /// Abandons the current session.
        /// </summary>
        void Abandon();
        
        /// <summary>
        /// Gets the type of the scope.
        /// </summary>
        /// <value>The type of the scope.</value>
        ScopeType ScopeType { get; }
    }
}
