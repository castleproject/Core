// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Tests
{
	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Model;

	[TestFixture]
	public class LazyTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void CanSaveAndLoadLazyEntityOutsideOfScope()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(),
				   typeof(BlogLazy), typeof(PostLazy));
			Recreate();
			BlogLazy blog = new BlogLazy();
			blog.Save();
			PostLazy post = new PostLazy(blog, "a", "b", "c");
			post.Save();

			PostLazy postFromDb = PostLazy.Find(post.Id);
			Assert.AreEqual("a", postFromDb.Title);
			
		}

		[Test]
		public void CanSaveAndLoadLazy()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(),typeof(VeryLazyObject));
			Recreate();
			VeryLazyObject lazy = new VeryLazyObject();
			lazy.Title = "test";

			ActiveRecordMediator.Save(lazy);

			VeryLazyObject lazyFromDb = (VeryLazyObject)ActiveRecordMediator.FindByPrimaryKey(typeof(VeryLazyObject),lazy.Id);
			Assert.AreEqual("test", lazyFromDb.Title);

			lazyFromDb.Title = "test for update";
			ActiveRecordMediator.Update(lazyFromDb);

			lazyFromDb = (VeryLazyObject)ActiveRecordMediator.FindByPrimaryKey(typeof(VeryLazyObject), lazy.Id);
			Assert.AreEqual("test for update", lazyFromDb.Title);
		}
	}
}
