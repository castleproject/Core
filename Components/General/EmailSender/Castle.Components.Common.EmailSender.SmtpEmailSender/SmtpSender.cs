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

namespace Castle.Components.Common.EmailSender.SmtpEmailSender
{
	using System;
	using System.Collections;
	using System.Web.Mail;

	using Castle.Components.Common.EmailSender;

	/// <summary>
	/// Uses Smtp to send emails.
	/// </summary>
	public class SmtpSender : IEmailSender
	{
		private readonly String hostname;

		/// <summary>
		/// This service implementation requires a host name (valid one, if possible)
		/// in order to work
		/// </summary>
		/// <param name="hostname">The smtp server name</param>
		public SmtpSender(String hostname)
		{
			this.hostname = hostname;

			SmtpMail.SmtpServer = hostname;
		}

		public String Hostname
		{
			get { return hostname; }
		}

		public void Send(String from, String to, String subject, String messageText)
		{
			if (from == null) throw new ArgumentNullException("from");
			if (to == null) throw new ArgumentNullException("to");
			if (subject == null) throw new ArgumentNullException("subject");
			if (messageText == null) throw new ArgumentNullException("messageText");

			SmtpMail.Send( from, to, subject, messageText );
		}

		public void Send(Message message)
		{
			if (message == null) throw new ArgumentNullException("message");

			SmtpMail.Send( MailMessageFrom(message) );
		}

		public void Send(Message[] messages)
		{
			foreach( Message message in messages )
			{
				Send( message );
			}
		}

		private MailMessage MailMessageFrom(Message message)
		{
			MailMessage mailMessage = new MailMessage();

			mailMessage.From = message.From;
			mailMessage.To = message.To;
			mailMessage.Cc = message.Cc;
			mailMessage.Bcc = message.Bcc;
			mailMessage.Subject = message.Subject;
			mailMessage.Body = message.Body;
			mailMessage.BodyEncoding = message.Encoding;
			mailMessage.BodyFormat = (MailFormat) Enum.Parse( typeof(MailFormat), message.Format.ToString() );
			mailMessage.Priority = (MailPriority) Enum.Parse( typeof(MailPriority), message.Priority.ToString() );
			
			foreach(DictionaryEntry entry in message.Headers)
			{
				mailMessage.Headers.Add(entry.Key, entry.Value);
			}

            foreach(DictionaryEntry entry in message.Fields)
            {
                mailMessage.Fields.Add(entry.Key, entry.Value);
            }

			foreach(MessageAttachment attachment in message.Attachments)
			{
				MailEncoding enc = (MailEncoding) Enum.Parse( typeof(MailEncoding), attachment.Encoding.ToString() );

				MailAttachment mailAttach = new MailAttachment(attachment.FileName, enc);
				
				mailMessage.Attachments.Add( mailAttach );
			}

			return mailMessage;
		}
	}
}
