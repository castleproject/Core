// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework.Validators
{
	using System;


	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class EmailValidator : RegularExpressionValidator
	{
		/// <summary>
		/// From http://www.codeproject.com/aspnet/Valid_Email_Addresses.asp
		/// </summary>
		private static readonly String emailRule = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
			 @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + 
			 @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

		public EmailValidator() : base(emailRule)
		{
		}

		protected override string BuildErrorMessage()
		{
			return String.Format("Field {0} doesn't seem like a valid e-mail.", Property.Name);
		}
	}
}
