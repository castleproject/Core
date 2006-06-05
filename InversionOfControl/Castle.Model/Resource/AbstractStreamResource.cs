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

namespace Castle.Model.Resource
{
	using System;
	using System.IO;
	using System.Text;
	using System.Runtime.Remoting;

	/// <summary>
	/// 
	/// </summary>
	public abstract class AbstractStreamResource : AbstractResource
	{
		public AbstractStreamResource()
		{
		}

		~AbstractStreamResource()
		{
			Dispose(false);
		}

		protected abstract Stream Stream { get; }

		public override TextReader GetStreamReader()
		{
			// The StreamReader can't take ownership of the stream
			// that's why we use StreamHideCloseDelegate
			return new StreamReader(new StreamHideCloseDelegate(Stream));
		}

		public override TextReader GetStreamReader(Encoding encoding)
		{
			// The StreamReader can't take ownership of the stream
			// that's why we use StreamHideCloseDelegate
			return new StreamReader(new StreamHideCloseDelegate(Stream), encoding);
		}

		public override void Dispose()
		{
			Dispose(true);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				GC.SuppressFinalize(this);
			}

			if (Stream != null)
			{
				Stream.Close();
			}
		}

		#region StreamHideCloseDelegate nested class

		/// <summary>
		/// Do not allow closing and disposal of the 
		/// underlying <see cref="Stream"/>.
		/// </summary>
		public class StreamHideCloseDelegate : Stream, IDisposable
		{
			private readonly Stream inner;

			public StreamHideCloseDelegate(Stream inner)
			{
				this.inner = inner;
			}

			#region Stream implementation

			public override bool CanRead
			{
				get { return inner.CanRead; }
			}

			public override bool CanSeek
			{
				get { return inner.CanSeek; }
			}

			public override bool CanWrite
			{
				get { return inner.CanWrite; }
			}

			public override long Length
			{
				get { return inner.Length; }
			}

			public override long Position
			{
				get { return inner.Position; }
				set { inner.Position = value; }
			}

			public override void Flush()
			{
				inner.Flush();
			}

			public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
			{
				return inner.BeginRead(buffer, offset, count, callback, state);
			}

			public override int EndRead(IAsyncResult asyncResult)
			{
				return inner.EndRead(asyncResult);
			}

			public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
			{
				return inner.BeginWrite(buffer, offset, count, callback, state);
			}

			public override void EndWrite(IAsyncResult asyncResult)
			{
				inner.EndWrite(asyncResult);
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				return inner.Seek(offset, origin);
			}

			public override void SetLength(long value)
			{
				inner.SetLength(value);
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				return inner.Read(buffer, offset, count);
			}

			public override int ReadByte()
			{
				return inner.ReadByte();
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				inner.Write(buffer, offset, count);
			}

			public override void WriteByte(byte value)
			{
				inner.WriteByte(value);
			}

			public override object InitializeLifetimeService()
			{
				return inner.InitializeLifetimeService();
			}

			public override ObjRef CreateObjRef(Type requestedType)
			{
				return inner.CreateObjRef(requestedType);
			}

			public override void Close()
			{
			}

			public void Dispose()
			{
			}

			#endregion
		}

		#endregion
	}
}