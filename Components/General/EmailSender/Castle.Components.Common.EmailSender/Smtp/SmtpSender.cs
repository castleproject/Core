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
	using System.Net;
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
#if DOTNET2
		private SmtpClient smtpClient;
		private bool asyncSend = false;
		private bool configured;
#endif
		private string hostname;
		private int port = 25;
		private NetworkCredential credentials = new NetworkCredential();

		/// <summary>
		/// This service implementation
		/// requires a host name in order to work
		/// </summary>
		/// <param name="hostname">The smtp server name</param>
		public SmtpSender(string hostname)
		{
			this.hostname = hostname;

#if DOTNET2
			smtpClient = new SmtpClient(hostname);
#endif
		}

		/// <summary>
		/// Gets or sets the port used to 
		/// access the SMTP server
		/// </summary>
		public int Port
		{
			get { return port; }
			set { port = value; }
		}

		/// <summary>
		/// Gets the hostname.
		/// </summary>
		/// <value>The hostname.</value>
		public string Hostname
		{
			get { return hostname; }
		}

#if DOTNET2
		/// <summary>
		/// Gets or sets a value which is used to 
		/// configure if emails are going to be sent asyncrhonously or not.
		/// </summary>
		public bool AsyncSend
		{
			get { return asyncSend; }
			set { asyncSend = value; }
		}
		
		/// <summary>
		/// Gets or sets a value that specifies 
		/// the amount of time after which a synchronous Send call times out.
		/// </summary>
		public int Timeout
		{
			get { return smtpClient.Timeout; }
			set { smtpClient.Timeout = value; }
		}
#endif

		/// <summary>
		/// Sends a message. 
		/// </summary>
		/// <exception cref="ArgumentNullException">If any of the parameters is null</exception>
		/// <param name="from">From field</param>
		/// <param name="to">To field</param>
		/// <param name="subject">e-mail's subject</param>
		/// <param name="messageText">message's body</param>
		public void Send(String from, String to, String subject, String messageText)
		{
			if (from == null) throw new ArgumentNullException("from");
			if (to == null) throw new ArgumentNullException("to");
			if (subject == null) throw new ArgumentNullException("subject");
			if (messageText == null) throw new ArgumentNullException("messageText");

			Send(new Message(from, to, subject, messageText));
		}

		/// <summary>
		/// Sends a message. 
		/// </summary>
		/// <exception cref="ArgumentNullException">If the message is null</exception>
		/// <param name="message">Message instance</param>
		public void Send(Message message)
		{
			if (message == null) throw new ArgumentNullException("message");
		
			ConfigureSender(message);

#if DOTNET2
			if (asyncSend)
			{
				smtpClient.SendAsync(CreateMailMessage(message), new object());
			}
			else
			{
				smtpClient.Send(CreateMailMessage(message));
			}
#else
			SmtpMail.Send(CreateMailMessage(message));
#endif
		}

		public void Send(Message[] messages)
		{
			foreach(Message message in messages)
			{
				Send(message);
			}
		}

#if DOTNET2
		/// <summary>
		/// Converts a message from Castle.Components.Common.EmailSender.Message  type
		/// to System.Web.Mail.MailMessage
		/// </summary>
		/// <param name="message">The message to convert.</param>
		/// <returns>The converted message .</returns>
		private MailMessage CreateMailMessage(Message message)
		{
			MailMessage mailMessage = new MailMessage(message.From, message.To);

			if (!String.IsNullOrEmpty(message.Cc))
			{
				mailMessage.CC.Add(message.Cc);
			}
			
			if (!String.IsNullOrEmpty(message.Bcc))
			{
				mailMessage.Bcc.Add(message.Bcc);
			}
			
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
				Attachment mailAttach;
				
				if (attachment.Stream != null)
				{
					mailAttach = new Attachment(attachment.Stream, attachment.MediaType);
				}
				else
				{
					mailAttach = new Attachment(attachment.FileName, attachment.MediaType);
				}
				
				mailMessage.Attachments.Add(mailAttach);
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
			mailMessage.BodyFormat = (MailFormat) Enum.Parse(typeof(MailFormat), message.Format.ToString());
			mailMessage.Priority = (MailPriority) Enum.Parse(typeof(MailPriority), message.Priority.ToString());

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
				MailEncoding enc = (MailEncoding) Enum.Parse(typeof(MailEncoding), attachment.Encoding.ToString());

				MailAttachment mailAttach = new MailAttachment(attachment.FileName, enc);

				mailMessage.Attachments.Add(mailAttach);
			}

			return mailMessage;
		}
#endif

		/// <summary>
		/// Gets or sets the domain.
		/// </summary>
		/// <value>The domain.</value>
		public String Domain
		{
			get { return credentials.Domain; }
			set { credentials.Domain = value;  }
		}

		/// <summary>
		/// Gets or sets the name of the user.
		/// </summary>
		/// <value>The name of the user.</value>
		public String UserName
		{
			get { return credentials.UserName; }
			set { credentials.UserName = value; }
		}

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		public String Password
		{
			get { return credentials.Password; }
			set { credentials.Password = value; }
		}

		/// <summary>
		/// Configures the message or the sender
		/// with port information and eventual credential
		/// informed
		/// </summary>
		/// <param name="message">Message instance</param>
		private void ConfigureSender(Message message)
		{
#if DOTNET2
			if (!configured)
			{
				if (HasCredentials)
				{
					smtpClient.Credentials = credentials;
				}

				smtpClient.Port = port;

				configured = true;
			}
#else
			message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserver", hostname);
			message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", port);
			
			if (HasCredentials)
			{
				message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
				message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", UserName);
				message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", Password);
			}
#endif
		}

		/// <summary>
		/// Gets a value indicating whether credentials were informed.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this instance has credentials; otherwise, <see langword="false"/>.
		/// </value>
		private bool HasCredentials
		{
			get { return credentials.UserName != null && credentials.Password != null ? true : false; }
		}
	}
}