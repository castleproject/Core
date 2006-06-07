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

namespace Castle.Components.Common.EmailSender.DotNet2Sender
{
    using System;
    using System.Collections;
    using System.Net.Mail;
    using System.Net.Mime;

    using Castle.Components.Common.EmailSender2;

    public class SmtpSender : IEmailSender
    {
        SmtpClient _client;

        public SmtpSender(String hostname)
        {
            this._client = new SmtpClient(hostname);
        }

        #region IEmailSender Members

        public void Send(string from, string to, string subject, string messageText)
        {
            if(from == null) throw new ArgumentNullException("from");
            if(to == null) throw new ArgumentNullException("to");
            if(subject == null) throw new ArgumentNullException("subject");
            if(messageText == null) throw new ArgumentNullException("messageText");

            this._client.Send(from, to, subject, messageText);
        }

        public void Send(Message message)
        {
            if(message == null) throw new ArgumentNullException("message");

            this._client.Send(MailMessageFrom(message));
        }

        public void Send(Message[] messages)
        {
            foreach(Message message in messages)
            {
                Send(message);
            }
        }

        #endregion

        private MailMessage MailMessageFrom(Message message)
        {
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(message.From);
			
        	// TODO: We should split the To, CC and BCC 
        	// and invoke add as many time as it's necessary
        	
        	if (message.To != null && message.To != String.Empty)
			{
				mailMessage.To.Add(message.To);
			}
			
        	if (message.Cc != null && message.Cc != String.Empty)
			{
				mailMessage.CC.Add(message.Cc);
			}

			if (message.Bcc != null && message.Bcc != String.Empty)
			{
				mailMessage.Bcc.Add(message.Bcc);
			}
        	
            mailMessage.Subject = message.Subject;
            mailMessage.Body = message.Body;
            mailMessage.BodyEncoding = message.Encoding;
            mailMessage.IsBodyHtml = (message.Format == Format.Html) ? true : false;
            mailMessage.Priority = (MailPriority)Enum.Parse(typeof(MailPriority), message.Priority.ToString());

            foreach(DictionaryEntry entry in message.Headers)
            {
                mailMessage.Headers.Add(entry.Key.ToString(), entry.Value.ToString());
            }

            mailMessage.BodyEncoding = message.Encoding;
            
            foreach(MessageAttachment attachment in message.Attachments)
            {
                Attachment item;
                if(attachment.Stream != null)
                {
                    item = new Attachment(attachment.Stream, attachment.MediaType);
                }
                else
                {
                    item = new Attachment(attachment.FileName, attachment.MediaType);
                }
                

                mailMessage.Attachments.Add(item);
            }

            return mailMessage;
        }

        /// <summary>
        /// Port to use
        /// </summary>
        /// <remarks>Optional</remarks>
        public int Port
        {
            get { return this._client.Port; }
            set { this._client.Port = value; }
        }

    }
}
