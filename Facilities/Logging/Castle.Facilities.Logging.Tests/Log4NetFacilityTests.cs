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

namespace Castle.Facilities.Logging.Tests
{
	using System;
	using System.IO;
	using Castle.Facilities.Logging.Tests.Classes;
	using Castle.Windsor;
	using log4net;
	using log4net.Appender;
	using log4net.Layout;
	using log4net.Repository.Hierarchy;
	using NUnit.Framework;

	/// <summary>
	/// Summary description for Log4NetFacilityTests.
	/// </summary>
	[TestFixture]
	public class Log4NetFacilityTests : BaseTest
	{
		private IWindsorContainer container;

		[SetUp]
		public void Setup()
		{
			container = base.CreateConfiguredContainer(LoggerImplementation.ExtendedLog4net);
		}

		[TearDown]
		public void Teardown()
		{
			container.Dispose();
		}

		[Test]
		public void SimpleTest()
		{
			container.AddComponent("component", typeof(SimpleLoggingComponent));
			SimpleLoggingComponent test = container["component"] as SimpleLoggingComponent;

			test.DoSomething();

			String expectedLogOutput = String.Format("[INFO ] [{0}] - Hello world" + Environment.NewLine, typeof(SimpleLoggingComponent).FullName);
			MemoryAppender memoryAppender = ((Hierarchy) LogManager.GetRepository()).Root.GetAppender("memory") as MemoryAppender;
			TextWriter actualLogOutput = new StringWriter();
			PatternLayout patternLayout = new PatternLayout("[%-5level] [%logger] - %message%newline");
			patternLayout.Format(actualLogOutput, memoryAppender.GetEvents()[0]);

			Assert.AreEqual(expectedLogOutput, actualLogOutput.ToString());
		}
	}
}
