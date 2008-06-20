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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Text;
	using System.Text.RegularExpressions;
	using Castle.Components.Common.EmailSender;
	using Castle.Core;
	using Castle.Core.Logging;

	/// <summary>
	/// Default implementation of <see cref="IEmailTemplateService"/>
	/// </summary>
	/// <remarks>
	/// Will work only during a MonoRail process as it needs a <see cref="IEngineContext"/>
	/// and a <see cref="Controller"/> instance to execute.
	/// </remarks>
	public class EmailTemplateService : IMRServiceEnabled, IEmailTemplateService
	{
		private static readonly String HeaderPattern = @"[ \t]*(?<header>(to|from|cc|bcc|subject|X-\w+)):[ \t]*(?<value>(.)+)(\r*\n*)?";
		private static readonly Regex HeaderRegEx = new Regex(HeaderPattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static readonly string EmailTemplatePath = "mail";

		/// <summary>
		/// The logger instance
		/// </summary>
		private ILogger logger = NullLogger.Instance;

		private IViewEngineManager viewEngineManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="EmailTemplateService"/> class.
		/// </summary>
		public EmailTemplateService()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EmailTemplateService"/> class.
		/// </summary>
		/// <param name="viewEngineManager">The view engine manager.</param>
		public EmailTemplateService(IViewEngineManager viewEngineManager)
		{
			this.viewEngineManager = viewEngineManager;
		}

		#region IMRServiceEnabled

		/// <summary>
		/// Invoked by the framework in order to give a chance to
		/// obtain other services
		/// </summary>
		/// <param name="serviceProvider">The service proviver</param>
		public void Service(IMonoRailServices serviceProvider)
		{
			ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();

			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(EmailTemplateService));
			}

			viewEngineManager = serviceProvider.ViewEngineManager;
		}

		#endregion

		/// <summary>
		/// Determines whether the specified template exists in the e-mail
		/// template folder (<c>views/mail</c>).
		/// </summary>
		/// <param name="templateName">Name of the e-mail template.</param>
		/// <returns>
		/// 	<c>true</c> if the template exists; otherwise, <c>false</c>.
		/// </returns>
		public bool HasMailTemplate(string templateName)
		{
			string normalized = NormalizeTemplatePath(templateName);

			return viewEngineManager.HasTemplate(normalized);
		}

		/// <summary>
		/// Creates an instance of <see cref="Message"/>
		/// using the specified template for the body
		/// </summary>
		/// <param name="templateName">Name of the template to load.
		/// Will look in <c>Views/mail</c> for that template file.</param>
		/// <param name="layoutName">Name of the layout.</param>
		/// <param name="parameters">Dictionary with parameters
		/// that you can use on the email template</param>
		/// <returns>An instance of <see cref="Message"/></returns>
		public Message RenderMailMessage(string templateName, string layoutName, object parameters)
		{
			return RenderMailMessage(templateName, layoutName, new ReflectionBasedDictionaryAdapter(parameters));
		}

		/// <summary>
		/// Creates an instance of <see cref="Message"/>
		/// using the specified template for the body
		/// </summary>
		/// <param name="templateName">Name of the template to load.
		/// Will look in <c>Views/mail</c> for that template file.</param>
		/// <param name="layoutName">Name of the layout.</param>
		/// <param name="parameters">Dictionary with parameters
		/// that you can use on the email template</param>
		/// <returns>An instance of <see cref="Message"/></returns>
		public Message RenderMailMessage(string templateName, string layoutName, IDictionary parameters)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Rendering email message. Template name {0}", templateName);
			}

			if (!HasMailTemplate(templateName))
			{
				throw new MonoRailException("Template for e-mail doesn't exist: " + templateName);
			}

			templateName = NormalizeTemplatePath(templateName);

			// use the template engine to generate the body of the message
			StringWriter writer = new StringWriter();

			viewEngineManager.Process(templateName, layoutName, writer, new StringObjectDictionaryAdapter(parameters));

			return CreateMessage(writer);
		}

		/// <summary>
		/// Creates an instance of <see cref="Message"/>
		/// using the specified template for the body
		/// </summary>
		/// <param name="templateName">Name of the template to load.
		/// Will look in Views/mail for that template file.</param>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">Controller instance</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="doNotApplyLayout">If <c>true</c>, it will skip the layout</param>
		/// <returns>An instance of <see cref="Message"/></returns>
		public Message RenderMailMessage(string templateName, IEngineContext engineContext,
		                                 IController controller, IControllerContext controllerContext, bool doNotApplyLayout)
		{
			// use the template engine to generate the body of the message
			StringWriter writer = new StringWriter();

			String[] oldLayout = controllerContext.LayoutNames;

			if (doNotApplyLayout)
			{
				controllerContext.LayoutNames = null;
			}

			if (!HasMailTemplate(templateName))
			{
				throw new MonoRailException("Template for e-mail doesn't exist: " + templateName);
			}

			templateName = NormalizeTemplatePath(templateName);

			viewEngineManager.Process(templateName, writer, engineContext, controller, controllerContext);

			controllerContext.LayoutNames = oldLayout;

			return CreateMessage(writer);
		}

		private string NormalizeTemplatePath(string templateName)
		{
			if (!templateName.StartsWith("/"))
			{
				templateName = Path.Combine(EmailTemplatePath, templateName);
			}
			return templateName;
		}

		private Message CreateMessage(StringWriter writer)
		{
			// create a message object
			Message message = new Message();

			StringReader reader = new StringReader(writer.ToString());

			bool isInBody = false;
			StringBuilder body = new StringBuilder();
			string line;

			while((line = reader.ReadLine()) != null)
			{
				string header, value;
				if (!isInBody && IsLineAHeader(line, out header, out value))
				{
					switch(header.ToLowerInvariant())
					{
						case "to":
							message.To = value;
							break;
						case "cc":
							message.Cc = value;
							break;
						case "bcc":
							message.Bcc = value;
							break;
						case "subject":
							message.Subject = value;
							break;
						case "from":
							message.From = value;
							break;
						default:
							message.Headers[header] = value;
							break;
					}
				}
				else
				{
					isInBody = true;

					if (line == string.Empty)
					{
						continue;
					}

					body.AppendLine(line);
				}
			}

			message.Body = body.ToString();

			if (message.Body.ToLowerInvariant().IndexOf("<html>") != -1)
			{
				message.Format = Format.Html;
			}

			return message;
		}

		private static bool IsLineAHeader(string line, out string header, out string value)
		{
			Match match = HeaderRegEx.Match(line);

			if (match.Success)
			{
				header = match.Groups["header"].ToString().ToLower();
				value = match.Groups["value"].ToString();
				return true;
			}
			else
			{
				header = value = null;
				return false;
			}
		}
	}
}