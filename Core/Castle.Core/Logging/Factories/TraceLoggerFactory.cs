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

namespace Castle.Core.Logging
{
	#if !SILVERLIGHT

	/// <summary>
	/// Used to create the TraceLogger implementation of ILogger interface. See <see cref="TraceLogger"/>. 
	/// </summary>
	public class TraceLoggerFactory : AbstractLoggerFactory
	{
		public override ILogger Create(string name)
		{
			return new TraceLogger(name);
		}

		public override ILogger Create(string name, LoggerLevel level)
		{
			return new TraceLogger(name, level);
		}
	}

	#endif
}
