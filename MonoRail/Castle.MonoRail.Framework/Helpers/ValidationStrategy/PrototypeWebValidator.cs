// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Helpers.ValidationStrategy
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Text;
	using Components.Validator;
	using Internal;

	/// <summary>
	/// Implements support for really easy field validation 
	/// http://tetlaw.id.au/view/javascript/really-easy-field-validation
	/// </summary>
	public class PrototypeWebValidator : IBrowserValidatorProvider
	{
		/// <summary>
		/// Implementors should attempt to read their specific configuration
		/// from the <paramref name="parameters"/>, configure and return
		/// a class that extends from <see cref="BrowserValidationConfiguration"/>
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns>
		/// An instance that extends from <see cref="BrowserValidationConfiguration"/>
		/// </returns>
		public BrowserValidationConfiguration CreateConfiguration(IDictionary parameters)
		{
			PrototypeValidationConfiguration config = new PrototypeValidationConfiguration();

			config.Configure(parameters);

			return config;
		}

		/// <summary>
		/// Implementors should return their generator instance.
		/// </summary>
		/// <param name="config"></param>
		/// <param name="inputType"></param>
		/// <param name="attributes"></param>
		/// <returns>A generator instance</returns>
		public IBrowserValidationGenerator CreateGenerator(BrowserValidationConfiguration config, InputElementType inputType, IDictionary attributes)
		{
			return new PrototypeValidationGenerator((PrototypeValidationConfiguration) config, inputType, attributes);
		}

		#region Configuration

		/// <summary>
		/// Configuration for the Prototype Field Validation
		/// </summary>
		public class PrototypeValidationConfiguration : BrowserValidationConfiguration
		{
			private IDictionary jsOptions = new Hashtable();
			private Dictionary<String, CustomRule> rules = new Dictionary<String, CustomRule>();

			/// <summary>
			/// Implementors should return any tag/js content
			/// to be rendered after the form tag is closed.
			/// </summary>
			/// <param name="formId">The form id.</param>
			/// <returns></returns>
			public override string CreateBeforeFormClosed(string formId)
			{
				StringBuilder sb = new StringBuilder();

				sb.Append("if (!window.prototypeValidators) prototypeValidators = $A([]);\n");
				sb.AppendFormat("var validator = new Validation('{0}', {1});\n", formId, AjaxHelper.JavascriptOptions(jsOptions));
				sb.AppendFormat("prototypeValidators['{0}'] = validator;\n", formId);

				if (rules.Count != 0)
				{
					sb.Append("Validation.addAllThese([\n");

					bool addedFirstRule = false;
					string Comma = "";

					foreach(CustomRule rule in rules.Values)
					{
						sb.AppendFormat("{0} ['{1}', '{2}', {{ {3} }}]\n", 
							Comma, rule.className, 
							rule.violationMessage != null ? rule.violationMessage.Replace("'", "\'") : null, 
							rule.rule);

						if (!addedFirstRule)
						{
							addedFirstRule = true;
							Comma = ",";
						}
					}

					sb.Append("]);\n");
				}

				return AbstractHelper.ScriptBlock(sb.ToString());
			}

			/// <summary>
			/// Configures the JS library based on the supplied parameters.
			/// </summary>
			/// <param name="parameters">The parameters.</param>
			public override void Configure(IDictionary parameters)
			{
				string onSubmit = CommonUtils.ObtainEntryAndRemove(parameters, "on_submit", "true");
				string stopOnFirst = CommonUtils.ObtainEntryAndRemove(parameters, "stopOnFirst", "false");
				string immediate = CommonUtils.ObtainEntryAndRemove(parameters, "immediate", "true");
				string focusOnError = CommonUtils.ObtainEntryAndRemove(parameters, "focusOnError", "true");
				string useTitles = CommonUtils.ObtainEntryAndRemove(parameters, "useTitles", "true");
				string onFormValidate = CommonUtils.ObtainEntryAndRemove(parameters, "onFormValidate");
				string onElementValidate = CommonUtils.ObtainEntryAndRemove(parameters, "onElementValidate");
				string onCreateAdvice = CommonUtils.ObtainEntryAndRemove(parameters, "onCreateAdvice");

				jsOptions["onSubmit"] = onSubmit;
				jsOptions["stopOnFirst"] = stopOnFirst;
				jsOptions["immediate"] = immediate;
				jsOptions["focusOnError"] = focusOnError;
				jsOptions["useTitles"] = useTitles;

				if (onFormValidate != null)
				{
					jsOptions["onFormValidate"] = onFormValidate;
				}
				if (onElementValidate != null)
				{
					jsOptions["onElementValidate"] = onElementValidate;
				}
				if (onCreateAdvice != null)
				{
					jsOptions["onCreateAdvice"] = onCreateAdvice;
				}
			}

			/// <summary>
			/// Adds a custom rule.
			/// </summary>
			/// <param name="className">Name of the class.</param>
			/// <param name="violationMessage">The violation message.</param>
			/// <param name="rule">The rule.</param>
			public void AddCustomRule(string className, string violationMessage, string rule)
			{
				rules[className] = new CustomRule(className, rule, violationMessage);
			}

			class CustomRule
			{
				public readonly string className, rule, violationMessage;

				public CustomRule(string className, string rule, string violationMessage)
				{
					this.className = className;
					this.rule = rule;
					this.violationMessage = violationMessage;
				}
			}
		}

		#endregion

		#region Validation Generator

		/// <summary>
		/// Generator for prototype field validation 
		/// </summary>
		public class PrototypeValidationGenerator : IBrowserValidationGenerator
		{
			private readonly PrototypeValidationConfiguration config;
			private readonly InputElementType inputType;
			private readonly IDictionary attributes;

			/// <summary>
			/// Initializes a new instance of the <see cref="PrototypeValidationGenerator"/> class.
			/// </summary>
			/// <param name="config">Validation configuration instance</param>
			/// <param name="inputType">Type of the input.</param>
			/// <param name="attributes">The attributes.</param>
			public PrototypeValidationGenerator(PrototypeValidationConfiguration config, InputElementType inputType, IDictionary attributes)
			{
				this.config = config;
				this.inputType = inputType;
				this.attributes = attributes;
			}

			/// <summary>
			/// Sets that a field is required.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetAsRequired(string target, string violationMessage)
			{
				if (inputType == InputElementType.Text)
				{
					AddClass("required");
				}
				else if (inputType == InputElementType.Select)
				{
					AddClass("validate-selection");
				}
				else if (inputType == InputElementType.Checkbox)
				{
					AddClass("required");
				}
				AddTitle(violationMessage);
			}

			/// <summary>
			/// Set that a field should only accept digits.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetDigitsOnly(string target, string violationMessage)
			{
				AddClass("validate-digits");
				AddTitle(violationMessage);
			}

			/// <summary>
			/// Set that a field should only accept numbers.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetNumberOnly(string target, string violationMessage)
			{
				AddClass("validate-number");
				AddTitle(violationMessage);
			}

			/// <summary>
			/// Sets that a field value must be a valid email address.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetEmail(string target, string violationMessage)
			{
				AddClass("validate-email");
				AddTitle(violationMessage);
			}

			/// <summary>
			/// Sets that a field value must match the specified regular expression.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="regExp">The reg exp.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetRegExp(string target, string regExp, string violationMessage)
			{
				string rule = "validate-regex" + regExp.GetHashCode();
				AddClass(rule);
				config.AddCustomRule(rule, violationMessage, "pattern : /" + regExp + "/");
				AddTitle(violationMessage);
			}

			/// <summary>
			/// Sets that field must have an exact lenght.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="length">The length.</param>
			public void SetExactLength(string target, int length)
			{
				SetExactLength(target, length, null);
			}

			/// <summary>
			/// Sets that field must have an exact lenght.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="length">The length.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetExactLength(string target, int length, string violationMessage)
			{
				string rule = "validate-exact-length-" + length;
				AddClass(rule);
				config.AddCustomRule(rule, violationMessage, "minLength: " + length + ", maxLength: " + length);
				AddTitle(violationMessage);
			}

			/// <summary>
			/// Sets that field must have an minimum lenght.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minLength">The minimum length.</param>
			public void SetMinLength(string target, int minLength)
			{
				SetMinLength(target, minLength, null);
			}

			/// <summary>
			/// Sets that field must have an minimum lenght.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minLength">The minimum length.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetMinLength(string target, int minLength, string violationMessage)
			{
				string rule = "validate-min-length-" + minLength;
				AddClass(rule);
				config.AddCustomRule(rule, violationMessage, "minLength: " + minLength);
				AddTitle(violationMessage);
			}

			/// <summary>
			/// Sets that field must have an maximum lenght.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="maxLength">The maximum length.</param>
			public void SetMaxLength(string target, int maxLength)
			{
				SetMaxLength(target, maxLength, null);
			}

			/// <summary>
			/// Sets that field must have an maximum lenght.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="maxLength">The maximum length.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetMaxLength(string target, int maxLength, string violationMessage)
			{
				if (inputType == InputElementType.Text)
				{
					attributes["maxlength"] = maxLength;
				}

				string rule = "validate-max-length-" + maxLength;
				AddClass(rule);
				config.AddCustomRule(rule, violationMessage, "maxLength: " + maxLength);
				AddTitle(violationMessage);
			}

			/// <summary>
			/// Sets that field must be between a length range.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minLength">The minimum length.</param>
			/// <param name="maxLength">The maximum length.</param>
			public void SetLengthRange(string target, int minLength, int maxLength)
			{
				SetMinLength(target, minLength);
				SetMaxLength(target, maxLength);
			}

			/// <summary>
			/// Sets that field must be between a length range.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minLength">The minimum length.</param>
			/// <param name="maxLength">The maximum length.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetLengthRange(string target, int minLength, int maxLength, string violationMessage)
			{
				SetMinLength(target, minLength, null);
				SetMaxLength(target, maxLength, null);
				AddTitle(violationMessage);
			}

			/// <summary>
			/// Sets that field must be between a value range.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minValue">Minimum value.</param>
			/// <param name="maxValue">Maximum value.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetValueRange(string target, int minValue, int maxValue, string violationMessage)
			{
			}

			/// <summary>
			/// Sets that field must be between a value range.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minValue">Minimum value.</param>
			/// <param name="maxValue">Maximum value.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetValueRange(string target, decimal minValue, decimal maxValue, string violationMessage)
			{
				
			}

			/// <summary>
			/// Sets that field must be between a value range.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minValue">Minimum value.</param>
			/// <param name="maxValue">Maximum value.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetValueRange(string target, DateTime minValue, DateTime maxValue, string violationMessage)
			{
			}

			/// <summary>
			/// Sets that field must be between a value range.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minValue">Minimum value.</param>
			/// <param name="maxValue">Maximum value.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetValueRange(string target, string minValue, string maxValue, string violationMessage)
			{
			}

			/// <summary>
			/// Set that a field value must be the same as another field's value.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="comparisonFieldName">The name of the field to compare with.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetAsSameAs(string target, string comparisonFieldName, string violationMessage)
			{
				string prefixedComparisonFieldName = GetPrefixedFieldld(target, comparisonFieldName);
				string rule = "validate-same-as-" + prefixedComparisonFieldName;
				AddClass(rule);
				config.AddCustomRule(rule, violationMessage, "equalToField : '" + prefixedComparisonFieldName + "'");
				AddTitle(violationMessage);
			}

			/// <summary>
			/// Set that a field value must _not_ be the same as another field's value.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="comparisonFieldName">The name of the field to compare with.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetAsNotSameAs(string target, string comparisonFieldName, string violationMessage)
			{
				string prefixedComparisonFieldName = GetPrefixedFieldld(target, comparisonFieldName);
				string rule = "validate-not-same-as-" + prefixedComparisonFieldName;
				AddClass(rule);
				config.AddCustomRule(rule, violationMessage, "notEqualToField : '" + prefixedComparisonFieldName + "'");
				AddTitle(violationMessage);
			}

			/// <summary>
			/// Set that a field value must be a valid date.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetDate(string target, string violationMessage)
			{
				AddClass("validate-date");
				AddTitle(violationMessage);
			}

			/// <summary>
			/// Sets that a field's value must greater than another field's value.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="comparisonFieldName">The name of the field to compare with.</param>
			/// <param name="validationType">The type of data to compare.</param>
			/// <param name="violationMessage">The violation message.</param>
			/// <remarks>Not implemented by the JQuery validate plugin. Done via a custom rule.</remarks>
			public void SetAsGreaterThan( string target, string comparisonFieldName, Castle.Components.Validator.IsGreaterValidationType validationType, string violationMessage )
			{
			}

			/// <summary>
			/// Sets that a field's value must be lesser than another field's value.
			/// </summary>
			/// <remarks>Not implemented by the JQuery validate plugin. Done via a custom rule.</remarks>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="comparisonFieldName">The name of the field to compare with.</param>
			/// <param name="validationType">The type of data to compare.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetAsLesserThan( string target, string comparisonFieldName, IsGreaterValidationType validationType, string violationMessage )
			{
			}

			/// <summary>
			/// Sets that a flied is part of a group validation.
			/// </summary>
			/// <param name="target">The target.</param>
			/// <param name="groupName">Name of the group.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetAsGroupValidation(string target, string groupName, string violationMessage)
			{
				
			}

			private void AddTitle(string message)
			{
				if (!string.IsNullOrEmpty(message))
				{
					string existingTitle = (string) attributes["title"];
					
					if (!message.EndsWith("."))
					{
						message += ".";
					}

					if (existingTitle != null)
					{
						attributes["title"] = existingTitle + " " + message;
					}
					else
					{
						attributes["title"] = message;
					}
				}
			}

			private void AddClass(string className)
			{
				string existingClass = (string)attributes["class"];

				if (existingClass != null)
				{
					attributes["class"] = existingClass + " " + className;
				}
				else
				{
					attributes["class"] = className;
				}
			}

			private static string GetPrefixedFieldld(string target, string field)
			{
				string[] parts = target.Split('_');

				return string.Join("_", parts, 0, parts.Length - 1) + "_" + field;
			}
		}

		#endregion
	}
}
