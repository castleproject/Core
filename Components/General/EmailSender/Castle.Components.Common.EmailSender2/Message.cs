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

namespace Castle.Components.Common.EmailSender2
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
		private String _to;
		private String _from;
		private String _cc;
		private String _bcc;
		private String _body;
		private String _subject;
		private Format _format = Format.Text;
		private Encoding _encoding = Encoding.ASCII;
		private IDictionary _headers = new HybridDictionary();
		private MessagePriority _priority = MessagePriority.Normal;
		private MessageAttachmentCollection _attachments = new MessageAttachmentCollection();
		private NetworkCredential _credentials;

		public Message()
		{
		}

		public String To
		{
			get { return _to; }
			set { _to = value; }
		}

		public String From
		{
			get { return _from; }
			set { _from = value; }
		}

		public String Cc
		{
			get { return _cc; }
			set { _cc = value; }
		}

		public String Bcc
		{
			get { return _bcc; }
			set { _bcc = value; }
		}

		public String Body
		{
			get { return _body; }
			set { _body = value; }
		}

		public String Subject
		{
			get { return _subject; }
			set { _subject = value; }
		}

		public Format Format
		{
			get { return _format; }
			set { _format = value; }
		}

		public Encoding Encoding
		{
			get { return _encoding; }
			set { _encoding = value; }
		}

		public MessagePriority Priority
		{
			get { return _priority; }
			set { _priority = value; }
		}

		public IDictionary Headers
		{
			get { return _headers; }
		}

		public MessageAttachmentCollection Attachments
		{
			get { return _attachments; }
		}

		[Obsolete("Use AddCredentials")]
		public void AddAuthentication(string username, string password)
		{
			_credentials = new NetworkCredential(username, password);
		}

		public void AddCredentials(string username, string password)
		{
			_credentials = new NetworkCredential(username, password);
		}

		public void AddCredentials(string username, string password, string domain)
		{
			_credentials = new NetworkCredential(username, password, domain);
		}

		public bool HasCredentials
		{
			get { return _credentials != null ? true : false; }
		}

		internal NetworkCredential Credentials
		{
			get { return _credentials; }
		}
	}
}