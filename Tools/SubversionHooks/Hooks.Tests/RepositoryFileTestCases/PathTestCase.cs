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
	
	using NUnit.Framework;

	using Castle.SvnHooks;
	
	/// <summary>
	/// Summary description for PathTestCase.
	/// </summary>
	[TestFixture] public class PathTestCase
	{

		private IRepository repository;
	
		#region Setup / Teardown

		[TestFixtureSetUp] public void FixtureSetUp()
		{
			repository = new MockRepository();
		}

				
		#endregion

		[ExpectedException(typeof(ArgumentNullException))]
		[Test] public void ConstructedWithNullPath()
		{
			RepositoryFile file = new RepositoryFile(repository, null, RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test] public void ConstructedWithEmptyPath()
		{
			RepositoryFile file = new RepositoryFile(repository, String.Empty, RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test] public void ConstructedWithBlankPath()
		{
			RepositoryFile file = new RepositoryFile(repository, "  ", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test] public void ConstructedWithDirectoryPath()
		{
			RepositoryFile file = new RepositoryFile(repository, "trunk/", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);
		}


		[ExpectedException(typeof(ArgumentException))]
		[Test] public void FileNameWithTrailingSpaces()
		{
			RepositoryFile file = new RepositoryFile(repository, "trunk/file ", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);
		}
		[ExpectedException(typeof(ArgumentException))]
		[Test] public void FileNameUnderRootWithTrailingSpaces()
		{
			RepositoryFile file = new RepositoryFile(repository, "file ", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);
		}
		[ExpectedException(typeof(ArgumentException))]
		[Test] public void FileNameWithLeadingSpaces()
		{
			RepositoryFile file = new RepositoryFile(repository, "trunk/ file", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);
		}
		[ExpectedException(typeof(ArgumentException))]
		[Test] public void FileNameUnderRootWithLeadingSpaces()
		{
			RepositoryFile file = new RepositoryFile(repository, " file", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);
		}
		

		[Test] public void ExtensionNonExistant()
		{
			RepositoryFile file = new RepositoryFile(repository, "file", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Assert.AreEqual(String.Empty, file.Extension, "RepositoryFile.Extension is not String.Empty when file does not have an extension");
		}
		[Test] public void ExtensionNonExistantWithTrailingDot()
		{
			RepositoryFile file = new RepositoryFile(repository, "file.", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Assert.AreEqual(String.Empty, file.Extension, "RepositoryFile.Extension is not String.Empty when file does not have an extension");
		}
		[Test] public void Extension()
		{
			RepositoryFile file = new RepositoryFile(repository, "file.extension", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Assert.AreEqual("extension", file.Extension, "RepositoryFile.Extension is not set properly");
		}

		/// <summary>
		/// This test is to counter a bug in SetPathRelatedFields
		/// in RepositoryFile, it used path.IndexOf rather than
		/// filename when gathering the extension, which broke
		/// on real paths.
		/// </summary>
		[Test] public void LongPathWithExtension()
		{
			RepositoryFile file = new RepositoryFile(repository, "trunk/Castle/File.cs", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Assert.AreEqual("cs", file.Extension, "RepositoryFile.Extension is not set properly");
		}


		[Test] public void FileName()
		{
			RepositoryFile file = new RepositoryFile(repository, "file", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Assert.AreEqual("file", file.FileName, "RepositoryFile.FileName is not set correctly");
		}

		[Test] public void FileNameWithTrailingDot()
		{
			RepositoryFile file = new RepositoryFile(repository, "file.", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Assert.AreEqual("file.", file.FileName, "RepositoryFile.FileName is not set correctly");
		}

		[Test] public void FileNameWithExtension()
		{
			RepositoryFile file = new RepositoryFile(repository, "file.ext", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Assert.AreEqual("file.ext", file.FileName, "RepositoryFile.FileName is not set correctly");
		}

		[Test] public void FileNameUnderPath()
		{
			RepositoryFile file = new RepositoryFile(repository, "trunk/Castle/file.txt", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Assert.AreEqual("file.txt", file.FileName, "RepositoryFile.FileName is not set correctly");
		}
		[Test] public void FileNameUnderRoot()
		{
			RepositoryFile file = new RepositoryFile(repository, "/file.txt", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Assert.AreEqual("file.txt", file.FileName, "RepositoryFile.FileName is not set correctly");
		}


		[Test] public new void ToString()
		{
			RepositoryFile file = new RepositoryFile(repository, "trunk/Castle/file.txt", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Assert.AreEqual("trunk/Castle/file.txt", file.ToString(), "RepositoryFile.ToString does not return the path");
		}
	}
}
