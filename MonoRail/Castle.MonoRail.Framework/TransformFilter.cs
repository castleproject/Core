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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.IO;

	/// <summary>
	/// Abstract base class for HttpFilters.
	/// </summary>
	public abstract class TransformFilter : Stream, ITransformFilter
	{
		private Stream baseStream;
		private bool closed;

		/// <summary>
		/// Base class holds the underlying stream.
		/// </summary>
		/// <param name="baseStream">The stream to write to after filtering.</param>
		public TransformFilter(Stream baseStream)
		{
			this.baseStream = baseStream;
			closed = false;
		}

		/// <summary>
		/// The stream to the filter can use to write write to
		/// </summary>
		protected Stream BaseStream
		{
			get { return baseStream; }
		}

		/// <summary>
		/// This method is not supported for an HttpFilter
		/// </summary>
		/// <exception cref="NotSupportedException">Always thrown</exception>
		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// This method is not supported for an HttpFilter
		/// </summary>
		public override bool CanRead
		{
			get { return false; }
		}

		/// <summary>
		/// This method is not supported for an HttpFilter
		/// </summary>
		public override bool CanSeek
		{
			get { return false; }
		}

		/// <summary>
		/// Indicates if the Stream is closed or open
		/// </summary>
		public override bool CanWrite
		{
			get { return !closed; }
		}

		/// <summary>
		/// Close implementation.
		/// </summary>
		/// <remarks>
		/// Don't forget to call base.Close is you override this function.
		/// </remarks>
		public override void Close()
		{
			closed = true;
			baseStream.Close();
		}

		/// <summary>
		/// Indicates if the Stream is closed or open
		/// </summary>
		/// <remarks>
		/// Implementors should always check Closed before writing anything to the BaseStream.
		/// </remarks>
		protected bool Closed
		{
			get { return closed; }
		}

		/// <summary>
		/// This method is not supported for an HttpFilter
		/// </summary>
		/// <exception cref="NotSupportedException">Always thrown</exception>
		public override long Length
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary>
		/// This method is not supported for an HttpFilter
		/// </summary>
		/// <exception cref="NotSupportedException">Always thrown</exception>
		public override long Position
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Flushes the base stream
		/// </summary>
		public override void Flush()
		{
			baseStream.Flush();
		}

		/// <summary>
		/// This method is not supported for an HttpFilter
		/// </summary>
		/// <exception cref="NotSupportedException">Always thrown</exception>
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// This method is not supported for an HttpFilter
		/// </summary>
		/// <exception cref="NotSupportedException">Always thrown</exception>
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}
	}
}
