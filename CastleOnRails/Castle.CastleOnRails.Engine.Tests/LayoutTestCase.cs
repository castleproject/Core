// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.CastleOnRails.Engine.Tests
{
	using System;
	using System.Net;

	using NUnit.Framework;


	[TestFixture]
	public class LayoutTestCase : AbstractCassiniTestCase
	{
		[Test]
		public void ContentWithLayout()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/layoutable/index.rails");
			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("/layoutable/index.rails", response.ResponseUri.PathAndQuery);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("\r\nWelcome!\r\n<p>Inner Content</p>\r\nFooter", response);
		}

		[Test]
		public void LayoutChanged()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/layoutable/ChangeLayout.rails");
			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("/layoutable/ChangeLayout.rails", response.ResponseUri.PathAndQuery);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("\r\nDifferent master page\r\n<p>Another Inner Content</p>\r\nFooter", response);
		}

		[Test]
		public void Redirect()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/layoutable/redirectAction.rails");
			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("/home/index.rails", response.ResponseUri.PathAndQuery);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("My View contents for Home\\Index", response);
		}
	}
}
