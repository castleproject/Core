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

namespace Castle.MonoRail.Framework.Services
{
	using System;

	using Castle.Components.Common.EmailSender;
	using Castle.Components.Common.EmailSender.SmtpEmailSender;
	using Castle.Core;
	using Castle.MonoRail.Framework.Configuration;

	/// <summary>
	/// Pendent
	/// </summary>
	public class MonoRailSmtpSender : IEmailSender, IServiceEnabledComponent
	{
		private SmtpSender sender;

		public MonoRailSmtpSender()
		{
		}
		
		#region IServiceEnabledComponent implementation

		public void Service(IServiceProvider provider)
		{
			MonoRailConfiguration config = (MonoRailConfiguration) 
				provider.GetService(typeof(MonoRailConfiguration));
			
			sender = new SmtpSender(config.SmtpConfig.Host);

#if DOTNET2
			// config.SmtpConfig.Username
			// config.SmtpConfig.Password
#endif
		}

		#endregion

		public void Send(string from, string to, string subject, string messageText)
		{
			sender.Send(from, to, subject, messageText);
		}

		public void Send(Message message)
		{
			sender.Send(message);
		}

		public void Send(Message[] messages)
		{
			sender.Send(messages);
		}
	}
}
