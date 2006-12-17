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
using System.Threading;

namespace Castle.Igloo.Contexts.Windows
{
    /// <summary>
    /// Provides access to application-state values in windows context.
    /// </summary>
    public class WindowsApplicationState : NameObjectCollectionBase , IApplicationState
    {
        private ReaderWriterLock _Lock; 

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsApplicationState"/> class.
        /// </summary>
        public WindowsApplicationState()
        {
            _Lock = new ReaderWriterLock();
        }
        
        #region IApplicationState Members

        /// <summary>
        /// Gets or sets the <see cref="Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        public object this[string key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Object"/> at the specified index.
        /// </summary>
        /// <value></value>
        public object this[int index]
        {
            get { return Get(index); }
            set { throw new NotImplementedException("Indexer cannot be assigned to, it's read only"); }
        }

        /// <summary>
        /// Gets the object at the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private object Get(string key)
        {
            object ret = null;

            _Lock.AcquireReaderLock(-1);
            try
            {
                ret = BaseGet(key);
            }
            finally
            {
                _Lock.ReleaseReaderLock();
            }

            return ret;
        }

        /// <summary>
        /// Gets the object at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private object Get(int index)
        {
            object ret = null;

            _Lock.AcquireReaderLock(-1);
            try
            {
                ret = BaseGet(index);
            }
            finally
            {
                _Lock.ReleaseReaderLock();
            }

            return ret;
        }  
        
        /// <summary>
        /// Stores the specified object on the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private void Set(string key, object value)
        {
            _Lock.AcquireWriterLock(-1);
            try
            {
                BaseSet(key, value);
            }
            finally
            {
                _Lock.ReleaseWriterLock();
            }
        } 
        

        /// <summary>
        /// Adds a new item to application state.
        /// </summary>
        /// <param name="key">The item key.</param>
        /// <param name="value">The value.</param>
        public void Add(string key, object value)
        {
            _Lock.AcquireWriterLock(-1);
            try
            {
                BaseAdd(key, value);
            }
            finally
            {
                _Lock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Clears all values from application state.
        /// </summary>
        public void Clear()
        {
            _Lock.AcquireWriterLock(-1);
            try
            {
                BaseClear();
            }
            finally
            {
                _Lock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Deletes an item from the application-state collection.
        /// </summary>
        /// <param name="key">The item key.</param>
        public void Remove(string key)
        {
            _Lock.AcquireWriterLock(-1);
            try
            {
                BaseRemove(key);
            }
            finally
            {
                _Lock.ReleaseWriterLock();
            } 
        }

        /// <summary>
        /// Removes all objects from an application-state collection.
        /// </summary>
        public void RemoveAll()
        {
            Clear();
        }

        #endregion
    }
}
