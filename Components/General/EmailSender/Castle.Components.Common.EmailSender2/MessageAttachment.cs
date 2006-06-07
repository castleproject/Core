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
	using System.IO;

	/// <summary>
	/// Represents a file attachment
	/// </summary>
	public class MessageAttachment
	{
        private readonly String _mediaType;
		private readonly String _fileName = null;
        private readonly Stream _stream = null;

        /// <summary>
        /// Creates a new attachment
        /// </summary>
        /// <param name="mediaType">Look at System.Net.Mimie.MediaTypeNames for help.</param>
        /// <param name="fileName">Path to the file.</param>
        public MessageAttachment(String mediaType, String fileName)
		{
            _mediaType = mediaType;

			if (fileName == null) throw new ArgumentNullException("fileName");

			FileInfo info = new FileInfo(fileName);

			if (!info.Exists) throw new ArgumentException("The specified file does not exists", "fileName");

			_fileName = fileName;
		}

        /// <summary>
        /// Creates a new attachment
        /// </summary>
        /// <param name="mediaType">Look at System.Net.Mimie.MediaTypeNames for help.</param>
		/// <param name="stream">File stream.</param>
        public MessageAttachment(String mediaType, Stream stream)
        {
            _mediaType = mediaType;

            if(stream == null) throw new ArgumentNullException("stream");

            _stream = stream;
        }

        public String MediaType
		{
            get { return _mediaType; }
		}

		public String FileName
		{
            get { return _fileName; }
		}
        public Stream Stream
        {
            get { return _stream; }
        }
	}
}
