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

namespace Castle.Facilities.NHibernateIntegration.Tests
{
	using System.Configuration;
	using System.IO;

	sealed class ConfigHelper
	{
		public static string ResolvePath(string configFilePath)
		{
#if DOTNET2
			return Path.Combine(ConfigurationManager.AppSettings["tests.src"], configFilePath);
#else
			return Path.Combine(ConfigurationSettings.AppSettings["tests.src"], configFilePath);
#endif
		}
	}
}
