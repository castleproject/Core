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
			get { return this._to; }
            set { this._to = value; }
		}

		public String From
		{
            get { return this._from; }
            set { this._from = value; }
		}

		public String Cc
		{
            get { return this._cc; }
            set { this._cc = value; }
		}

		public String Bcc
		{
            get { return this._bcc; }
            set { this._bcc = value; }
		}

		public String Body
		{
            get { return this._body; }
            set { this._body = value; }
		}

		public String Subject
		{
            get { return this._subject; }
            set { this._subject = value; }
		}

		public Format Format
		{
            get { return this._format; }
            set { this._format = value; }
		}

		public Encoding Encoding
		{
            get { return this._encoding; }
            set { this._encoding = value; }
		}

		public MessagePriority Priority
		{
            get { return this._priority; }
            set { this._priority = value; }
		}

		public IDictionary Headers
		{
            get { return this._headers; }
		}

		public MessageAttachmentCollection Attachments
		{
            get { return this._attachments; }
		}

        [Obsolete("Use AddCredentials")]
        public void AddAuthentication(string username, string password)
        {
            this._credentials = new NetworkCredential(username, password);
        }

        public void AddCredentials(string username, string password)
        {
            this._credentials = new NetworkCredential(username, password);
        }
        public void AddCredentials(string username, string password, string domain)
        {
            this._credentials = new NetworkCredential(username, password, domain);
        }

        internal NetworkCredential Credentials
        {
            get
            {
                return this._credentials;
            }
        }
        public bool HasCredentials
        {
            get
            {
                return this._credentials != null ? true : false;
            }
        }
	}
}
