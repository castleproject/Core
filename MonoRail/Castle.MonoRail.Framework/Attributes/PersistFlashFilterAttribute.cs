// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.MonoRail.Framework.Attributes
{
    using Castle.MonoRail.Framework.Filters;

	/// <summary>
	/// Apply PersistFlashFilter, which preserve all flash contents after execution of all actions on the applied controller. 
	/// You may also use the flashKeys string array in the constructor to select specific entries inside the filter to be kept.
	/// </summary>
    public class PersistFlashFilterAttribute : FilterAttribute
    {
        private string[] flashKeys =null;

        #region Constructors

        /// <summary>
        /// Default constructor. This would persist the entire flash bag.
        /// </summary>
        public PersistFlashFilterAttribute() : base(ExecuteEnum.AfterAction,typeof(PersistFlashFilter))
        {

        }

		/// <summary>
		/// Initializes a new instance of the <see cref="PersistFlashFilterAttribute"/> class.
		/// </summary>
		/// <param name="flashKeys">The flash keys.</param>
        public PersistFlashFilterAttribute(string[] flashKeys) : this()
        {
            this.flashKeys = flashKeys;
        }

        #endregion

        #region Properties 

		/// <summary>
		/// Gets the flash keys.
		/// </summary>
		/// <value>The flash keys.</value>
        public string[] FlashKeys
        {
            get { return flashKeys; }
        }

        #endregion
    }
}