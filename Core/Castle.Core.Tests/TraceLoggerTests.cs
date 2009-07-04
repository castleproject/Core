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

#if !SILVERLIGHT
namespace Castle.Core.Logging.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Diagnostics;

	using NUnit.Framework;

	/// <summary>
	/// Tests the TraceLogger and TraceLoggerFactory classes
	/// </summary>
	[TestFixture]
	public class TraceLoggerTests
	{
		[SetUp]
		public void Initialize()
		{
			Listener.ClearMessages();
		}

		[TearDown]
		public void Cleanup()
		{
			Listener.ClearMessages();
		}

		[Test]
		public void WritingToLoggerByType()
		{
			TraceLoggerFactory factory = new TraceLoggerFactory();
			ILogger logger = factory.Create(typeof(TraceLoggerTests), LoggerLevel.Debug);
			logger.Debug("this is a tracing message");

			Listener.AssertContains("testsrule", "Castle.Core.Logging.Tests.TraceLoggerTests");
			Listener.AssertContains("testsrule", "this is a tracing message");
		}

		[Test]
		public void TracingErrorInformation()
		{
			TraceLoggerFactory factory = new TraceLoggerFactory();
			ILogger logger = factory.Create(typeof(TraceLoggerTests), LoggerLevel.Debug);
			try
			{
				try
				{
					string fakearg = "Thisisavalue";
					throw new ArgumentOutOfRangeException("fakearg", fakearg, "Thisisamessage" );
				}
				catch (Exception ex)
				{
					throw new ApplicationException("Inner error is " + ex.Message, ex);
				}
			}
			catch (Exception ex)
			{
				logger.Error("Problem handled", ex);
			}

			Listener.AssertContains("testsrule", "Castle.Core.Logging.Tests.TraceLoggerTests");
			Listener.AssertContains("testsrule", "Problem handled");
			Listener.AssertContains("testsrule", "ApplicationException");
			Listener.AssertContains("testsrule", "Inner error is");
			Listener.AssertContains("testsrule", "ArgumentOutOfRangeException");
			Listener.AssertContains("testsrule", "fakearg");
			Listener.AssertContains("testsrule", "Thisisavalue");
			Listener.AssertContains("testsrule", "Thisisamessage");
		}

		[Test]
		public void FallUpToShorterSourceName()
		{
			TraceLoggerFactory factory = new TraceLoggerFactory();
			ILogger logger = factory.Create(typeof(Castle.Core.Configuration.Xml.XmlConfigurationDeserializer), LoggerLevel.Debug);
			logger.Info("Logging to config namespace");

			Listener.AssertContains("configrule", "Castle.Core.Configuration.Xml.XmlConfigurationDeserializer");
			Listener.AssertContains("configrule", "Logging to config namespace");            
		}

		[Test]
		public void FallUpToDefaultSource()
		{
			TraceLoggerFactory factory = new TraceLoggerFactory();
			ILogger logger = factory.Create("System.Xml.XmlDocument", LoggerLevel.Debug);
			logger.Info("Logging to non-configured namespace namespace");

			Listener.AssertContains("defaultrule", "System.Xml.XmlDocument");
			Listener.AssertContains("defaultrule", "Logging to non-configured namespace namespace");
		}

		#region in-memory listener class

		/// <summary>
		/// This class captures trace text and records it to StringBuilders in a static dictionary.   
		/// Used for the sake of unit testing.
		/// </summary>
		public class Listener : TraceListener
		{
			public Listener()
			{
			}

			public Listener(string initializationData)
			{
				traceName = initializationData;
			}

			static Dictionary<string, StringBuilder> traces = new Dictionary<string, StringBuilder>();
			readonly string traceName;

			StringBuilder GetStringBuilder()
			{
				lock (traces)
				{
					if (!traces.ContainsKey(traceName))
						traces.Add(traceName, new StringBuilder());

					return traces[traceName];
				}
			}

			public override void Write(string message)
			{
				GetStringBuilder().Append(message);
			}

			public override void WriteLine(string message)
			{
				GetStringBuilder().AppendLine(message);
			}

			public static void AssertContains(string traceName, string expected)
			{
				Assert.IsTrue(traces.ContainsKey(traceName), "Trace named {0} not found", traceName);
				Assert.IsTrue(traces[traceName].ToString().Contains(expected), string.Format("Trace text expected to contain '{0}'", expected));
			}

			public static void ClearMessages()
			{
				traces = new Dictionary<string, StringBuilder>();
			}
		}
		#endregion
	}
}
#endif
