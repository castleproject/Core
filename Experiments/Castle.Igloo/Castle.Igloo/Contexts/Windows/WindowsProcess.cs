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

namespace Castle.Igloo.Contexts.Windows
{
    /// <summary>
    /// Defines an object that represents the processing context for request-level operations.
    /// </summary>
    public class WindowsProcess : IProcessContext
    {
        private ISessionState _sessionState = null;
        private IApplicationState _applicationState = null;
        private IDictionary _items = new HybridDictionary();

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsProcess"/> class.
        /// </summary>
        public WindowsProcess()
        {
            _sessionState = new WindowsSessionState();
            _applicationState = new WindowsApplicationState();
        }

        #region IProcessContext Members


        /// <summary>
        /// Gets the request items.
        /// </summary>
        /// <value>The items.</value>
        public IDictionary Items
        {
            get { return _items; }
        }


        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <value>The session.</value>
        public ISessionState Session
        {
            get { return _sessionState; }
        }


        /// <summary>
        /// Gets the state of the application.
        /// </summary>
        /// <value>The state of the application.</value>
        public IApplicationState ApplicationState
        {
            get { return _applicationState; }
        }

        #endregion
    }
}
