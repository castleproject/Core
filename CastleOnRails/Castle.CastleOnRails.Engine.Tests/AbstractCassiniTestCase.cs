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
	using System.IO;
	using System.Net;
	using System.Text;

	using NUnit.Framework;

	using Cassini;

	public abstract class AbstractCassiniTestCase
	{
		private Server server;

		[SetUp]
		public void Init()
		{
			server = new Server(8083, ObtainVirtualDir(), ObtainPhysicalDir());
			server.Start();
		}

		[TearDown]
		public void Terminate()
		{
			server.Stop();
		}

		protected virtual String ObtainPhysicalDir()
		{
			String path = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"..\AspnetSample" );
			return path;
		}

		protected virtual String ObtainVirtualDir()
		{
			return "/";
		}

		protected void AssertContents(String expected, HttpWebResponse response)
		{
			int size = expected.Length;
			byte[] contentsArray = new byte[size];
			response.GetResponseStream().Read(contentsArray, 0, size);
			Encoding encoding = Encoding.Default;
			String contents = encoding.GetString(contentsArray);
			Assert.AreEqual( expected, contents );
		}
	}
}
