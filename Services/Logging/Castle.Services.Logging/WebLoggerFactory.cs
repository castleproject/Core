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

namespace Castle.Services.Logging
{
	using System;

	public class WebLoggerFactory : ILoggerFactory
	{
		public ILogger Create(Type type)
		{
			return new WebLogger( type.FullName );
		}

		public ILogger Create(String name)
		{
			return new WebLogger( name );
		}

		public ILogger Create(Type type, LoggerLevel level)
		{
			return new WebLogger( type.FullName, level );
		}

		public ILogger Create(String name, LoggerLevel level)
		{
			return new WebLogger( name, level );
		}
	}
}
