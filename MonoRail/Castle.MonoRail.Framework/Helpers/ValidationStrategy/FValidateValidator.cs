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
	using Castle.Components.Validator;

	public class FValidateWebValidator : IWebValidatorProvider
	{
		/// <summary>
		/// Pendent
		/// </summary>
		/// <returns></returns>
		public WebValidationConfiguration CreateConfiguration(IDictionary parameters)
		{
			FValidateConfiguration config = new FValidateConfiguration();
			config.Configure(parameters);
			return config;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <returns></returns>
		public IWebValidationGenerator CreateGenerator(WebValidationConfiguration config, InputElementType inputType, IDictionary attributes)
		{
			return new FValidateGenerator(inputType, attributes);
		}

		#region Configuration

		public class FValidateConfiguration : WebValidationConfiguration
		{
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

		public class FValidateGenerator : IWebValidationGenerator
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

			public void SetAsRequired(string target, string violationMessage)
			{
				AddValidator(target, "blank");
				AddErrorMessage(violationMessage);
			}

			public void SetRegExp(string target, string regExp, string violationMessage)
			{
				throw new NotImplementedException();
			}

			public void SetEmail(string target, string violationMessage)
			{
				AddValidator(target, "email|1");
				AddErrorMessage(violationMessage);
			}

			public void SetDigitsOnly(string target, string violationMessage)
			{
				// TODO
			}

			public void SetNumberOnly(string target, string violationMessage)
			{
				// TODO
			}

			public void SetExactLength(string target, int length)
			{
				// Not supported
			}

			public void SetExactLength(string target, int length, string violationMessage)
			{
				// Not supported
			}

			public void SetMinLength(string target, int minLength)
			{
				SetMinLength(target, minLength, null);
			}

			public void SetMinLength(string target, int minLength, string violationMessage)
			{
				AddValidator(target, "length|" + minLength);
			}

			public void SetMaxLength(string target, int maxLength)
			{
				SetMaxLength(target, maxLength, null);
			}

			public void SetMaxLength(string target, int maxLength, string violationMessage)
			{
				// Not supported
			}

			public void SetLengthRange(string target, int minLength, int maxLength)
			{
				AddValidator(target, "length|" + minLength + "|" + maxLength);
			}

			public void SetLengthRange(string target, int minLength, int maxLength, string violationMessage)
			{
				AddValidator(target, "length|" + minLength + "|" + maxLength);
			}

			public void SetAsSameAs(string target, string comparisonFieldName, string violationMessage)
			{
				throw new NotImplementedException();
			}

			public void SetDate(string target, string violationMessage)
			{
				throw new NotImplementedException();
			}

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
