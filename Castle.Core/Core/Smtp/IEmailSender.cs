// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Smtp
{
	#if !SILVERLIGHT

	using System.Collections.Generic;
	using System.Net.Mail;

	/// <summary>
	/// Email sender abstraction.
	/// </summary>
	public interface IEmailSender
	{
		/// <summary>
		/// Sends a mail message.
		/// </summary>
		/// <param name="from">From field</param>
		/// <param name="to">To field</param>
		/// <param name="subject">E-mail's subject</param>
		/// <param name="messageText">message's body</param>
		void Send(string from, string to, string subject, string messageText);

		/// <summary>
		/// Sends a <see cref="MailMessage">message</see>. 
		/// </summary>
		/// <param name="message"><see cref="MailMessage">Message</see> instance</param>
		void Send(MailMessage message);

		/// <summary>
		/// Sends multiple <see cref="MailMessage">messages</see>. 
		/// </summary>
		/// <param name="messages">List of <see cref="MailMessage">messages</see></param>
		void Send(IEnumerable<MailMessage> messages);
	}
	#endif
}