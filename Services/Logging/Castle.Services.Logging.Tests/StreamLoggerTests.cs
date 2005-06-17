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

namespace Castle.Services.Logging.Tests
{
	using System;
	using System.IO;
	using System.Text.RegularExpressions;

	using NUnit.Framework;

	/// <summary>
	/// Summary description for StreamLoggerTests.
	/// </summary>
	[TestFixture]
	public class StreamLoggerTests
	{
		private const string Name = "Test";

		private StreamLogger logger;
		private MemoryStream stream;

		[SetUp]
		public void SetUp()
		{
			stream = new MemoryStream();

			logger = new StreamLogger(Name, stream);
			logger.Level = LoggerLevel.Debug;
		}


		[Test]
		public void Debug()
		{
			string message = "Debug message";
			LoggerLevel level = LoggerLevel.Debug;
			Exception exception = null;
			
			logger.Debug(message);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void DebugWithException()
		{
			string message = "Debug message 2";
			LoggerLevel level = LoggerLevel.Debug;
			Exception exception = new Exception();
			
			logger.Debug(message, exception);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void Info()
		{
			string message = "Info message";
			LoggerLevel level = LoggerLevel.Info;
			Exception exception = null;
			
			logger.Info(message);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void InfoWithException()
		{
			string message = "Info message 2";
			LoggerLevel level = LoggerLevel.Info;
			Exception exception = new Exception();
			
			logger.Info(message, exception);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void Warn()
		{
			string message = "Warn message";
			LoggerLevel level = LoggerLevel.Warn;
			Exception exception = null;
			
			logger.Warn(message);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void WarnWithException()
		{
			string message = "Warn message 2";
			LoggerLevel level = LoggerLevel.Warn;
			Exception exception = new Exception();
			
			logger.Warn(message, exception);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void Error()
		{
			string message = "Error message";
			LoggerLevel level = LoggerLevel.Error;
			Exception exception = null;
			
			logger.Error(message);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void ErrorWithException()
		{
			string message = "Error message 2";
			LoggerLevel level = LoggerLevel.Error;
			Exception exception = new Exception();
			
			logger.Error(message, exception);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void FatalError()
		{
			string message = "FatalError message";
			LoggerLevel level = LoggerLevel.Fatal;
			Exception exception = null;
			
			logger.FatalError(message);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void FatalErrorWithException()
		{
			string message = "FatalError message 2";
			LoggerLevel level = LoggerLevel.Fatal;
			Exception exception = new Exception();
			
			logger.FatalError(message, exception);

			ValidateCall(level, message, exception);
		}


		private void ValidateCall(LoggerLevel level, String expectedMessage, Exception expectedException)
		{
			
			stream.Seek(0, SeekOrigin.Begin);
			
			StreamReader reader = new StreamReader(stream);
			String line = reader.ReadLine();

			Match match = Regex.Match(line, @"^\[(?<level>[^]]+)\] '(?<name>[^']+)' (?<message>.*)$");

			Assert.IsTrue(match.Success, "StreamLogger.Log did not match the format");
			Assert.AreEqual(Name, match.Groups["name"].Value, "StreamLogger.Log did not write the correct Name");
			Assert.AreEqual(level.ToString(), match.Groups["level"].Value, "StreamLogger.Log did not write the correct Level");
			Assert.AreEqual(expectedMessage, match.Groups["message"].Value, "StreamLogger.Log did not write the correct Message");

			line = reader.ReadLine();
			
			if (expectedException == null)
			{
				Assert.IsNull(line);
			}
			else
			{
				match = Regex.Match(line, @"^\[(?<level>[^]]+)\] '(?<name>[^']+)' (?<type>[^:]+): (?<message>.*)$");

				Assert.IsTrue(match.Success, "StreamLogger.Log did not match the format");
				Assert.AreEqual(Name, match.Groups["name"].Value, "StreamLogger.Log did not write the correct Name");
				Assert.AreEqual(level.ToString(), match.Groups["level"].Value, "StreamLogger.Log did not write the correct Level");
				Assert.AreEqual(expectedException.GetType().FullName, match.Groups["type"].Value, "StreamLogger.Log did not write the correct Exception Type");
				Assert.AreEqual(expectedException.Message, match.Groups["message"].Value, "StreamLogger.Log did not write the correct Exception Message");

				if (expectedException.StackTrace == null)
				{
					line = reader.ReadLine();
					Assert.IsNull(line, "StreamLogger.Log wrote an extra line");
				}
				else
				{
					foreach (String stackLine in expectedException.StackTrace.Split('\n'))
					{
						line = reader.ReadLine();

						Assert.AreEqual(stackLine, line, "StreamLogger.Log did not write the correct Exception StackTrace");
					}
				}
			}
		}
	
	}
}
