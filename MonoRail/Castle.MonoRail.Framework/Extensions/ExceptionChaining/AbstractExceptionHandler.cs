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
	using System.Collections;
	using System.Collections.Specialized;
	using System.Text;

	/// <summary>
	/// Provides a basic implementation of <see cref="IExceptionHandler"/>
	/// </summary>
	public abstract class AbstractExceptionHandler : IExceptionHandler
	{
		private IExceptionHandler nextHandler;

		/// <summary>
		/// Implementors should perform any required
		/// initialization
		/// </summary>
		public virtual void Initialize()
		{
		}

		/// <summary>
		/// Implementors should perform the action
		/// on the exception. Note that the exception
		/// is available in <see cref="IEngineContext.LastException"/>
		/// </summary>
		/// <param name="context"></param>
		public abstract void Process(IEngineContext context);

		/// <summary>
		/// The next exception in the sink
		/// or null if none exists.
		/// </summary>
		/// <value></value>
		public IExceptionHandler Next
		{
			get { return nextHandler; }
			set { nextHandler = value; }
		}

		/// <summary>
		/// Invokes the next handler.
		/// </summary>
		/// <param name="context">The context.</param>
		protected void InvokeNext(IEngineContext context)
		{
			if (nextHandler != null)
			{
				nextHandler.Process(context);
			}
		}

		/// <summary>
		/// Builds the standard message.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		protected string BuildStandardMessage(IEngineContext context)
		{
			StringBuilder sbMessage = new StringBuilder();
	
			sbMessage.Append("Controller details\r\n");
			sbMessage.Append("==================\r\n\r\n");
			sbMessage.AppendFormat("Url {0}\r\n", context.Request.Url);
			sbMessage.AppendFormat("Area {0}\r\n", context.UrlInfo.Area);
			sbMessage.AppendFormat("Controller {0}\r\n", context.UrlInfo.Controller);
			sbMessage.AppendFormat("Action {0}\r\n", context.UrlInfo.Action);
			sbMessage.AppendFormat("Extension {0}\r\n\r\n", context.UrlInfo.Extension);
	
			sbMessage.Append("Exception details\r\n");
			sbMessage.Append("=================\r\n\r\n");
			sbMessage.AppendFormat("Exception occured at {0}\r\n", DateTime.Now);
			RecursiveDumpException(context.LastException, sbMessage, 0);
	
			sbMessage.Append("\r\nEnvironment and params");
			sbMessage.Append("\r\n======================\r\n\r\n");
			sbMessage.AppendFormat("Machine {0}\r\n", Environment.MachineName);
			sbMessage.AppendFormat("ApplicationPath {0}\r\n", context.ApplicationPath);
//			sbMessage.AppendFormat("ApplicationPhysicalPath {0}\r\n", context.ApplicationPhysicalPath);
			sbMessage.AppendFormat("HttpMethod {0}\r\n", context.Request.HttpMethod);
			sbMessage.AppendFormat("UrlReferrer {0}\r\n", context.Request.UrlReferrer);
	
			if (context.CurrentUser != null)
			{
				sbMessage.AppendFormat("CurrentUser.Name {0}\r\n", context.CurrentUser.Identity.Name);
				sbMessage.AppendFormat("CurrentUser.AuthenticationType {0}\r\n", context.CurrentUser.Identity.AuthenticationType);
				sbMessage.AppendFormat("CurrentUser.IsAuthenticated {0}\r\n", context.CurrentUser.Identity.IsAuthenticated);
			}
	
			DumpDictionary(context.Flash, "Flash", sbMessage);
			DumpDictionary(context.Request.QueryString, "QueryString", sbMessage);
			DumpDictionary(context.Request.Form, "Form", sbMessage);
			DumpDictionary(context.Session, "Session", sbMessage);

			return sbMessage.ToString();
		}

		private void DumpDictionary(IDictionary dict, String title, StringBuilder message)
		{
			if (dict == null || dict.Count == 0)
			{
				return;
			}

			message.AppendFormat("\r\n{0}: \r\n", title);

			try
			{
				foreach(string key in dict.Keys)
				{
					message.AppendFormat("\r\n\t{0}: {1}", key, dict[key]);
				}
			}
			catch(Exception)
			{
				// Ignores
			}
		}

		private void DumpDictionary(NameValueCollection dict, String title, StringBuilder message)
		{
			if (dict == null || dict.Count == 0)
			{
				return;
			}

			message.AppendFormat("\r\n{0}: \r\n", title);

			try
			{
				foreach(String key in dict.Keys)
				{
					message.AppendFormat("\r\n\t{0}: {1} \r\n", key, dict[key]);
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

			message.AppendFormat("\r\n{0}Exception: {1}\r\n", space, exception.Message);
			message.AppendFormat("{0}Stack Trace:\r\n{0}{1}\r\n", space, exception.StackTrace);

			RecursiveDumpException(exception.InnerException, message, nested + 1);
		}
	}
}
