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

namespace Castle.MonoRail.ActiveRecordScaffold.Tests
{
	using System;
	using System.IO;

	using NUnit.Framework;

	using Castle.MonoRail.Engine.Tests;


	[TestFixture]
	public class BlogTestCase : AbstractMRTestCase
	{
		[Test]
		public void ListBlog()
		{
			TestDBUtils.Recreate();
			
			string url = "/blogs/listblog.rails";
			FileStream expFS = File.OpenRead("../Castle.MonoRail.ActiveRecordScaffold.Tests/bloglist.txt");
			byte[] barray = new byte[expFS.Length];
			expFS.Read( barray, 0, barray.Length );

			// Execute(url, System.Text.ASCIIEncoding.Default.GetString(barray) );
		}

		[Test]
		public void NewBlog()
		{
			TestDBUtils.Recreate();
			
			string url = "/blogs/newblog.rails";
			FileStream expFS = File.OpenRead("../Castle.MonoRail.ActiveRecordScaffold.Tests/blognew.txt");
			byte[] barray = new byte[expFS.Length];
			expFS.Read( barray, 0, barray.Length );

			// Execute(url, System.Text.ASCIIEncoding.Default.GetString(barray) );
		}

		[Test]
		public void EditBlog()
		{
			TestDBUtils.Recreate();
			
			string url = "/blogs/editblog.rails?id=1";
			FileStream expFS = File.OpenRead("../Castle.MonoRail.ActiveRecordScaffold.Tests/blogedit.txt");
			byte[] barray = new byte[expFS.Length];
			expFS.Read( barray, 0, barray.Length );

			// Execute(url, System.Text.ASCIIEncoding.Default.GetString(barray) );
		}

		[Test]
		public void RemoveBlog()
		{
			TestDBUtils.Recreate();
			
			string url = "/blogs/confirmblog.rails?id=1";
			FileStream expFS = File.OpenRead("../Castle.MonoRail.ActiveRecordScaffold.Tests/blogconfirmremoval.txt");
			byte[] barray = new byte[expFS.Length];
			expFS.Read( barray, 0, barray.Length );

			// Execute(url, System.Text.ASCIIEncoding.Default.GetString(barray) );
		}
	}
}
