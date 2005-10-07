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

namespace Castle.MonoRail.Framework.Tests
{
	using System;
	using System.Reflection;

	using NUnit.Framework;

	using Castle.MonoRail.Framework.Internal;

	[TestFixture]
	public class DefaultControllerFactoryTestCase
	{
		readonly String extension = "rails";

		DefaultControllerFactory factory;

		[TestFixtureSetUp]
		public void Init()
		{
			factory = new DefaultControllerFactory();
			factory.Inspect( Assembly.GetExecutingAssembly() );
		}

		[Test]
		public void EmptyArea()
		{			
			Controller controller = factory.CreateController( new UrlInfo("", "", "home", "", extension) );

			Assert.IsNotNull( controller );
			Assert.AreEqual("Castle.MonoRail.Framework.Tests.Controllers.HomeController", 
				controller.GetType().FullName );
		}

		[Test]
		public void OneLevelArea()
		{
			Controller controller = factory.CreateController( new UrlInfo("", "clients", "home", "", extension) );

			Assert.IsNotNull( controller );
			Assert.AreEqual("Castle.MonoRail.Framework.Tests.Controllers.Clients.HomeController", 
				controller.GetType().FullName );

			controller = factory.CreateController( new UrlInfo("", "clients", "hire-us", "", extension) );

			Assert.IsNotNull( controller );
			Assert.AreEqual("Castle.MonoRail.Framework.Tests.Controllers.Clients.OtherController", 
				controller.GetType().FullName );

			controller = factory.CreateController( new UrlInfo("", "ourproducts", "shoppingcart", "", extension) );

			Assert.IsNotNull( controller );
			Assert.AreEqual("Castle.MonoRail.Framework.Tests.Controllers.Products.CartController", 
				controller.GetType().FullName );

			controller = factory.CreateController( new UrlInfo("", "ourproducts", "lista", "", extension) );

			Assert.IsNotNull( controller );
			Assert.AreEqual("Castle.MonoRail.Framework.Tests.Controllers.Products.ListController", 
				controller.GetType().FullName );
		}
	}
}
