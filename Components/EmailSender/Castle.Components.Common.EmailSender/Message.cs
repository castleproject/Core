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

namespace Castle.Components.Common.EmailSender
{
	using System;
	using System.Collections.Generic;
	using System.Net.Mail;
	using System.Runtime.Serialization;
	using System.Text;
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// Message formats
	/// </summary>
	[Serializable]
	public enum Format
	{
		/// <summary>
		/// The body is composed of html content
		/// </summary>
		Html,
		/// <summary>
		/// The body is pure text
		/// </summary>
		Text
	}

	/// <summary>
	/// Message priority
	/// </summary>
	[Serializable]
	public enum MessagePriority
	{
		Normal,
		High,
		Low
	}

	/// <summary>
	/// Abstracts an e-mail message
	/// </summary>
	[Serializable]
	public class Message : ISerializable
	{
		private String to;
		private String from;
		private String cc;
		private String bcc;
		private String body;
		private String subject;
		private MailAddress replyTo;
		private Format format = Format.Text;
		private Encoding encoding = Encoding.ASCII;
		private MessagePriority priority = MessagePriority.Normal;
		private readonly IDictionary headers = new HybridDictionary();
		private readonly IDictionary fields = new HybridDictionary();
		private readonly MessageAttachmentCollection attachments = new MessageAttachmentCollection();
		private readonly IDictionary<string, LinkedResource> linkedResources = new Dictionary<string, LinkedResource>();

		/// <summary>
		/// Initializes a new instance of the <see cref="Message"/> class.
		/// </summary>
		public Message()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Message"/> class.
		/// </summary>
		/// <param name="from">From header.</param>
		/// <param name="to">To header.</param>
		/// <param name="subject">The subject header.</param>
		/// <param name="body">The message body.</param>
		public Message(string from, string to, string subject, string body)
		{
			this.to = to;
			this.from = from;
			this.body = body;
			this.subject = subject;
		}

		public Message(SerializationInfo info, StreamingContext context)
		{
			to = info.GetString("to");
			from = info.GetString("from");
			cc = info.GetString("cc");
			bcc = info.GetString("bcc");
			body = info.GetString("body");
			subject = info.GetString("subject");

			bool hasReplyTo = info.GetBoolean("hasReplyTo");

			if (hasReplyTo)
			{
				replyTo = new MailAddress(info.GetString(replyTo.Address), info.GetString("replyTo.DisplayName"));
			}

			format = (Format) info.GetValue("format", typeof (Format));
			encoding = (Encoding) info.GetValue("encoding", typeof (Encoding));
			priority = (MessagePriority) info.GetValue("priority", typeof (MessagePriority));

			headers = (IDictionary) info.GetValue("headers", typeof (IDictionary));
			fields = (IDictionary) info.GetValue("fields", typeof (IDictionary));

			attachments = (MessageAttachmentCollection) info.GetValue("attachments", typeof (MessageAttachmentCollection));
			linkedResources =
				(IDictionary<string, LinkedResource>) info.GetValue("linkedResources", typeof (IDictionary<string, LinkedResource>));
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

		public MailAddress ReplyTo
		{
			get { return replyTo; }
			set { replyTo = value; }
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

		/// <summary>
		/// You can add any number of inline attachments to this mail message. Inline attachments 
		/// differ from normal attachments in that they can be displayed withing the email body, 
		/// which makes this very handy for displaying images that can be viewed without having to 
		/// access an external server. 
		/// Provide an unique identifier (id) and use it with a &lt;img src="cid:my_id" /> tag from 
		/// within your view code.
		/// </summary>
		public IDictionary<string, LinkedResource> Resources
		{
			get { return linkedResources; }
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
            info.AddValue("to", to);
			info.AddValue("from", from);
			info.AddValue("cc", cc);
			info.AddValue("bcc", bcc);
			info.AddValue("body", body);
			info.AddValue("subject", subject);

			bool hasReplyTo = replyTo != null;

			info.AddValue("hasReplyTo", hasReplyTo);

			if (hasReplyTo)
			{
				info.AddValue("replyTo.Address", replyTo.Address);
				info.AddValue("replyTo.DisplayName", replyTo.DisplayName);			
			}

			info.AddValue("format", format);
			info.AddValue("encoding", encoding);
			info.AddValue("priority", priority);

			info.AddValue("headers", headers);
			info.AddValue("fields", fields);
			info.AddValue("attachments", attachments);
			info.AddValue("linkedResources", linkedResources);
		}
	}
}
