// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.TransformFilters
{
	using System;
	using System.IO;
	using System.Text;

	/// <summary>
	/// Simple proof of concept filter that converts the stream data to uppercase.
	/// </summary>
	public class UpperCaseTransformFilter : TransformFilter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UpperCaseTransformFilter"/> class.
		/// </summary>
		/// <param name="baseStream">The stream to write to after filtering.</param>
		public UpperCaseTransformFilter(Stream baseStream) : base(baseStream)
		{
		}

		/// <summary>
		/// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
		/// </summary>
		/// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
		/// <param name="count">The number of bytes to be written to the current stream.</param>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support writing. </exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
		/// <exception cref="T:System.ArgumentNullException">buffer is null. </exception>
		/// <exception cref="T:System.ArgumentException">The sum of offset and count is greater than the buffer length. </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">offset or count is negative. </exception>
		public override void Write(byte[] buffer, int offset, int count)
		{
			if (Closed) throw new ObjectDisposedException("UpperCaseTransformFilter");

			string content = Encoding.Default.GetString(buffer, offset, count);

			content = content.ToUpper();
			
			byte[] output = Encoding.Default.GetBytes(content);
			BaseStream.Write(output,0, output.Length);
		}
	}
}
