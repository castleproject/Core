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

namespace Castle.Core.Logging.Tests
{
	using System;
	using System.ComponentModel;

	using Castle.Core.Logging;

	using Xunit;

	/// <summary>
	/// Contains all tests relating to the properties.
	/// </summary>
	public class PropertyTests
	{
		private LevelFilteredLogger logger;

		public PropertyTests()
		{
			logger = new LevelFilteredLoggerInstance(null);
		}

		[Fact]
		public void LevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			Assert.True(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning true when the level is Debug");
			Assert.True(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning true when the level is Debug");
			Assert.True(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning true when the level is Debug");
			Assert.True(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning true when the level is Debug");
			Assert.True(logger.IsFatalEnabled,
				"LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Debug");
		}

		[Fact]
		public void LevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			Assert.False(logger.IsDebugEnabled,
				"LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Info");
			Assert.True(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning true when the level is Info");
			Assert.True(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning true when the level is Info");
			Assert.True(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning true when the level is Info");
			Assert.True(logger.IsFatalEnabled,
				"LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Info");
		}

		[Fact]
		public void LevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			Assert.False(logger.IsDebugEnabled,
				"LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Warn");
			Assert.False(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning false when the level is Warn");
			Assert.True(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning true when the level is Warn");
			Assert.True(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning true when the level is Warn");
			Assert.True(logger.IsFatalEnabled,
				"LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Warn");
		}

		[Fact]
		public void LevelError()
		{
			logger.Level = LoggerLevel.Error;

			Assert.False(logger.IsDebugEnabled,
				"LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Error");
			Assert.False(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning false when the level is Error");
			Assert.False(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning false when the level is Error");
			Assert.True(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning true when the level is Error");
			Assert.True(logger.IsFatalEnabled,
				"LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Error");
		}

		[Fact]
		public void LevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			Assert.False(logger.IsDebugEnabled,
				"LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Fatal");
			Assert.False(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning false when the level is Fatal");
			Assert.False(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning false when the level is Fatal");
			Assert.False(logger.IsErrorEnabled,
				"LevelFilteredLogger.IsErrorEnabled is not returning false when the level is Fatal");
			Assert.True(logger.IsFatalEnabled,
				"LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Fatal");
		}

		[Fact]
		public void LevelOff()
		{
			logger.Level = LoggerLevel.Off;

			Assert.False(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Off");
			Assert.False(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning false when the level is Off");
			Assert.False(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning false when the level is Off");
			Assert.False(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning false when the level is Off");
			Assert.False(logger.IsFatalEnabled,
				"LevelFilteredLogger.IsFatalErrorEnabled is not returning false when the level is Off");
		}

		[Fact]
		public void DefaultLevel()
		{
			Assert.Equal(LoggerLevel.Off, logger.Level); //, "Default LevelFilteredLogger.Level is not Off");
		}

#if !SILVERLIGHT && !NETCORE
		[Fact]
		public void Level()
		{
			// Set the level to all available levels,
			// and then check that it was properly set
			foreach (LoggerLevel level in Enum.GetValues(typeof(LoggerLevel)))
			{
				logger.Level = level;
				Assert.Equal(level, logger.Level); //, "LevelFilteredLogger.Level did not change");
			}
		}
#endif

		[Fact]
		public void DefaultName()
		{
			Assert.Equal("unnamed", logger.Name); //, "Default LevelFilteredLogger.Name is not String.Empty");
		}

		[Fact]
		public void Name()
		{
			((LevelFilteredLoggerInstance)logger).ChangeName("Main");
			Assert.Equal("Main", logger.Name); //, "LevelFilteredLogger.Name did not change");

			((LevelFilteredLoggerInstance)logger).ChangeName("GUI");
			Assert.Equal("GUI", logger.Name); //, "LevelFilteredLogger.Name did not change");
		}

		//[Fact]
		//[ExpectedException(typeof(ArgumentNullException))]
		public void SettingNameToNull()
		{
			((LevelFilteredLoggerInstance)logger).ChangeName(null);
		}
	}

	/// <summary>
	/// Contains all tests relating to the logging methods.
	/// </summary>
	public class LoggingTests
	{
		private LevelFilteredLogger logger;

		internal LoggerLevel level;
		internal String name;
		internal String message;
		internal Exception exception;
		internal int calls;

		public LoggingTests()
		{
			logger = new LevelFilteredLoggerInstance(this);

			// setting the default level to debug to simplify
			// the tests.

			logger.Level = LoggerLevel.Debug;

			// setting the level to an undefined value so
			// we dont have to wonder if it changed from Off
			// to Off (for instance).
			level = (LoggerLevel)(-1);
			name = null;
			message = null;
			exception = null;
			calls = 0;
		}

		#region Debug tests

		[Fact]
		public void Debug()
		{
			string message = "Debug message";
			LoggerLevel level = LoggerLevel.Debug;
			Exception exception = null;

			logger.Debug(message);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void DebugWithException()
		{
			string message = "Debug message 2";
			LoggerLevel level = LoggerLevel.Debug;
			Exception exception = new Exception();

			logger.Debug(message, exception);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void DebugWithArgs()
		{
			string message = "Debug message 3";
			LoggerLevel level = LoggerLevel.Debug;
			Exception exception = null;

			logger.DebugFormat("{0} {1} {2}", "Debug", "message", 3);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void DebugLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Debug("Test");

			ValidateCall(LoggerLevel.Debug, "Test", null);
		}

		[Fact]
		public void DebugLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelError()
		{
			logger.Level = LoggerLevel.Error;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			Exception exception = new Exception();

			logger.Debug("Test", exception);

			ValidateCall(LoggerLevel.Debug, "Test", exception);
		}

		[Fact]
		public void DebugLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			Exception exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Warn;
			Exception exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Error;
			Exception exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			Exception exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			Exception exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.DebugFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Debug, "Test", null);
		}

		[Fact]
		public void DebugLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.DebugFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Warn;

			logger.DebugFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.DebugFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.DebugFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.DebugFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		#endregion

		#region Info tests

		[Fact]
		public void Info()
		{
			string message = "Info message";
			LoggerLevel level = LoggerLevel.Info;
			Exception exception = null;

			logger.Info(message);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void InfoWithException()
		{
			string message = "Info message 2";
			LoggerLevel level = LoggerLevel.Info;
			Exception exception = new Exception();

			logger.Info(message, exception);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void InfoWithArgs()
		{
			string message = "Info message 3";
			LoggerLevel level = LoggerLevel.Info;
			Exception exception = null;

			logger.InfoFormat("{0} {1} {2}", "Info", "message", 3);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void InfoLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Info("Test");

			ValidateCall(LoggerLevel.Info, "Test", null);
		}

		[Fact]
		public void InfoLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Info("Test");

			ValidateCall(LoggerLevel.Info, "Test", null);
		}

		[Fact]
		public void InfoLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Info("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelError()
		{
			logger.Level = LoggerLevel.Error;

			logger.Info("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Info("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Info("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			Exception exception = new Exception();

			logger.Info("Test", exception);

			ValidateCall(LoggerLevel.Info, "Test", exception);
		}

		[Fact]
		public void InfoLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			Exception exception = new Exception();

			logger.Info("Test", exception);

			ValidateCall(LoggerLevel.Info, "Test", exception);
		}

		[Fact]
		public void InfoLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Warn;
			Exception exception = new Exception();

			logger.Info("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Error;
			Exception exception = new Exception();

			logger.Info("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			Exception exception = new Exception();

			logger.Info("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			Exception exception = new Exception();

			logger.Info("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.InfoFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Info, "Test", null);
		}

		[Fact]
		public void InfoLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.InfoFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Info, "Test", null);
		}

		[Fact]
		public void InfoLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Warn;

			logger.InfoFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.InfoFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.InfoFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.InfoFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		#endregion

		#region Warn tests

		[Fact]
		public void Warn()
		{
			string message = "Warn message";
			LoggerLevel level = LoggerLevel.Warn;
			Exception exception = null;

			logger.Warn(message);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void WarnWithException()
		{
			string message = "Warn message 2";
			LoggerLevel level = LoggerLevel.Warn;
			Exception exception = new Exception();

			logger.Warn(message, exception);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void WarnWithArgs()
		{
			string message = "Warn message 3";
			LoggerLevel level = LoggerLevel.Warn;
			Exception exception = null;

			logger.WarnFormat("{0} {1} {2}", "Warn", "message", 3);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void WarnLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Warn("Test");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Fact]
		public void WarnLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Warn("Test");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Fact]
		public void WarnLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Warn("Test");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Fact]
		public void WarnLevelError()
		{
			logger.Level = LoggerLevel.Error;

			logger.Warn("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void WarnLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Warn("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void WarnLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Warn("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void WarnLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			Exception exception = new Exception();

			logger.Warn("Test", exception);

			ValidateCall(LoggerLevel.Warn, "Test", exception);
		}

		[Fact]
		public void WarnLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			Exception exception = new Exception();

			logger.Warn("Test", exception);

			ValidateCall(LoggerLevel.Warn, "Test", exception);
		}

		[Fact]
		public void WarnLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Warn;
			Exception exception = new Exception();

			logger.Warn("Test", exception);

			ValidateCall(LoggerLevel.Warn, "Test", exception);
		}

		[Fact]
		public void WarnLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Error;
			Exception exception = new Exception();

			logger.Warn("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void WarnLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			Exception exception = new Exception();

			logger.Warn("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void WarnLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			Exception exception = new Exception();

			logger.Warn("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void WarnLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.WarnFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Fact]
		public void WarnLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.WarnFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Fact]
		public void WarnLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Warn;

			logger.WarnFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Fact]
		public void WarnLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.WarnFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void WarnLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.WarnFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void WarnLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.WarnFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		#endregion

		#region Error tests

		[Fact]
		public void Error()
		{
			string message = "Error message";
			LoggerLevel level = LoggerLevel.Error;
			Exception exception = null;

			logger.Error(message);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void ErrorWithException()
		{
			string message = "Error message 2";
			LoggerLevel level = LoggerLevel.Error;
			Exception exception = new Exception();

			logger.Error(message, exception);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void ErrorWithArgs()
		{
			string message = "Error message 3";
			LoggerLevel level = LoggerLevel.Error;
			Exception exception = null;

			logger.ErrorFormat("{0} {1} {2}", "Error", "message", 3);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void ErrorLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Error("Test");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Fact]
		public void ErrorLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Error("Test");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Fact]
		public void ErrorLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Error("Test");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Fact]
		public void ErrorLevelError()
		{
			logger.Level = LoggerLevel.Error;

			logger.Error("Test");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Fact]
		public void ErrorLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Error("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void ErrorLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Error("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void ErrorLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			Exception exception = new Exception();

			logger.Error("Test", exception);

			ValidateCall(LoggerLevel.Error, "Test", exception);
		}

		[Fact]
		public void ErrorLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			Exception exception = new Exception();

			logger.Error("Test", exception);

			ValidateCall(LoggerLevel.Error, "Test", exception);
		}

		[Fact]
		public void ErrorLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Error;
			Exception exception = new Exception();

			logger.Error("Test", exception);

			ValidateCall(LoggerLevel.Error, "Test", exception);
		}

		[Fact]
		public void ErrorLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Error;
			Exception exception = new Exception();

			logger.Error("Test", exception);

			ValidateCall(LoggerLevel.Error, "Test", exception);
		}

		[Fact]
		public void ErrorLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			Exception exception = new Exception();

			logger.Error("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void ErrorLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			Exception exception = new Exception();

			logger.Error("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void ErrorLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.ErrorFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Fact]
		public void ErrorLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.ErrorFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Fact]
		public void ErrorLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.ErrorFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Fact]
		public void ErrorLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.ErrorFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Fact]
		public void ErrorLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.ErrorFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void ErrorLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.ErrorFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		#endregion

		#region FatalError tests

		[Fact]
		public void FatalError()
		{
			string message = "FatalError message";
			LoggerLevel level = LoggerLevel.Fatal;
			Exception exception = null;

			logger.Fatal(message);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void FatalErrorWithException()
		{
			string message = "FatalError message 2";
			LoggerLevel level = LoggerLevel.Fatal;
			Exception exception = new Exception();

			logger.Fatal(message, exception);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void FatalErrorWithArgs()
		{
			string message = "FatalError message 3";
			LoggerLevel level = LoggerLevel.Fatal;
			Exception exception = null;

			logger.FatalFormat("{0} {1} {2}", "FatalError", "message", 3);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void FatalErrorLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Fatal("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Fact]
		public void FatalErrorLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Fatal("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Fact]
		public void FatalErrorLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Fatal("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Fact]
		public void FatalErrorLevelError()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Fatal("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Fact]
		public void FatalErrorLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Fatal("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Fact]
		public void FatalErrorLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Fatal("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void FatalErrorLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			Exception exception = new Exception();

			logger.Fatal("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Fact]
		public void FatalErrorLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			Exception exception = new Exception();

			logger.Fatal("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Fact]
		public void FatalErrorLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			Exception exception = new Exception();

			logger.Fatal("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Fact]
		public void FatalErrorLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			Exception exception = new Exception();

			logger.Fatal("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Fact]
		public void FatalErrorLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			Exception exception = new Exception();

			logger.Fatal("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Fact]
		public void FatalErrorLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			Exception exception = new Exception();

			logger.Fatal("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void FatalErrorLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.FatalFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Fact]
		public void FatalErrorLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.FatalFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Fact]
		public void FatalErrorLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.FatalFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Fact]
		public void FatalErrorLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.FatalFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Fact]
		public void FatalErrorLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.FatalFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Fact]
		public void FatalErrorLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.FatalFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		#endregion

		private void ValidateCall(LoggerLevel expectedLevel, String expectedMessage, Exception expectedException)
		{
			Assert.Equal(1, calls); //, "LevelFilteredLogger.Log was not called the right number of times");
			Assert.Equal(expectedLevel, level); //, "LevelFilteredLogger.Log was not called with the right level");
			Assert.Equal(expectedMessage, message); //, "LevelFilteredLogger.Log was not called with the right message");
			Assert.Same(expectedException, exception); //, "LevelFilteredLogger.Log was not called with the right exception");
			Assert.Equal("unnamed", name); //, "LevelFilteredLogger.Log was not called with the right name");
		}

		private void ValidateNoCalls()
		{
			Assert.Equal(0, calls); //, "LevelFilteredLogger.Log was called with logging " + logger.Level);
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

		protected override void Log(LoggerLevel loggerLevel, string loggerName, string message, Exception exception)
		{
			Fixture.level = loggerLevel;
			Fixture.name = loggerName;
			Fixture.message = message;
			Fixture.exception = exception;

			Fixture.calls++;
		}

		public override ILogger CreateChildLogger(string loggerName)
		{
			return null;
		}
	}
}