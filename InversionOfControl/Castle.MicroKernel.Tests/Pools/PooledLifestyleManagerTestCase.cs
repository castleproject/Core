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

namespace Castle.MicroKernel.Tests.Pools
{
	using System;
	using System.Collections;

	using NUnit.Framework;

	[TestFixture]
	public class PooledLifestyleManagerTestCase
	{
		[Test]
		public void SimpleUsage()
		{
			IKernel kernel = new DefaultKernel();
			kernel.AddComponent( "a", typeof(PoolableComponent1) );

			PoolableComponent1 inst1 = kernel["a"] as PoolableComponent1;
			PoolableComponent1 inst2 = kernel["a"] as PoolableComponent1;

			Assert.IsNotNull(inst1);
			Assert.IsNotNull(inst2);

			kernel.ReleaseComponent(inst2);
			kernel.ReleaseComponent(inst1);

			PoolableComponent1 other1 = kernel["a"] as PoolableComponent1;
			PoolableComponent1 other2 = kernel["a"] as PoolableComponent1;

			Assert.IsNotNull(other1);
			Assert.IsNotNull(other2);

			Assert.AreSame( inst1, other1 );
			Assert.AreSame( inst2, other2 );

			kernel.ReleaseComponent(inst2);
			kernel.ReleaseComponent(inst1);
		}

		[Test]
		public void MaxSize()
		{
			IKernel kernel = new DefaultKernel();
			kernel.AddComponent( "a", typeof(PoolableComponent1) );

			ArrayList instances = new ArrayList();

			instances.Add( kernel["a"] as PoolableComponent1 );
			instances.Add( kernel["a"] as PoolableComponent1 );
			instances.Add( kernel["a"] as PoolableComponent1 );
			instances.Add( kernel["a"] as PoolableComponent1 );
			instances.Add( kernel["a"] as PoolableComponent1 );

			PoolableComponent1 other1 = kernel["a"] as PoolableComponent1;

			Assert.IsNotNull( other1 );
			Assert.IsTrue( !instances.Contains(other1) );

			kernel.ReleaseComponent(other1);

			PoolableComponent1 other2 = kernel["a"] as PoolableComponent1;
			Assert.IsNotNull( other2 );
			Assert.IsTrue( other1 != other2 );
			Assert.IsTrue( !instances.Contains(other2) );

			kernel.ReleaseComponent(other2);
		}
	}
}
