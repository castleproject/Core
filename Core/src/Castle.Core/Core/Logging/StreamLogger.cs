// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Logging
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
	[Serializable]
	public class StreamLogger : LevelFilteredLogger, IDisposable
	{
		private StreamWriter writer;

		///<summary>
		///  Creates a new <c>StreamLogger</c> with default encoding 
		///  and buffer size. Initial Level is set to Debug.
		///</summary>
		///<param name = "name">
		///  The name of the log.
		///</param>
		///<param name = "stream">
		///  The stream that will be used for logging,
		///  seeking while the logger is alive 
		///</param>
		public StreamLogger(String name, Stream stream) : this(name, new StreamWriter(stream))
		{
		}

		///<summary>
		///  Creates a new <c>StreamLogger</c> with default buffer size.
		///  Initial Level is set to Debug.
		///</summary>
		///<param name = "name">
		///  The name of the log.
		///</param>
		///<param name = "stream">
		///  The stream that will be used for logging,
		///  seeking while the logger is alive 
		///</param>
		///<param name = "encoding">
		///  The encoding that will be used for this stream.
		///  <see cref = "StreamWriter" />
		///</param>
		public StreamLogger(String name, Stream stream, Encoding encoding) : this(name, new StreamWriter(stream, encoding))
		{
		}

		///<summary>
		///  Creates a new <c>StreamLogger</c>. 
		///  Initial Level is set to Debug.
		///</summary>
		///<param name = "name">
		///  The name of the log.
		///</param>
		///<param name = "stream">
		///  The stream that will be used for logging,
		///  seeking while the logger is alive 
		///</param>
		///<param name = "encoding">
		///  The encoding that will be used for this stream.
		///  <see cref = "StreamWriter" />
		///</param>
		///<param name = "bufferSize">
		///  The buffer size that will be used for this stream.
		///  <see cref = "StreamWriter" />
		///</param>
		public StreamLogger(String name, Stream stream, Encoding encoding, int bufferSize)
			: this(name, new StreamWriter(stream, encoding, bufferSize))
		{
		}

		~StreamLogger()
		{
			Dispose(false);
		}

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (writer != null)
				{
					writer.Close();
					writer = null;
				}
			}
		}

		/// <summary>
		///   Creates a new <c>StreamLogger</c> with 
		///   Debug as default Level.
		/// </summary>
		/// <param name = "name">The name of the log.</param>
		/// <param name = "writer">The <c>StreamWriter</c> the log will write to.</param>
		protected StreamLogger(String name, StreamWriter writer) : base(name, LoggerLevel.Debug)
		{
			this.writer = writer;
			writer.AutoFlush = true;
		}

		protected override void Log(LoggerLevel loggerLevel, String loggerName, String message, Exception exception)
		{
			if (writer == null)
			{
				return; // just in case it's been disposed
			}

			writer.WriteLine("[{0}] '{1}' {2}", loggerLevel, loggerName, message);

			if (exception != null)
			{
				writer.WriteLine("[{0}] '{1}' {2}: {3} {4}",
				                 loggerLevel,
				                 loggerName,
				                 exception.GetType().FullName,
				                 exception.Message,
				                 exception.StackTrace);
			}
		}

		public override ILogger CreateChildLogger(string loggerName)
		{
			// TODO: We could create a ChildStreamLogger that didn't take ownership of the stream

			throw new NotSupportedException("A streamlogger does not support child loggers");
		}
	}
}