// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

#pragma warning disable 618 // Mono marked SmtpClient obsolete

namespace Castle.Core.Smtp
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Net;
	using System.Net.Mail;

	using Castle.Core.Internal;

	/// <summary>
	/// Default <see cref="IEmailSender"/> implementation.
	/// </summary>
	public class DefaultSmtpSender : IEmailSender
	{
		private bool asyncSend;
		private readonly string hostname;
		private int port = 25;
		private int? timeout;
		private bool useSsl;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultSmtpSender"/> class based on the <see cref="SmtpClient"/> configuration provided in the application configuration file.
		/// </summary>
		/// <remarks>
		/// This constructor is based on the default <see cref="SmtpClient"/> configuration in the application configuration file.
		/// </remarks>
		public DefaultSmtpSender() { }

		/// <summary>
		/// This service implementation
		/// requires a host name in order to work
		/// </summary>
		/// <param name="hostname">The smtp server name</param>
		public DefaultSmtpSender(string hostname)
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
		/// configure if emails are going to be sent asynchronously or not.
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
		public bool UseSsl
		{
			get { return useSsl; }
			set { useSsl = value; }
		}

		/// <summary>
		/// Sends a message.
		/// </summary>
		/// <exception cref="ArgumentNullException">If any of the parameters is null</exception>
		/// <param name="from">From field</param>
		/// <param name="to">To field</param>
		/// <param name="subject">e-mail's subject</param>
		/// <param name="messageText">message's body</param>
		public void Send(string from, string to, string subject, string messageText)
		{
			if (from == null) throw new ArgumentNullException(nameof(from));
			if (to == null) throw new ArgumentNullException(nameof(to));
			if (subject == null) throw new ArgumentNullException(nameof(subject));
			if (messageText == null) throw new ArgumentNullException(nameof(messageText));

			Send(new MailMessage(from, to, subject, messageText));
		}

		/// <summary>
		/// Sends a message.
		/// </summary>
		/// <exception cref="ArgumentNullException">If the message is null</exception>
		/// <param name="message">Message instance</param>
		public void Send(MailMessage message)
		{
			InternalSend(message);
		}

		private void InternalSend(MailMessage message)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));

			if (asyncSend)
			{
				// The MailMessage must be disposed after sending the email.
				// The code creates a delegate for deleting the mail and adds
				// it to the smtpClient.
				// After the mail is sent, the message is disposed and the
				// eventHandler removed from the smtpClient.

				SmtpClient smtpClient = CreateSmtpClient();
				Guid msgGuid = new Guid();
				SendCompletedEventHandler sceh = null;
				sceh = delegate(object sender, AsyncCompletedEventArgs e)
				{
					if (msgGuid == (Guid)e.UserState)
						message.Dispose();
					// The handler itself, cannot be null, test omitted
					smtpClient.SendCompleted -= sceh;
				};
				smtpClient.SendCompleted += sceh;
				smtpClient.SendAsync(message, msgGuid);
			}
			else
			{
				using (message)
				{
					SmtpClient smtpClient = CreateSmtpClient();

					smtpClient.Send(message);
				}
			}
		}

		public void Send(IEnumerable<MailMessage> messages)
		{
			foreach (MailMessage message in messages)
			{
				Send(message);
			}
		}

		/// <summary>
		/// Gets or sets the domain.
		/// </summary>
		/// <value>The domain.</value>
		public string Domain { get; set; }

		/// <summary>
		/// Gets or sets the name of the user.
		/// </summary>
		/// <value>The name of the user.</value>
		public string UserName { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		public string Password { get; set; }

		/// <summary>
		/// Configures the sender
		/// with port information and eventual credential
		/// informed
		/// </summary>
		/// <param name="smtpClient">Message instance</param>
		protected virtual void Configure(SmtpClient smtpClient)
		{
			smtpClient.Credentials = null;

			if (HasCredentials)
			{
				var credentials = new NetworkCredential(UserName, Password, Domain);
				smtpClient.Credentials = credentials;
			}

			if (timeout.HasValue)
			{
				smtpClient.Timeout = timeout.Value;
			}

			if (useSsl)
			{
				smtpClient.EnableSsl = useSsl;
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
			get { return !string.IsNullOrEmpty(UserName); }
		}

		private SmtpClient CreateSmtpClient()
		{
			if (string.IsNullOrEmpty(hostname))
			{
				// No hostname configured, use the settings provided in system.net.smtp (SmtpClient default behavior)
				return new SmtpClient();
			}

			// A hostname is provided - init and configure using configured settings
			var smtpClient = new SmtpClient(hostname, port);
			Configure(smtpClient);
			return smtpClient;
		}
	}
}

#pragma warning restore 618 // Mono marked SmtpClient obsolete
