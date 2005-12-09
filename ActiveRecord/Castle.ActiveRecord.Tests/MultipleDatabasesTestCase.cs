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

namespace Castle.ActiveRecord.Tests
{
	using Castle.ActiveRecord.Tests.Model;
	
	using NUnit.Framework;

	[TestFixture]
	public class MultipleDatabasesTestCase : AbstractActiveRecordTest
	{
		[SetUp]
		public void Setup()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), 
				typeof(Blog), typeof(Post), typeof(Hand), typeof(Test2ARBase) );
			Recreate();
		}

		[Test]
		public void OperateOne()
		{
			Blog[] blogs = Blog.FindAll();

			Assert.AreEqual(0, blogs.Length);

			CreateBlog();

			blogs = Blog.FindAll();
			Assert.AreEqual(1, blogs.Length);
		}

		private static void CreateBlog()
		{
			Blog blog = new Blog();
	
			blog.Author = "Henry";
			blog.Name = "Senseless";
            blog.Save();
		}

		[Test]
		public void OperateTheOtherOne()
		{
			Hand[] hands = Hand.FindAll();

			Assert.AreEqual(0, hands.Length);

			CreateHand();

			hands = Hand.FindAll();

			Assert.AreEqual(1, hands.Length);
		}

		private static void CreateHand()
		{
			Hand hand = new Hand();
	
			hand.Side = "Right";
	
			hand.Save();
		}

		[Test]
		public void OperateBoth()
		{
			Blog[] blogs = Blog.FindAll();
			Hand[] hands = Hand.FindAll();

			Assert.AreEqual(0, blogs.Length);
			Assert.AreEqual(0, hands.Length);

			CreateBlog();
			CreateHand();

			blogs = Blog.FindAll();
			hands = Hand.FindAll();

			Assert.AreEqual(1, blogs.Length);
			Assert.AreEqual(1, hands.Length);
		}
	}
}
