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
	using System.Collections.Generic;

	/// <summary>
	/// 
	/// </summary>
	public interface IValidationPerformer
	{
		/// <summary>
		/// Performs validation on a given object instance
		/// </summary>
		/// <param name="objectInstance">object instance to validate</param>
		/// <param name="validators">validators to apply</param>
		/// <param name="contributors">validation contributors to apply</param>
		/// <param name="runWhen">Restrict the set returned to the phase specified</param>
		/// <param name="summaryToPopulate">instance which will be populated by the performed validation</param>
		/// <returns>wether the instance is valid or not</returns>
		bool PerformValidation(
			object objectInstance,
			IEnumerable<IValidator> validators,
			IEnumerable<IValidationContributor> contributors,
			RunWhen runWhen,
			ErrorSummary summaryToPopulate
			);

		/// <summary>
		/// Executes the validation contributors.
		/// </summary>
		/// <param name="objectInstance">The object instance.</param>
		/// <param name="contributors">contributors to apply</param>
		/// <param name="summaryToPopulate">The summary to populate.</param>
		/// <param name="runWhen">Restrict the set returned to the phase specified</param>
		void ExecuteContributors(
			object objectInstance,
			IEnumerable<IValidationContributor> contributors,
			ErrorSummary summaryToPopulate,
			RunWhen runWhen);
	}
}