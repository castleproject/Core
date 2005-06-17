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

namespace Castle.MonoRail.Framework.Views.NVelocity.Tests
{
	using System;
	using System.IO;
	using System.Net;

	using NUnit.Framework;

	using Castle.MonoRail.Engine.Tests;

	[TestFixture]
	public class RoutingTestCase : AbstractCassiniTestCase
	{
		protected override String ObtainPhysicalDir()
		{
			return Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"..\TestSiteNVelocity" );
		}
		
		[Test]
		public void BlogRoutingRule()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/blog/posts/2005/07/");
			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("/blog/posts/2005/07/", response.ResponseUri.PathAndQuery);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("Blog: year=2005 month=7", response);
		}

		[Test]
		public void NewsRoutingRule()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/news/2004/11/");
			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("/news/2004/11/", response.ResponseUri.PathAndQuery);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("News: year=2004 month=11", response);
		}
	}
}
