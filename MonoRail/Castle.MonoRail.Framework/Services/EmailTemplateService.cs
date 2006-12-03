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
	using System.Collections;
	using System.IO;
	using System.Text.RegularExpressions;
	using System.Web;

	using Castle.Components.Common.EmailSender;
	using Castle.Core;
	using Castle.Core.Logging;

	/// <summary>
	/// Default implementation of <see cref="IEmailTemplateService"/>
	/// </summary>
	/// <remarks>
	/// Will work only during a MonoRail process as it needs a <see cref="IRailsEngineContext"/>
	/// and a <see cref="Controller"/> instance to execute.
	/// </remarks>
	public class EmailTemplateService : IServiceEnabledComponent, IEmailTemplateService
	{
		/// <summary>
		/// The logger instance
		/// </summary>
		private ILogger logger = NullLogger.Instance;

		public EmailTemplateService()
		{
		}
		
		#region IServiceEnabledComponent implementation
		
		/// <summary>
		/// Invoked by the framework in order to give a chance to
		/// obtain other services
		/// </summary>
		/// <param name="provider">The service proviver</param>
		public void Service(IServiceProvider provider)
		{
			ILoggerFactory loggerFactory = (ILoggerFactory) provider.GetService(typeof(ILoggerFactory));
			
			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(EmailTemplateService));
			}
		}

		#endregion

		/// <summary>
		/// Creates an instance of <see cref="Message"/>
		/// using the specified template for the body
		/// </summary>
		/// <param name="templateName">
		/// Name of the template to load. 
		/// Will look in <c>Views/mail</c> for that template file.
		/// </param>
		/// <param name="parameters">
		/// Dictionary with parameters 
		/// that you can use on the email template
		/// </param>
		/// <returns>An instance of <see cref="Message"/></returns>
		public Message RenderMailMessage(String templateName, IDictionary parameters)
		{
			if (HttpContext.Current == null)
			{
				throw new RailsException("No http context available");
			}
			
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Rendering email message. Template name {0}", templateName);
			}

			IRailsEngineContext context = EngineContextModule.ObtainRailsEngineContext(HttpContext.Current);

			ControllerLifecycleExecutor executor = 
				(ControllerLifecycleExecutor) context.UnderlyingContext.Items[ControllerLifecycleExecutor.ExecutorEntry];
			
			Controller controller = executor.Controller;

			if (controller == null)
			{
				throw new RailsException("No controller found on the executing activity");
			}

			if (parameters != null && parameters.Count != 0)
			{
				foreach(DictionaryEntry entry in parameters)
				{
					controller.PropertyBag.Add(entry.Key, entry.Value);
				}
			}

			try
			{
				return RenderMailMessage(templateName, context, controller);
			}
			finally
			{
				if (parameters != null && parameters.Count != 0)
				{
					foreach(DictionaryEntry entry in parameters)
					{
						controller.PropertyBag.Remove(entry.Key);
					}
				}
			}
		}

		/// <summary>
		/// Creates an instance of <see cref="Message"/>
		/// using the specified template for the body
		/// </summary>
		/// <param name="templateName">
		/// Name of the template to load. 
		/// Will look in Views/mail for that template file.
		/// </param>
		/// <param name="context">Context that represents the current request</param>
		/// <param name="controller">Controller instance</param>
		/// <returns>An instance of <see cref="Message"/></returns>
		public Message RenderMailMessage(String templateName, IRailsEngineContext context, Controller controller)
		{
			// create a message object
			Message message = new Message();

			// use the template engine to generate the body of the message
			StringWriter writer = new StringWriter();

			controller.InPlaceRenderSharedView(writer, Path.Combine(Constants.EmailTemplatePath, templateName));

			String body = writer.ToString();
			
			// process delivery addresses from template.
			MatchCollection matches1 = Constants.readdress.Matches(body);
			
			for(int i=0; i< matches1.Count; i++)
			{
				String header  = matches1[i].Groups[Constants.HeaderKey].ToString().ToLower();
				String address = matches1[i].Groups[Constants.ValueKey].ToString();

				switch(header)
				{
					case Constants.To :
						message.To = address;
						break;
					case Constants.Cc :
						message.Cc = address;
						break;
					case Constants.Bcc :
						message.Bcc = address;
						break;
				}
			}
			
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Rendering email message to {0} cc {1} bcc {2}", message.To, message.Cc, message.Bcc);
			}

			body = Constants.readdress.Replace(body, String.Empty);

			// process from address from template
			Match match = Constants.refrom.Match(body);
			
			if (match.Success)
			{
				message.From = match.Groups[Constants.ValueKey].ToString();
				body = Constants.refrom.Replace(body, String.Empty);
			}

			// process subject and X headers from template
			MatchCollection matches2 = Constants.reheader.Matches(body);
			
			for(int i=0; i< matches2.Count; i++)
			{
				String header	= matches2[i].Groups[Constants.HeaderKey].ToString();
				String strval	= matches2[i].Groups[Constants.ValueKey].ToString();

				if (header.ToLower(System.Globalization.CultureInfo.InvariantCulture) == Constants.Subject)
				{
					message.Subject = strval;
				}
				else
				{
					message.Headers.Add(header, strval);
				}
				
				if (logger.IsDebugEnabled)
				{
					logger.DebugFormat("Adding header {0} value {1}", header, strval);
				}
			}
			
			body = Constants.reheader.Replace(body, String.Empty);

			message.Body = body;
			
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Email message body {0}", body);
			}

			// a little magic to see if the body is html
			if (message.Body.ToLower(System.Globalization.CultureInfo.InvariantCulture).IndexOf(Constants.HtmlTag) != -1)
			{
				message.Format = Format.Html;
				
				if (logger.IsDebugEnabled)
				{
					logger.Debug("Content set to Html");
				}
			}
			
			return message;
		}
	}
}
