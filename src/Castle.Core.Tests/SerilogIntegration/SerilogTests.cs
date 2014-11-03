// Copyright 2004-2014 Castle Project - http://www.castleproject.org/
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

namespace CastleTests.SerilogIntegration
{
#if DOTNET45 || DOTNET40
    using System;
    using System.IO;

    using Castle.Core.Logging;
    using Castle.Services.Logging.SerilogIntegration;

    using Serilog;
    using Serilog.Events;

    using NUnit.Framework;
    public class SerilogTests
    {
        [Test]
        public void should_log_work_with_first_creator()
        {
            var output = new StringWriter();

            var config = new LoggerConfiguration()
                .WriteTo.TextWriter(output)
                .MinimumLevel.Is(LogEventLevel.Debug);

            var factory = new SerilogFactory(config);
            var logger = factory.Create("TestingLogger");

            WriteTestLogs(logger);

            var logs = output.ToString();

            StringAssert.Contains("Testing debug", logs);
            StringAssert.Contains("Testing warning", logs);
        }

        [Test]
        public void should_log_work_with_second_creator()
        {
            var output = new StringWriter();

            var config = new LoggerConfiguration()
                .WriteTo.TextWriter(output);

            var factory = new SerilogFactory(config);
            var logger = factory.Create("TestingLogger", LoggerLevel.Debug);

            WriteTestLogs(logger);

            var logs = output.ToString();

            StringAssert.Contains("Testing debug", logs);
            StringAssert.Contains("Testing warning", logs);
        }

        [Test]
        public void should_log_info_when_off()
        {
            var output = new StringWriter();

            var config = new LoggerConfiguration()
                .WriteTo.TextWriter(output);

            var factory = new SerilogFactory(config);
            var logger = factory.Create("TestingLogger", LoggerLevel.Off);

            WriteTestLogs(logger);

            var logs = output.ToString();

            StringAssert.Contains("Testing info", logs);
            StringAssert.DoesNotContain("Testing debug", logs);
            StringAssert.Contains("Testing warning", logs);
        }

        [Test]
        public void should_log_objects()
        {
            var output = new StringWriter();

            var config = new LoggerConfiguration()
                .WriteTo.TextWriter(output);

            var factory = new SerilogFactory(config);
            var logger = factory.Create("TestingLogger", LoggerLevel.Debug);

            logger.DebugFormat("Testing Debug {@TestingData}", new { Name = "test", Value = 55 });

            var logs = output.ToString();

            StringAssert.Contains("{ Name: \"test\", Value: 55 }", logs);
        }

        [Test]
        public void should_log_exceptions()
        {
            var output = new StringWriter();

            var config = new LoggerConfiguration()
                .WriteTo.TextWriter(output);

            var factory = new SerilogFactory(config);
            var logger = factory.Create("TestingLogger", LoggerLevel.Debug);

            logger.Debug("Debug", new Exception("Debug Exception 1"));
            logger.DebugFormat(new Exception("Debug Exception 2"), "Debug");
            logger.DebugFormat(new Exception("Debug Exception 3"), null, "Debug");
            logger.Error("Error", new Exception("Error Exception 1"));
            logger.ErrorFormat(new Exception("Error Exception 2"), "Error");
            logger.ErrorFormat(new Exception("Error Exception 3"), null, "Error");
            logger.Fatal("Fatal", new Exception("Fatal Exception 1"));
            logger.FatalFormat(new Exception("Fatal Exception 2"), "Fatal");
            logger.FatalFormat(new Exception("Fatal Exception 3"), null, "Fatal");
            logger.Info("Info", new Exception("Info Exception 1"));
            logger.InfoFormat(new Exception("Info Exception 2"), "Info");
            logger.InfoFormat(new Exception("Info Exception 3"), null, "Info");
            logger.Warn("Warn", new Exception("Warn Exception 1"));
            logger.WarnFormat(new Exception("Warn Exception 2"), "Warn");
            logger.WarnFormat(new Exception("Warn Exception 3"), null, "Warn");

            var logs = output.ToString();
            StringAssert.Contains("Debug Exception 1", logs);
            StringAssert.Contains("Debug Exception 2", logs);
            StringAssert.Contains("Debug Exception 3", logs);
            StringAssert.Contains("Error Exception 1", logs);
            StringAssert.Contains("Error Exception 2", logs);
            StringAssert.Contains("Error Exception 3", logs);
            StringAssert.Contains("Fatal Exception 1", logs);
            StringAssert.Contains("Fatal Exception 2", logs);
            StringAssert.Contains("Fatal Exception 3", logs);
            StringAssert.Contains("Info Exception 1", logs);
            StringAssert.Contains("Info Exception 2", logs);
            StringAssert.Contains("Info Exception 3", logs);
            StringAssert.Contains("Warn Exception 1", logs);
            StringAssert.Contains("Warn Exception 2", logs);
            StringAssert.Contains("Warn Exception 3", logs);
        }

        private void WriteTestLogs(Castle.Core.Logging.ILogger logger)
        {
            logger.Debug("Testing debug");
            logger.Warn("Testing warning");
            logger.Info("Testing info");
        }
    }
#endif
}