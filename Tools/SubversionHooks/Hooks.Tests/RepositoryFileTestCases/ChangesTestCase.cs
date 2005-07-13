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

	/// <summary>
	/// Summary description for ChangesTestCase.
	/// </summary>
	[TestFixture] public class ChangesTestCase
	{

		private IRepository repository;
	
		#region Setup / Teardown

		[TestFixtureSetUp] public void FixtureSetUp()
		{
			repository = new MockRepository();
		}

		[SetUp] public void SetUp()
		{
			// TODO: Add setup code
		}

		[TearDown] public void TearDown()
		{
			// TODO: Add teardown code
		}

		[TestFixtureTearDown] public void FixtureTearDown()
		{
			// TODO: Add fixture teardown code
		}
				
		#endregion

		[Test] public void ContentsUpdated()
		{
			RepositoryFile file = new RepositoryFile(repository, "file", 
				RepositoryStatus.Updated, RepositoryStatus.Unchanged);

			Assert.AreEqual(RepositoryStatus.Updated, file.ContentsStatus);
			Assert.AreEqual(RepositoryStatus.Unchanged, file.PropertiesStatus);
		}

		[Test] public void ContentsAdded()
		{
			RepositoryFile file = new RepositoryFile(repository, "file", 
				RepositoryStatus.Added, RepositoryStatus.Unchanged);

			Assert.AreEqual(RepositoryStatus.Added, file.ContentsStatus);
			Assert.AreEqual(RepositoryStatus.Unchanged, file.PropertiesStatus);
		}

		[Test] public void ContentsDeleted()
		{
			RepositoryFile file = new RepositoryFile(repository, "file", 
				RepositoryStatus.Deleted, RepositoryStatus.Unchanged);

			Assert.AreEqual(RepositoryStatus.Deleted, file.ContentsStatus);
			Assert.AreEqual(RepositoryStatus.Unchanged, file.PropertiesStatus);
		}

		[Test] public void ContentsUnchanged()
		{
			RepositoryFile file = new RepositoryFile(repository, "file", 
				RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);

			Assert.AreEqual(RepositoryStatus.Unchanged, file.ContentsStatus);
			Assert.AreEqual(RepositoryStatus.Unchanged, file.PropertiesStatus);
		}


		[Test] public void ContentsUpdatedPropertiesUpdated()
		{
			RepositoryFile file = new RepositoryFile(repository, "file", 
				RepositoryStatus.Updated, RepositoryStatus.Updated);

			Assert.AreEqual(RepositoryStatus.Updated, file.ContentsStatus);
			Assert.AreEqual(RepositoryStatus.Updated, file.PropertiesStatus);
		}

		[Test] public void ContentsAddedPropertiesUpdated()
		{
			RepositoryFile file = new RepositoryFile(repository, "file", 
				RepositoryStatus.Added, RepositoryStatus.Updated);

			Assert.AreEqual(RepositoryStatus.Added, file.ContentsStatus);
			Assert.AreEqual(RepositoryStatus.Updated, file.PropertiesStatus);
		}

		[Test] public void ContentsDeletedPropertiesUpdated()
		{
			RepositoryFile file = new RepositoryFile(repository, "file", 
				RepositoryStatus.Deleted, RepositoryStatus.Updated);

			Assert.AreEqual(RepositoryStatus.Deleted, file.ContentsStatus);
			Assert.AreEqual(RepositoryStatus.Updated, file.PropertiesStatus);
		}

		[Test] public void ContentsUnchangedPropertiesUpdated()
		{
			RepositoryFile file = new RepositoryFile(repository, "file", 
				RepositoryStatus.Unchanged, RepositoryStatus.Updated);

			Assert.AreEqual(RepositoryStatus.Unchanged, file.ContentsStatus);
			Assert.AreEqual(RepositoryStatus.Updated, file.PropertiesStatus);
		}


		[ExpectedException(typeof(ArgumentException))]
		[Test] public void PropertiesAdded()
		{
			RepositoryFile file = new RepositoryFile(repository, "file", 
				RepositoryStatus.Unchanged, RepositoryStatus.Added);
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test] public void PropertiesDeleted()
		{
			RepositoryFile file = new RepositoryFile(repository, "file", 
				RepositoryStatus.Unchanged, RepositoryStatus.Added);
		}


	}
}
