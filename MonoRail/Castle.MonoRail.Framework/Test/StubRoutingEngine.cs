// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Test
{
	using Routing;

	/// <summary>
	/// Lot to do here
	/// </summary>
	public class StubRoutingEngine : RoutingRuleContainer, IRoutingEngine
	{
		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="domainName">Name of the domain.</param>
		/// <returns></returns>
		public IRoutingRuleContainer ForDomain(string domainName)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="subdomain">The subdomain.</param>
		/// <returns></returns>
		public IRoutingRuleContainer ForSubDomain(string subdomain)
		{
			throw new System.NotImplementedException();
		}
	}
}