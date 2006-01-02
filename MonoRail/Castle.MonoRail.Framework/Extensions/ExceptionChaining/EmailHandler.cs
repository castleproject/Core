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

namespace Castle.MonoRail.Framework.Extensions.ExceptionChaining
{
	using System;
	using System.Configuration;
	using System.Xml;

	using Castle.Components.Common.EmailSender;

	/// <summary>
	/// 
	/// </summary>
	public class EmailHandler : AbstractExceptionHandler, IConfigurableHandler
	{
		private String mailTo;
		private String mailFrom = "donotreply@castleproject.org";

		public void Configure(XmlNode exceptionHandlerNode)
		{
			XmlAttribute mailToAtt = exceptionHandlerNode.Attributes["mailto"];

			if (mailToAtt == null || mailToAtt.Value == String.Empty)
			{
				throw new ConfigurationException("'mailto' is a required attribute " + 
					"for EmailHandler (part of ExceptionChaining extension)");
			}

			mailTo = mailToAtt.Value;

			XmlAttribute mailFromAtt = exceptionHandlerNode.Attributes["mailfrom"];

			if (mailFromAtt != null && mailFromAtt.Value != String.Empty)
			{
				mailFrom = mailFromAtt.Value;
			}
		}

		public override void Process(IRailsEngineContext context, IServiceProvider serviceProvider)
		{
			IEmailSender emailSender = (IEmailSender) serviceProvider.GetService( typeof(IEmailSender) );

			String message = BuildStandardMessage(context);

			try
			{
				emailSender.Send( mailFrom, mailTo, 
					"MonoRail Exception: " + context.LastException.GetType().Name, message );
			}
			catch(Exception)
			{
				// We ignore errors during send
			}

			InvokeNext(context, serviceProvider);
		}
	}
}
