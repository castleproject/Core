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
	using System.Net;
	using System.Text;
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// Message formats
	/// </summary>
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
	public enum MessagePriority
	{
		Normal,
		High,
		Low
	}

	/// <summary>
	/// Abstracts an e-mail message
	/// </summary>
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
	}
}
