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

namespace Castle.Services.Logging
{
	using System;
	using System.IO;
	using System.Text;

	/// <summary>
	///	The Stream Logger class.  This class can stream log information
	///	to any stream, it is suitable for storing a log file to disk,
	///	or to a <c>MemoryStream</c> for testing your components.
	/// </summary>
	/// <remarks>
	/// This logger is not thread safe.
	/// </remarks>
	public class StreamLogger : LevelFilteredLogger, IDisposable
	{		        
		private StreamWriter writer;

		/// <summary>
		/// Creates a new <c>StreamLogger</c> with default encoding 
		/// and buffer size. Initial Level is set to Debug.
		/// </summary>
		/// <param name="name">
		/// The name of the log.
		/// </param>
		/// <param name="stream">
		///	The stream that will be used for logging,
		///	seeking while the logger is alive 
		///	</param>
		public StreamLogger(String name, Stream stream) : this(name, new StreamWriter(stream))
		{
		}

		/// <summary>
		/// Creates a new <c>StreamLogger</c> with default buffer size.
		/// Initial Level is set to Debug.
		/// </summary>
		/// <param name="name">
		/// The name of the log.
		/// </param>
		/// <param name="stream">
		///	The stream that will be used for logging,
		///	seeking while the logger is alive 
		///	</param>
		///	<param name="encoding">
		///	The encoding that will be used for this stream.
		///	<see cref="System.IO.StreamWriter"/>
		///	</param>
		public StreamLogger(String name, Stream stream, Encoding encoding) : this(name, new StreamWriter(stream, encoding))
		{
		}

		/// <summary>
		/// Creates a new <c>StreamLogger</c>. 
		/// Initial Level is set to Debug.
		/// </summary>
		/// <param name="name">
		/// The name of the log.
		/// </param>
		/// <param name="stream">
		///	The stream that will be used for logging,
		///	seeking while the logger is alive 
		///	</param>
		///	<param name="encoding">
		///	The encoding that will be used for this stream.
		///	<see cref="System.IO.StreamWriter"/>
		///	</param>
		///	<param name="bufferSize">
		///	The buffer size that will be used for this stream.
		///	<see cref="System.IO.StreamWriter"/>
		///	</param>
		public StreamLogger(String name, Stream stream, Encoding encoding, int bufferSize) : this(name, new StreamWriter(stream, encoding, bufferSize))
		{
		}

		~StreamLogger()
		{
			Dispose();
		}

		/// <summary>
		/// Creates a new <c>StreamLogger</c> with 
		/// Debug as default Level.
		/// </summary>
		/// <param name="name">The name of the log.</param>
		/// <param name="writer">The <c>StreamWriter</c> the log will write to.</param>
		protected StreamLogger(String name, StreamWriter writer) : base(name, LoggerLevel.Debug)
		{
			this.writer = writer;
			writer.AutoFlush = true;
		}

		protected override void Log(LoggerLevel level, String name, String message, Exception exception)
		{
			writer.WriteLine("[{0}] '{1}' {2}", level.ToString(), name, message);

			if (exception != null)
			{
				if (exception.StackTrace != null)
				{
					writer.WriteLine("[{0}] '{1}' {2}: {3}\n{4}", 
						level.ToString(), 
						name, 
						exception.GetType().FullName, 
						exception.Message, 
						exception.StackTrace);
				}
				else
				{
					writer.WriteLine("[{0}] '{1}' {2}: {3}", 
						level.ToString(), 
						name, 
						exception.GetType().FullName, 
						exception.Message);
				}
			}
		}

		public override ILogger CreateChildLogger(string name)
		{
			return null;
		}

		public void Dispose()
		{
			if (writer != null)
			{
				writer.Close();
			}
			
			writer = null;
		}
	}
}