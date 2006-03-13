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

namespace Castle.Services.Logging
{
	using System;
	using System.IO;


    public abstract class AbstractLoggerFactory : ILoggerFactory
	{
    	public AbstractLoggerFactory()
		{
		}

		public virtual ILogger Create(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");

			return this.Create(type.FullName);
		}

		public virtual ILogger Create(Type type, LoggerLevel level)
		{
			if (type == null) throw new ArgumentNullException("type");

			return this.Create(type.FullName, level);
		}

		public abstract ILogger Create(String name);

		public abstract ILogger Create(String name, LoggerLevel level);

        /// <summary>
        /// Gets the configuration file.
        /// </summary>
        /// <param name="filename">i.e. log4net.config</param>
        /// <returns></returns>
        protected FileInfo GetConfigFile(string filename)
        {
			FileInfo result;

			if (Path.IsPathRooted(filename))
			{
				result = new FileInfo(filename);
			}
			else
			{
				result = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename));
			}
            
			return result;
        }
	}
}
