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

namespace Castle.MonoRail.Framework.Tests
{
	using NUnit.Framework;
	
	using Castle.MonoRail.Framework;

	[TestFixture]
	public class FlashTestCase
	{
		[Test]
		public void SimpleTest()
		{
			Flash flash = new Flash();

			flash["test"] = "hello";

			flash.Sweep();

			Assert.IsTrue( flash.ContainsKey("test") );

			flash = new Flash(flash);

			Assert.IsTrue( flash.ContainsKey("test") );
		}

		[Test]
		public void FlashNow()
		{
			Flash flash = new Flash();

			flash.Now("test","hello");

			Assert.IsTrue( flash.ContainsKey("test") );

			flash.Sweep();

			Assert.IsFalse( flash.ContainsKey("test") );
		}

		[Test]
		public void FlashKeep()
		{
			Flash flash = new Flash();

			flash.Now("test1","hello");
			flash.Now("test2","hello");

			flash.Keep("test1");

			flash.Sweep();

			Assert.IsTrue( flash.ContainsKey("test1") );
			Assert.IsFalse( flash.ContainsKey("test2") );

			flash = new Flash(flash);
			flash.Sweep();

			Assert.IsTrue( flash.Count == 0 );

			flash.Now("test1","hello");
			flash.Now("test2","hello");

			flash.Keep();

			flash.Sweep();

			Assert.IsTrue( flash.ContainsKey("test1") );
			Assert.IsTrue( flash.ContainsKey("test2") );
		}

		[Test]
		public void FlashDiscard()
		{
			Flash flash = new Flash();

			flash.Add("test1","hello");
			flash.Add("test2","hello");

			flash.Discard("test2");

			flash.Sweep();

			Assert.IsTrue( flash.ContainsKey("test1") );
			Assert.IsFalse( flash.ContainsKey("test2") );

			flash = new Flash(flash);
			flash.Sweep();

			Assert.IsTrue( flash.Count == 0 );

			flash.Add("test1","hello");
			flash.Add("test2","hello");

			flash.Discard();

			flash = new Flash(flash);
			flash.Sweep();

			Assert.IsFalse( flash.ContainsKey("test1") );
			Assert.IsFalse( flash.ContainsKey("test2") );
		}
	}
}
