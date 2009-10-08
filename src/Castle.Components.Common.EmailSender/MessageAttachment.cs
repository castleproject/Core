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
	using System.IO;
	
	public enum AttachmentEncoding
	{
		Base64,
		UUEncode
	}
	
	/// <summary>
	/// Represents a file attachment
	/// </summary>
	[Serializable]
	public class MessageAttachment
	{
		private String fileName;
		private readonly String mediaType;
		private readonly Stream stream;

		/// <summary>
		/// Creates a new attachment
		/// </summary>
		public MessageAttachment()
		{
		}

		/// <summary>
		/// Creates a new attachment
		/// </summary>
		/// <param name="mediaType">Look at System.Net.Mimie.MediaTypeNames for help.</param>
		/// <param name="fileName">Path to the file.</param>
		public MessageAttachment(String mediaType, String fileName)
		{
			this.mediaType = mediaType;

			if (fileName == null) throw new ArgumentNullException("fileName");

			FileInfo info = new FileInfo(fileName);

			if (!info.Exists) throw new ArgumentException("The specified file does not exists", "fileName");

			this.fileName = fileName;
		}

		/// <summary>
		/// Creates a new attachment
		/// </summary>
		/// <param name="mediaType">Look at System.Net.Mime.MediaTypeNames for help.</param>
		/// <param name="stream">File stream.</param>
		public MessageAttachment(String mediaType, Stream stream)
		{
			this.mediaType = mediaType;

			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}

			this.stream = stream;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageAttachment"/> class.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="mediaType">Type of the media.</param>
		/// <param name="stream">The stream.</param>
		public MessageAttachment(string fileName, string mediaType, Stream stream) : this(mediaType, stream)
		{
			this.fileName = fileName;
		}

		/// <summary>
		/// Gets the name of the file.
		/// </summary>
		/// <value>The name of the file.</value>
		public String FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}
		
		/// <summary>
		/// Gets the type of the media.
		/// </summary>
		/// <value>The type of the media.</value>
		public String MediaType
		{
			get { return mediaType; }
		}

		/// <summary>
		/// Gets the stream.
		/// </summary>
		/// <value>The stream.</value>
		public Stream Stream
		{
			get { return stream; }
		}
	}
}
