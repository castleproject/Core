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
using System.Web;
using System.Web.SessionState;

namespace Castle.Igloo.Contexts.Web
{
    public class WebSessionState : ISessionState
    {
        private HttpSessionState _sessionState = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSessionState"/> class.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        public WebSessionState(HttpContext httpContext)
        {
            _sessionState = httpContext.Session;
        }
        
        #region ISessionState Members

        /// <summary>
        /// Gets or sets the <see cref="Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        public object this[string key]
        {
            get { return _sessionState[key]; }
            set { _sessionState[key] = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Object"/> at the specified index.
        /// </summary>
        /// <value></value>
        public object this[int index]
        {
            get { return _sessionState[index]; }
            set { _sessionState[index] = value; }
        }

        /// <summary>
        /// Gets a collection of the keys of all values stored in the session.
        /// </summary>
        /// <value>The keys.</value>
        public NameObjectCollectionBase.KeysCollection Keys
        {
            get { return _sessionState.Keys; }
        }

        /// <summary>
        /// Adds a new item to session state.
        /// </summary>
        /// <param name="name">The item name.</param>
        /// <param name="value">The value.</param>
        public void Add(string name, object value)
        {
            _sessionState.Add(name, value);
        }

        /// <summary>
        /// Clears all values from session state.
        /// </summary>
        public void Clear()
        {
            _sessionState.Clear();
        }
        
        /// <summary>
        /// Abandons the current session.
        /// </summary>
        public void Abandon()
        {
            _sessionState.Abandon();
        }

        /// <summary>
        /// Deletes an item from the session-state collection.
        /// </summary>
        /// <param name="name">The item name.</param>
        public void Remove(string name)
        {
            _sessionState.Remove(name);
        }

        #endregion

        #region ICollection Members

        /// <summary>
        /// Copies the elements of the <see cref="System.Collections.ICollection"></see> to an <see cref="System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"></see>. The <see cref="System.Array"></see> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">array is null. </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">index is less than zero. </exception>
        /// <exception cref="System.ArgumentException">array is multidimensional.-or- index is equal to or greater than the length of array.-or- The number of elements in the source <see cref="System.Collections.ICollection"></see> is greater than the available space from index to the end of the destination array. </exception>
        /// <exception cref="System.InvalidCastException">The type of the source <see cref="System.Collections.ICollection"></see> cannot be cast automatically to the type of the destination array. </exception>
        public void CopyTo(Array array, int index)
        {
            _sessionState.CopyTo(array, index);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="System.Collections.ICollection"></see>.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the <see cref="System.Collections.ICollection"></see>.</returns>
        public int Count
        {
            get { return _sessionState.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="System.Collections.ICollection"></see> is synchronized (thread safe).
        /// </summary>
        /// <value></value>
        /// <returns>true if access to the <see cref="System.Collections.ICollection"></see> is synchronized (thread safe); otherwise, false.</returns>
        public bool IsSynchronized
        {
            get { return _sessionState.IsSynchronized; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="System.Collections.ICollection"></see>.
        /// </summary>
        /// <value></value>
        /// <returns>An object that can be used to synchronize access to the <see cref="System.Collections.ICollection"></see>.</returns>
        public object SyncRoot
        {
            get { return _sessionState.SyncRoot; }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return _sessionState.GetEnumerator();
        }

        #endregion
    }
}
