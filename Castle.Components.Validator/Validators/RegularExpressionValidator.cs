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
		/// Gets the regular expression object.
		/// </summary>
		/// <value>The regular expression object.</value>
		public Regex RegexRule
		{
			get { return regexRule; }
		}

		/// <summary>
		/// Gets the expression.
		/// </summary>
		/// <value>The expression.</value>
		public string Expression
		{
			get { return expression; }
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
				string value = fieldValue.ToString();

				if (value.Length > 0)
				{
					return regexRule.IsMatch(value);
				}
			}

			return true;
		}

		/// <summary>
		/// Gets a value indicating whether this validator supports browser validation.
		/// </summary>
		/// <value>
		/// 	<see langword="true"/> if browser validation is supported; otherwise, <see langword="false"/>.
		/// </value>
		public override bool SupportsBrowserValidation
		{
			get { return true; }
		}

		/// <summary>
		/// Applies the browser validation by setting up one or
		/// more input rules on <see cref="IBrowserValidationGenerator"/>.
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
			base.ApplyBrowserValidation(config, inputType, generator, attributes, target);

			generator.SetRegExp(target, expression, BuildErrorMessage());
		}
	}
}