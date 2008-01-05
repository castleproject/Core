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

namespace Castle.MonoRail.Framework.Tests
{
	using Castle.MonoRail.Framework.Services;
	using NUnit.Framework;

	[TestFixture]
	public class UrlPartsBuilderTestCase
	{
		[Test]
		public void CanBuildPathUrls()
		{
			UrlPartsBuilder builder = new UrlPartsBuilder("controller", "action");
		
			Assert.AreEqual("controller/action", builder.BuildPath());
		}

		[Test]
		public void BuildPathWithPathInfo()
		{
			UrlPartsBuilder builder = new UrlPartsBuilder("controller", "action");
			builder.PathInfo.Add("State").Add("FL");

			Assert.AreEqual("controller/action/State/FL", builder.BuildPath());
		}

		[Test]
		public void BuildPathWithPathInfoDictionary()
		{
			UrlPartsBuilder builder = new UrlPartsBuilder("controller", "action");
			builder.PathInfoDict["State"] ="FL";

			Assert.AreEqual("controller/action/State/FL", builder.BuildPath());
		}

		[Test]
		public void BuildPathWithPathInfoAndQueryString()
		{
			UrlPartsBuilder builder = new UrlPartsBuilder("controller", "action");
			builder.PathInfoDict["State"] = "FL";
			builder.SetQueryString("type=Residential");

			Assert.AreEqual("controller/action/State/FL?type=Residential", builder.BuildPath());
		}

		[Test]
		public void PathInfoDictParse_AcceptsNull()
		{
			UrlPartsBuilder builder = new UrlPartsBuilder("controller", "action");
			builder.PathInfoDict.Parse(null);

			Assert.AreEqual("controller/action", builder.BuildPath());
		}

		[Test]
		public void PathInfoDictParse_AcceptsEmptyString()
		{
			UrlPartsBuilder builder = new UrlPartsBuilder("controller", "action");
			builder.PathInfoDict.Parse("");

			Assert.AreEqual("controller/action", builder.BuildPath());
		}

		[Test]
		public void PathInfoDictParse_CanHandleMissingSlash()
		{
			UrlPartsBuilder builder = new UrlPartsBuilder("controller", "action");
			builder.PathInfoDict.Parse("State/Fl");

			Assert.AreEqual("controller/action/State/Fl", builder.BuildPath());
		}

		[Test]
		public void PathInfoDictParse_CanHandleMultipleEntries()
		{
			UrlPartsBuilder builder = new UrlPartsBuilder("controller", "action");
			builder.PathInfoDict.Parse("/State/FL/Type/Home");

			Assert.AreEqual("controller/action/State/FL/Type/Home", builder.BuildPath());
		}

		[Test]
		public void PathInfoDictParse_CanHandleOddNumberOfEntries()
		{
			UrlPartsBuilder builder = new UrlPartsBuilder("controller", "action");
			builder.PathInfoDict.Parse("/State/FL/Type/");

			Assert.AreEqual("controller/action/State/FL/Type", builder.BuildPath());
		}

		[Test]
		public void PathInfoDictParse_CanHandleOddNumberOfEntries2()
		{
			UrlPartsBuilder builder = new UrlPartsBuilder("controller", "action");
			builder.PathInfoDict.Parse("/State/FL/Type");

			Assert.AreEqual("controller/action/State/FL/Type", builder.BuildPath());
		}

		[Test]
		public void ParseCanHandleAbsolutePaths()
		{
			UrlPartsBuilder builder = UrlPartsBuilder.Parse("http://localhost/home/index.ext");

			Assert.AreEqual(0, builder.PathInfoDict.Count);

			Assert.AreEqual("http://localhost/home/index.ext", builder.BuildPath());
		}

		[Test]
		public void ParseCanHandleAbsolutePathsWithQueryString()
		{
			UrlPartsBuilder builder = UrlPartsBuilder.Parse("http://localhost/home/index.ext?id=1&type=home");

			Assert.AreEqual(0, builder.PathInfoDict.Count);
			Assert.AreEqual("id=1&type=home", builder.QueryStringAsString());

			Assert.AreEqual("http://localhost/home/index.ext?id=1&type=home", builder.BuildPath());
		}

		[Test]
		public void ParseCanHandleAbsolutePathsWithPathInfo()
		{
			UrlPartsBuilder builder = UrlPartsBuilder.Parse("http://localhost/home/index.ext/state/fl/");

			Assert.AreEqual(1, builder.PathInfoDict.Count);
			Assert.AreEqual("fl", builder.PathInfoDict["state"]);
			Assert.AreEqual("", builder.QueryStringAsString());

			Assert.AreEqual("http://localhost/home/index.ext/state/fl", builder.BuildPath());
		}

