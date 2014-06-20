// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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

        private void WriteTestLogs(Castle.Core.Logging.ILogger logger)
        {
            logger.Debug("Testing debug");
            logger.Warn("Testing warning");
            logger.Info("Testing info");
        }
    }
#endif
}