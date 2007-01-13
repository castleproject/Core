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
using System.Collections.Generic;
using System.Threading;
using Castle.Core;
using Castle.Igloo.Scopes;
using Castle.MicroKernel;

namespace Castle.Igloo.Scopes.Windows
{
    public class ThreadScope : IScope
    {
        private const string THREAD_TOKEN = "_THREAD_SCOPE_";
        
        [NonSerialized]
        private static LocalDataStoreSlot _slot = Thread.AllocateNamedDataSlot(THREAD_TOKEN);
       
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
        /// Gets the <see cref="Object"/> with the specified name.
        /// </summary>
        /// <value></value>
        public object this[string name]
        {
            get 
            {               
                lock (_slot)
                {
                    IDictionary<string, object> map = GetMap();

                    if (map.ContainsKey(name))
                    {
                        return map[name];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the IScope object.
        /// </summary>
        /// <param name="name">The name of the element to add.</param>
        /// <param name="instance">The Object to use as the value of the element to add.</param>
        public void Add(string name, object instance)
        {
            lock (_slot)
            {
                IDictionary<string, object> map = GetMap();

                map[name] = instance;
            }
        }

        /// <summary>
        /// Removes the element with the specified name from the IScope object.
        /// </summary>
        /// <param name="name">The name of the element to remove.</param>
        public void Remove(string name)
        {
            lock (_slot)
            {
                IDictionary<string, object> map = GetMap();

                map.Remove(name);
            }
        }

        /// <summary>
        /// Determines whether the IDictionary object contains an element with the specified name.
        /// </summary>
        /// <param name="name">The name to locate in the IScope object.</param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            lock (_slot)
            {
                IDictionary<string, object> map = GetMap();

                return map.Keys.Contains(name);
            }
        }

        public ICollection Names
        {
            get
            {
                lock (_slot)
                {
                    IDictionary<string, object> map = GetMap();

                    return (ICollection)map.Keys;
                }
            }
        }

        public void Flush()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RegisterForEviction(ILifestyleManager manager, ComponentModel componentModel, object instance)
        {
        }

        public void CheckInitialisation()
        {
            IDictionary<string, object> map = (IDictionary<string, object>)Thread.GetData(_slot);

            if (map == null)
            {
                map = new Dictionary<string, object>();

                Thread.SetData(_slot, map);
            }
        }

        /// <summary>
        /// Gets the type of the scope.
        /// </summary>
        /// <value>The type of the scope.</value>
        public string ScopeType
        {
            get { return Castle.Igloo.ScopeType.Thread; }
        }

        #endregion
        
        private IDictionary<string, object> GetMap()
        {
            IDictionary<string, object> map = (IDictionary<string, object>)Thread.GetData(_slot);

            if (map == null)
            {
                map = new Dictionary<string, object>();

                Thread.SetData(_slot, map);
            }
            
            return map;
        }
    }
}
