// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Validator.Tests
{
	using Castle.Components.Validator.Tests.Models;
	using NUnit.Framework;

	[TestFixture]
	public class CachedValidationRegistryTestCase
	{
		private CachedValidationRegistry registry;

		[SetUp]
		public void Init()
		{
			registry = new CachedValidationRegistry();
		}

		[Test]
		public void RunWhenEverytimeTest()
		{
			IValidator[] validators = registry.GetValidators(typeof(Client), RunWhen.Everytime);
			
			Assert.IsNotNull(validators);
			Assert.AreEqual(6, validators.Length);

			foreach(IValidator val in validators)
			{
				Assert.IsTrue((val.RunWhen & RunWhen.Everytime) != 0);
			}
		}

		[Test]
		public void RunWhenCustomTest()
		{
			IValidator[] validators = registry.GetValidators(typeof(Client), RunWhen.Custom);

			Assert.IsNotNull(validators);
			Assert.AreEqual(7, validators.Length); // RunWhen.Everytime is returned too

			foreach(IValidator val in validators)
			{
				Assert.IsTrue((val.RunWhen & RunWhen.Custom) != 0 || (val.RunWhen & RunWhen.Everytime) != 0);
			}
		}
	}
}
