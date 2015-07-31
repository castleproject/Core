// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Logging
{
	using System;
	using System.IO;

#if FEATURE_SERIALIZATION
	[Serializable]
#endif
#if SILVERLIGHT || NETCORE
	public abstract class AbstractLoggerFactory : ILoggerFactory
#else
	public abstract class AbstractLoggerFactory : MarshalByRefObject, ILoggerFactory
#endif
	{
		public virtual ILogger Create(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			return Create(type.FullName);
		}

		public virtual ILogger Create(Type type, LoggerLevel level)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			return Create(type.FullName, level);
		}

		public abstract ILogger Create(String name);

		public abstract ILogger Create(String name, LoggerLevel level);

		/// <summary>
		///   Gets the configuration file.
		/// </summary>
		/// <param name = "fileName">i.e. log4net.config</param>
		/// <returns></returns>
		protected static FileInfo GetConfigFile(string fileName)
		{
#if !SILVERLIGHT
			FileInfo result;

			if (Path.IsPathRooted(fileName))
			{
				result = new FileInfo(fileName);
			}
			else
			{
#if NETCORE
				result = new FileInfo(Path.Combine(AppContext.BaseDirectory, fileName));
#else
				result = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName));
#endif
			}

			return result;
#else
			return new FileInfo(fileName);
#endif
		}
	}
}