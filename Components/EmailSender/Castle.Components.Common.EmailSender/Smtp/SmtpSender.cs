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

namespace Castle.Components.Common.EmailSender.Smtp
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Net;
	using System.Net.Mail;
	using System.Security;
	using System.Security.Permissions;

	/// <summary>
	/// Uses Smtp to send emails.
	/// </summary>
	public class SmtpSender : IEmailSender
	{
		private bool asyncSend;
		private readonly string hostname;
		private int port = 25;
		private int? timeout;
		private bool useSSL;
		private readonly NetworkCredential credentials = new NetworkCredential();

		/// <summary>
		/// This service implementation
		/// requires a host name in order to work
		/// </summary>
		/// <param name="hostname">The smtp server name</param>
		public SmtpSender(string hostname)
		{
			this.hostname = hostname;
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
			get { return timeout.HasValue ? timeout.Value : 0; }
			set { timeout = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the email should be sent using 
		/// a secure communication channel.
		/// </summary>
		/// <value><c>true</c> if should use SSL; otherwise, <c>false</c>.</value>
		public bool UseSSL
		{
			get { return useSSL; }
			set { useSSL = value; }
		}

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

			if (asyncSend)
			{
				// The MailMessage must be diposed after sending the email.
				// The code creates a delegate for deleting the mail and adds
				// it to the smtpClient.
				// After the mail is sent, the message is disposed and the
				// eventHandler removed from the smtpClient.
				SmtpClient smtpClient = new SmtpClient(hostname, port);
				Configure(smtpClient);

				MailMessage msg = CreateMailMessage(message);
				Guid msgGuid = new Guid();
				SendCompletedEventHandler sceh = null;
				sceh = delegate(object sender, AsyncCompletedEventArgs e)
					{
						if (msgGuid == (Guid)e.UserState)
							msg.Dispose();
						// The handler itself, cannot be null, test omitted
						smtpClient.SendCompleted -= sceh;
					};
				smtpClient.SendCompleted += sceh;
				smtpClient.SendAsync(msg, msgGuid);
			}
			else
			{
				using (MailMessage msg = CreateMailMessage(message))
				{
					SmtpClient smtpClient = new SmtpClient(hostname, port);
					Configure(smtpClient);

					smtpClient.Send(msg);
				}
			}
		}

		public void Send(Message[] messages)
		{
			foreach (Message message in messages)
			{
				Send(message);
			}
		}

		/// <summary>
		/// Converts a message from Castle.Components.Common.EmailSender.Message  type
		/// to System.Net.Mail.MailMessage
		/// </summary>
		/// <param name="message">The message to convert.</param>
		/// <returns>The converted message .</returns>
		public MailMessage CreateMailMessage(Message message)
		{
			MailMessage mailMessage = new MailMessage(message.From, message.To.Replace(';', ','));

			if (!String.IsNullOrEmpty(message.Cc))
			{
				mailMessage.CC.Add(message.Cc.Replace(';', ','));
			}

			if (!String.IsNullOrEmpty(message.Bcc))
			{
				mailMessage.Bcc.Add(message.Bcc.Replace(';', ','));
			}

			mailMessage.Subject = message.Subject;
			mailMessage.Body = message.Body;
			mailMessage.BodyEncoding = message.Encoding;
			mailMessage.IsBodyHtml = (message.Format == Format.Html);
			mailMessage.Priority = (MailPriority)Enum.Parse(typeof(MailPriority), message.Priority.ToString());
			mailMessage.ReplyTo = message.ReplyTo;

			foreach (DictionaryEntry entry in message.Headers)
			{
				mailMessage.Headers.Add((string)entry.Key, (string)entry.Value);
			}

			foreach (MessageAttachment attachment in message.Attachments)
			{
				Attachment mailAttach;

				if (attachment.Stream != null)
				{
					mailAttach = new Attachment(attachment.Stream, attachment.FileName, attachment.MediaType);
				}
				else
				{
					mailAttach = new Attachment(attachment.FileName, attachment.MediaType);
				}

				mailMessage.Attachments.Add(mailAttach);
			}

			if (message.Resources != null && message.Resources.Count > 0)
			{
				AlternateView htmlView = AlternateView.CreateAlternateViewFromString(message.Body, message.Encoding, "text/html");
				foreach (string id in message.Resources.Keys)
				{
					LinkedResource r = message.Resources[id];
					r.ContentId = id;
					if (r.ContentStream != null)
					{
						htmlView.LinkedResources.Add(r);
					}
				}
				mailMessage.AlternateViews.Add(htmlView);
			}
			return mailMessage;
		}

		/// <summary>
		/// Gets or sets the domain.
		/// </summary>
		/// <value>The domain.</value>
		public String Domain
		{
			get { return credentials.Domain; }
			set { credentials.Domain = value; }
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
		/// Configures the sender
		/// with port information and eventual credential
		/// informed
		/// </summary>
		/// <param name="smtpClient">Message instance</param>
		protected virtual void Configure(SmtpClient smtpClient)
		{
			smtpClient.Credentials = null;

			if (CanAccessCredentials && HasCredentials)
			{
				smtpClient.Credentials = credentials;
			}

			if (timeout.HasValue)
			{
				smtpClient.Timeout = timeout.Value;
			}

			if (useSSL)
			{
				smtpClient.EnableSsl = useSSL;
			}
		}

		/// <summary>
		/// Gets a value indicating whether credentials were informed.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this instance has credentials; otherwise, <see langword="false"/>.
		/// </value>
		private bool HasCredentials
		{
			get { return !string.IsNullOrEmpty(credentials.UserName); }
		}

		private static bool CanAccessCredentials
		{
			get
			{
				return SecurityManager.IsGranted(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode));
			}
		}
	}
}
