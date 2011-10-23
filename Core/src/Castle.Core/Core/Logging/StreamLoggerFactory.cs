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
#if !SILVERLIGHT
	using System;
	using System.IO;
	using System.Text;

	/// <summary>
	///   Creates <see cref = "StreamLogger" /> outputing 
	///   to files. The name of the file is derived from the log name
	///   plus the 'log' extension.
	/// </summary>
	[Serializable]
	public class StreamLoggerFactory : AbstractLoggerFactory
	{
		public override ILogger Create(string name)
		{
			return new StreamLogger(name, new FileStream(name + ".log", FileMode.Append, FileAccess.Write), Encoding.Default);
		}

		public override ILogger Create(string name, LoggerLevel level)
		{
			var logger =
				new StreamLogger(name, new FileStream(name + ".log", FileMode.Append, FileAccess.Write), Encoding.Default);
			logger.Level = level;
			return logger;
		}
	}

#endif
}