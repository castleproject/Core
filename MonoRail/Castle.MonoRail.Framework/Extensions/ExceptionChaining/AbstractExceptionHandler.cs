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
	using System.Text;

	/// <summary>
	/// Provides a basic implementation of <see cref="IExceptionHandler"/>
	/// </summary>
	public abstract class AbstractExceptionHandler : IExceptionHandler
	{
		private IExceptionHandler nextHandler;

		public virtual void Initialize()
		{
		}

		public abstract void Process(IRailsEngineContext context, IServiceProvider serviceProvider);

		public IExceptionHandler Next
		{
			get { return nextHandler; }
			set { nextHandler = value; }
		}

		protected void InvokeNext(IRailsEngineContext context, IServiceProvider serviceProvider)
		{
			if (nextHandler != null)
			{
				nextHandler.Process(context, serviceProvider);
			}
		}

		protected string BuildStandardMessage(IRailsEngineContext context)
		{
			StringBuilder sbMessage = new StringBuilder();
	
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
	
			sbMessage.Append("\r\nEnvironment and params");
			sbMessage.Append("\r\n======================\r\n\r\n");
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

			return sbMessage.ToString();
		}

		private void DumpDictionary(IDictionary dict, String title, StringBuilder message)
		{
			message.AppendFormat("{0}: \r\n", title);

			try
			{
				foreach(DictionaryEntry entry in dict)
				{
					message.AppendFormat("\r\n\t{0}:{1}", entry.Key, entry.Value);
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
					message.AppendFormat("\r\n\t{0}:{1} \r\n", key, dict[key]);
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
			message.AppendFormat("{0}Stack Trace:\r\n{0}{1}\r\n", space, exception.StackTrace);

			RecursiveDumpException(exception.InnerException, message, nested + 1);
		}
	}
}
