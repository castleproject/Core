// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	using System.IO;
	using System.Collections;


	/// <summary>
	/// Helper that provides client-side validation.
	/// </summary>
	/// <remarks>The javascript core lib is extension of Peter Bailey's 
	/// fValidate(http://www.peterbailey.net/fValidate/).</remarks>
	public class ValidationHelper : AbstractHelper
	{
		private const string AutoIncludeTag = "<script type=\"text/javascript\" src=\"{0}/{1}.{2}\"></script>\r\n";
		private const string UserIncludeTag = "<script type=\"text/javascript\" src=\"{0}\"></script>\r\n";

		private string _virtualDir;
		private IDictionary _submitOptions;
		private string _extension;

		/// <summary>
		/// Constructor.
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
		/// Gets or Sets the Virtual Directory the scripts are in.
		/// </summary>
		/// <remarks>The default is <see cref="Controller.Context.ApplicationPath"/>.</remarks>
		public virtual string VirtualDir
		{
			get
			{
				if (_virtualDir == null)
				{
					return Controller.Context.ApplicationPath;
				}

				return _virtualDir;
			}
			set
			{
				_virtualDir = value;
			}
		}

		protected string Extension
		{
			get
			{
				if (_extension == null)
				{
					if (Controller != null)
					{
						_extension = Controller.Context.UrlInfo.Extension;
					}
					else
					{
						_extension = "rails";
					}
				}
				
				return _extension;
			}
		}

		/// <summary>
		/// Automatic Script installer.
		/// </summary>
		/// <returns></returns>
		public string InstallScripts()
		{
			string baseDir = VirtualDir + @"/MonoRail/Files";

			return BuildScriptInclude(baseDir, "ValidateConfig", Extension) +
				BuildScriptInclude(baseDir, "ValidateCore", Extension) + 
				BuildScriptInclude(baseDir, "ValidateValidators", Extension) +
				BuildScriptInclude(baseDir, "ValidateLang", Extension);
		}

		protected virtual string BuildScriptInclude(string baseDir, string js, string extension)
		{
			return string.Format(AutoIncludeTag, baseDir, js, extension);
		}

		/// <summary>
		/// Install the script with a custom message file(based on the fValidate I18N file).
		/// </summary>
		/// <param name="scriptFilePath">A <see cref="String"/> represeting the path and file name</param>
		/// <returns></returns>
		public string InstallWithCustomMsg(string scriptFilePath)
		{
			string baseDir = VirtualDir + @"/MonoRail/Files";

			return BuildScriptInclude(baseDir, "ValidateConfig", Extension) +
				BuildScriptInclude(baseDir, "ValidateCore", Extension) + 
				BuildScriptInclude(baseDir, "ValidateValidators", Extension) +
				string.Format(UserIncludeTag, scriptFilePath);
		}

		/// <summary>
		/// Manual Script Installer.
		/// </summary>
		/// <param name="baseDir">The virtual path of the dir where the fValidate are.</param>
		/// <remarks>You'll need to have the files physically</remarks>
		/// <returns></returns>
		public string InstallScripts(string baseDir)
		{
			return InstallScripts(baseDir, "enUS");
		}

		/// <summary>
		/// Manual Script Installer.
		/// </summary>
		/// <param name="baseDir">The virtual path of the dir where the fValidate are.</param>
		/// <param name="lang">The language of the messages.</param>
		/// <remarks>You'll need to have the files physically</remarks>
		/// <returns></returns>
		public string InstallScripts(string baseDir, string lang)
		{
			return BuildScriptInclude(baseDir, "fValidate.config.js") +
				BuildScriptInclude(baseDir, "fValidate.core.js") + 
				BuildScriptInclude(baseDir, "fValidate.validators.js") +
				BuildScriptInclude(baseDir, "fValidate.lang-"+ lang +".js");
		}

		private string BuildScriptInclude(string baseDir, string js)
		{
			return string.Format(UserIncludeTag, Path.Combine(baseDir, js).Replace('\\','/'));
		}

		/// <summary>
		/// Returns the form validation function.
		/// </summary>
		/// <returns></returns>
		public string GetValidationTriggerFunction()
		{
			return GetValidationTriggerFunction("this");
		}

		/// <summary>
		/// Returns the form validation function.
		/// </summary>
		/// <param name="formElement">Javascript expression that return the desired form.</param>
		/// <returns></returns>
		public string GetValidationTriggerFunction(string formElement)
		{
			return string.Format("return validateForm( {0}, {1}, {2}, {3}, {4}, {5} );", 
				formElement,
			                     _submitOptions["confirm"].ToString().ToLower(),
			                     _submitOptions["disable"].ToString().ToLower(),
			                     _submitOptions["disable"].ToString().ToLower(),
			                     _submitOptions["groupError"].ToString().ToLower(),
			                     _submitOptions["errorMode"]);
		}

		/// <summary>
		/// Configure the submit and validation options.
		/// </summary>
		/// <param name="confirm"><b>True</b> for submit confirmation. Otherwise, <b>false</b>.</param>
		/// <param name="disable"><b>True</b> for submit buttons disabling.</param>
		/// <param name="groupError"><b>True</b> for error grouping.</param>
		/// <param name="errorMode"><see cref="Int32"/> representing the error mode.</param>
		public void SetSubmitOptions(bool confirm, bool disable, bool groupError, int errorMode)
		{
			_submitOptions["confirm"] = confirm;
			_submitOptions["disable"] = disable;
			_submitOptions["groupError"] = groupError;
			_submitOptions["errorMode"] = errorMode;
		}
	}
}