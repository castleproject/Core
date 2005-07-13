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

namespace Castle.SvnHooks.Tests.RepositoryFileTestCases
{
	using System;
	using System.Collections;
	
	using NUnit.Framework;

	/// <summary>
	/// Summary description for GetPropertyTestCase.
	/// </summary>
	[TestFixture] public class GetPropertyTestCase
	{
		private const String SVN_MIME_TYPE = "svn:mime-type";

		private const String NO_PROPS_PATH = "noprops";
		private const String MIME_PROP_PATH = "mime";
		private const String EOL_STYLE_PROP_PATH = "eol";
		private const String LOGREGEGX_PROP_PATH = "logregex";

		private IRepository repository;
	
		#region Setup / Teardown

		[TestFixtureSetUp] public void FixtureSetUp()
		{
			MockRepository mockRepository = new MockRepository();
			mockRepository.AddFile(NO_PROPS_PATH, null, null, null);
			mockRepository.AddFile(MIME_PROP_PATH, null, Props("svn:mime-type", "text/source"), null);
			mockRepository.AddFile(EOL_STYLE_PROP_PATH, null, Props("svn:eol-style", "native"), null);
			mockRepository.AddFile(LOGREGEGX_PROP_PATH, null, Props("bugtraq:logregex", @"^.*$", @"\d+"), null);

			repository = mockRepository;
		}


		private static IDictionary Props(String property, params String[] values)
		{
			IDictionary dictionary = new Hashtable();
			dictionary.Add(property, values);

			return dictionary;
		}
				

		#endregion

		[Test] public void NoProps()
		{
			RepositoryFile file = new RepositoryFile(repository, NO_PROPS_PATH, RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Assert.IsNull(file.GetProperty("svn:mime-type"));

		}

		[Test] public void MimeProps()
		{
			RepositoryFile file = new RepositoryFile(repository, MIME_PROP_PATH, RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			String[] props = file.GetProperty("svn:mime-type");
            Assert.AreEqual(1, props.Length);			
			Assert.AreEqual("text/source", props[0]);
			Assert.AreEqual("text/source", file.MimeType);

		}
		
		[Test] public void EolStyleProps()
		{
			RepositoryFile file = new RepositoryFile(repository, EOL_STYLE_PROP_PATH, RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			String[] props = file.GetProperty("svn:eol-style");
			Assert.AreEqual(1, props.Length);
			Assert.AreEqual("native", props[0]);
			
		}
		[Test] public void LogRegexProps()
		{
			RepositoryFile file = new RepositoryFile(repository, LOGREGEGX_PROP_PATH, RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			String[] props = file.GetProperty("bugtraq:logregex");
			Assert.AreEqual(2, props.Length);
			Assert.AreEqual(@"^.*$", props[0]);
			Assert.AreEqual(@"\d+", props[1]);
			
		}


		[ExpectedException(typeof(ArgumentNullException))]
		[Test] public void GetPropertyWithNullName()
		{
			RepositoryFile file = new RepositoryFile(repository, NO_PROPS_PATH, RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			file.GetProperty(null);
		}

	}
}
