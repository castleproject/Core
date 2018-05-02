// Copyright 2004-2016 Castle Project - http://www.castleproject.org/
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

#if FEATURE_TEST_SERILOGINTEGRATION

namespace Castle.Services.Logging.SerilogIntegration.Tests
{
    using System;
    using System.IO;

    using Serilog;

    using NUnit.Framework;

    public class SerilogTests
    {
        [Test]
        public void should_use_serilog_silent_logger()
        {
            var factory = new SerilogFactory();
            var logger = factory.Create("TestingLogger");

            WriteTestLogs(logger);
        }

        [Test]
        public void should_log_work_with_first_creator()
        {
            var output = new StringWriter();

            var serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.TextWriter(output)
                .CreateLogger();

            var factory = new SerilogFactory(serilogLogger);
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

            var serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.TextWriter(output)
                .CreateLogger();

            var factory = new SerilogFactory(serilogLogger);
            var logger = factory.Create("TestingLogger");

            WriteTestLogs(logger);

            var logs = output.ToString();

            StringAssert.Contains("Testing debug", logs);
            StringAssert.Contains("Testing warning", logs);
        }

        [Test]
        public void should_not_log_debug_with_new_serilog_config()
        {
            var output = new StringWriter();

            // New LoggerConfiguration which defaults to Information
            var serilogLogger = new LoggerConfiguration()
                .WriteTo.TextWriter(output)
                .CreateLogger();

            var factory = new SerilogFactory(serilogLogger);
            var logger = factory.Create("TestingLogger");

            WriteTestLogs(logger);

            var logs = output.ToString();

            StringAssert.DoesNotContain("Testing debug", logs);
            StringAssert.Contains("Testing info", logs);
            StringAssert.Contains("Testing warning", logs);
        }

        private void WriteTestLogs(Castle.Core.Logging.ILogger logger)
        {
            logger.Debug("Testing debug");
            logger.Info("Testing info");
            logger.Warn("Testing warning");
        }

        [Test]
        public void should_log_objects()
        {
            var output = new StringWriter();

            var serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.TextWriter(output)
                .CreateLogger();

            var factory = new SerilogFactory(serilogLogger);
            var logger = factory.Create("TestingLogger");

            logger.DebugFormat("Testing Debug {@TestingData}", new { Name = "test", Value = 55 });

            var logs = output.ToString();

            StringAssert.Contains("{ Name: \"test\", Value: 55 }", logs);
        }

        [Test]
        public void should_log_exceptions()
        {
            var output = new StringWriter();

            var serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.TextWriter(output)
                .CreateLogger();

            var factory = new SerilogFactory(serilogLogger);
            var logger = factory.Create("TestingLogger");

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

        [Test]
        public void should_log_with_source_context()
        {
            var output = new StringWriter();
            var serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.TextWriter(output)
                .CreateLogger();

            var factory = new SerilogFactory(serilogLogger);

            var logger1 = factory.Create("MyLogger1");
            var logger2 = factory.Create("MyLogger2");

            logger1.Info("Test1 using {SourceContext}");
            logger2.Debug("Test2 using {SourceContext}");

            var logs = output.ToString();
            StringAssert.Contains("MyLogger1", logs);
            StringAssert.Contains("MyLogger2", logs);
        }
    }
}

#endif