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
	using System.ComponentModel;
	
	using NUnit.Framework;

	using Castle.Core.Logging;


	/// <summary>
	/// Contains all tests relating to the properties.
	/// </summary>
	[TestFixture]
	public class PropertyTests
	{
		private LevelFilteredLogger logger;

		[SetUp]
		public void SetUp()
		{
			logger = new LevelFilteredLoggerInstance(null);
		}

		[Test]
		public void LevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			Assert.IsTrue(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning true when the level is Debug");
			Assert.IsTrue(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning true when the level is Debug");
			Assert.IsTrue(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning true when the level is Debug");
			Assert.IsTrue(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning true when the level is Debug");
			Assert.IsTrue(logger.IsFatalErrorEnabled, "LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Debug");
		}

		[Test]
		public void LevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			Assert.IsFalse(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Info");
			Assert.IsTrue(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning true when the level is Info");
			Assert.IsTrue(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning true when the level is Info");
			Assert.IsTrue(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning true when the level is Info");
			Assert.IsTrue(logger.IsFatalErrorEnabled, "LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Info");
		}

		[Test]
		public void LevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			Assert.IsFalse(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Warn");
			Assert.IsFalse(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning false when the level is Warn");
			Assert.IsTrue(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning true when the level is Warn");
			Assert.IsTrue(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning true when the level is Warn");
			Assert.IsTrue(logger.IsFatalErrorEnabled, "LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Warn");
		}

		[Test]
		public void LevelError()
		{
			logger.Level = LoggerLevel.Error;

			Assert.IsFalse(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Error");
			Assert.IsFalse(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning false when the level is Error");
			Assert.IsFalse(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning false when the level is Error");
			Assert.IsTrue(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning true when the level is Error");
			Assert.IsTrue(logger.IsFatalErrorEnabled, "LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Error");
		}

		[Test]
		public void LevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			Assert.IsFalse(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Fatal");
			Assert.IsFalse(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning false when the level is Fatal");
			Assert.IsFalse(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning false when the level is Fatal");
			Assert.IsFalse(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning false when the level is Fatal");
			Assert.IsTrue(logger.IsFatalErrorEnabled, "LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Fatal");
		}

		[Test]
		public void LevelOff()
		{
			logger.Level = LoggerLevel.Off;

			Assert.IsFalse(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Off");
			Assert.IsFalse(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning false when the level is Off");
			Assert.IsFalse(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning false when the level is Off");
			Assert.IsFalse(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning false when the level is Off");
			Assert.IsFalse(logger.IsFatalErrorEnabled, "LevelFilteredLogger.IsFatalErrorEnabled is not returning false when the level is Off");
		}

		[Test]
		public void DefaultLevel()
		{
			Assert.AreEqual(LoggerLevel.Off, logger.Level, "Default LevelFilteredLogger.Level is not Off");
		}

		[Test]
		public void Level()
		{
			// Set the level to all available levels,
			// and then check that it was properly set
			foreach (LoggerLevel level in Enum.GetValues(typeof(LoggerLevel)))
			{
				logger.Level = level;
				Assert.AreEqual(level, logger.Level, "LevelFilteredLogger.Level did not change");
			}
		}

		[Test]
		public void DefaultName()
		{
			Assert.AreEqual("unnamed", logger.Name, "Default LevelFilteredLogger.Name is not String.Empty");
		}

		[Test]
		public void Name()
		{
			((LevelFilteredLoggerInstance) logger).ChangeName("Main");
			Assert.AreEqual("Main", logger.Name, "LevelFilteredLogger.Name did not change");

			((LevelFilteredLoggerInstance) logger).ChangeName("GUI");
			Assert.AreEqual("GUI", logger.Name, "LevelFilteredLogger.Name did not change");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SettingNameToNull()
		{
			((LevelFilteredLoggerInstance) logger).ChangeName(null);
		}
	}

	/// <summary>
	/// Contains all tests relating to the logging methods.
	/// </summary>
	[TestFixture]
	public class LoggingTests
	{
		private LevelFilteredLogger logger;

		internal LoggerLevel level;
		internal String name;
		internal String message;
		internal Exception exception;
		internal int calls;

		[SetUp]
		public void SetUp()
		{
			logger = new LevelFilteredLoggerInstance(this);

			// setting the default level to debug to simplify
			// the tests.

			logger.Level = LoggerLevel.Debug;

			// setting the level to an undefined value so
			// we dont have to wonder if it changed from Off
			// to Off (for instance).
			level = (LoggerLevel) (-1);
			name = null;
			message = null;
			exception = null;
			calls = 0;
		}

		#region Debug tests

		[Test]
		public void Debug()
		{
			string message = "Debug message";
			LoggerLevel level = LoggerLevel.Debug;
			Exception exception = null;

			logger.Debug(message);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void DebugWithException()
		{
			string message = "Debug message 2";
			LoggerLevel level = LoggerLevel.Debug;
			Exception exception = new Exception();

			logger.Debug(message, exception);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void DebugWithArgs()
		{
			string message = "Debug message 3";
			LoggerLevel level = LoggerLevel.Debug;
			Exception exception = null;

			logger.Debug("{0} {1} {2}", "Debug", "message", 3);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void DebugLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Debug("Test");

			ValidateCall(LoggerLevel.Debug, "Test", null);
		}

		[Test]
		public void DebugLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelError()
		{
			logger.Level = LoggerLevel.Error;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Debug("Test");

			ValidateNoCalls();
		}


		[Test]
		public void DebugLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			Exception exception = new Exception();

			logger.Debug("Test", exception);

			ValidateCall(LoggerLevel.Debug, "Test", exception);
		}

		[Test]
		public void DebugLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			Exception exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Warn;
			Exception exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Error;
			Exception exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			Exception exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			Exception exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}


		[Test]
		public void DebugLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Debug("{0}st", "Te");

			ValidateCall(LoggerLevel.Debug, "Test", null);
		}

		[Test]
		public void DebugLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.Debug("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Debug("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.Debug("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Debug("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.Debug("{0}st", "Te");

			ValidateNoCalls();
		}

		#endregion

		#region Info tests

		[Test]
		public void Info()
		{
			string message = "Info message";
			LoggerLevel level = LoggerLevel.Info;
			Exception exception = null;

			logger.Info(message);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void InfoWithException()
		{
			string message = "Info message 2";
			LoggerLevel level = LoggerLevel.Info;
			Exception exception = new Exception();

			logger.Info(message, exception);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void InfoWithArgs()
		{
			string message = "Info message 3";
			LoggerLevel level = LoggerLevel.Info;
			Exception exception = null;

			logger.Info("{0} {1} {2}", "Info", "message", 3);

			ValidateCall(level, message, exception);
		}


		[Test]
		public void InfoLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Info("Test");

			ValidateCall(LoggerLevel.Info, "Test", null);
		}

		[Test]
		public void InfoLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Info("Test");

			ValidateCall(LoggerLevel.Info, "Test", null);
		}

		[Test]
		public void InfoLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Info("Test");

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelError()
		{
			logger.Level = LoggerLevel.Error;

			logger.Info("Test");

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Info("Test");

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Info("Test");

			ValidateNoCalls();
		}


		[Test]
		public void InfoLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			Exception exception = new Exception();

			logger.Info("Test", exception);

			ValidateCall(LoggerLevel.Info, "Test", exception);
		}

		[Test]
		public void InfoLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			Exception exception = new Exception();

			logger.Info("Test", exception);

			ValidateCall(LoggerLevel.Info, "Test", exception);
		}

		[Test]
		public void InfoLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Warn;
			Exception exception = new Exception();

			logger.Info("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Error;
			Exception exception = new Exception();

			logger.Info("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			Exception exception = new Exception();

			logger.Info("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			Exception exception = new Exception();

			logger.Info("Test", exception);

			ValidateNoCalls();
		}


		[Test]
		public void InfoLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Info("{0}st", "Te");

			ValidateCall(LoggerLevel.Info, "Test", null);
		}

		[Test]
		public void InfoLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.Info("{0}st", "Te");

			ValidateCall(LoggerLevel.Info, "Test", null);
		}

		[Test]
		public void InfoLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Info("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.Info("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Info("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.Info("{0}st", "Te");

			ValidateNoCalls();
		}

		#endregion

		#region Warn tests

		[Test]
		public void Warn()
		{
			string message = "Warn message";
			LoggerLevel level = LoggerLevel.Warn;
			Exception exception = null;

			logger.Warn(message);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void WarnWithException()
		{
			string message = "Warn message 2";
			LoggerLevel level = LoggerLevel.Warn;
			Exception exception = new Exception();

			logger.Warn(message, exception);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void WarnWithArgs()
		{
			string message = "Warn message 3";
			LoggerLevel level = LoggerLevel.Warn;
			Exception exception = null;

			logger.Warn("{0} {1} {2}", "Warn", "message", 3);

			ValidateCall(level, message, exception);
		}


		[Test]
		public void WarnLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Warn("Test");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Test]
		public void WarnLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Warn("Test");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Test]
		public void WarnLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Warn("Test");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Test]
		public void WarnLevelError()
		{
			logger.Level = LoggerLevel.Error;

			logger.Warn("Test");

			ValidateNoCalls();
		}

		[Test]
		public void WarnLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Warn("Test");

			ValidateNoCalls();
		}

		[Test]
		public void WarnLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Warn("Test");

			ValidateNoCalls();
		}


		[Test]
		public void WarnLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			Exception exception = new Exception();

			logger.Warn("Test", exception);

			ValidateCall(LoggerLevel.Warn, "Test", exception);
		}

		[Test]
		public void WarnLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			Exception exception = new Exception();

			logger.Warn("Test", exception);

			ValidateCall(LoggerLevel.Warn, "Test", exception);
		}

		[Test]
		public void WarnLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Warn;
			Exception exception = new Exception();

			logger.Warn("Test", exception);

			ValidateCall(LoggerLevel.Warn, "Test", exception);
		}

		[Test]
		public void WarnLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Error;
			Exception exception = new Exception();

			logger.Warn("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void WarnLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			Exception exception = new Exception();

			logger.Warn("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void WarnLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			Exception exception = new Exception();

			logger.Warn("Test", exception);

			ValidateNoCalls();
		}


		[Test]
		public void WarnLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Warn("{0}st", "Te");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Test]
		public void WarnLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.Warn("{0}st", "Te");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Test]
		public void WarnLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Warn("{0}st", "Te");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Test]
		public void WarnLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.Warn("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void WarnLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Warn("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void WarnLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.Warn("{0}st", "Te");

			ValidateNoCalls();
		}

		#endregion

		#region Error tests

		[Test]
		public void Error()
		{
			string message = "Error message";
			LoggerLevel level = LoggerLevel.Error;
			Exception exception = null;

			logger.Error(message);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void ErrorWithException()
		{
			string message = "Error message 2";
			LoggerLevel level = LoggerLevel.Error;
			Exception exception = new Exception();

			logger.Error(message, exception);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void ErrorWithArgs()
		{
			string message = "Error message 3";
			LoggerLevel level = LoggerLevel.Error;
			Exception exception = null;

			logger.Error("{0} {1} {2}", "Error", "message", 3);

			ValidateCall(level, message, exception);
		}


		[Test]
		public void ErrorLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Error("Test");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Test]
		public void ErrorLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Error("Test");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Test]
		public void ErrorLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Error("Test");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Test]
		public void ErrorLevelError()
		{
			logger.Level = LoggerLevel.Error;

			logger.Error("Test");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Test]
		public void ErrorLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Error("Test");

			ValidateNoCalls();
		}

		[Test]
		public void ErrorLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Error("Test");

			ValidateNoCalls();
		}


		[Test]
		public void ErrorLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			Exception exception = new Exception();

			logger.Error("Test", exception);

			ValidateCall(LoggerLevel.Error, "Test", exception);
		}

		[Test]
		public void ErrorLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			Exception exception = new Exception();

			logger.Error("Test", exception);

			ValidateCall(LoggerLevel.Error, "Test", exception);
		}

		[Test]
		public void ErrorLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Error;
			Exception exception = new Exception();

			logger.Error("Test", exception);

			ValidateCall(LoggerLevel.Error, "Test", exception);
		}

		[Test]
		public void ErrorLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Error;
			Exception exception = new Exception();

			logger.Error("Test", exception);

			ValidateCall(LoggerLevel.Error, "Test", exception);
		}

		[Test]
		public void ErrorLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			Exception exception = new Exception();

			logger.Error("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void ErrorLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			Exception exception = new Exception();

			logger.Error("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void ErrorLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Error("{0}st", "Te");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Test]
		public void ErrorLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.Error("{0}st", "Te");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Test]
		public void ErrorLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.Error("{0}st", "Te");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Test]
		public void ErrorLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.Error("{0}st", "Te");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Test]
		public void ErrorLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Error("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void ErrorLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.Error("{0}st", "Te");

			ValidateNoCalls();
		}

		#endregion

		#region FatalError tests

		[Test]
		public void FatalError()
		{
			string message = "FatalError message";
			LoggerLevel level = LoggerLevel.Fatal;
			Exception exception = null;

			logger.FatalError(message);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void FatalErrorWithException()
		{
			string message = "FatalError message 2";
			LoggerLevel level = LoggerLevel.Fatal;
			Exception exception = new Exception();

			logger.FatalError(message, exception);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void FatalErrorWithArgs()
		{
			string message = "FatalError message 3";
			LoggerLevel level = LoggerLevel.Fatal;
			Exception exception = null;

			logger.FatalError("{0} {1} {2}", "FatalError", "message", 3);

			ValidateCall(level, message, exception);
		}


		[Test]
		public void FatalErrorLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.FatalError("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Test]
		public void FatalErrorLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.FatalError("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Test]
		public void FatalErrorLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.FatalError("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Test]
		public void FatalErrorLevelError()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.FatalError("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Test]
		public void FatalErrorLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.FatalError("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Test]
		public void FatalErrorLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.FatalError("Test");

			ValidateNoCalls();
		}


		[Test]
		public void FatalErrorLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			Exception exception = new Exception();

			logger.FatalError("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Test]
		public void FatalErrorLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			Exception exception = new Exception();

			logger.FatalError("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Test]
		public void FatalErrorLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			Exception exception = new Exception();

			logger.FatalError("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Test]
		public void FatalErrorLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			Exception exception = new Exception();

			logger.FatalError("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Test]
		public void FatalErrorLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			Exception exception = new Exception();

			logger.FatalError("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Test]
		public void FatalErrorLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			Exception exception = new Exception();

			logger.FatalError("Test", exception);

			ValidateNoCalls();
		}


		[Test]
		public void FatalErrorLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.FatalError("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Test]
		public void FatalErrorLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.FatalError("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Test]
		public void FatalErrorLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.FatalError("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Test]
		public void FatalErrorLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.FatalError("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Test]
		public void FatalErrorLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.FatalError("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Test]
		public void FatalErrorLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.FatalError("{0}st", "Te");

			ValidateNoCalls();
		}

		#endregion

		private void ValidateCall(LoggerLevel expectedLevel, String expectedMessage, Exception expectedException)
		{
			Assert.AreEqual(1, calls, "LevelFilteredLogger.Log was not called the right number of times");
			Assert.AreEqual(expectedLevel, level, "LevelFilteredLogger.Log was not called with the right level");
			Assert.AreEqual(expectedMessage, message, "LevelFilteredLogger.Log was not called with the right message");
			Assert.AreSame(expectedException, exception, "LevelFilteredLogger.Log was not called with the right exception");
			Assert.AreEqual("unnamed", name, "LevelFilteredLogger.Log was not called with the right name");
		}

		private void ValidateNoCalls()
		{
			Assert.AreEqual(0, calls, "LevelFilteredLogger.Log was called with logging " + logger.Level);
		}

		private void ValidateCalled()
		{
			Assert.AreEqual(1, calls, "LevelFilteredLogger.Log was not called with logging " + logger.Level);
		}

	}

	internal class LevelFilteredLoggerInstance : LevelFilteredLogger
	{
		private readonly LoggingTests Fixture;

		public LevelFilteredLoggerInstance(LoggingTests fixture)
		{
			Fixture = fixture;
		}


		public new void ChangeName(String name)
		{
			base.ChangeName(name);
		}

		protected override void Log(LoggerLevel level, string name, string message, Exception exception)
		{
			Fixture.level = level;
			Fixture.name = name;
			Fixture.message = message;
			Fixture.exception = exception;

			Fixture.calls++;
		}

		public override ILogger CreateChildLogger(string name)
		{
			return null;
		}
	}
}