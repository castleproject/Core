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
using Castle.Igloo.Util;

namespace Castle.Igloo.Contexts
{
    public sealed class RequestContext : IContext
    {
        private IDictionary _items = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestContext"/> class.
        /// </summary>
        /// <param name="processContext">The process context.</param>
        public RequestContext(IProcessContext processContext)
        {
            AssertUtils.ArgumentNotNull(processContext, "processContext");

            _items = processContext.Items;
        }
        
        #region IContext Members

        /// <summary>
        /// Gets the <see cref="Object"/> with the specified name.
        /// </summary>
        /// <value></value>
        public object this[string name]
        {
            get { return _items[name]; }
        }

        /// <summary>
        /// Gets the <see cref="Object"/> with the specified type.
        /// </summary>
        /// <value></value>
        public object this[Type clazz]
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the IContext object.
        /// </summary>
        /// <param name="name">The name of the element to add.</param>
        /// <param name="value">The Object to use as the value of the element to add.</param>
        public void Add(string name, object value)
        {
            _items.Add(name, value);
        }

        /// <summary>
        /// Removes the element with the specified name from the IContext object.
        /// </summary>
        /// <param name="name">The name of the element to remove.</param>
        public void Remove(string name)
        {
            _items.Remove(name);
        }

        /// <summary>
        /// Determines whether the IDictionary object contains an element with the specified name.
        /// </summary>
        /// <param name="name">The name to locate in the IContext object.</param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return _items.Contains(name);
        }

        /// <summary>
        /// Gets All the objects names contain in the IContext object.
        /// </summary>
        /// <value>The names.</value>
        public ICollection Names
        {
            get { return _items.Keys; }
        }

        /// <summary>
        /// Removes all the elements from the IContext object.
        /// </summary>
        public void Flush()
        {
            _items.Clear();
        }

        /// <summary>
        /// Abandons the current session.
        /// </summary>
        public void Abandon()
        {
            Flush();
        }

        /// <summary>
        /// Gets the type of the scope.
        /// </summary>
        /// <value>The type of the scope.</value>
        public ScopeType ScopeType
        {
            get { return ScopeType.Request; }
        }

        #endregion
    }
}
