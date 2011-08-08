// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

#if SILVERLIGHT
	public class ConsoleFactory : ILoggerFactory
#else
	[Serializable]
	public class ConsoleFactory : MarshalByRefObject, ILoggerFactory
#endif
	{
		private LoggerLevel? level;

		public ConsoleFactory()
		{
		}

		public ConsoleFactory(LoggerLevel level)
		{
			this.level = level;
		}

		public ILogger Create(Type type)
		{
			return Create(type.FullName);
		}

		public ILogger Create(String name)
		{
			if (level.HasValue)
			{
				return Create(name, level.Value);
			}
			return new ConsoleLogger(name);
		}

		public ILogger Create(Type type, LoggerLevel level)
		{
			return new ConsoleLogger(type.Name, level);
		}

		public ILogger Create(String name, LoggerLevel level)
		{
			return new ConsoleLogger(name, level);
		}
	}
}