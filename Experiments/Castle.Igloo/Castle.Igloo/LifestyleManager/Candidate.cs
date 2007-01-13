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

using Castle.Core;

namespace Castle.Igloo.LifestyleManager
{
    /// <summary>
    /// Represents a component candidate for context eviction
    /// </summary>
    public class Candidate
    {
        private ComponentModel _componentModel = null;
        private object _instance = null;

        /// <summary>
        /// Gets the component model.
        /// </summary>
        /// <value>The component model.</value>
        public ComponentModel ComponentModel
        {
            get { return _componentModel; }
        }

        /// <summary>
        /// Gets the intance.
        /// </summary>
        /// <value>The intance.</value>
        public object Intance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Candidate"/> class.
        /// </summary>
        /// <param name="componentModel">The component model.</param>
        /// <param name="instance">The instance.</param>
        public Candidate(ComponentModel componentModel, object instance)
        {
            _componentModel = componentModel;
            _instance = instance;
        }
    }
}
