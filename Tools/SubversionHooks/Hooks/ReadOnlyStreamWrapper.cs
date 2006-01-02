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

namespace Castle.SvnHooks
{
	using System;
	using System.IO;

	/// <summary>
	/// Summary description for ReadOnlyStreamWrapper.
	/// </summary>
	public class ReadOnlyStreamWrapper : Stream
	{
		public ReadOnlyStreamWrapper(Stream source)
		{
			if (!source.CanRead)
				throw new ArgumentException("Source stream must be readable", "source");

			this.Source = source;
		}


		public override int Read(byte[] buffer, int offset, int count)
		{
			return Source.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return Source.Seek(offset, origin);
		}


		public override long Length
		{
			get
			{
				return Source.Length;
			}
		}

		public override long Position
		{
			get
			{
				return Source.Position;
			}
			set
			{
				Source.Position = value;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return Source.CanSeek;
			}
		}


		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}


		#region Write related overrides

		public override void Flush()
		{
		}

		public override void SetLength(long value)
		{
			throw new InvalidOperationException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new InvalidOperationException();
		}


		#endregion
		
		private readonly Stream Source;
	}
}
