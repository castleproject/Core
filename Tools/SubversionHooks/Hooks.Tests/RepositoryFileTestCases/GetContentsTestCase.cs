// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using System.IO;
	
	using NUnit.Framework;

	using Castle.SvnHooks;

	/// <summary>
	/// Summary description for GetContentsTestCase.
	/// </summary>
	[TestFixture] public class GetContentsTestCase
	{
		private const String SVN_MIME_TYPE = "svn:mime-type";

		//private const String NO_CONTENTS_PATH = "nocontents";
		private const String BINARY_WITH_CONTENTS_PATH = "bin";
		private const String CONTENTS_PATH = "text";

		private IRepository repository;
	
		#region Setup / Teardown

		[TestFixtureSetUp] public void FixtureSetUp()
		{
			MockRepository mockRepository = new MockRepository();
			//mockRepository.AddFile(NO_CONTENTS_PATH, null, null, null);
			mockRepository.AddFile(BINARY_WITH_CONTENTS_PATH, null, MimeType("application/octet-stream"), "Contents");
			mockRepository.AddFile(CONTENTS_PATH, null, null, "This is the first line\nSecond Line...\nAnd the third\n\nFinal line!");

			repository = mockRepository;
		}


		private static IDictionary MimeType(String mimeType)
		{
			IDictionary dictionary = new Hashtable();
			dictionary.Add(SVN_MIME_TYPE, new String[]{mimeType});

			return dictionary;
		}
				

		#endregion

		[ExpectedException(typeof(InvalidOperationException))]
		[Test] public void BinaryWithContents()
		{
			RepositoryFile file = new RepositoryFile(repository, BINARY_WITH_CONTENTS_PATH, RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Stream stream = file.GetContents();

		}
		
		[Test] public void Contents()
		{
			RepositoryFile file = new RepositoryFile(repository, CONTENTS_PATH, RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			using(Stream stream = file.GetContents())			
			using(TextReader reader = new StreamReader(stream))
			{
				
				Assert.AreEqual("This is the first line", reader.ReadLine());
				Assert.AreEqual("Second Line...", reader.ReadLine());
				Assert.AreEqual("And the third", reader.ReadLine());
				Assert.AreEqual("", reader.ReadLine());
				Assert.AreEqual("Final line!", reader.ReadLine());
				Assert.AreEqual(null, reader.ReadLine());

			}

		}
		[Test] public void RepeatedGetContents()
		{
			RepositoryFile file = new RepositoryFile(repository, CONTENTS_PATH, RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			for(int i = 0; i < 2; i++)
			{
				using(Stream stream = file.GetContents())			
				using(TextReader reader = new StreamReader(stream))
				{
				
					Assert.AreEqual("This is the first line", reader.ReadLine());
					Assert.AreEqual("Second Line...", reader.ReadLine());
					Assert.AreEqual("And the third", reader.ReadLine());
					Assert.AreEqual("", reader.ReadLine());
					Assert.AreEqual("Final line!", reader.ReadLine());
					Assert.AreEqual(null, reader.ReadLine());

				}
			}
		}
	}
}
