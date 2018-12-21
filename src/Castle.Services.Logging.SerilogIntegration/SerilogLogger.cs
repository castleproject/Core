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

namespace Castle.Services.Logging.SerilogIntegration
{
    using System;

    using Serilog;
    using Serilog.Events;

#if FEATURE_SERIALIZATION
    [Serializable]
#endif
    public class SerilogLogger :
#if FEATURE_APPDOMAIN
        MarshalByRefObject,
#endif
        Castle.Core.Logging.ILogger
    {
        public SerilogLogger(ILogger logger, SerilogFactory factory)
        {
            Logger = logger;
            Factory = factory;
        }

        internal SerilogLogger() { }

        protected internal ILogger Logger { get; set; }

        protected internal SerilogFactory Factory { get; set; }

        public bool IsTraceEnabled
        {
            get { return Logger.IsEnabled(LogEventLevel.Verbose); }
        }

        public bool IsDebugEnabled
        {
            get { return Logger.IsEnabled(LogEventLevel.Debug); }
        }

        public bool IsErrorEnabled
        {
            get { return Logger.IsEnabled(LogEventLevel.Error); }
        }

        public bool IsFatalEnabled
        {
            get { return Logger.IsEnabled(LogEventLevel.Fatal); }
        }

        public bool IsInfoEnabled
        {
            get { return Logger.IsEnabled(LogEventLevel.Information); }
        }

        public bool IsWarnEnabled
        {
            get { return Logger.IsEnabled(LogEventLevel.Warning); }
        }

        public override string ToString()
        {
            return Logger.ToString();
        }

        public Castle.Core.Logging.ILogger CreateChildLogger(string loggerName)
        {
            // Serilog calls these sub loggers. We might be able to do something here but for now I'm going leave it like this.
            throw new NotImplementedException("Creating child loggers for Serilog is not supported");
        }

        public void Trace(string message, Exception exception)
        {
            Logger.Verbose(exception, message);
        }

        public void Trace(Func<string> messageFactory)
        {
            if (IsTraceEnabled)
            {
                Logger.Verbose(messageFactory.Invoke());
            }
        }

        public void Trace(string message)
        {
            if (IsTraceEnabled)
            {
                Logger.Verbose(message);
            }
        }

        public void TraceFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsTraceEnabled)
            {
                //TODO: This honours the formatProvider rather than passing through args for structured logging
                Logger.Verbose(exception, string.Format(formatProvider, format, args));
            }
        }

        public void TraceFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsTraceEnabled)
            {
                //TODO: This honours the formatProvider rather than passing through args for structured logging
                Logger.Verbose(string.Format(formatProvider, format, args));
            }
        }

        public void TraceFormat(Exception exception, string format, params object[] args)
        {
            if (IsTraceEnabled)
            {
                Logger.Verbose(exception, format, args);
            }
        }

        public void TraceFormat(string format, params object[] args)
        {
            if (IsTraceEnabled)
            {
                Logger.Verbose(format, args);
            }
        }

        public void Debug(string message, Exception exception)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(exception, message);
            }
        }

        public void Debug(Func<string> messageFactory)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(messageFactory.Invoke());
            }
        }

        public void Debug(string message)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(message);
            }
        }

        public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsDebugEnabled)
            {
                //TODO: This honours the formatProvider rather than passing through args for structured logging
                Logger.Debug(exception, string.Format(formatProvider, format, args));
            }
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsDebugEnabled)
            {
                //TODO: This honours the formatProvider rather than passing through args for structured logging
                Logger.Debug(string.Format(formatProvider, format, args));
            }
        }

        public void DebugFormat(Exception exception, string format, params object[] args)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(exception, format, args);
            }
        }

        public void DebugFormat(string format, params object[] args)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(format, args);
            }
        }

        public void Error(string message, Exception exception)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(exception, message);
            }
        }

        public void Error(Func<string> messageFactory)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(messageFactory.Invoke());
            }
        }

        public void Error(string message)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(message);
            }
        }

        public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsErrorEnabled)
            {
                //TODO: This honours the formatProvider rather than passing through args for structured logging
                Logger.Error(exception, string.Format(formatProvider, format, args));
            }
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsErrorEnabled)
            {
                //TODO: This honours the formatProvider rather than passing through args for structured logging
                Logger.Error(string.Format(formatProvider, format, args));
            }
        }

        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(exception, format, args);
            }
        }

        public void ErrorFormat(string format, params object[] args)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(format, args);
            }
        }

        public void Fatal(string message, Exception exception)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(exception, message);
            }
        }

        public void Fatal(Func<string> messageFactory)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(messageFactory.Invoke());
            }
        }

        public void Fatal(string message)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(message);
            }
        }

        public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsFatalEnabled)
            {
                //TODO: This honours the formatProvider rather than passing through args for structured logging
                Logger.Fatal(exception, string.Format(formatProvider, format, args));
            }
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsFatalEnabled)
            {
                //TODO: This honours the formatProvider rather than passing through args for structured logging
                Logger.Fatal(string.Format(formatProvider, format, args));
            }
        }

        public void FatalFormat(Exception exception, string format, params object[] args)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(exception, format, args);
            }
        }

        public void FatalFormat(string format, params object[] args)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(format, args);
            }
        }

        public void Info(string message, Exception exception)
        {
            if (IsInfoEnabled)
            {
                Logger.Information(exception, message);
            }
        }

        public void Info(Func<string> messageFactory)
        {
            if (IsInfoEnabled)
            {
                Logger.Information(messageFactory.Invoke());
            }
        }

        public void Info(string message)
        {
            if (IsInfoEnabled)
            {
                Logger.Information(message);
            }
        }

        public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsInfoEnabled)
            {
                //TODO: This honours the formatProvider rather than passing through args for structured logging
                Logger.Information(exception, string.Format(formatProvider, format, args));
            }
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsInfoEnabled)
            {
                //TODO: This honours the formatProvider rather than passing through args for structured logging
                Logger.Information(string.Format(formatProvider, format, args));
            }
        }

        public void InfoFormat(Exception exception, string format, params object[] args)
        {
            if (IsInfoEnabled)
            {
                Logger.Information(exception, format, args);
            }
        }

        public void InfoFormat(string format, params object[] args)
        {
            if (IsInfoEnabled)
            {
                Logger.Information(format, args);
            }
        }

        public void Warn(string message, Exception exception)
        {
            if (IsWarnEnabled)
            {
                Logger.Warning(exception, message);
            }
        }

        public void Warn(Func<string> messageFactory)
        {
            if (IsWarnEnabled)
            {
                Logger.Warning(messageFactory.Invoke());
            }
        }

        public void Warn(string message)
        {
            if (IsWarnEnabled)
            {
                Logger.Warning(message);
            }
        }

        public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsWarnEnabled)
            {
                //TODO: This honours the formatProvider rather than passing through args for structured logging
                Logger.Warning(exception, string.Format(formatProvider, format, args));
            }
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsWarnEnabled)
            {
                //TODO: This honours the formatProvider rather than passing through args for structured logging
                Logger.Warning(string.Format(formatProvider, format, args));
            }
        }

        public void WarnFormat(Exception exception, string format, params object[] args)
        {
            if (IsWarnEnabled)
            {
                Logger.Warning(exception, format, args);
            }
        }

        public void WarnFormat(string format, params object[] args)
        {
            if (IsWarnEnabled)
            {
                Logger.Warning(format, args);
            }
        }
    }
}