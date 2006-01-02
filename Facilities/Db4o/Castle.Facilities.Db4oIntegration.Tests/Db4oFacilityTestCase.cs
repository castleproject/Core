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


namespace Castle.Facilities.Db4oIntegration.Tests
{
	using System;

	using NUnit.Framework;

	using com.db4o;

	using Castle.Facilities.Db4oIntegration.Tests.Components;
	
	[TestFixture]
	public class Db4oFacilityTestCase : AbstractDb4oTestCase
	{
		public Db4oFacilityTestCase()
		{
		}

		[Test]
		public void Usage()
		{
			ObjectContainer db4oContainer = (ObjectContainer) Container["db4o.container"];

			Assert.IsNotNull(db4oContainer);

			Guid id = Guid.NewGuid();

			try
			{
				db4oContainer.Set(new Beer(id));
			}
			catch
			{
				db4oContainer.Rollback();
			}
			finally
			{
				db4oContainer.Commit();
			}

			ObjectSet results = db4oContainer.Get(typeof(Beer));

			Assert.AreEqual(1, results.Size());
			Beer loaded = (Beer)results.Next();
			Assert.AreEqual(id, loaded.Id);

			db4oContainer.Delete(loaded);
			db4oContainer.Commit();

			results = db4oContainer.Get(typeof(Beer));
			Assert.AreEqual(0, results.Size());
		}
	}
}
