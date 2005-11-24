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

namespace Castle.MonoRail.Framework.Extensions.ExceptionChaining
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Configuration;
	using System.Text;
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

			StringBuilder sbMessage = new StringBuilder();

			sbMessage.Append("Hello, this message was sent by EmailHandler (part of MonoRail framework) ");
			sbMessage.Append("which was enabled possibly by you. The following details an unexpected exception ");
			sbMessage.Append("that happened during the process of an action. It might or might not be handled by  ");
			sbMessage.Append("a rescue.\r\n\r\n");

			sbMessage.Append("Controller details\r\n");
			sbMessage.Append("==================\r\n\r\n");
			sbMessage.AppendFormat("Url {0}\r\n", context.Url);
			sbMessage.AppendFormat("Area {0}\r\n", context.UrlInfo.Area);
			sbMessage.AppendFormat("Controller {0}\r\n", context.UrlInfo.Controller);
			sbMessage.AppendFormat("Action {0}\r\n", context.UrlInfo.Action);
			sbMessage.AppendFormat("Extension {0}\r\n\r\n", context.UrlInfo.Extension);

			sbMessage.Append("Exception details\r\n");
			sbMessage.Append("=================\r\n\r\n");
			sbMessage.AppendFormat("Exception occured at {0}\r\n", DateTime.Now);
			RecursiveDumpException(context.LastException, sbMessage, 0);

			sbMessage.Append("\r\nEnvironment and params\r\n");
			sbMessage.Append("======================\r\n\r\n");
			sbMessage.AppendFormat("ApplicationPath {0}\r\n", context.ApplicationPath);
			sbMessage.AppendFormat("ApplicationPhysicalPath {0}\r\n", context.ApplicationPhysicalPath);
			sbMessage.AppendFormat("RequestType {0}\r\n", context.RequestType);
			sbMessage.AppendFormat("UrlReferrer {0}\r\n", context.UrlReferrer);

			if (context.CurrentUser != null)
			{
				sbMessage.AppendFormat("CurrentUser.Name {0}\r\n", context.CurrentUser.Identity.Name);
				sbMessage.AppendFormat("CurrentUser.AuthenticationType {0}\r\n", context.CurrentUser.Identity.AuthenticationType);
				sbMessage.AppendFormat("CurrentUser.IsAuthenticated {0}\r\n", context.CurrentUser.Identity.IsAuthenticated);
			}

			DumpDictionary(context.Flash, "Flash", sbMessage);

			DumpDictionary(context.Params, "Params", sbMessage);

			DumpDictionary(context.Session, "Session", sbMessage);

			try
			{
				emailSender.Send( mailFrom, mailTo, "MonoRail Exception", sbMessage.ToString() );
			}
			catch(Exception)
			{
				// We ignore errors during send
			}

			InvokeNext(context, serviceProvider);
		}

		private void DumpDictionary(IDictionary dict, String title, StringBuilder message)
		{
			message.AppendFormat("{0}: \r\n", title);

			try
			{
				foreach(DictionaryEntry entry in dict)
				{
					message.AppendFormat("    {0}:{1} \r\n", entry.Key, entry.Value);
				}
			}
			catch(Exception)
			{
				// Ignores
			}
		}

		private void DumpDictionary(NameValueCollection dict, String title, StringBuilder message)
		{
			message.AppendFormat("{0}: \r\n", title);

			try
			{
				foreach(String key in dict.Keys)
				{
					message.AppendFormat("    {0}:{1} \r\n", key, dict[key]);
				}
			}
			catch(Exception)
			{
				// Ignores
			}
		}

		private void RecursiveDumpException(Exception exception, StringBuilder message, int nested)
		{
			if (exception == null) return;

			char[] spaceBuff = new char[nested * 2]; 
			for(int i = 0; i < nested * 2; i++) spaceBuff[i] = ' ';

			String space = new String(spaceBuff);

			message.AppendFormat("{0}Exception: {1}\r\n", space, exception.Message);
			message.AppendFormat("{0}Stack Trace:\r\n{1}\r\n\r\n", space, exception.StackTrace);

			RecursiveDumpException(exception.InnerException, message, nested + 1);
		}
	}
}
