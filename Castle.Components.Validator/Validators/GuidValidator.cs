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

namespace Castle.Components.Validator.Validators
{
	using System;
	using System.Collections;
	using System.Text.RegularExpressions;

	/// <summary>
	/// Validate that this is a valid GUID using regex; optionally, accept/reject Guid.Empty
	/// </summary>
	[Serializable]
	public class GuidValidator : RegularExpressionValidator
	{
		private const string guidRule =
			@"[0-9a-f]{8,8}-[0-9a-f]{4,4}-[0-9a-f]{4,4}-[0-9a-f]{4,4}-[0-9a-f]{12,12}";

		private readonly bool acceptEmptyGuid;

		/// <summary>
		/// Initializes a new instance of the <see cref="GuidValidator"/> class
		/// </summary>
		/// <remarks>
		/// Will consider Guid.Empty ('00000000-0000-0000-0000-000000000000') as valid
		/// </remarks>
		public GuidValidator() : this(true)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GuidValidator"/> class and specifies whether to consider Guid.Empty as valid
		/// </summary>
		/// <param name="acceptEmptyGuid">true to consider Guid.Empty as valid</param>
		public GuidValidator(bool acceptEmptyGuid) : base(guidRule, RegexOptions.Compiled | RegexOptions.IgnoreCase)
		{
			this.acceptEmptyGuid = acceptEmptyGuid;
		}

		/// <summary>
		/// Gets a value indicating whether [supports browser validation].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [supports browser validation]; otherwise, <c>false</c>.
		/// </value>
		public override bool SupportsBrowserValidation
		{
			get { return true; }
		}

		/// <summary>
		/// Returns the key used to internationalize error messages
		/// </summary>
		/// <value></value>
		protected override string MessageKey
		{
			get { return MessageConstants.InvalidGuidMessage; }
		}

		/// <summary>
		/// Applies the browser validation.
		/// </summary>
		/// <param name="config">The config.</param>
		/// <param name="inputType">Type of the input.</param>
		/// <param name="generator">The generator.</param>
		/// <param name="attributes">The attributes.</param>
		/// <param name="target">The target.</param>
		public override void ApplyBrowserValidation(BrowserValidationConfiguration config, InputElementType inputType,
		                                            IBrowserValidationGenerator generator, IDictionary attributes,
		                                            string target)
		{
			generator.SetEmail(target, BuildErrorMessage());
		}

		/// <summary>
		/// Validate that the property value match the given regex.  Null or empty values are allowed.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="fieldValue"></param>
		/// <returns><c>true</c> if the field is OK</returns>
		public override bool IsValid(object instance, object fieldValue)
		{
			if (fieldValue != null && fieldValue.ToString() != "")
			{
				var value = fieldValue.ToString();

				if (value.Length > 0)
				{
					if (value.Length != 36) 
					{
						return false;
					}
					if (RegexRule.IsMatch(value))
					{
						if (acceptEmptyGuid)
						{
							return true;
						}
						return value != Guid.Empty.ToString();
					}
					return false;
				}
			}

			return true;
		}
	}
}
