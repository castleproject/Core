// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Validator
{
	using System.Collections;

	/// <summary>
	/// Represents the base of a browser configuration.
	/// </summary>
	public abstract class BrowserValidationConfiguration
	{
		/// <summary>
		/// Configures the JS library based on the supplied parameters.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		public virtual void Configure(IDictionary parameters)
		{
		}

		/// <summary>
		/// Implementors should return any tag/js content
		/// to be rendered after the form tag is rendered.
		/// </summary>
		/// <param name="formId">The form id.</param>
		/// <returns></returns>
		public virtual string CreateAfterFormOpened(string formId)
		{
			return string.Empty;
		}

		/// <summary>
		/// Implementors should return any tag/js content
		/// to be rendered after the form tag is closed.
		/// </summary>
		/// <param name="formId">The form id.</param>
		/// <returns></returns>
		public virtual string CreateBeforeFormClosed(string formId)
		{
			return string.Empty;
		}
	}
}