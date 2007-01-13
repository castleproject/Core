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
using System.Diagnostics;
using Castle.Igloo.Attributes;
using Castle.Igloo.Util;

namespace Castle.Igloo.Scopes.Web
{
    //[Scope(Scope = ScopeType.Application)]
    public sealed class WebApplicationScope : IApplicationScope
    {
        #region IApplicationScope Members

        /// <summary>
        /// Gets a value indicating whether this context is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return WebUtil.GetCurrentHttpContext()!=null; }
        }
 
        /// <summary>
        /// Gets the <see cref="Object"/> with the specified name.
        /// </summary>
        /// <value></value>
        public object this[string name]
        {
            get { return WebUtil.GetCurrentHttpContext().ApplicationInstance.Application[name]; }
        }

        /// <summary>
        /// Add the specified object under the name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void Add(string name, object value)
        {
            Trace.WriteLine("Add to application scope : " + name);

            WebUtil.GetCurrentHttpContext().ApplicationInstance.Application.Add(name, value);
        }

        /// <summary>
        /// Removes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Remove(string name)
        {
            Trace.WriteLine("Remove from application scope : " + name);

            WebUtil.GetCurrentHttpContext().ApplicationInstance.Application.Remove(name);
        }


        /// <summary>
        /// Determines whether [contains] [the specified name].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified name]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string name)
        {
            return (WebUtil.GetCurrentHttpContext().ApplicationInstance.Application[name] != null);
        }

        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <value>The names.</value>
        public ICollection Names
        {
            get { return WebUtil.GetCurrentHttpContext().ApplicationInstance.Application.Keys; }
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public void Flush()
        {
            Trace.WriteLine("Flush application scope.");

            WebUtil.GetCurrentHttpContext().ApplicationInstance.Application.RemoveAll();
        }
        
        /// <summary>
        /// Gets the type of the scope.
        /// </summary>
        /// <value>The type of the scope.</value>
        public string ScopeType
        {
            get { return Igloo.ScopeType.Application; }
        }

        #endregion
    }
}
