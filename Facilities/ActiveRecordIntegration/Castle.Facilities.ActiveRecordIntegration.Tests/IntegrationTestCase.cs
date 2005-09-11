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

namespace Castle.Facilities.ActiveRecordIntegration.Tests
{
	using NUnit.Framework;

	using Castle.Facilities.ActiveRecordIntegration.Tests.Model;


	[TestFixture]
	public class IntegrationTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void SimpleUsage()
		{
			Post.DeleteAll();
			Blog.DeleteAll();

			Blog[] blogs = Blog.FindAll();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 0, blogs.Length );

			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

			blogs = Blog.FindAll();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 1, blogs.Length );

			Blog retrieved = blogs[0];
			Assert.IsNotNull( retrieved );

			Assert.AreEqual( blog.Name, retrieved.Name );
			Assert.AreEqual( blog.Author, retrieved.Author );
		}

		[Test]
		public void ComponentAutoWiring()
		{
			WiringSession wsession = (WiringSession) container["wiring.service"];

			wsession.UsingISessionFactory();

			wsession.UsingISessionFactoryHolder();
		}
	}
}
