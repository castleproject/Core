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

namespace Castle.MicroKernel.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.Model;

	using Castle.MicroKernel.SubSystems.Naming;

	using Castle.MicroKernel.Tests.ClassComponents;

	[TestFixture]
	public class NamingPartsSubSystemTestCase
	{
		[Test]
		public void ComponentQuery()
		{
			IKernel kernel = new DefaultKernel();
			kernel.AddSubSystem( SubSystemConstants.NamingKey, new NamingPartsSubSystem() );

			kernel.AddComponent( "common:key1=true", typeof(ICommon), typeof(CommonImpl1) );
			kernel.AddComponent( "common:secure=true", typeof(ICommon), typeof(CommonImpl2) );

			ICommon common = kernel["common"] as ICommon;

			Assert.IsNotNull( common );
			Assert.AreEqual( typeof(CommonImpl1), common.GetType() );

			common = kernel["common:key1=true"] as ICommon;

			Assert.IsNotNull( common );
			Assert.AreEqual( typeof(CommonImpl1), common.GetType() );

			common = kernel["common:secure=true"] as ICommon;

			Assert.IsNotNull( common );
			Assert.AreEqual( typeof(CommonImpl2), common.GetType() );
		}

		[Test]
		public void ComponentGraph()
		{
			IKernel kernel = new DefaultKernel();
			kernel.AddSubSystem( SubSystemConstants.NamingKey, new NamingPartsSubSystem() );

			kernel.AddComponent( "common:key1=true", typeof(ICommon), typeof(CommonImpl1) );
			kernel.AddComponent( "common:secure=true", typeof(ICommon), typeof(CommonImpl2) );

			GraphNode[] nodes = kernel.GraphNodes;
			Assert.IsNotNull( nodes );
			Assert.AreEqual( 2, nodes.Length );
		}

		[Test]
		public void ServiceLookup()
		{
			IKernel kernel = new DefaultKernel();
			kernel.AddSubSystem( SubSystemConstants.NamingKey, new NamingPartsSubSystem() );

			kernel.AddComponent( "common:key1=true", typeof(ICommon), typeof(CommonImpl1) );
			kernel.AddComponent( "common:secure=true", typeof(ICommon), typeof(CommonImpl2) );

			ICommon common = kernel[ typeof(ICommon) ] as ICommon;

			Assert.IsNotNull( common );
			Assert.AreEqual( typeof(CommonImpl1), common.GetType() );
		}

		[Test]
		public void GetAssignableHandlers()
		{
			IKernel kernel = new DefaultKernel();
			kernel.AddSubSystem( SubSystemConstants.NamingKey, new NamingPartsSubSystem() );

			kernel.AddComponent( "common:key1=true", typeof(ICommon), typeof(CommonImpl1) );
			kernel.AddComponent( "common:secure=true", typeof(ICommon), typeof(CommonImpl2) );

			IHandler[] handlers = kernel.GetAssignableHandlers( typeof(ICommon) );

			Assert.IsNotNull( handlers );
			Assert.AreEqual( 2, handlers.Length );
		}
	}
}
