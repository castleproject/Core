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

namespace Castle.SvnHooks.Tests
{
	using System;
	
	using NUnit.Framework;

	/// <summary>
	/// Summary description for ErrorTestCase.
	/// </summary>
	[TestFixture] public class ErrorTestCase : IRepository
	{
	
		private RepositoryFile File1;
		private RepositoryFile File2;

		#region Setup / Teardown

		[TestFixtureSetUp] public void FixtureSetUp()
		{
			File1 = new RepositoryFile(this, "nowhere", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);
			File2 = new RepositoryFile(this, "here", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);
		}


		#endregion
		#region IRepository Members

		public string[] GetProperty(string path, string property)
		{
			// TODO:  Add ErrorTestCase.GetProperty implementation
			return null;
		}

		public System.IO.Stream GetFileContents(string path)
		{
			// TODO:  Add ErrorTestCase.GetFileContents implementation
			return null;
		}


		#endregion

		[Test] public void Equals()
		{
			Error E1 = new Error(File1, "Error");
			Error E2 = new Error(File1, "Error");

			Assert.IsTrue(E1.Equals(E2), "Error.Equals does not return true for identical errors");
		}

		[Test] public void EqualsWithDifferentMessages()
		{
			Error E1 = new Error(File1, "Error");
			Error E2 = new Error(File1, "Warning");

			Assert.IsFalse(E1.Equals(E2), "Error.Equals does not return false for errors with differing descriptions");
		}
		[Test] public void EqualsWithDifferentFiles()
		{
			Error E1 = new Error(File1, "Error");
			Error E2 = new Error(File2, "Error");

			Assert.IsFalse(E1.Equals(E2), "Error.Equals does not return false for errors with differing files");
		}
	}
}
