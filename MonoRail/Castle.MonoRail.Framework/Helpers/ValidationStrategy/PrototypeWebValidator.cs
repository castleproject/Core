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

namespace Castle.MonoRail.Framework.Helpers.ValidationStrategy
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Text;
	using Castle.Components.Validator;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Implements support for really easy field validation 
	/// http://tetlaw.id.au/view/javascript/really-easy-field-validation
	/// </summary>
	public class PrototypeWebValidator : IBrowserValidatorProvider
	{
		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public BrowserValidationConfiguration CreateConfiguration(IDictionary parameters)
		{
			PrototypeValidationConfiguration config = new PrototypeValidationConfiguration();

			config.Configure(parameters);

			return config;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="config"></param>
		/// <param name="inputType"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public IBrowserValidationGenerator CreateGenerator(BrowserValidationConfiguration config, InputElementType inputType, IDictionary attributes)
		{
			return new PrototypeValidationGenerator((PrototypeValidationConfiguration) config, inputType, attributes);
		}

		#region Configuration

		/// <summary>
		/// 
		/// </summary>
		public class PrototypeValidationConfiguration : BrowserValidationConfiguration
		{
			private IDictionary jsOptions = new Hashtable();
			private Dictionary<String, CustomRule> rules = new Dictionary<String, CustomRule>();

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
					AddClass("validate-checked");
				}
				AddTitle(violationMessage);
			}

			public void SetDigitsOnly(string target, string violationMessage)
			{
				AddClass("validate-digits");
				AddTitle(violationMessage);
			}

			public void SetNumberOnly(string target, string violationMessage)
			{
				AddClass("validate-number");
				AddTitle(violationMessage);
			}

			public void SetEmail(string target, string violationMessage)
			{
				AddClass("validate-email");
				AddTitle(violationMessage);
			}

			public void SetRegExp(string target, string regExp, string violationMessage)
			{
				string rule = "validate-regex" + regExp.GetHashCode();
				AddClass(rule);
				config.AddCustomRule(rule, violationMessage, "pattern : new RegExp(\"" + regExp + "\")");
				AddTitle(violationMessage);
			}

			public void SetExactLength(string target, int length)
			{
				SetExactLength(target, length, null);
			}

			public void SetExactLength(string target, int length, string violationMessage)
			{
				string rule = "validate-exact-length-" + length;
				AddClass(rule);
				config.AddCustomRule(rule, violationMessage, "minLength: " + length + ", maxLength: " + length);
				AddTitle(violationMessage);
			}

			public void SetMinLength(string target, int minLength)
			{
				SetMinLength(target, minLength, null);
			}

			public void SetMinLength(string target, int minLength, string violationMessage)
			{
				string rule = "validate-min-length-" + minLength;
				AddClass(rule);
				config.AddCustomRule(rule, violationMessage, "minLength: " + minLength);
				AddTitle(violationMessage);
			}

			public void SetMaxLength(string target, int maxLength)
			{
				SetMaxLength(target, maxLength, null);
			}

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

			public void SetLengthRange(string target, int minLength, int maxLength)
			{
				SetMinLength(target, minLength);
				SetMaxLength(target, maxLength);
			}

			public void SetLengthRange(string target, int minLength, int maxLength, string violationMessage)
			{
				SetMinLength(target, minLength, null);
				SetMaxLength(target, maxLength, null);
				AddTitle(violationMessage);
			}
			
			/// <summary>
			/// Sets the value range.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minValue">Value of the min.</param>
			/// <param name="maxValue">Value of the max.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetValueRange(string target, int minValue, int maxValue, string violationMessage)
			{
			}

			/// <summary>
			/// Sets the value range.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minValue">Value of the min.</param>
			/// <param name="maxValue">Value of the max.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetValueRange(string target, decimal minValue, decimal maxValue, string violationMessage)
			{
				
			}

			/// <summary>
			/// Sets the value range.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minValue">Value of the min.</param>
			/// <param name="maxValue">Value of the max.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetValueRange(string target, DateTime minValue, DateTime maxValue, string violationMessage)
			{
			}

			/// <summary>
			/// Sets the value range.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minValue">Value of the min.</param>
			/// <param name="maxValue">Value of the max.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetValueRange(string target, string minValue, string maxValue, string violationMessage)
			{
			}

			public void SetAsSameAs(string target, string comparisonFieldName, string violationMessage)
			{
				string rule = "validate-same-as-" + comparisonFieldName.ToLowerInvariant();
				AddClass(rule);
				config.AddCustomRule(rule, violationMessage, "equalToField : '" + GetPrefixedFieldld(target, comparisonFieldName.ToLowerInvariant()) + "'");
				AddTitle(violationMessage);
			}

			public void SetDate(string target, string violationMessage)
			{
				AddClass("validate-date");
				AddTitle(violationMessage);
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
				string[] parts = target.Split('.');

				return string.Join("_", parts, 0, parts.Length - 1) + "_" + field;
			}
		}

		#endregion
	}
}
