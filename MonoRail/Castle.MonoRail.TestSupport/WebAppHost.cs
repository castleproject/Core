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

namespace Castle.MonoRail.TestSupport
{
	using System;
	using System.IO;
	using System.Web;


	/// <summary>
	/// 
	/// </summary>
	public class WebAppHost : MarshalByRefObject
	{
		public string appVirtualPath;
		public string appPhysicalPath;

		public WebAppHost()
		{
		}

		public void Configure(String appVirtualPath, String appPhysicalPath)
		{
			this.appVirtualPath = appVirtualPath;
			this.appPhysicalPath = appPhysicalPath;

			// AppDomain.CurrentDomain.SetupInformation.ConfigurationFile = "web-test.config";
		}

		public TestResponse Process(TestRequest request, TextWriter writer)
		{
			MonoRailTestWorkerRequest workerRequest = 
				new MonoRailTestWorkerRequest(request, appVirtualPath, appPhysicalPath, writer);

			workerRequest.Prepare();

			HttpRuntime.ProcessRequest( workerRequest );

			return workerRequest.Response;
		}

		public void Dispose()
		{
			HttpRuntime.UnloadAppDomain();
		}
	}
}
