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
using System.Collections.Specialized;

namespace Castle.Igloo.Contexts
{
    /// <summary>
    /// Provides access to session-state values.
    /// </summary>
    public interface ISessionState : ICollection, IEnumerable
    {
        /// <summary>
        /// Gets or sets the <see cref="Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        object this[string key]{get; set;}

        /// <summary>
        /// Gets or sets the <see cref="Object"/> at the specified index.
        /// </summary>
        /// <value></value>
        object this[int index] {get; set;}

        /// <summary>
        /// Gets a collection of the keys of all values stored in the session.
        /// </summary>
        /// <value>The keys.</value>
        NameObjectCollectionBase.KeysCollection Keys { get;}

        /// <summary>
        /// Adds a new item to session state.
        /// </summary>
        /// <param name="name">The item name.</param>
        /// <param name="value">The value.</param>
        void Add( string name, object value );
        
        /// <summary>
        /// Clears all values from session state.
        /// </summary>
        void Clear();
        
        /// <summary>
        /// Abandons the current session.
        /// </summary>
        void Abandon();

        /// <summary>
        /// Deletes an item from the session-state collection.
        /// </summary>
        /// <param name="name">The item name.</param>
        void Remove( string name );
    }
}
