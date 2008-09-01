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
	using System.Globalization;
	using System.Text;
	using System.Threading;
	using Castle.Components.Validator;
	using Internal;

	/// <summary>
	/// Provides an interface for the Zebda client side JS validator
	/// http://labs.cavorite.com/zebda
	/// </summary>
	public class ZebdaWebValidator : IBrowserValidatorProvider
	{
		/// <summary>
		/// Read the configuration
		/// </summary>
		/// <returns></returns>
		public BrowserValidationConfiguration CreateConfiguration(IDictionary parameters)
		{
			ZebdaValidationConfiguration config = new ZebdaValidationConfiguration();
			config.Configure(parameters);

			return config;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <returns></returns>
		public IBrowserValidationGenerator CreateGenerator(BrowserValidationConfiguration config, InputElementType inputType,
		                                                   IDictionary attributes)
		{
			return new ZebdaWebValidationGenerator(inputType, attributes);
		}
	}

	#region Configuration

	///<summary>
	/// Configuration for the Zebda validation
	///</summary>
	public class ZebdaValidationConfiguration : BrowserValidationConfiguration
	{
		private IDictionary jsOptions = new Hashtable();

		/// <summary>
		/// Render the validation init script
		/// </summary>
		/// <param name="formId"></param>
		/// <returns></returns>
		public override string CreateAfterFormOpened(string formId)
		{
			string display = CommonUtils.ObtainEntryAndRemove(jsOptions, "display");

			StringBuilder script = new StringBuilder();
			script.Append("$('" + formId + "').setAttribute('z:options','" + AjaxHelper.JavascriptOptions(jsOptions) + "')");
			script.AppendLine(";");

			script.Append("$('" + formId + "').setAttribute('z:display','" + display + "')");
			script.AppendLine(";");

			return AbstractHelper.ScriptBlock(script.ToString());
		}

		/// <summary>
		/// read the validator configuration values
		/// </summary>
		/// <param name="parameters"></param>
		public override void Configure(IDictionary parameters)
		{
			CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;

			string display = CommonUtils.ObtainEntryAndRemove(parameters, "display", "inline");
			string dateFormat = CommonUtils.ObtainEntryAndRemove(parameters, "dateFormat", string.Empty);
			string thousandSeparator = CommonUtils.ObtainEntryAndRemove(parameters, "thousandSeparator", string.Empty);
			string decimalSeparator = CommonUtils.ObtainEntryAndRemove(parameters, "decimalSeparator", string.Empty);
			string immediate = CommonUtils.ObtainEntryAndRemove(parameters, "immediate", "true");
			string inlineFilters = CommonUtils.ObtainEntryAndRemove(parameters, "inlineFilters", "false");

			if (dateFormat == string.Empty)
			{
				dateFormat = cultureInfo.DateTimeFormat.ShortDatePattern;
			}

			if (thousandSeparator == string.Empty)
			{
				thousandSeparator = cultureInfo.NumberFormat.NumberGroupSeparator;
			}

			if (decimalSeparator == string.Empty)
			{
				decimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator;
			}

			jsOptions["inline"] = immediate;
			jsOptions["inlineFilters"] = inlineFilters;
			jsOptions["dateFormat"] = "\\'" + dateFormat + "\\'";
			jsOptions["thousandSeparator"] = "\\'" + thousandSeparator + "\\'";
			jsOptions["decimalSeparator"] = "\\'" + decimalSeparator + "\\'";
			jsOptions["display"] = display;
		}
	}

	#endregion

	#region Validation Generator

	/// <summary>
	/// The generator for the Zebda JS validator
	/// </summary>
	public class ZebdaWebValidationGenerator : IBrowserValidationGenerator
	{
		private readonly IDictionary attributes;

		/// <summary>
		/// Initializes a new instance of the <see cref="ZebdaWebValidationGenerator"/> class.
		/// </summary>
		/// <param name="inputType">Type of the input.</param>
		/// <param name="attributes">The attributes.</param>
		public ZebdaWebValidationGenerator(InputElementType inputType, IDictionary attributes)
		{
			this.attributes = attributes;
		}

		/// <summary>
		/// Set that a field should only accept digits.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="violationMessage">The violation message.</param>
		public void SetDigitsOnly(string target, string violationMessage)
		{
			AddZebdaAttribute("numeric", "{isFloat:false}");
			AddTitle(violationMessage);
		}

		/// <summary>
		/// Set that a field should only accept numbers.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="violationMessage">The violation message.</param>
		public void SetNumberOnly(string target, string violationMessage)
		{
			AddZebdaAttribute("numeric", "{isFloat:true}");
			AddTitle(violationMessage);
		}

		/// <summary>
		/// Sets that a field is required.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="violationMessage">The violation message.</param>
		public void SetAsRequired(string target, string violationMessage)
		{
			AddZebdaAttribute("required", "true");
			AddClass("required");
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
			AddZebdaAttribute("regexp", "{exp: " + regExp + "}");
			AddTitle(violationMessage);
		}

		/// <summary>
		/// Sets that a field value must be a valid email address.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="violationMessage">The violation message.</param>
		public void SetEmail(string target, string violationMessage)
		{
			AddZebdaAttribute("email", "true");
			AddTitle(violationMessage);
		}

		/// <summary>
		/// Sets that field must have an exact lenght.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="length">The length.</param>
		public void SetExactLength(string target, int length)
		{
			AddZebdaAttribute("length", "{max: " + length + ", min: " + length + "}");
		}

		/// <summary>
		/// Sets that field must have an exact lenght.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="length">The length.</param>
		/// <param name="violationMessage">The violation message.</param>
		public void SetExactLength(string target, int length, string violationMessage)
		{
			AddZebdaAttribute("length", "{max: " + length + ", min: " + length + "}");
			AddTitle(violationMessage);
		}

		/// <summary>
		/// Sets that field must have an minimum lenght.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="minLength">The minimum length.</param>
		public void SetMinLength(string target, int minLength)
		{
			AddZebdaAttribute("length", "{min: " + minLength + "}");
		}

		/// <summary>
		/// Sets that field must have an minimum lenght.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="minLength">The minimum length.</param>
		/// <param name="violationMessage">The violation message.</param>
		public void SetMinLength(string target, int minLength, string violationMessage)
		{
			AddZebdaAttribute("length", "{min: " + minLength + "}");
			AddTitle(violationMessage);
		}

		/// <summary>
		/// Sets that field must have an maximum lenght.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="maxLength">The maximum length.</param>
		public void SetMaxLength(string target, int maxLength)
		{
			AddZebdaAttribute("length", "{max: " + maxLength + "}");
		}

		/// <summary>
		/// Sets that field must have an maximum lenght.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="maxLength">The maximum length.</param>
		/// <param name="violationMessage">The violation message.</param>
		public void SetMaxLength(string target, int maxLength, string violationMessage)
		{
			AddZebdaAttribute("length", "{max: " + maxLength + "}");
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
			AddZebdaAttribute("length", "{max: " + maxLength + ", min: " + minLength + "}");
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
			AddZebdaAttribute("length", "{max: " + maxLength + ", min: " + minLength + "}");
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
			AddZebdaAttribute("numeric", "{isFloat:false, minValue: " + minValue + ", maxValue: " + maxValue + "}");
			AddTitle(violationMessage);
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
			AddZebdaAttribute("numeric", "{isFloat:true, minValue: " + minValue + ", maxValue: " + maxValue + "}");
			AddTitle(violationMessage);
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
			AddZebdaAttribute("compare", "{field:'" + comparisonFieldName + "'}");
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
		}

		/// <summary>
		/// Set that a field value must be a valid date.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="violationMessage">The violation message.</param>
		public void SetDate(string target, string violationMessage)
		{
			AddZebdaAttribute("date", "true");
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
		public void SetAsLesserThan( string target, string comparisonFieldName, IsLesserValidationType validationType, string violationMessage )
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
			AddZebdaAttribute("message", message);
		}

		private void AddZebdaAttribute(string attributeName, string attriubteValue)
		{
			if (attributeName.IndexOf("z:") == -1)
				attributeName = "z:" + attributeName;

			string existingAttributeValue = (string) attributes[attributeName];

			if (existingAttributeValue != null)
			{
				attributes[attributeName] = existingAttributeValue + " " + attriubteValue;
			}
			else
			{
				attributes[attributeName] = attriubteValue;
			}
		}

		private void AddClass(string className)
		{
			string existingClass = (string) attributes["class"];

			if (existingClass != null)
			{
				attributes["class"] = existingClass + " " + className;
			}
			else
			{
				attributes["class"] = className;
			}
		}
	}

	#endregion
}
