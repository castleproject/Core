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

namespace Castle.MonoRail.Framework.Tests.Routing
{
	using System.Collections;
	using Framework.Routing;
	using Test;

	public abstract class BaseRuleTestFixture
	{
		protected static IRouteContext CreateGetContext()
		{
			return new RouteContext(new StubRequest("GET"), null, "/", new Hashtable());
		}

		protected static IRouteContext CreateGetContext(string appPath)
		{
			return new RouteContext(new StubRequest("GET"), null, appPath, new Hashtable());
		}

		protected static IRouteContext CreateContext(string appPath, Verb httpMethod)
		{
			return new RouteContext(new StubRequest(httpMethod.ToString()), null, appPath, new Hashtable());
		}	
	}
}
