// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.Services
{
	using System.Reflection;
	using Castle.MonoRail.Framework.Services;
	using NUnit.Framework;


	[TestFixture]
	public class DefaultControllerFactoryTestCase
	{
		private DefaultControllerFactory factory;

		[TestFixtureSetUp]
		public void Init()
		{
			factory = new DefaultControllerFactory();
			factory.Service(new TestServiceContainer());
			factory.Inspect(Assembly.GetExecutingAssembly());
		}

		[Test]
		public void EmptyArea()
		{
			IController controller = factory.CreateController("", "home");

			Assert.IsNotNull(controller);
			Assert.AreEqual("Castle.MonoRail.Framework.Tests.Controllers.HomeController",
			                controller.GetType().FullName);
		}

		[Test]
		public void OneLevelArea()
		{
			IController controller = factory.CreateController("clients", "home");

			Assert.IsNotNull(controller);
			Assert.AreEqual("Castle.MonoRail.Framework.Tests.Controllers.Clients.ClientHomeController",
			                controller.GetType().FullName);

			controller = factory.CreateController("clients", "hire-us");

			Assert.IsNotNull(controller);
			Assert.AreEqual("Castle.MonoRail.Framework.Tests.Controllers.Clients.OtherController",
			                controller.GetType().FullName);

			controller = factory.CreateController("ourproducts", "shoppingcart");

			Assert.IsNotNull(controller);
			Assert.AreEqual("Castle.MonoRail.Framework.Tests.Controllers.Products.CartController",
			                controller.GetType().FullName);

			controller = factory.CreateController("ourproducts", "lista");

			Assert.IsNotNull(controller);
			Assert.AreEqual("Castle.MonoRail.Framework.Tests.Controllers.Products.ListController",
			                controller.GetType().FullName);
		}
	}
}