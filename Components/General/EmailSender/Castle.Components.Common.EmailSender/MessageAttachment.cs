// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	using System.IO;

	public enum AttachmentEncoding
	{
		Base64,
		UUEncode
	}

	/// <summary>
	/// Represents a file attachment
	/// </summary>
	public class MessageAttachment
	{
		private readonly AttachmentEncoding encoding;
		private readonly String fileName;

		public MessageAttachment(AttachmentEncoding encoding, String fileName)
		{
			this.encoding = encoding;

			if (fileName == null) throw new ArgumentNullException("fileName");

			FileInfo info = new FileInfo(fileName);

			if (!info.Exists) throw new ArgumentException("The specified file does not exists", "fileName");

			this.fileName = fileName;
		}

		public AttachmentEncoding Encoding
		{
			get { return encoding; }
		}

		public String FileName
		{
			get { return fileName; }
		}
	}
}
