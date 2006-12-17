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
using System.Collections.Specialized;
using System.Web;

namespace Castle.Igloo.Contexts.Web
{
    /// <summary>
    /// Provides access to application-state values in web context.
    /// </summary>
    public class WebApplicationState : IApplicationState
    {
        private HttpApplicationState _applicationState = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApplicationState"/> class.
        /// </summary>
        /// <param name="applicationState">State of the application.</param>
        public WebApplicationState(HttpApplicationState applicationState)
        {
            _applicationState = applicationState;
        }
        
        #region IApplicationState Members

        /// <summary>
        /// Gets or sets the <see cref="Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        public object this[string key]
        {
            get { return _applicationState[key]; }
            set { _applicationState[key] = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Object"/> at the specified index.
        /// </summary>
        /// <value></value>
        public object this[int index]
        {
            get { return _applicationState[index]; }
            set { throw new NotImplementedException("Indexer cannot be assigned to, it's read only"); }

        }

        /// <summary>
        /// Gets a collection of the keys of all values stored in the application state.
        /// </summary>
        /// <value>The keys.</value>
        public NameObjectCollectionBase.KeysCollection Keys
        {
            get { return _applicationState.Keys; }
        }

        /// <summary>
        /// Adds a new item to application state.
        /// </summary>
        /// <param name="name">The item name.</param>
        /// <param name="value">The value.</param>
        public void Add(string name, object value)
        {
            _applicationState.Add(name, value);
        }

        /// <summary>
        /// Clears all values from application state.
        /// </summary>
        public void Clear()
        {
            _applicationState.Clear();
        }

        /// <summary>
        /// Deletes an item from the application-state collection.
        /// </summary>
        /// <param name="name">The item name.</param>
        public void Remove(string name)
        {
            _applicationState.Remove(name);
        }

        /// <summary>
        /// Removes all objects from an application-state collection.
        /// </summary>
        public void RemoveAll()
        {
            _applicationState.RemoveAll();
        }

        #endregion
    }
}
