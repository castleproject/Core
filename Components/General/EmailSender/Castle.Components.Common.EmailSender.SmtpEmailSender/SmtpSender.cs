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

namespace Castle.Components.Common.EmailSender.SmtpEmailSender
{
	using System;
	using System.Collections;
#if DOTNET2
	using System.Net.Mail;
#else
	using System.Web.Mail;
	using MailMessage=System.Web.Mail.MailMessage;
	using MailPriority=System.Web.Mail.MailPriority;
#endif

	/// <summary>
	/// Uses Smtp to send emails.
	/// </summary>
	public class SmtpSender : IEmailSender
	{
		private readonly String hostname;
#if DOTNET2
		private SmtpClient smtpClient;
		private bool asyncSend = false;
#endif

		/// <summary>
		/// This service implementation requires a host name (valid one, if possible)
		/// in order to work
		/// </summary>
		/// <param name="hostname">The smtp server name</param>
		public SmtpSender(String hostname)
		{
			this.hostname = hostname;

#if DOTNET2
			this.smtpClient = new SmtpClient(hostname);
#else
			SmtpMail.SmtpServer = hostname;
#endif
		}

		public String Hostname
		{
			get { return hostname; }
		}
		
#if DOTNET2
		/// <summary>
		/// Gets or sets a value which is used to configure if emails are going to be sent asyncrhonously or not.
		/// </summary>
		public bool AsyncSend
		{
			get { return asyncSend; }
			set { asyncSend = value; }
		}
		
		/// <summary>
		/// Gets or sets the port used for SMTP transactions.
		/// </summary>
		public int Port
		{
			get { return this.smtpClient.Port; }
			set { this.smtpClient.Port = value; }
		}
		
		/// <summary>
		/// Gets or sets a value that specifies the amount of time after which a synchronous Send call times out.
		/// </summary>
		public int Timeout
		{
			get { return this.smtpClient.Timeout; }
			set { this.smtpClient.Timeout = value; }
		}
#endif

		public void Send(String from, String to, String subject, String messageText)
		{
			if (from == null) throw new ArgumentNullException("from");
			if (to == null) throw new ArgumentNullException("to");
			if (subject == null) throw new ArgumentNullException("subject");
			if (messageText == null) throw new ArgumentNullException("messageText");

#if DOTNET2
			smtpClient.Send(from, to, subject, messageText);
#else
			SmtpMail.Send( from, to, subject, messageText );
#endif
		}

		public void Send(Message message)
		{
			if (message == null) throw new ArgumentNullException("message");

#if DOTNET2
			if (asyncSend)
				smtpClient.SendAsync(CreateMailMessage(message), message);
			else
				smtpClient.Send(CreateMailMessage(message));
#else
			SmtpMail.Send( CreateMailMessage(message) );
#endif
		}

		public void Send(Message[] messages)
		{
			foreach( Message message in messages )
			{
				Send( message );
			}
		}

#if DOTNET2
		private MailMessage CreateMailMessage(Message message)
		{
			MailMessage mailMessage = new MailMessage(message.From, message.To);

			if (!String.IsNullOrEmpty(message.Cc))
				mailMessage.CC.Add(message.Cc);
			if (!String.IsNullOrEmpty(message.Bcc))
				mailMessage.Bcc.Add(message.Bcc);
			mailMessage.Subject = message.Subject;
			mailMessage.Body = message.Body;
			mailMessage.BodyEncoding = message.Encoding;
			mailMessage.IsBodyHtml = (message.Format == Format.Html);
			mailMessage.Priority = (MailPriority) Enum.Parse( typeof(MailPriority), message.Priority.ToString() );
			
			foreach(DictionaryEntry entry in message.Headers)
			{
				mailMessage.Headers.Add((string)entry.Key, (string)entry.Value);
			}

			foreach(MessageAttachment attachment in message.Attachments)
			{
				Attachment mailAttach = new Attachment(attachment.FileName);
				
				mailMessage.Attachments.Add( mailAttach );
			}

			return mailMessage;
		}
#else
		private MailMessage CreateMailMessage(Message message)
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
#endif
	}
}
