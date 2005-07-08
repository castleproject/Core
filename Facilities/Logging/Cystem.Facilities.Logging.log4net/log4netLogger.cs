// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Cystem.Facilities.Logging.log4net
{
    using System;
    using Castle.Services.Logging;
    using log4net;

	/// <summary>
	/// Summary description for log4netLogger.
	/// </summary>
	public class log4netLogger : ILogger
	{
        private ILog _log;

		internal log4netLogger(ILog log)
		{
			this._log = log;
        }

        #region ILogger Members

        public ILogger CreateChildLogger(string name) {
            // TODO:  Add log4netLogger.CreateChildLogger implementation
            return null;
        }

        public void Info(string format, params object[] args) {
            // TODO:  Add log4netLogger.Info implementation
        }

        void Castle.Services.Logging.ILogger.Info(string message, Exception exception) {
            this._log.Info(message, exception);
        }

        void Castle.Services.Logging.ILogger.Info(string message) {
            this._log.Info(message);
        }

        public void Debug(string format, params object[] args) {
            // TODO:  Add log4netLogger.Debug implementation
        }

        void Castle.Services.Logging.ILogger.Debug(string message, Exception exception) {
            this._log.Debug(message, exception);
        }

        void Castle.Services.Logging.ILogger.Debug(string message) {
            this._log.Debug(message);
        }

        public bool IsErrorEnabled {
            get {
                return this._log.IsErrorEnabled;
            }
        }

        public bool IsWarnEnabled {
            get {
                return this._log.IsWarnEnabled;
            }
        }

        public void Warn(string format, params object[] args) {
            this._log.WarnFormat(format, args);
        }

        void Castle.Services.Logging.ILogger.Warn(string message, Exception exception) {
            this._log.Warn(message, exception);
        }

        void Castle.Services.Logging.ILogger.Warn(string message) {
            this._log.Warn(message);
        }

        public bool IsFatalErrorEnabled {
            get {
                return this._log.IsFatalEnabled;
            }
        }

        public bool IsInfoEnabled {
            get {
                // TODO:  Add log4netLogger.IsInfoEnabled getter implementation
                return false;
            }
        }

        public void FatalError(string format, params object[] args) {
            // TODO:  Add log4netLogger.FatalError implementation
        }

        void Castle.Services.Logging.ILogger.FatalError(string message, Exception exception) {
            // TODO:  Add log4netLogger.Castle.Services.Logging.ILogger.FatalError implementation
        }

        void Castle.Services.Logging.ILogger.FatalError(string message) {
            // TODO:  Add log4netLogger.Castle.Services.Logging.ILogger.FatalError implementation
        }

        public bool IsDebugEnabled {
            get {
                // TODO:  Add log4netLogger.IsDebugEnabled getter implementation
                return false;
            }
        }

        public void Error(string format, params object[] args) {
            // TODO:  Add log4netLogger.Error implementation
        }

        void Castle.Services.Logging.ILogger.Error(string message, Exception exception) {
            // TODO:  Add log4netLogger.Castle.Services.Logging.ILogger.Error implementation
        }

        void Castle.Services.Logging.ILogger.Error(string message) {
            // TODO:  Add log4netLogger.Castle.Services.Logging.ILogger.Error implementation
        }

        #endregion
    }
}
