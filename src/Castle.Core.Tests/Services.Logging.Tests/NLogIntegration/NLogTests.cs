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

namespace Castle.Services.Logging.NLogIntegration.Tests
{
	using System.Runtime.CompilerServices;

	using NLog;
	using NLog.Config;
	using NLog.Targets;

	using NUnit.Framework;

	public class NLogTests
	{
		[Test]
		[Bug("CORE-44")]
		public void Should_preserve_correct_callsite_information()
		{
			// Step 1. Create configuration object 
			var config = new LoggingConfiguration();

			// Step 2. Create targets and add them to the configuration 
			var target = new MemoryTarget();
			config.AddTarget("target", target);

			// Step 3. Set target properties 
			target.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${callsite} ${message}";

			// Step 4. Define rules
			var rule = new LoggingRule("*", LogLevel.Debug, target);
			config.LoggingRules.Add(rule);


			var factory = new NLogFactory(config);

			WriteLogMessage(factory);
			var logMessage = target.Logs[0];
			StringAssert.Contains("NLogTests.WriteLogMessage", logMessage);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void WriteLogMessage(NLogFactory factory)
		{
			var logger = factory.Create("MyLoggerName");
			logger.Debug("something");
		}
	}
}