		[Test]
		public void ParseCanHandleRelativePaths()
		{
			UrlPartsBuilder builder = UrlPartsBuilder.Parse("home/index.ext");

			Assert.AreEqual("home/index.ext", builder.BuildPath());
		}

		[Test]
		public void ParseCanHandleRelativePathsWithEmptyPathInfo()
		{
			UrlPartsBuilder builder = UrlPartsBuilder.Parse("home/index.ext/");

			Assert.AreEqual("home/index.ext", builder.BuildPath());
		}

		[Test]
		public void ParseCanHandleRelativePathsWithPathInfo()
		{
			UrlPartsBuilder builder = UrlPartsBuilder.Parse("home/index.ext/state/fl");

			Assert.AreEqual(1, builder.PathInfoDict.Count);
			Assert.AreEqual("fl", builder.PathInfoDict["state"]);
			Assert.IsNull(builder.QueryStringAsString());

			Assert.AreEqual("home/index.ext/state/fl", builder.BuildPath());
		}

		[Test]
		public void ParseCanHandleRelativePathsWithPathInfo2()
		{
			UrlPartsBuilder builder = UrlPartsBuilder.Parse("home/index.ext/state/fl/");

			Assert.AreEqual(1, builder.PathInfoDict.Count);
			Assert.AreEqual("fl", builder.PathInfoDict["state"]);
			Assert.IsNull(builder.QueryStringAsString());

			Assert.AreEqual("home/index.ext/state/fl", builder.BuildPath());
		}

		[Test]
		public void ParseCanHandleRelativePathsWithQueryString()
		{
			UrlPartsBuilder builder = UrlPartsBuilder.Parse("home/index.ext?id=1&name=john");

			Assert.AreEqual(0, builder.PathInfoDict.Count);
			Assert.AreEqual("id=1&name=john", builder.QueryStringAsString());

			Assert.AreEqual("home/index.ext?id=1&name=john", builder.BuildPath());
		}

		[Test]
		public void ParseCanHandleRelativePathsWithEmptyPathInfoAndQueryString()
		{
			UrlPartsBuilder builder = UrlPartsBuilder.Parse("home/index.ext/?id=1&name=john");

			Assert.AreEqual(0, builder.PathInfoDict.Count);
			Assert.AreEqual("id=1&name=john", builder.QueryStringAsString());

			Assert.AreEqual("home/index.ext?id=1&name=john", builder.BuildPath());
		}

		[Test]
		public void ParseCanHandleRelativePathsWithPathInfoAndQueryString()
		{
			UrlPartsBuilder builder = UrlPartsBuilder.Parse("home/index.ext/state/fl/?id=1&name=john");

			Assert.AreEqual(1, builder.PathInfoDict.Count);
			Assert.AreEqual("fl", builder.PathInfoDict["state"]);
			Assert.AreEqual("id=1&name=john", builder.QueryStringAsString());

			Assert.AreEqual("home/index.ext/state/fl?id=1&name=john", builder.BuildPath());
		}

		[Test]
		public void QueryStringParsesStringCorrectly()
		{
			UrlPartsBuilder builder = new UrlPartsBuilder("home/index.ext");

			builder.QueryString["state"] = "FL";

			Assert.AreEqual("home/index.ext?state=FL", builder.BuildPath());
		}

		[Test]
		public void QueryStringIsExtractedAndParsed()
		{
			UrlPartsBuilder builder = new UrlPartsBuilder("home/index.ext");

			builder.SetQueryString("City=SP&State=MD");

			builder.QueryString["type"] = "home";

			Assert.AreEqual("home/index.ext?City=SP&State=MD&type=home", builder.BuildPath());
		}

		[Test]
		public void QueryStringCanHandleDuplicatedEntries()
		{
			UrlPartsBuilder builder = new UrlPartsBuilder("home/index.ext");

			builder.SetQueryString("City=SP&State=MD&State=NY");

			Assert.AreEqual("home/index.ext?City=SP&State=MD&State=NY", builder.BuildPath());
		}

		[Test]
		public void QueryStringCanReplaceEntries()
		{
			UrlPartsBuilder builder = new UrlPartsBuilder("home/index.ext");

			builder.QueryString["page"] = "1";

			Assert.AreEqual("home/index.ext?page=1", builder.BuildPath());

			builder.QueryString.Set("page", "2");

			Assert.AreEqual("home/index.ext?page=2", builder.BuildPath());

			builder.QueryString.Set("page", "3");

			Assert.AreEqual("home/index.ext?page=3", builder.BuildPath());
		}
	}
}
