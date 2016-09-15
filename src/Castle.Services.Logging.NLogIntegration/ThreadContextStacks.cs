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

namespace Castle.Services.Logging.NLogIntegration
{
	using Castle.Core.Logging;

	///<summary>
	///</summary>
	public class ThreadContextStacks : IContextStacks
	{
		/// <summary>
		///   Gets the single <see cref = "IContextStack" />.
		/// </summary>
		/// <value>The value of <param name = "key"></param> is ignored because NLog only has a single stack</value>
		public IContextStack this[string key]
		{
			get
			{
				// NLog only has a single stack - NDC
				return new ThreadContextStack();
			}
		}
	}
}