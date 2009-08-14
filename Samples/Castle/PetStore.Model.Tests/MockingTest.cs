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

namespace PetStore.Model.Tests
{
	using System.Collections.Generic;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class MockingTest
	{
		[Test]
		public void CanMockUserAccess()
		{
			var dao = MockRepository.GenerateMock<IDao<User>>();
			var savedUsers = new List<User>();
			
			dao
				.Expect(d => d.Save(null))
				.IgnoreArguments()
				.Repeat.AtLeastOnce();
			Storage<User>.RegisterDao(dao);

			(new User()).Save();

			dao.VerifyAllExpectations();
		}
	}
}