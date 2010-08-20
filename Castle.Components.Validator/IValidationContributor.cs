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
	/// <summary>
	/// Provides hook for allowing custom validation of an instance beyond
	/// the <see cref="IValidator">IValidator</see> instances registered for
	/// the object.
	/// </summary>
	public interface IValidationContributor
	{
		/// <summary>
		/// Determines whether the specified instance is valid.  Returns an
		/// <see cref="ErrorSummary"/> that will be appended to the existing
		/// error summary for an object.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="runWhen">The run when.</param>
		/// <returns></returns>
		ErrorSummary IsValid(object instance, RunWhen runWhen);
	}
}
