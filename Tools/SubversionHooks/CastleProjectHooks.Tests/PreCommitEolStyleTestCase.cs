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

namespace Castel.SvnHooks.CastleProject.Tests
{
	using System;
	
	using NUnit.Framework;

	using Castle.SvnHooks;
	using Castle.SvnHooks.CastleProject;

	/// <summary>
	/// Summary description for PreCommitEolStyleTestCase.
	/// </summary>
	[TestFixture] public class PreCommitEolStyleTestCase
	{
	
		IPreCommit hook;
	
		#region Setup / Teardown

		[TestFixtureSetUp] public void FixtureSetUp()
		{
			hook = new PreCommitEolStyle("cs");
		}
		
		
		#endregion

		[Test] public void NoProperties()
		{
			AssertErrors(null, 1,
				"This file must have svn:eol-style set to native");
		}
		[Test] public void EolStyleLF()
		{
			AssertErrors("LF", 1,
				"This file must have svn:eol-style set to native");
		}
		[Test] public void EolStyleCR()
		{
			AssertErrors("CR", 1,
				"This file must have svn:eol-style set to native");
		}
		[Test] public void EolStyleCRLF()
		{
			AssertErrors("CRLF", 1,
				"This file must have svn:eol-style set to native");
		}
		[Test] public void EolStyleNative()
		{
			AssertNoErrors("native");
		}


		[Test] public void UncheckedExtension()
		{
			AssertNoErrors(null, "trunk/file.txt");
		}


		[Test] public void Deleted()
		{

			using(RepositoryFile file = new RepositoryFile(new MockRepository(null), "File.cs", RepositoryStatus.Deleted, RepositoryStatus.Unchanged))
			{
				Error[] errors = hook.PreCommit(file);

				Assert.AreEqual(0, errors.Length);
			}

		}

		private void AssertNoErrors(String eolStyle, String fileName)
		{

			using(RepositoryFile file = MockRepository.GetFile(eolStyle, fileName))
			{
				Error[] errors = hook.PreCommit(file);

				Assert.AreEqual(0, errors.Length);
			}

		}
		private void AssertNoErrors(String eolStyle)
		{

			AssertNoErrors(eolStyle, "trunk/File.cs");

		}
		private void AssertErrors(String eolStyle, int count, params String[] messages)
		{

			using(RepositoryFile file = MockRepository.GetFile(eolStyle, "trunk/File.cs"))
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
			public MockRepository(String eolStyle)
			{
				this.EolStyle = eolStyle;
			}

			public static RepositoryFile GetFile(String eolStyle, String fileName)
			{
				return new RepositoryFile(new MockRepository(eolStyle), fileName, RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);
			}

			#region IRepository Members

			public string[] GetProperty(string path, string property)
			{
				if (EolStyle != null && property == "svn:eol-style")
					return new String[]{EolStyle};

				return null;
			}

			public System.IO.Stream GetFileContents(string path)
			{
				return null;
			}


			#endregion

			private readonly String EolStyle;
		}

	}
}
