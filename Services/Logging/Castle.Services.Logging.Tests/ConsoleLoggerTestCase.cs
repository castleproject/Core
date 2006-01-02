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

namespace Castle.Facilities.Logging.Tests
{
	using System;
	using System.IO;

	using Castle.Services.Logging;

	using NUnit.Framework;

	[TestFixture]
	public class ConsoleLoggerTestCase
	{
		private StringWriter outWriter = new StringWriter();
		private StringWriter errorWriter = new StringWriter();

		[SetUp]
		public void ReplaceOut()
		{
			outWriter.GetStringBuilder().Length = 0;
			errorWriter.GetStringBuilder().Length = 0;

			Console.SetOut(outWriter);
			Console.SetError(errorWriter);
		}

		[Test]
		public void InfoLogger()
		{
			ConsoleLogger log = new ConsoleLogger("Logger", LoggerLevel.Info);

			log.Debug("Some debug message");
			log.Info("Some info message");
			log.Error("Some error message");
			log.FatalError("Some fatal error message");
			log.Warn("Some warn message");

			String logcontents = outWriter.GetStringBuilder().ToString();
			Assert.AreEqual("[Info] 'Logger' Some info message\r\n[Error] 'Logger' Some error message\r\n[Fatal] 'Logger' Some fatal error message\r\n[Warn] 'Logger' Some warn message\r\n", logcontents, "logcontents don't match");
		}

		[Test]
		public void DebugLogger()
		{
			ConsoleLogger log = new ConsoleLogger("Logger", LoggerLevel.Debug);

			log.Debug("Some debug message");
			log.Info("Some info message");
			log.Error("Some error message");
			log.FatalError("Some fatal error message");
			log.Warn("Some warn message");

			String logcontents = outWriter.GetStringBuilder().ToString();
			Assert.AreEqual("[Debug] 'Logger' Some debug message\r\n[Info] 'Logger' Some info message\r\n[Error] 'Logger' Some error message\r\n[Fatal] 'Logger' Some fatal error message\r\n[Warn] 'Logger' Some warn message\r\n", logcontents);
		}

        [Test]
        public void WarnLogger() {
            ConsoleLogger log = new ConsoleLogger("Logger", LoggerLevel.Warn);

            log.Debug("Some debug message");
            log.Info("Some info message");
            log.Error("Some error message");
            log.FatalError("Some fatal error message");
            log.Warn("Some warn message");

            String logcontents = outWriter.GetStringBuilder().ToString();
            Assert.AreEqual("[Error] 'Logger' Some error message\r\n[Fatal] 'Logger' Some fatal error message\r\n[Warn] 'Logger' Some warn message\r\n", logcontents);            
        }

		[Test]
		public void ExceptionLogging()
		{
			ConsoleLogger log = new ConsoleLogger("Logger", LoggerLevel.Debug);

			log.Debug("Some debug message", new ApplicationException("Some exception message"));

			String logcontents = outWriter.GetStringBuilder().ToString();
			Assert.AreEqual("[Debug] 'Logger' Some debug message\r\n[Debug] 'Logger' System.ApplicationException: Some exception message \r\n", logcontents);
		}
	}
}
