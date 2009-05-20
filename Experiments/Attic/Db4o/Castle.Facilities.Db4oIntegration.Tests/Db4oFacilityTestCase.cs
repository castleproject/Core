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


namespace Castle.Facilities.Db4oIntegration.Tests
{
	using System;
	using Db4objects.Db4o;
	using NUnit.Framework;

	using Castle.Facilities.Db4oIntegration.Tests.Components;
	
	[TestFixture]
	public class Db4oFacilityTestCase : AbstractDb4oTestCase
	{
		[Test]
		public void Usage()
		{
			IObjectContainer db4oContainer = (IObjectContainer) Container["db4o.container"];

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

			IObjectSet results = db4oContainer.Get(typeof(Beer));

			Assert.AreEqual(1, results.Count);
			Beer loaded = (Beer)results[0];
			Assert.AreEqual(id, loaded.Id);

			db4oContainer.Delete(loaded);
			db4oContainer.Commit();

			results = db4oContainer.Get(typeof(Beer));
			Assert.AreEqual(0, results.Count);
		}
	}
}
