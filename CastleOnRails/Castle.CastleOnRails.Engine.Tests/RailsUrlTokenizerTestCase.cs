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

namespace RailsEngineTest
{
	using System;

	using NUnit.Framework;

	using Castle.CastleOnRails.Engine;

	/// <summary>
	/// Summary description for RailsUrlTokenizerTestCase.
	/// </summary>
	[TestFixture]
	public class RailsUrlTokenizerTestCase
	{
		[Test]
		public void SimpleUsage()
		{
			UrlInfo info = UrlTokenizer.ExtractInfo( "/home/index.rails" );
			Assert.IsNotNull( info );
			Assert.AreEqual( "home", info.Controller );
			Assert.AreEqual( "index", info.Action );
		}

		[Test]
		public void ExtensionIgnored()
		{
			UrlInfo info = UrlTokenizer.ExtractInfo( "/home/index.something" );
			Assert.IsNotNull( info );
			Assert.AreEqual( "home", info.Controller );
			Assert.AreEqual( "index", info.Action );
		}

		[Test]
		public void VirtualDirectory()
		{
			UrlInfo info = UrlTokenizer.ExtractInfo( "/myvirdirectory/home/index.rails" );
			Assert.IsNotNull( info );
			Assert.AreEqual( "home", info.Controller );
			Assert.AreEqual( "index", info.Action );
		}
	}
}
