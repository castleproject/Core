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

namespace Castle.Components.Common.EmailSender
{
	using System;
	using System.Text;
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// Message formats
	/// </summary>
	public enum Format
	{
		Html,
		Text
	}

	public enum MessagePriority
	{
		Normal,
		High,
		Low
	}

	public class Message
	{
		private String to;
		private String from;
		private String cc;
		private String bcc;
		private String body;
		private String subject;
		private Format format = Format.Text;
		private Encoding encoding = Encoding.ASCII;
		private IDictionary headers = new HybridDictionary();
        private IDictionary fields = new HybridDictionary();
		private MessagePriority priority = MessagePriority.Normal;
		private MessageAttachmentCollection attachments = new MessageAttachmentCollection();

		public Message()
		{
		}

		public String To
		{
			get { return to; }
			set { to = value; }
		}

		public String From
		{
			get { return from; }
			set { from = value; }
		}

		public String Cc
		{
			get { return cc; }
			set { cc = value; }
		}

		public String Bcc
		{
			get { return bcc; }
			set { bcc = value; }
		}

		public String Body
		{
			get { return body; }
			set { body = value; }
		}

		public String Subject
		{
			get { return subject; }
			set { subject = value; }
		}

		public Format Format
		{
			get { return format; }
			set { format = value; }
		}

		public Encoding Encoding
		{
			get { return encoding; }
			set { encoding = value; }
		}

		public MessagePriority Priority
		{
			get { return priority; }
			set { priority = value; }
		}

		public IDictionary Headers
		{
			get { return headers; }
		}

        public IDictionary Fields
        {
            get { return fields; }
        }

		public MessageAttachmentCollection Attachments
		{
			get { return attachments; }
		}

        public void AddAuthentication(string username, string password)
        {
            this.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
            this.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", username);
            this.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", password);
        }
	}
}
