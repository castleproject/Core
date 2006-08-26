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

namespace Castle.Core.Logging.Tests
{
	using System;
	using NUnit.Framework;
	using Castle.Core.Logging;
	
	/// <summary>
	/// All MixedLogger tests are placed within this class
	/// </summary>
	[TestFixture]
	public class MixedLoggerTests
	{
		private MixedLogger logger;

		[SetUp]
		public void SetUp()
		{
			logger = new MixedLogger();
		}


		[Test]
		public void SetLogger()
		{
			ILogger childLogger = new NullLogger();

			logger[LoggerLevel.Debug] = childLogger;

			Assert.AreSame(childLogger, logger[LoggerLevel.Debug], "MixedLogger[] does not set the logger");
			
		}

		[Test]
		public void SetLoggerTwice()
		{
			ILogger childLogger1 = new NullLogger();
			ILogger childLogger2 = new NullLogger();

			logger[LoggerLevel.Debug] = childLogger1;
			logger[LoggerLevel.Debug] = childLogger2;

			Assert.AreSame(childLogger2, logger[LoggerLevel.Debug], "MixedLogger[] does not change the logger");
			
		}

		[Test]
		public void SetLoggerToNull()
		{
			ILogger childLogger = new NullLogger();

			logger[LoggerLevel.Debug] = childLogger;
			logger[LoggerLevel.Debug] = null;

			Assert.IsNull(logger[LoggerLevel.Debug], "MixedLogger[] does not change the logger to null");
			
		}


		#region Is<Level>Enabled tests

		[Test]
		public void IsDebugEnabledNoneSet()
		{
			Assert.IsFalse(logger.IsDebugEnabled, "MixedLogger.IsDebugEnabled returns true without having a logger for that level");
		}
		[Test]
		public void IsDebugEnabled()
		{
			logger[LoggerLevel.Debug] = new NullLogger();

			Assert.IsTrue(logger.IsDebugEnabled, "MixedLogger.IsDebugEnabled does not react to a logger being set");
		}
		[Test]
		public void IsDebugEnabledResetToNull()
		{
			logger[LoggerLevel.Debug] = new NullLogger();
			logger[LoggerLevel.Debug] = null;

			Assert.IsFalse(logger.IsDebugEnabled, "MixedLogger.IsDebugEnabled does not react to a logger being removed");
		}


		[Test]
		public void IsInfoEnabledNoneSet()
		{
			Assert.IsFalse(logger.IsInfoEnabled, "MixedLogger.IsInfoEnabled returns true without having a logger for that level");
		}

		[Test]
		public void IsInfoEnabled()
		{
			logger[LoggerLevel.Info] = new NullLogger();
			Assert.IsTrue(logger.IsInfoEnabled, "MixedLogger.IsInfoEnabled does not react to a logger being set");
		}

		[Test]
		public void IsInfoEnabledResetToNull()
		{
			logger[LoggerLevel.Info] = new NullLogger();
			logger[LoggerLevel.Info] = null;

			Assert.IsFalse(logger.IsInfoEnabled, "MixedLogger.IsInfoEnabled does not react to a logger being removed");
		}


		[Test]
		public void IsWarnEnabledNoneSet()
		{
			Assert.IsFalse(logger.IsWarnEnabled, "MixedLogger.IsWarnEnabled returns true without having a logger for that level");
		}

		[Test]
		public void IsWarnEnabled()
		{
			logger[LoggerLevel.Warn] = new NullLogger();
			Assert.IsTrue(logger.IsWarnEnabled, "MixedLogger.IsWarnEnabled does not react to a logger being set");
		}

		[Test]
		public void IsWarnEnabledResetToNull()
		{
			logger[LoggerLevel.Warn] = new NullLogger();
			logger[LoggerLevel.Warn] = null;

			Assert.IsFalse(logger.IsWarnEnabled, "MixedLogger.IsWarnEnabled does not react to a logger being removed");
		}


		[Test]
		public void IsErrorEnabledNoneSet()
		{
			Assert.IsFalse(logger.IsErrorEnabled, "MixedLogger.IsErrorEnabled returns true without having a logger for that level");
		}

		[Test]
		public void IsErrorEnabled()
		{
			logger[LoggerLevel.Error] = new NullLogger();
			Assert.IsTrue(logger.IsErrorEnabled, "MixedLogger.IsErrorEnabled does not react to a logger being set");
		}

		[Test]
		public void IsErrorEnabledResetToNull()
		{
			logger[LoggerLevel.Error] = new NullLogger();
			logger[LoggerLevel.Error] = null;

			Assert.IsFalse(logger.IsErrorEnabled, "MixedLogger.IsErrorEnabled does not react to a logger being removed");
		}


		[Test]
		public void IsFatalErrorEnabledNoneSet()
		{
			Assert.IsFalse(logger.IsFatalErrorEnabled, "MixedLogger.IsFatalErrorEnabled returns true without having a logger for that level");
		}

		[Test]
		public void IsFatalErrorEnabled()
		{
			logger[LoggerLevel.Fatal] = new NullLogger();
			Assert.IsTrue(logger.IsFatalErrorEnabled, "MixedLogger.IsFatalErrorEnabled does not react to a logger being set");
		}

		[Test]
		public void IsFatalErrorEnabledResetToNull()
		{
			logger[LoggerLevel.Fatal] = new NullLogger();
			logger[LoggerLevel.Fatal] = null;

			Assert.IsFalse(logger.IsFatalErrorEnabled, "MixedLogger.IsFatalErrorEnabled does not react to a logger being removed");
		}

		#endregion

		
	}
}
