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
	
	using NUnit.Framework;

	using Castle.SvnHooks;	

	/// <summary>
	/// Summary description for MimeTestCase.
	/// </summary>
	[TestFixture] public class MimeTestCase
	{

		private const String SVN_MIME_TYPE = "svn:mime-type";

		private const String NO_MIME_PATH = "nomime";
		private const String CSHARP_PATH = "cs";
		private const String BINARY_PATH = "bin";

		private const String TEXT_PATH = "text";
		private const String TEXTING_PATH = "texting";
		private const String TEXT_SLASH_PATH = "textslash";

		private IRepository repository;
	
		#region Setup / Teardown

		[TestFixtureSetUp] public void FixtureSetUp()
		{
			MockRepository mockRepository = new MockRepository();
			mockRepository.AddFile(NO_MIME_PATH, null, null, null);
			mockRepository.AddFile(CSHARP_PATH, null, MimeType("text/cs"), null);
			mockRepository.AddFile(BINARY_PATH, null, MimeType("application/octet-stream"), null);
			mockRepository.AddFile(TEXT_PATH, null, MimeType("text"), null);
			mockRepository.AddFile(TEXTING_PATH, null, MimeType("texting"), null);
			mockRepository.AddFile(TEXT_SLASH_PATH, null, MimeType("text/"), null);

			repository = mockRepository;
		}


		private static IDictionary MimeType(String mimeType)
		{
			IDictionary dictionary = new Hashtable();
			dictionary.Add(SVN_MIME_TYPE, new String[]{mimeType});

			return dictionary;
		}
				

		#endregion

		[Test] public void NoMimeTypeSet()
		{
			RepositoryFile file = new RepositoryFile(repository, NO_MIME_PATH, RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Assert.AreEqual("text/plain", file.MimeType, "RepositoryFile.MimeType does not return \"text/plain\" when svn:mime-type is not set");
			Assert.IsTrue(file.IsText, "RepositoryFile.IsText does not return true when svn:mime-type is not set");

		}

		[Test] public void CSharpMimeSet()
		{
			RepositoryFile file = new RepositoryFile(repository, CSHARP_PATH, RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Assert.AreEqual("text/cs", file.MimeType, "RepositoryFile.MimeType does not return the svn:mime-type");
			Assert.IsTrue(file.IsText, "RepositoryFile.IsText does not return true when svn:mime-type starts with \"text/\"");

		}
		
		[Test] public void BinaryMimeSet()
		{
			RepositoryFile file = new RepositoryFile(repository, BINARY_PATH, RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Assert.AreEqual("application/octet-stream", file.MimeType, "RepositoryFile.MimeType does not return the svn:mime-type");
			Assert.IsFalse(file.IsText, "RepositoryFile.IsText does not return false when svn:mime-type does NOT start with \"text/\"");

		}


		/// <summary>
		/// This test ensures that a mime type of 
		/// "text" does not register as a text file;
		/// Subversion only considers a file a text
		/// file if it begins with "text/".
		/// </summary>
		[Test] public void TextMimeSet()
		{
			RepositoryFile file = new RepositoryFile(repository, TEXT_PATH, RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Assert.IsFalse(file.IsText, "RepositoryFile.IsText does not return false when svn:mime-type is set to only \"text\"");
		
		}
		/// <summary>
		/// This test ensures that a mime type beginning
		/// with "text" but not having a slash after text
		/// does not register as a text file;
		/// Subversion only considers a file a text
		/// file if it begins with "text/".
		/// </summary>
		[Test] public void TextingMimeSet()
		{
			RepositoryFile file = new RepositoryFile(repository, TEXTING_PATH, RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Assert.IsFalse(file.IsText, "RepositoryFile.IsText does not return false when svn:mime-type begins with only \"text\"");

		}
		[Test] public void TextSlashMimeSet()
		{
			RepositoryFile file = new RepositoryFile(repository, TEXT_SLASH_PATH, RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Assert.IsTrue(file.IsText, "RepositoryFile.IsText does not return true when svn:mime-type is set to only \"text/\"");
		}
	}
}
