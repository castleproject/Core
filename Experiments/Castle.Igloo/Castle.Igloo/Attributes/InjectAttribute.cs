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

namespace Castle.Igloo.Attributes
{
    /// <summary>
    /// Specifies that a component should be injected to
    /// the annotated field or setter properties.
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Field 
                             | AttributeTargets.Property
                             | AttributeTargets.Parameter 
                             | AttributeTargets.GenericParameter)]
    public class InjectAttribute : Attribute 
    {
	    private string _name = string.Empty;
        private bool _needInstanciation = false;
	    private bool _isRequired= false;
        private string _scope = ScopeType.UnSpecified;

        /// <summary>
        /// The context variable name. Defaults to the name of
        /// the annotated field or getter method.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Specifies that a component should be instantiated
        /// if the context variable is null.
        /// </summary>
        public bool Create
        {
            get { return _needInstanciation; }
            set { _needInstanciation = value; }
        }
        
        /// <summary>S
        /// Specifies that the injected value must not be
        /// null, by default true.
        /// </summary>
        public bool Required
        {
            get { return _isRequired; }
            set { _isRequired = value; }
        }

        /// <summary>S
        /// Explicitly specify the scope to search, instead of
        /// searching all scopes.
        /// </summary>
        public string Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }
    }
}
