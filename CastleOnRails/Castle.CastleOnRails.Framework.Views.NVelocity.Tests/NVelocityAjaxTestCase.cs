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

namespace Castle.CastleOnRails.Framework.Views.NVelocity.Tests
{
	using System;
	using System.Net;
	using System.IO;

	using NUnit.Framework;

	using Castle.CastleOnRails.Engine.Tests;


	[TestFixture]
	public class NVelocityAjaxTestCase : AbstractCassiniTestCase
	{
		[Test]
		public void JsFunctions()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/ajax/JsFunctions.rails");

			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("\r\n<script type=\"text/javascript\">", response);
		}

		[Test]
		public void LinkToFunction()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/ajax/LinkToFunction.rails");

			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("<a href=\"#\" onclick=\"alert('Ok'); return false;\" ><img src='myimg.gid'></a>", response);
		}

		[Test]
		public void LinkToRemote()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/ajax/LinkToRemote.rails");

			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("<a href=\"#\" onclick=\"new " + 
				"Ajax.Request('/controller/action.rails', {asynchronous:true}); " + 
				"return false;\" ><img src='myimg.gid'></a>", response);
		}

		[Test]
		public void BuildFormRemoteTag()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/ajax/BuildFormRemoteTag.rails");

			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("<form method=\"post\" onsubmit=\"new Ajax.Request('url', " +
				"{method:post, asynchronous:true, parameters:Form.serialize(this)}); " + 
				"return false;\">", response);
		}

		[Test]
		public void ObserveField()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/ajax/ObserveField.rails");

			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("<script type=\"text/javascript\">new Form.Element.Observer('myfieldid', 2, " + 
				"function(element, value) { new Ajax.Updater('elementToBeUpdated', '/url', " + 
				"{asynchronous:true, parameters:newcontent}) })</script>", response);
		}

		[Test]
		public void ObserveForm()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/ajax/ObserveForm.rails");

			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("<script type=\"text/javascript\">new Form.Observer('myfieldid', 2, " + 
				"function(element, value) { new Ajax.Updater('elementToBeUpdated', '/url', " + 
				"{asynchronous:true, parameters:newcontent}) ", response);
		}

		protected override String ObtainPhysicalDir()
		{
			return Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"..\TestSiteNVelocity" );
		}

	}
}
