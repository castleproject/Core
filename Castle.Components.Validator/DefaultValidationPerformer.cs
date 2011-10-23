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
	/// Default validation performer implementation
	/// </summary>
	public class DefaultValidationPerformer : IValidationPerformer {

		bool IValidationPerformer.PerformValidation(object objectInstance, IEnumerable<IValidator> validators, IEnumerable<IValidationContributor> contributors, RunWhen runWhen, ErrorSummary summaryToPopulate) {
			foreach (IValidator validator in validators) {
				if (!validator.IsValid(objectInstance)) {
					string name = validator.FriendlyName ?? validator.Name;
					summaryToPopulate.RegisterErrorMessage(name, validator.ErrorMessage);
				}
			}

			if(contributors != null)
				(this as IValidationPerformer).ExecuteContributors(objectInstance, contributors, summaryToPopulate, runWhen);

			bool isValid = !summaryToPopulate.HasError;
			return isValid;
		}

		void IValidationPerformer.ExecuteContributors(object objectInstance, IEnumerable<IValidationContributor> contributors, ErrorSummary summaryToPopulate, RunWhen runWhen) {
			foreach (IValidationContributor contributor in contributors) {
				ErrorSummary errors = contributor.IsValid(objectInstance, runWhen);
				summaryToPopulate.RegisterErrorsFrom(errors);
			}
		}

	}
}
