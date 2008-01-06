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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Collections;

	/// <summary>
	/// Helper that provides client-side validation.
	/// </summary>
	/// <remarks>The javascript core lib is extension of Peter Bailey's 
	/// fValidate(http://www.peterbailey.net/fValidate/).</remarks>
	public class ValidationHelper : AbstractHelper
	{
		private IDictionary _submitOptions;


		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationHelper"/> class.
		/// </summary>
		public ValidationHelper()
		{
			_submitOptions = new Hashtable();

			_submitOptions["confirm"] = false;
			_submitOptions["disable"] = false;
			_submitOptions["groupError"] = false;
			_submitOptions["errorMode"] = 0;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationHelper"/> class.
		/// setting the Controller, Context and ControllerContext.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		public ValidationHelper(IEngineContext engineContext)
			: base(engineContext)
		{
			_submitOptions = new Hashtable();

			_submitOptions["confirm"] = false;
			_submitOptions["disable"] = false;
			_submitOptions["groupError"] = false;
			_submitOptions["errorMode"] = 0;
		}

		/// <summary>
		/// Automatic Script installer.
		/// </summary>
		/// <returns></returns>
		public String InstallScripts()
		{
			return InstallScripts(string.Empty);
		}

		/// <summary>
		/// Installs the scripts.
		/// </summary>
		/// <param name="locale">The locale.</param>
		/// <returns></returns>
		public string InstallScripts(string locale)
		{
			string queryString = null;
			if (!string.IsNullOrEmpty(locale))
			{
				queryString = string.Format("Locale={0}", UrlEncode(locale));
			}
			return RenderScriptBlockToSource("/MonoRail/Files/ValidateConfig") + Environment.NewLine +
			       RenderScriptBlockToSource("/MonoRail/Files/ValidateCore") + Environment.NewLine +
			       RenderScriptBlockToSource("/MonoRail/Files/ValidateValidators") + Environment.NewLine +
			       RenderScriptBlockToSource("/MonoRail/Files/ValidateLang", queryString) + Environment.NewLine;
		}

		/// <summary>
		/// Configure the submit and validation options.
		/// </summary>
		public void SetSubmitOptions(IDictionary parameters)
		{
			_submitOptions["confirm"] = parameters["confirm"];
			_submitOptions["disable"] = parameters["disable"];
			_submitOptions["groupError"] = parameters["groupError"];
			_submitOptions["errorMode"] = parameters["errorMode"];
		}

		/// <summary>
		/// Configure the submit and validation options.
		/// </summary>
		/// <param name="confirm"><b>True</b> for submit confirmation. Otherwise, <b>false</b>.</param>
		/// <param name="disable"><b>True</b> for submit buttons disabling.</param>
		/// <param name="groupError"><b>True</b> for error grouping.</param>
		/// <param name="errorMode"><see cref="int"/> representing the error mode.</param>
		public void SetSubmitOptions(bool confirm, bool disable, bool groupError, int errorMode)
		{
			_submitOptions["confirm"] = confirm;
			_submitOptions["disable"] = disable;
			_submitOptions["groupError"] = groupError;
			_submitOptions["errorMode"] = errorMode;
		}

		/// <summary>
		/// Returns the form validation function.
		/// </summary>
		/// <returns></returns>
		public String GetValidationTriggerFunction()
		{
			return GetValidationTriggerFunction("this");
		}

		/// <summary>
		/// Returns the form validation function.
		/// </summary>
		/// <param name="formElement">Javascript expression that return the desired form.</param>
		/// <returns></returns>
		public String GetValidationTriggerFunction(String formElement)
		{
			return String.Format("return validateForm( {0}, {1}, {2}, {3}, {4}, {5} );",
			                     formElement,
			                     _submitOptions["confirm"].ToString().ToLower(),
			                     _submitOptions["disable"].ToString().ToLower(),
			                     _submitOptions["disable"].ToString().ToLower(),
			                     _submitOptions["groupError"].ToString().ToLower(),
			                     _submitOptions["errorMode"]);
		}

		/// <summary>
		/// Returns the form validation function where you can override the options:
		/// </summary>
		/// <remarks>
		/// The options that can be overriden:
		/// confirm (bool), disable (bool), groupError (bool), errorMode (int)
		/// </remarks>
		/// <param name="formElement">Javascript expression that return the desired form.</param>
		/// <param name="options">Custom options</param>
		/// <returns></returns>
		public String GetValidationTriggerFunction(String formElement, IDictionary options)
		{
			if (options == null)
			{
				options = new Hashtable(_submitOptions);
			}
			else
			{
				MergeOptions(options, _submitOptions);
			}

			return String.Format("return validateForm( {0}, {1}, {2}, {3}, {4}, {5} );",
			                     formElement,
			                     options["confirm"].ToString().ToLower(),
			                     options["disable"].ToString().ToLower(),
			                     options["disable"].ToString().ToLower(),
			                     options["groupError"].ToString().ToLower(),
			                     options["errorMode"]);
		}
	}
}
