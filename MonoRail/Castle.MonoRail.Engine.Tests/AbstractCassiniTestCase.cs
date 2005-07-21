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

namespace Castle.MonoRail.Engine.Tests
{
	using System;
	using System.IO;
	using System.Net;

	using Cassini;
	
	using NUnit.Framework;

	public abstract class AbstractCassiniTestCase
	{
		private Server server;

		[TestFixtureSetUp]
		public void Init()
		{
			String physicalDir = Normalize(ObtainPhysicalDir());
			server = new Server(8083, ObtainVirtualDir(), physicalDir);
			server.Start();
		}

		[TestFixtureTearDown]
		public void Terminate()
		{
			server.Stop();
		}

		private String Normalize(String possibleRelativePath)
		{
			DirectoryInfo dir = new DirectoryInfo(possibleRelativePath);
			return dir.FullName;
		}

		protected virtual String ObtainPhysicalDir()
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\TestSite");
		}

		protected virtual String ObtainVirtualDir()
		{
			return "/";
		}

		protected void AssertContents(String expected, HttpWebResponse response)
		{
			AssertContents(expected, response, false);
		}

		protected void AssertContents(String expected, HttpWebResponse response, bool startsWith)
		{
			String contents = GetContents(response);

			if (startsWith)
			{
				if (expected.Length > contents.Length)
					Assert.Fail("Expected String with length " + expected.Length + " but was " + contents.Length);

				Assert.AreEqual(expected, contents.Substring(0, expected.Length));
			}
			else
			{
				Assert.AreEqual(expected, contents);
			}
		}

		private static String GetContents(HttpWebResponse response)
		{
			StreamReader sr = new StreamReader(response.GetResponseStream());
			return sr.ReadToEnd();
		}

		protected void Execute(String url, String expected)
		{
			Execute(url, expected, url, false);
		}

		protected void Execute(String url, String expected, bool startsWith)
		{
			Execute(url, expected, url, startsWith);
		}

		protected void Execute(String url, String expected, String expectedUrl)
		{
			Execute(url, expected, expectedUrl, false);
		}

		protected void Execute(String url, String expected, String expectedUrl, bool startsWith)
		{
			HttpWebRequest myReq = (HttpWebRequest)
				WebRequest.Create("http://localhost:8083" + url);

			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual(expectedUrl, response.ResponseUri.PathAndQuery);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents(expected, response, startsWith);
		}
	}
}