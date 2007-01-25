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

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Castle.Core;
using Castle.Igloo.Scopes;
using Castle.MicroKernel;

namespace Castle.Igloo.Mock
{
    /// <summary>
    /// Mock implementation of <see cref="IScope"/>
    /// </summary>
    public abstract class MockScope : Dictionary<string, object> , IScope
    {
        private ReaderWriterLock _lock = null; 

        /// <summary>
        /// Initializes a new instance of the <see cref="MockApplicationScope"/> class.
        /// </summary>
        public MockScope()
        {
            _lock = new ReaderWriterLock();
        }
        
        #region IScope Members

        /// <summary>
        /// Gets a value indicating whether this context is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the <see cref="object"/> with the specified name.
        /// </summary>
        /// <value></value>
        public new object this[string name]
        {
            get { return Get(name); }
            set { Set(name, value); }
        }

        /// <summary>
        /// Removes the element with the specified name from the IScope object.
        /// </summary>
        /// <param name="name">The name of the element to remove.</param>
        public new void Remove(string name)
        {
            _lock.AcquireWriterLock(-1);
            try
            {
                base.Remove(name);
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Determines whether the IDictionary object contains an element with the specified name.
        /// </summary>
        /// <param name="name">The name to locate in the IScope object.</param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return ContainsKey(name);
        }

        /// <summary>
        /// Gets All the objects names contain in the IScope object.
        /// </summary>
        /// <value>The names.</value>
        public ICollection Names
        {
            get { return Keys; }
        }

        /// <summary>
        /// Removes all the elements from the IScope object.
        /// </summary>
        public void Flush()
        {
            _lock.AcquireWriterLock(-1);
            try
            {
                Clear();
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Registers for eviction.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="componentModel">The componentModel.</param>
        /// <param name="instance">The instance.</param>
        public void RegisterForEviction(ILifestyleManager manager, ComponentModel componentModel, object instance)
        {
        }

        /// <summary>
        /// Checks the initialisation.
        /// </summary>
        public void CheckInitialisation()
        {
        }

        /// <summary>
        /// Gets the type of the scope.
        /// </summary>
        /// <value>The type of the scope.</value>
        public abstract string ScopeType { get;}
        
        #endregion

        /// <summary>
        /// Gets the object at the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private object Get(string key)
        {
            object ret = null;

            _lock.AcquireReaderLock(-1);
            try
            {
                TryGetValue(key, out ret);
            }
            finally
            {
                _lock.ReleaseReaderLock();
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
            _lock.AcquireWriterLock(-1);
            try
            {
               base[key] = value;
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        } 
    }
}

