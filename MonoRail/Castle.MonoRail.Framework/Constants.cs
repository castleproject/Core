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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Text.RegularExpressions;

	public abstract class Constants
	{
		#region Controller Constants

		/// <summary>
		/// TODO: Document why this is necessary
		/// </summary>
		public static readonly String ControllerContextKey = "rails.controller";

		/// <summary>
		/// TODO: Document why this is necessary
		/// </summary>
		internal static readonly String OriginalViewKey = "rails.original_view";

		#endregion

		#region Email Constants

		public const String To  = "to";
		public const String Cc  = "cc";
		public const String Bcc = "bcc";

		public static readonly String EmailTemplatePath  = "mail";
		public static readonly String ToAddressPattern   = @"[ \t]*(?<header>(to|cc|bcc)):[ \t]*(?<value>([\w-\.]+@([\w\.]){1,}\w+[ \t]*;?[ \t]*)+)[ \t]*(\r*\n*)?";
		public static readonly String FromAddressPattern = @"[ \t]*from:[ \t]*(?<value>(\w+[ \t]*)*<*[ \t]*[\w-\.]+@([\w\.]){1,}[\w][ \t]*>*)[ \t]*(\r*\n*)?";
		public static readonly String HeaderPattern      = @"[ \t]*(?<header>(subject|X-\w+)):[ \t]*(?<value>(\w+[ \t]*)+)(\r*\n*)?";
		public static readonly String HeaderKey          = "header";
		public static readonly String ValueKey           = "value";
		public static readonly String Subject            = "subject";
		public static readonly String HtmlTag            = "<html>";
		public static readonly String SmtpUsername       = "SMTP_USERNAME";
		public static readonly String SmtpPassword       = "SMTP_PASSWORD";
		public static readonly String SmtpUsernameSchema = "http://schemas.microsoft.com/cdo/configuration/sendusername";
		public static readonly String SmtpPasswordSchema = "http://schemas.microsoft.com/cdo/configuration/sendpassword";
		public static readonly String SmtpAuthSchema     = "http://schemas.microsoft.com/cdo/configuration/smtpauthenticate";
		public static readonly String SmtpAuthEnabled    = "1";
		public static readonly String SmtpServer         = "SMTP_SERVER";
		public static readonly String DefaultSmtpServer  = "localhost";

		public static readonly Regex readdress = new Regex(ToAddressPattern,   RegexOptions.IgnoreCase | RegexOptions.Compiled);
		public static readonly Regex refrom    = new Regex(FromAddressPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
		public static readonly Regex reheader  = new Regex(HeaderPattern,      RegexOptions.IgnoreCase | RegexOptions.Compiled);

		#endregion
	}
}
