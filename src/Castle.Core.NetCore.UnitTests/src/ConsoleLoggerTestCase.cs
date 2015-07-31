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


#if !SILVERLIGHT && !NETCORE // In Silverlight Console.SetOut throws security exception

namespace Castle.Core.Logging.Tests
{
	using System;
	using System.IO;

	using Castle.Core.Logging;

	using Xunit;

	public class ConsoleLoggerTestCase
	{
		private StringWriter outWriter = new StringWriter();
		private StringWriter errorWriter = new StringWriter();

		public ConsoleLoggerTestCase()
		{
			outWriter.GetStringBuilder().Length = 0;
			errorWriter.GetStringBuilder().Length = 0;

			Console.SetOut(outWriter);
			Console.SetError(errorWriter);
		}

		[Fact]
		public void InfoLogger()
		{
			ConsoleLogger log = new ConsoleLogger("Logger", LoggerLevel.Info);

			log.Debug("Some debug message");
			log.Info("Some info message");
			log.Error("Some error message");
			log.Fatal("Some fatal error message");
			log.Warn("Some warn message");

			String logcontents = outWriter.GetStringBuilder().ToString();

			StringWriter expected = new StringWriter();
			expected.WriteLine("[Info] 'Logger' Some info message");
			expected.WriteLine("[Error] 'Logger' Some error message");
			expected.WriteLine("[Fatal] 'Logger' Some fatal error message");
			expected.WriteLine("[Warn] 'Logger' Some warn message");

			Assert.Equal(expected.GetStringBuilder().ToString(), logcontents); //, "logcontents don't match");
		}

		[Fact]
		public void DebugLogger()
		{
			ConsoleLogger log = new ConsoleLogger("Logger", LoggerLevel.Debug);

			log.Debug("Some debug message");
			log.Info("Some info message");
			log.Error("Some error message");
			log.Fatal("Some fatal error message");
			log.Warn("Some warn message");

			String logcontents = outWriter.GetStringBuilder().ToString();

			StringWriter expected = new StringWriter();
			expected.WriteLine("[Debug] 'Logger' Some debug message");
			expected.WriteLine("[Info] 'Logger' Some info message");
			expected.WriteLine("[Error] 'Logger' Some error message");
			expected.WriteLine("[Fatal] 'Logger' Some fatal error message");
			expected.WriteLine("[Warn] 'Logger' Some warn message");

			Assert.Equal(expected.GetStringBuilder().ToString(), logcontents); //, "logcontents don't match");
		}

		[Fact]
		public void WarnLogger()
		{
			ConsoleLogger log = new ConsoleLogger("Logger", LoggerLevel.Warn);

			log.Debug("Some debug message");
			log.Info("Some info message");
			log.Error("Some error message");
			log.Fatal("Some fatal error message");
			log.Warn("Some warn message");

			String logcontents = outWriter.GetStringBuilder().ToString();

			StringWriter expected = new StringWriter();
			expected.WriteLine("[Error] 'Logger' Some error message");
			expected.WriteLine("[Fatal] 'Logger' Some fatal error message");
			expected.WriteLine("[Warn] 'Logger' Some warn message");

			Assert.Equal(expected.GetStringBuilder().ToString(), logcontents); //, "logcontents don't match");
		}

		[Fact]
		public void ExceptionLogging()
		{
			ConsoleLogger log = new ConsoleLogger("Logger", LoggerLevel.Debug);

			log.Debug("Some debug message", new ApplicationException("Some exception message"));

			String logcontents = outWriter.GetStringBuilder().ToString();

			StringWriter expected = new StringWriter();
			expected.WriteLine("[Debug] 'Logger' Some debug message");
			expected.WriteLine("[Debug] 'Logger' System.ApplicationException: Some exception message ");

			Assert.Equal(expected.GetStringBuilder().ToString(), logcontents); //, "logcontents don't match");
		}
	}
}

#endif