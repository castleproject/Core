// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

#if FEATURE_SERIALIZATION
	[Serializable]
#endif
	public class DiagnosticsLoggerFactory : AbstractLoggerFactory
	{
		private const string DefaultLogName = "CastleDefaultLogger";

		public override ILogger Create(string name)
		{
			return new DiagnosticsLogger(DefaultLogName, name);
		}

		public override ILogger Create(string name, LoggerLevel level)
		{
			var logger = new DiagnosticsLogger(DefaultLogName, name);
			logger.Level = level;
			return logger;
		}
	}
}
