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
	using System.Collections;
	using Castle.Components.Validator;
	using Castle.MonoRail.Framework.Internal;

	public class PrototypeWebValidator : IWebValidatorProvider
	{
		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public WebValidationConfiguration CreateConfiguration(IDictionary parameters)
		{
			PrototypeValidationConfiguration config = new PrototypeValidationConfiguration();

			config.Configure(parameters);

			return config;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="inputType"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public IWebValidationGenerator CreateGenerator(InputElementType inputType, IDictionary attributes)
		{
			return new PrototypeValidationGenerator(inputType, attributes);
		}

		#region Configuration

		/// <summary>
		/// 
		/// </summary>
		public class PrototypeValidationConfiguration : WebValidationConfiguration
		{
			private IDictionary jsOptions = new Hashtable();

			public override string CreateBeforeFormClosed(string formId)
			{
				return AbstractHelper.ScriptBlock("new Validation('" + formId + "', " + 
					AjaxHelper.JavascriptOptions(jsOptions) + ");");
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
			}
		}

		#endregion

		#region Validation Generator

		/// <summary>
		/// 
		/// </summary>
		public class PrototypeValidationGenerator : IWebValidationGenerator
		{
			private readonly InputElementType inputType;
			private readonly IDictionary attributes;

			/// <summary>
			/// Initializes a new instance of the <see cref="PrototypeValidationGenerator"/> class.
			/// </summary>
			/// <param name="inputType">Type of the input.</param>
			/// <param name="attributes">The attributes.</param>
			public PrototypeValidationGenerator(InputElementType inputType, IDictionary attributes)
			{
				this.inputType = inputType;
				this.attributes = attributes;
			}

			public void SetAsRequired(string violationMessage)
			{
				if (inputType == InputElementType.Text)
				{
					AddClass("required");
				}
				else if (inputType == InputElementType.Select)
				{
					AddClass("validate-selection");
				}

				AddTitle(violationMessage);
			}

			public void SetDigitsOnly(string violationMessage)
			{
				AddClass("validate-digits");
				AddTitle(violationMessage);
			}

			public void SetNumberOnly(string violationMessage)
			{
				AddClass("validate-number");
				AddTitle(violationMessage);
			}

			public void SetEmail(string violationMessage)
			{
				AddClass("validate-email");
				AddTitle(violationMessage);
			}

			public void SetRegExp(string regExp, string violationMessage)
			{
				// Not supported by the prototype validation
			}

			public void SetExactLength(int length)
			{
				// Not supported by the prototype validation
			}

			public void SetMinLength(int minLength)
			{
				// Not supported by the prototype validation
			}

			public void SetMaxLength(int maxLength)
			{
				// Not supported by the prototype validation, 
				// but we can set the maxlength on the input element

				if (inputType == InputElementType.Text)
				{
					attributes["maxlength"] = maxLength;
				}
			}

			public void SetLengthRange(int minLength, int maxLength)
			{
				// Not supported by the prototype validation
			}

			private void AddTitle(string message)
			{
				string existingTitle = (string) attributes["title"];

				if (existingTitle != null)
				{
					attributes["title"] = existingTitle + ", " + message;
				}
				else
				{
					attributes["title"] = message;
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
		}

		#endregion
	}
}
