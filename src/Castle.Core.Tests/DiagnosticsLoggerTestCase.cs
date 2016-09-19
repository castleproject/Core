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

#if FEATURE_EVENTLOG
namespace Castle.Core.Logging.Tests
{
	using System;
	using System.Diagnostics;
	using System.Security;
	using System.Security.Principal;

	using NUnit.Framework;

	[TestFixture]
	public class DiagnosticsLoggerTestCase
	{
		private static bool ignore;

		[SetUp]
		public void Clear()
		{
			AssertAdmin();
			if (EventLog.Exists("castle_testlog"))
			{
				EventLog.Delete("castle_testlog");
			}
		}

		[TearDown]
		public void Reset()
		{
			if (ignore) return;
			EventLog.Delete("castle_testlog");
		}

		private void AssertAdmin()
		{
			if (RunningOnNIX((int)Environment.OSVersion.Platform))
			{
				Environment.SetEnvironmentVariable("MONO_EVENTLOG_TYPE", "local:" + Environment.CurrentDirectory);
				return;
			}

			WindowsPrincipal windowsPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
			try
			{
				if (windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator) == false)
				{
					ignore = true;
					Assert.Ignore("This test case only valid when running as admin");
				}
			}
			catch(SecurityException)
			{
				// turns out, although undocumented, checking for role can throw SecurityException. Thanks Microsoft.
				ignore = true;
				Assert.Ignore("This test case only valid when running as admin");
			}
		}

		private bool RunningOnNIX(int p)
		{
			// taken from http://www.mono-project.com/FAQ:_Technical
			return (p == 4) || (p == 6) || (p == 128);
		}

		[Test]
		public void SimpleUsage()
		{
			DiagnosticsLogger logger = new DiagnosticsLogger("castle_testlog", "test_source");

			logger.Warn("my message");
			logger.Error("my other message", new Exception("Bad, bad exception"));

			EventLog log = new EventLog();
			log.Log = "castle_testlog";
			log.MachineName = ".";

			Assert.AreEqual(2, log.Entries.Count);
		}
	}
}
#endif
