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

namespace Castle.MonoRail.Framework.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Summary description for RailsUrlTokenizerTestCase.
	/// </summary>
	[TestFixture]
	public class RailsUrlTokenizerTestCase
	{
		[Test]
		public void SimpleUsage()
		{
			UrlInfo info = UrlTokenizer.ExtractInfo( "/home/index.rails", null );
			Assert.IsNotNull( info );
			Assert.AreEqual( "home", info.Controller );
			Assert.AreEqual( "index", info.Action );
			Assert.AreEqual( String.Empty, info.Area );
			Assert.AreEqual( "rails", info.Extension );
		}

		[Test]
		public void ExtensionIgnored()
		{
			UrlInfo info = UrlTokenizer.ExtractInfo( "/home/index.something", null );
			Assert.IsNotNull( info );
			Assert.AreEqual( "home", info.Controller );
			Assert.AreEqual( "index", info.Action );
			Assert.AreEqual( String.Empty, info.Area );
			Assert.AreEqual( "something", info.Extension );
		}

		[Test]
		public void Area()
		{
			UrlInfo info = UrlTokenizer.ExtractInfo( "/myvirdirectory/home/index.rails", null );
			Assert.IsNotNull( info );
			Assert.AreEqual( "home", info.Controller );
			Assert.AreEqual( "index", info.Action );
			Assert.AreEqual( "myvirdirectory", info.Area );
		}

		[Test]
		public void Area2()
		{
			UrlInfo info = UrlTokenizer.ExtractInfo( "/myvirdirectory/clients/home/index.rails", "/myvirdirectory" );
			Assert.IsNotNull( info );
			Assert.AreEqual( "home", info.Controller );
			Assert.AreEqual( "index", info.Action );
			Assert.AreEqual( "clients", info.Area );
		}

		[Test]
		public void Area3()
		{
			UrlInfo info = UrlTokenizer.ExtractInfo( "/myvirdirectory/mysite/clients/home/index.rails", "/myvirdirectory" );
			Assert.IsNotNull( info );
			Assert.AreEqual( "home", info.Controller );
			Assert.AreEqual( "index", info.Action );
			Assert.AreEqual( "mysite/clients", info.Area );
		}

		[Test]
		public void VirDir()
		{
			UrlInfo info = UrlTokenizer.ExtractInfo( "/myvirdirectory/home/index.rails", "/myvirdirectory" );
			Assert.IsNotNull( info );
			Assert.AreEqual( "home", info.Controller );
			Assert.AreEqual( "index", info.Action );
			Assert.AreEqual( String.Empty, info.Area );
		}

		[Test]
		public void EmptyVirDir()
		{
			UrlInfo info = UrlTokenizer.ExtractInfo( "/home/index.rails", "" );
			Assert.IsNotNull( info );
			Assert.AreEqual( "home", info.Controller );
			Assert.AreEqual( "index", info.Action );
			Assert.AreEqual( String.Empty, info.Area );
		}
	}
}
