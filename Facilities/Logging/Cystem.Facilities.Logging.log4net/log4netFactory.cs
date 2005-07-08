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

	/// <summary>
	/// Summary description for log4netFactory.
	/// </summary>
	public class log4netFactory : ILoggerFactory
	{
		public log4netFactory()
		{
			//
			// TODO: Add constructor logic here
			//
        }

        #region ILoggerFactory Members

        public ILogger Create(string name, Castle.Services.Logging.LoggerLevel level) {
            // TODO:  Add log4netFactory.Create implementation
            return null;
        }

        ILogger Castle.Services.Logging.ILoggerFactory.Create(Type type, Castle.Services.Logging.LoggerLevel level) {
            // TODO:  Add log4netFactory.Castle.Services.Logging.ILoggerFactory.Create implementation
            return null;
        }

        ILogger Castle.Services.Logging.ILoggerFactory.Create(string name) {
            // TODO:  Add log4netFactory.Castle.Services.Logging.ILoggerFactory.Create implementation
            return null;
        }

        ILogger Castle.Services.Logging.ILoggerFactory.Create(Type type) {
            // TODO:  Add log4netFactory.Castle.Services.Logging.ILoggerFactory.Create implementation
            return null;
        }

        #endregion
    }
}
