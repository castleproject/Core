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
	using NLog.Targets;
	using NUnit.Framework;

	/// <summary>
	/// Summary description for ExtendedLog4NetFacilityTests.
	/// </summary>
	[TestFixture]
	public class ExtendedNLogFacilityTests : BaseTest
	{
		private IWindsorContainer container;

		[SetUp]
		public void Setup()
		{
			container = base.CreateConfiguredContainer(LoggerImplementation.ExtendedNLog);
		}

		[TearDown]
		public void Teardown()
		{
			container.Dispose();
		}

		[Test]
		public void SimpleTest()
		{

			container.AddComponent("component1", typeof(SimpleLoggingComponent));
			SimpleLoggingComponent test = container["component1"] as SimpleLoggingComponent;

			test.DoSomething();

			String expectedLogOutput = String.Format("|INFO|{0}|Hello world", typeof(SimpleLoggingComponent).FullName);
			String actualLogOutput = (NLog.LogManager.Configuration.FindTargetByName("memory") as MemoryTarget).Logs[0].ToString();
			actualLogOutput = actualLogOutput.Substring(actualLogOutput.IndexOf('|'));
			
			Assert.AreEqual(expectedLogOutput, actualLogOutput);

			container.AddComponent("component2", typeof(SmtpServer));
			ISmtpServer smtpServer = container["component2"] as ISmtpServer;

			smtpServer.Start();
			smtpServer.InternalSend("rbellamy@pteradigm.com", "jobs@castlestronghold.com", "We're looking for a few good porgrammars.");
			smtpServer.Stop();

			expectedLogOutput = String.Format("|INFO|Castle.Facilities.Logging.Tests.Classes.SmtpServer|InternalSend rbellamy@pteradigm.com jobs@castlestronghold.com We're looking for a few good porgrammars.", typeof(SmtpServer).FullName);
			actualLogOutput = (NLog.LogManager.Configuration.FindTargetByName("memory") as MemoryTarget).Logs[1].ToString();
			actualLogOutput = actualLogOutput.Substring(actualLogOutput.IndexOf('|'));
			
			Assert.AreEqual(expectedLogOutput, actualLogOutput.ToString());

		}

		[Test]
		public void ContextTest()
		{
			container.AddComponent("component1", typeof(ComplexLoggingComponent));
			ComplexLoggingComponent complexLoggingComponent = container["component1"] as ComplexLoggingComponent;

			complexLoggingComponent.DoSomeContextual();

			String expectedLogOutput = String.Format("|DEBUG|Castle.Facilities.Logging.Tests.Classes.ComplexLoggingComponent|flam|bar|Outside Inside0|Bim, bam boom.", typeof(ComplexLoggingComponent).FullName);
			String actualLogOutput = (NLog.LogManager.Configuration.FindTargetByName("memory1") as MemoryTarget).Logs[0].ToString();
			actualLogOutput = actualLogOutput.Substring(actualLogOutput.IndexOf('|'));

			Assert.AreEqual(expectedLogOutput, actualLogOutput.ToString());
		}
	}
}
