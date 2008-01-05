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

namespace Castle.MonoRail.Framework.Extensions.ExceptionChaining
{
	using System;
	using System.Configuration;
	using Castle.Components.Common.EmailSender;
	using Castle.Core.Configuration;

	/// <summary>
	/// A handle that sends the exception by email
	/// </summary>
	public class EmailHandler : AbstractExceptionHandler, IConfigurableHandler
	{
		private String mailTo;
		private String mailFrom = "donotreply@castleproject.org";

		/// <summary>
		/// Implementors should check for known attributes and child nodes
		/// within the <c>exceptionHandlerNode</c>
		/// </summary>
		/// <param name="exceptionHandlerNode">The Xml node
		/// that represents this handler on the configuration file</param>
		public void Configure(IConfiguration exceptionHandlerNode)
		{
			string mailToAtt = exceptionHandlerNode.Attributes["mailTo"];

			if (mailToAtt == null)
			{
				String message = "'mailTo' is a required attribute " +
				                 "for EmailHandler (part of ExceptionChaining extension)";
				throw new ConfigurationErrorsException(message);
			}

			mailTo = mailToAtt;

			string mailFromAtt = exceptionHandlerNode.Attributes["mailFrom"];

			if (mailFromAtt != null)
			{
				mailFrom = mailFromAtt;
			}
		}

		/// <summary>
		/// Implementors should perform the action
		/// on the exception. Note that the exception
		/// is available in <see cref="IEngineContext.LastException"/>
		/// </summary>
		/// <param name="context"></param>
		public override void Process(IEngineContext context)
		{
			IEmailSender emailSender = (IEmailSender) context.GetService(typeof(IEmailSender));

			String message = BuildStandardMessage(context);

			try
			{
				emailSender.Send(mailFrom, mailTo,
				                 "MonoRail Exception: " + context.LastException.GetType().Name, message);
			}
			catch(Exception)
			{
				// We ignore errors during send
			}

			InvokeNext(context);
		}
	}
}