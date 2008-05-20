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

namespace Castle.MonoRail.Framework.Tests.Services
{
	using System;
	using Castle.MonoRail.Framework.Services;
	using NUnit.Framework;

	[TestFixture]
	public class DefaultUrlTokenizerTestCase
	{
		private DefaultUrlTokenizer tokenizer;

		[SetUp]
		public void Init()
		{
			tokenizer = new DefaultUrlTokenizer();
		}

		[Test]
		public void AddingSimpleDefault()
		{
			tokenizer.AddDefaultRule("index.rails", "", "client", "list");

			UrlInfo info = tokenizer.TokenizeUrl("/index.rails", null, new Uri("http://localhost"), true, null);
			Assert.IsNotNull(info);
			Assert.AreEqual("client", info.Controller);
			Assert.AreEqual("list", info.Action);
			Assert.AreEqual(String.Empty, info.Area);
			Assert.AreEqual("rails", info.Extension);
		}

		[Test]
		public void AddingSimpleDefault2()
		{
			tokenizer.AddDefaultRule("index.rails", "", "client", "list");
			tokenizer.AddDefaultRule("list.rails", "", "product", "list");

			UrlInfo info = tokenizer.TokenizeUrl("/list.rails", null, new Uri("http://localhost"), true, null);
			Assert.IsNotNull(info);
			Assert.AreEqual("product", info.Controller);
			Assert.AreEqual("list", info.Action);
			Assert.AreEqual(String.Empty, info.Area);
			Assert.AreEqual("rails", info.Extension);
		}

		[Test]
		public void AddingSimpleDefaultWithArea()
		{
			tokenizer.AddDefaultRule("index.rails", "public", "client", "list");
			tokenizer.AddDefaultRule("list.rails", "public", "product", "list");

			UrlInfo info = tokenizer.TokenizeUrl("/list.rails", null, new Uri("http://localhost"), true, null);
			Assert.IsNotNull(info);
			Assert.AreEqual("product", info.Controller);
			Assert.AreEqual("list", info.Action);
			Assert.AreEqual("public", info.Area);
			Assert.AreEqual("rails", info.Extension);
		}

		[Test]
		public void AddingSimpleDefaultWithArea2()
		{
			tokenizer.AddDefaultRule("index.rails", "public", "client", "list");
			tokenizer.AddDefaultRule("list.rails", "public/simple", "product", "list");

			UrlInfo info = tokenizer.TokenizeUrl("/list.rails", null, new Uri("http://localhost"), true, null);
			Assert.IsNotNull(info);
			Assert.AreEqual("product", info.Controller);
			Assert.AreEqual("list", info.Action);
			Assert.AreEqual("public/simple", info.Area);
			Assert.AreEqual("rails", info.Extension);
		}

		[Test]
		public void SimpleUsage()
		{
			UrlInfo info = tokenizer.TokenizeUrl("/home/index.rails", null, new Uri("http://localhost"), true, null);
			Assert.IsNotNull( info );
			Assert.AreEqual( "home", info.Controller );
			Assert.AreEqual( "index", info.Action );
			Assert.AreEqual( String.Empty, info.Area );
			Assert.AreEqual( "rails", info.Extension );
		}

		[Test]
		public void ExtensionIgnored()
		{
			UrlInfo info = tokenizer.TokenizeUrl("/home/index.something", null, new Uri("http://localhost"), true, null);
			Assert.IsNotNull( info );
			Assert.AreEqual( "home", info.Controller );
			Assert.AreEqual( "index", info.Action );
			Assert.AreEqual( String.Empty, info.Area );
			Assert.AreEqual( "something", info.Extension );
		}

		[Test]
		public void Area()
		{
			UrlInfo info = tokenizer.TokenizeUrl("/myvirdirectory/home/index.rails", null, new Uri("http://localhost"), true, null);
			Assert.IsNotNull( info );
			Assert.AreEqual( "home", info.Controller );
			Assert.AreEqual( "index", info.Action );
			Assert.AreEqual( "myvirdirectory", info.Area );
		}

		[Test]
		public void Area2()
		{
			UrlInfo info = tokenizer.TokenizeUrl("/myvirdirectory/clients/home/index.rails", null, new Uri("http://localhost"), true, "/myvirdirectory");
			Assert.IsNotNull( info );
			Assert.AreEqual( "home", info.Controller );
			Assert.AreEqual( "index", info.Action );
			Assert.AreEqual( "clients", info.Area );
		}

		[Test]
		public void Area3()
		{
			UrlInfo info = tokenizer.TokenizeUrl("/myvirdirectory/mysite/clients/home/index.rails", null, new Uri("http://localhost"), true, "/myvirdirectory");
			Assert.IsNotNull( info );
			Assert.AreEqual( "home", info.Controller );
			Assert.AreEqual( "index", info.Action );
			Assert.AreEqual( "mysite/clients", info.Area );
		}

		[Test]
		public void VirDir()
		{
			UrlInfo info = tokenizer.TokenizeUrl("/myvirdirectory/home/index.rails", null, new Uri("http://localhost"), true, "/myvirdirectory");
			Assert.IsNotNull( info );
			Assert.AreEqual( "home", info.Controller );
			Assert.AreEqual( "index", info.Action );
			Assert.AreEqual( String.Empty, info.Area );
		}

		[Test]
		public void VirDirNonLowerCase()
		{
			UrlInfo info = tokenizer.TokenizeUrl("/MyVirDirectory/home/index.rails", null, new Uri("http://localhost"), true, "/myvirdirectory");
			Assert.IsNotNull(info);
			Assert.AreEqual("home", info.Controller);
			Assert.AreEqual("index", info.Action);
			Assert.AreEqual(String.Empty, info.Area);
		}

		[Test]
		public void EmptyVirDir()
		{
			UrlInfo info = tokenizer.TokenizeUrl("/home/index.rails", null, new Uri("http://localhost"), true, "");
			Assert.IsNotNull( info );
			Assert.AreEqual( "home", info.Controller );
			Assert.AreEqual( "index", info.Action );
			Assert.AreEqual( String.Empty, info.Area );
		}

		[Test, ExpectedException(typeof(UrlTokenizerException), ExpectedMessage = "Url smaller than 2 tokens")]
	    public void BadUrl()
	    {
			tokenizer.TokenizeUrl("/index.rails", null, new Uri("http://localhost"), true, null);
	    }

		[Test]
		public void DomainSubDomainExpectWwwSubdomain()
		{
			UrlInfo info = tokenizer.TokenizeUrl("/area/home/index.castle", null,
								  new Uri("http://www.castleproject.org"), true, string.Empty);
			Assert.AreEqual("castleproject.org", info.Domain);
			Assert.AreEqual("www", info.Subdomain);
			Assert.AreEqual("area", info.Area);
			Assert.AreEqual("home", info.Controller);
			Assert.AreEqual("index", info.Action);
		}

		[Test]
		public void DomainSubDomainExpectCastleprojectSubdomain()
		{
			UrlInfo info = tokenizer.TokenizeUrl("/area/home/index.castle", null,
								  new Uri("http://castleproject.org"), true, string.Empty);
			Assert.AreEqual("org", info.Domain);
			Assert.AreEqual("castleproject", info.Subdomain);
			Assert.AreEqual("area", info.Area);
			Assert.AreEqual("home", info.Controller);
			Assert.AreEqual("index", info.Action);
		}
	}
}
