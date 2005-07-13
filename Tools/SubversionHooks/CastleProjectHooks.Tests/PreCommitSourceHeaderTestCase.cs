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

namespace CastleProjectHooks.Tests
{
	using System;
	using System.Text.RegularExpressions;
	
	using NUnit.Framework;

	using Castle.SvnHooks;
	using Castle.SvnHooks.CastleProject;

	/// <summary>
	/// Summary description for PreCommitSourceHeaderTestCase.
	/// </summary>
	[TestFixture] public class PreCommitSourceHeaderTestCase
	{

		private const String HEADER = 
			"// Copyright 2004-2005 Castle Project - http://www.castleproject.org/\n" +
			"// \n" +
			"// Licensed under the Apache License, Version 2.0 (the \"License\");\n" +
			"// you may not use this file except in compliance with the License.\n" +
			"// You may obtain a copy of the License at\n" +
			"// \n" +
			"//     http://www.apache.org/licenses/LICENSE-2.0\n" +
			"// \n" +
			"// Unless required by applicable law or agreed to in writing, software\n" +
			"// distributed under the License is distributed on an \"AS IS\" BASIS,\n" +
			"// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.\n" +
			"// See the License for the specific language governing permissions and\n" +
			"// limitations under the License.\n";

		IPreCommit hook;
	
		#region Setup / Teardown

		[TestFixtureSetUp] public void FixtureSetUp()
		{
			hook = new PreCommitSourceHeader("cs");
		}
		
		
		#endregion

		[Test] public void OnlyHeader()
		{
			AssertNoErrors(HEADER);
		}

		[Test] public void StartsWithHeader()
		{
			String contents = HEADER + '\n' +
				"\n" +
				"namespace Testing\n" + 
				"{\n" + 
				"    using System;\n" +
				"    public class Test\n" +
				"    {\n"+
				"    }\n"+
				"}"; 

			AssertNoErrors(contents);
		}
	
		[Test] public void LeadingBlankRows()
		{
			String contents = 
				'\n' +
				"      \n" +
				"\t\t\t\n" + 
				HEADER; 

			AssertNoErrors(contents);
		}
		
		[Test] public void ExtraLeadingAndTrailingWhiteSpace()
		{
			String contents = Regex.Replace(HEADER, "^//", " \t //").Replace("\n", " \t \n");

			AssertNoErrors(contents);
		}

		[Test] public void ExtraWhiteSpaceInHeader()
		{
			String contents = HEADER.Replace(" ", " \t ");

			AssertNoErrors(contents);
		}


		[Test] public void ExtraLineFeedsInHeader()
		{
			String contents = HEADER.Replace("\n", "\n\n");

			AssertErrors(contents, 1,
				"Blank lines are not allowed in the Apache License 2.0 header");
		}

		[Test] public void UsingBeforeHeader()
		{
			String contents = 
				"using System;\n" +
				"\n" + 
				HEADER; 

			AssertErrors(contents, 1, 
				"No text or code is allowed before the Apache License 2.0 header");
		}

		[Test] public void MalformedHeader()
		{
			String contents = HEADER.Replace("www.apache.org", "www.microsoft.com"); 

			AssertErrors(contents, 1, 
				"Apache License 2.0 header has errors on line 7");
		}

		[Test] public void MalformedHeaderMultipleRows()
		{
			String contents = HEADER.Replace("f", "?"); 

			AssertErrors(contents, 4, 
				"Apache License 2.0 header has errors on line 4",
				"Apache License 2.0 header has errors on line 5",
				"Apache License 2.0 header has errors on line 9",
				"Apache License 2.0 header has errors on line 12");
		}

		[Test] public void MalformedFirstLineHeader()
		{
			String contents = HEADER.Replace("Castle Project - http://www.castleproject.org/", "Me!"); 

			AssertErrors(contents, 1, 
				"Apache License 2.0 header is missing or there are errors on the first line");
		}
		[Test] public void EmptyFile()
		{
			AssertErrors(String.Empty, 1, 
				"Apache License 2.0 header is missing or there are errors on the first line");
		}

		[Test] public void FileWithoutHeader()
		{
			String contents = "using System;\n" +
				"\n" +
				"namespace Testing\n" + 
				"{\n" + 
				"    using System;\n" +
				"    public class Test\n" +
				"    {\n"+
				"    }\n"+
				"}"; 

			AssertErrors(contents, 1, 
				"Apache License 2.0 header is missing or there are errors on the first line");
		}


		[Test] public void UncheckedExtension()
		{
			AssertNoErrors(String.Empty, "trunk/file.txt");
		}


		[Test] public void Deleted()
		{

			using(RepositoryFile file = new RepositoryFile(new MockRepository(String.Empty), "File.cs", RepositoryStatus.Deleted, RepositoryStatus.Unchanged))
			{
				Error[] errors = hook.PreCommit(file);

				Assert.AreEqual(0, errors.Length);
			}

		}


		[Test] public void AssemblyInfo()
		{
			AssertNoErrors(String.Empty, "trunk/AssemblyInfo.cs");
		}

		private void AssertNoErrors(String contents, String fileName)
		{

			using(RepositoryFile file = MockRepository.GetFile(contents, fileName))
			{
				Error[] errors = hook.PreCommit(file);

				Assert.AreEqual(0, errors.Length);
			}

		}
		private void AssertNoErrors(String contents)
		{
			AssertNoErrors(contents, "trunk/File.cs");
		}
		private void AssertErrors(String contents, int count, params String[] messages)
		{

			using(RepositoryFile file = MockRepository.GetFile(contents, "trunk/File.cs"))
			{
				Error[] errors = hook.PreCommit(file);

				Assert.AreEqual(count, errors.Length);
				for(int i = 0; i < count; i++)
				{
					Assert.AreEqual(messages[i], errors[i].Description);
					Assert.AreEqual(file, errors[i].File);
				}
			}

		}


		private class MockRepository : IRepository
		{
			public MockRepository(String contents)
			{
				if (contents == null)
					throw new ArgumentNullException("contents");

				this.Contents = contents;
			}

			public static RepositoryFile GetFile(String contents, String fileName)
			{
				return new RepositoryFile(new MockRepository(contents), fileName, RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);
			}

			#region IRepository Members

			public string[] GetProperty(string path, string property)
			{
				// TODO:  Add MockRepository.GetProperty implementation
				return null;
			}

			public System.IO.Stream GetFileContents(string path)
			{
            	return new System.IO.MemoryStream(System.Text.Encoding.ASCII.GetBytes(Contents), false);
			}


			#endregion

			private readonly String Contents;
		}
		
	}
}
