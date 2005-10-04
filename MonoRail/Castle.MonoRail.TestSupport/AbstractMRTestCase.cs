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

namespace Castle.MonoRail.TestSupport
{
	using System;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using System.Web;
	using System.Collections;

	using NUnit.Framework;

	using Castle.MonoRail.Engine;
	using Castle.MonoRail.Engine.Adapters;
	using Castle.MonoRail.Engine.Configuration;


	public abstract class AbstractMRTestCase
	{
		private MonoRailConfiguration customConfig;
		private ProcessEngineFactory processEngineFactory;
		private ProcessEngine processEngine;
		private MockWorkerRequest request;
		private HttpResponse response;
		private StringBuilder outputContents = new StringBuilder();

		[TestFixtureSetUp]
		public virtual void FixtureInitialize()
		{
			if (customConfig != null)
			{
				processEngineFactory = new ProcessEngineFactory( customConfig );
			}
			else
			{
				processEngineFactory = new ProcessEngineFactory();
			}
		}

		[SetUp]
		public virtual void Initialize()
		{
			outputContents.Length = 0;

			processEngine = processEngineFactory.Create();

			CustomizeProcessEngine(processEngine);

			StringWriter writer = new StringWriter(outputContents);

			request = new MockWorkerRequest(writer);
			request.VirtualPath = "/";
			
			response = null;
		}

		[TearDown]
		public virtual void Terminate()
		{
			processEngine = null;
			request = null;
			response = null;
		}

		[TestFixtureTearDown]
		public virtual void FixtureTerminate()
		{
			processEngineFactory = null;
		}

		protected virtual void CustomizeProcessEngine(ProcessEngine processEngine)
		{
			
		}

		protected void DoGet(String path, params String[] queryStringParams)
		{
			// url-decode path

			if (path.IndexOf('%') >= 0) 
			{
				path = HttpUtility.UrlDecode(path);
			}

			// path info

			String filePath = String.Empty;
			String pathInfo = String.Empty;

			int lastDot = path.LastIndexOf('.');
			int lastSlh = path.LastIndexOf('/');

			if (lastDot >= 0 && lastSlh >= 0 && lastDot < lastSlh) 
			{
				int ipi = path.IndexOf('/', lastDot);
				filePath = path.Substring(0, ipi);
				pathInfo = path.Substring(ipi);
			}
			else 
			{
				filePath = path;
				pathInfo = String.Empty;
			}

			request.FilePath = filePath;
			request.PathInfo = pathInfo;

			outputContents.Length = 0;

			HttpContext context = new HttpContext( request );
			
			// context.Items["AspSession"] = new Hashtable();

			// TODO: Extract query string from url

			request.Prepare();

			HttpRuntime.ProcessRequest( request );

			response = context.Response;

			processEngine.Process( new RailsEngineContextAdapter(context, path) );
		}

		protected void DoPost(String url, params String[] postParams)
		{
			outputContents.Length = 0;

			HttpContext context = new HttpContext( request );

			request.PostParams = postParams;
			
			context.Items["AspSession"] = new Hashtable();

			// TODO: Extract query string from url

			request.Prepare();

			processEngine.Process( new RailsEngineContextAdapter(context, url) );

			response = context.Response;
			response.Flush();
		}

//		protected void DoPostFile(String url, String fileName)
//		{
//			
//		}

		protected void AssertSuccess()
		{
			Assert.IsNotNull(response, "No requests performed with DoGet or DoPost (?)");
			Assert.AreEqual(200, response.StatusCode, "Status code different than 200");
		}

		protected void AssertReplyEqualsTo(String expectedContents)
		{
			
		}

		public MockWorkerRequest Request
		{
			get { return request; }
		}

		public MonoRailConfiguration Config
		{
			get
			{
				if (customConfig == null)
				{
					customConfig = new MonoRailConfiguration();
				}
				return customConfig;
			}
		}
	}
}