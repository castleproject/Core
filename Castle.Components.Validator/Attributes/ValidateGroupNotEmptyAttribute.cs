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
	using System;
	using System.Collections;

	/// <summary>
	/// Validate that at least one of the properties in the group is not null or empty (for strings)
	/// </summary>
	[Serializable]
	public class ValidateGroupNotEmptyAttribute : AbstractValidationAttribute
	{
		private string group;
		// warning not used
		//private static IDictionary groupsPerType = Hashtable.Synchronized(new Hashtable());
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateGroupNotEmptyAttribute"/> class.
		/// </summary>
		/// <param name="group">The group.</param>
		public ValidateGroupNotEmptyAttribute(string group)
		{
			this.group = group;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateGroupNotEmptyAttribute"/> class.
		/// </summary>
		/// <param name="group">The group.</param>
		/// <param name="errorMessage">The error message.</param>
		public ValidateGroupNotEmptyAttribute(string group, string errorMessage) : base(errorMessage)
		{
			this.group = group;
		}

		/// <summary>
		/// Constructs and configures an <see cref="IValidator"/>
		/// instance based on the properties set on the attribute instance.
		/// </summary>
		/// <returns></returns>
		public override IValidator Build()
		{
			throw new NotSupportedException("You must call Build with a type parameter");
		}

		/// <summary>
		/// Constructs and configures an <see cref="IValidator"/>
		/// instance based on the properties set on the attribute instance.
		/// </summary>
		/// <param name="validatorRunner"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public override IValidator Build(IValidatorRunner validatorRunner, Type type)
		{
			GroupNotEmptyValidator validator = (GroupNotEmptyValidator)
			                                   validatorRunner.ExtendedProperties[group];
			if (validator == null)
			{
				validatorRunner.ExtendedProperties[group] = validator
				                                            = new GroupNotEmptyValidator(group);
			}
			ConfigureValidatorMessage(validator);

			return validator;
		}
	}
}
