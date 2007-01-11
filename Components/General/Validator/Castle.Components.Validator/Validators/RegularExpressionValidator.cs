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

namespace Castle.Components.Validator
{
	using System;
	using System.Collections;
	using System.Text.RegularExpressions;

	/// <summary>
	/// Validate a property using regular expression
	/// </summary>
	[Serializable]
	public class RegularExpressionValidator : AbstractValidator
	{
		private readonly Regex regexRule;
		private readonly string expression;

		/// <summary>
		/// Initializes a new instance of the <see cref="RegularExpressionValidator"/> class.
		/// </summary>
		/// <param name="expression">The expression.</param>
		public RegularExpressionValidator(String expression) : this(expression, RegexOptions.Compiled)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="RegularExpressionValidator"/> class.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="options">The regular expression options.</param>
		public RegularExpressionValidator(String expression, RegexOptions options)
		{
			this.expression = expression;
			regexRule = new Regex(expression, options);
		}

		/// <summary>
		/// Validate that the property value match the given regex
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="fieldValue"></param>
		/// <returns><c>true</c> if the field is OK</returns>
		public override bool IsValid(object instance, object fieldValue)
		{
			if (fieldValue != null)
			{
				return regexRule.IsMatch(fieldValue.ToString());
			}

			return true;
		}

		public override bool SupportWebValidation
		{
			get { return true; }
		}

		public override void ApplyWebValidation(WebValidationConfiguration config, InputElementType inputType,
		                                        IWebValidationGenerator generator, IDictionary attributes)
		{
			generator.SetRegExp(expression, BuildErrorMessage());
		}
	}
}
