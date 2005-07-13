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
	/// This test case ensures that the RepositoryFile
	/// does not call the IRepository functions excessively,
	/// this is to allow for speedier processing of the
	/// hooks since unnecessary calls will not be made.
	/// </summary>
	[TestFixture] public class PerformanceTestCase : IRepository
	{

		private RepositoryFile file;
		private int count;
	
		#region Setup / Teardown

		[SetUp] public void SetUp()
		{
			count = 0; // this line has to be before the RepositoryFile line
			file = new RepositoryFile(this, "file", RepositoryStatus.Unchanged, RepositoryStatus.Unchanged);
		}
				

		#endregion

		#region IRepository Members

		public string[] GetProperty(string path, string property)
		{
			count++;
			return new String[]{"property"};
		}

		public System.IO.Stream GetFileContents(string path)
		{
			count++;
			return null;
		}


		#endregion

		/// <summary>
		/// This test simply ensures that the
		/// constructor for RepositoryFile never
		/// calls the IRepository, the information
		/// should be collected on-demand, not on
		/// initialization.
		/// </summary>
		[Test] public void Constructor()
		{
			Assert.AreEqual(0, count, "RespositoryFile's constructor calls the repository");
		}

		[Test] public void MimeType()
		{
			String ether = file.MimeType;
			ether = file.MimeType;

			Assert.AreEqual(1, count, "RepositoryFile.MimeType called the repository more than once");
		}

		[Test] public void IsText()
		{
			bool ether = file.IsText;
			ether = file.IsText;

			Assert.AreEqual(1, count, "RepositoryFile.IsText called the repository more than once");
		}
	}
}
