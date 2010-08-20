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

	/// <summary>
	/// Validate that this is a valid (formatted) email using regex
	/// </summary>
	[Serializable]
	public class EmailValidator : RegularExpressionValidator
	{
		/// <summary>
		/// From http://www.codeproject.com/aspnet/Valid_Email_Addresses.asp
		/// </summary>
		private static readonly String emailRule = @"^([a-zA-Z0-9_\-\.\'\+]+)@((\[[0-9]{1,3}" +
		                                           @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
		                                           @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

		/// <summary>
		/// Initializes a new instance of the <see cref="EmailValidator"/> class.
		/// </summary>
		public EmailValidator() : base(emailRule)
		{
		}

		/// <summary>
		/// Gets a value indicating whether [supports browser validation].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [supports browser validation]; otherwise, <c>false</c>.
		/// </value>
		public override bool SupportsBrowserValidation
		{
			get { return true; }
		}

		/// <summary>
		/// Applies the browser validation.
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
			generator.SetEmail(target, BuildErrorMessage());
		}

		/// <summary>
		/// Returns the key used to internationalize error messages
		/// </summary>
		/// <value></value>
		protected override string MessageKey
		{
			get { return MessageConstants.InvalidEmailMessage; }
		}
	}
}
