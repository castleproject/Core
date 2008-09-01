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
	using Castle.Components.Validator;

	/// <summary>
	/// Implementation of a browser validator that uses the <c>fValidate</c>
	/// javascript library.
	/// </summary>
	public class FValidateWebValidator : IBrowserValidatorProvider
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
			FValidateConfiguration config = new FValidateConfiguration();
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
			return new FValidateGenerator(inputType, attributes);
		}

		#region Configuration

		/// <summary>
		/// Supported configuration for fValidate.
		/// </summary>
		public class FValidateConfiguration : BrowserValidationConfiguration
		{
			/// <summary>
			/// Configures the JS library based on the supplied parameters.
			/// </summary>
			/// <param name="parameters">The parameters.</param>
			public override void Configure(IDictionary parameters)
			{
				// ( f, bConfirm, bDisable, bDisableR, groupError, errorMode )
				parameters["onsubmit"] = "return validateForm( this, 0, 1, 0, 1, 16 );";
				/*
			case 0  : alertError(); break;
			case 1  : inputError(); break;
			case 2  : labelError(); break;
			case 3  : appendError(); break;
			case 4  : boxError(); break;
			case 5  : inputError(); labelError(); break;
			case 6  : inputError(); appendError(); break;
			case 7  : inputError(); boxError(); break;
			case 8  : inputError(); alertError(); break;
			case 9  : labelError(); appendError(); break;
			case 10 : labelError(); boxError(); break;
			case 11 : labelError(); alertError(); break;
			case 12 : appendError(); boxError(); break;
			case 13 : appendError(); alertError(); break;
			case 14 : boxError(); alertError(); break;
			case 15 : inputError(); labelError(); appendError(); break;
			case 16 : inputError(); labelError(); boxError(); break;
			case 17 : inputError(); labelError(); alertError(); break;
			case 18 : inputError(); appendError(); boxError(); break;
			case 19 : inputError(); appendError(); alertError(); break;
			case 20 : inputError(); boxError(); alertError(); break;
			case 21 : labelError(); appendError(); boxError(); break;
			case 22 : labelError(); appendError(); alertError(); break;
			case 23 : appendError(); boxError(); alertError(); break;
			case 24 : inputError(); labelError(); appendError(); boxError(); break;
			case 25 : inputError(); labelError(); appendError(); alertError(); break;
			case 26 : inputError(); appendError(); boxError(); alertError(); break;
			case 27 : labelError(); appendError(); boxError(); alertError(); break;
			case 28 : inputError(); labelError(); appendError(); boxError(); alertError(); break;
				 */
			}
		}

		#endregion 

		#region Generator

		/// <summary>
		/// Generator for fValidate validation.
		/// </summary>
		public class FValidateGenerator : IBrowserValidationGenerator
		{
			private readonly InputElementType inputType;
			private readonly IDictionary attributes;

			/// <summary>
			/// Initializes a new instance of the <see cref="FValidateGenerator"/> class.
			/// </summary>
			/// <param name="inputType">Type of the input.</param>
			/// <param name="attributes">The attributes.</param>
			public FValidateGenerator(InputElementType inputType, IDictionary attributes)
			{
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
				AddValidator(target, "blank");
				AddErrorMessage(violationMessage);
			}

			/// <summary>
			/// Sets that a field value must match the specified regular expression.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="regExp">The reg exp.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetRegExp(string target, string regExp, string violationMessage)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// Sets that a field value must be a valid email address.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetEmail(string target, string violationMessage)
			{
				AddValidator(target, "email|1");
				AddErrorMessage(violationMessage);
			}

			/// <summary>
			/// Set that a field should only accept digits.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetDigitsOnly(string target, string violationMessage)
			{
			}

			/// <summary>
			/// Set that a field should only accept numbers.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetNumberOnly(string target, string violationMessage)
			{
			}

			/// <summary>
			/// Sets that field must have an exact lenght.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="length">The length.</param>
			public void SetExactLength(string target, int length)
			{
				// Not supported
			}

			/// <summary>
			/// Sets that field must have an exact lenght.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="length">The length.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetExactLength(string target, int length, string violationMessage)
			{
				// Not supported
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
				AddValidator(target, "length|" + minLength);
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
				// Not supported
			}

			/// <summary>
			/// Sets that field must be between a length range.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minLength">The minimum length.</param>
			/// <param name="maxLength">The maximum length.</param>
			public void SetLengthRange(string target, int minLength, int maxLength)
			{
				AddValidator(target, "length|" + minLength + "|" + maxLength);
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
				AddValidator(target, "length|" + minLength + "|" + maxLength);
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
			}

			/// <summary>
			/// Sets that a field's value must greater than another field's value.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="comparisonFieldName">The name of the field to compare with.</param>
			/// <param name="validationType">The type of data to compare.</param>
			/// <param name="violationMessage">The violation message.</param>
			/// <remarks>Not implemented by the JQuery validate plugin. Done via a custom rule.</remarks>
			public void SetAsGreaterThan( string target, string comparisonFieldName, IsGreaterValidationType validationType, string violationMessage )
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

			/// <summary>
			/// Adds the validator.
			/// </summary>
			/// <param name="target">The target.</param>
			/// <param name="validator">The validator.</param>
			private void AddValidator(string target, string validator)
			{
				string existingValidators = (string) attributes["validators"];

				if (existingValidators != null)
				{
					attributes["validators"] = existingValidators + "|" + validator;
				}
				else
				{
					attributes["validators"] = validator;
				}
			}

			private void AddErrorMessage(string violationMessage)
			{
				string existingMessage = (string) attributes["emsg"];

				if (existingMessage != null)
				{
					attributes["emsg"] = existingMessage + "," + violationMessage;
				}
				else
				{
					attributes["emsg"] = violationMessage;
				}
			}
		}

		#endregion
	}
}
