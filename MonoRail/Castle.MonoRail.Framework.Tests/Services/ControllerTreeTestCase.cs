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
	using System;
	using Castle.MonoRail.Framework.Services;
	using Castle.MonoRail.Framework.Tests.Controllers.Clients;
	using Castle.MonoRail.Framework.Tests.Controllers.Products;
	using Controllers;
	using NUnit.Framework;

	[TestFixture]
	public class ControllerTreeTestCase
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void InvalidConstruction()
		{
			new DefaultControllerTree(null);
		}

		[Test]
		public void EmptyArea()
		{
			DefaultControllerTree tree = new DefaultControllerTree();
			tree.AddController("", "home", typeof(HomeController));
			tree.AddController("", "contact", typeof(ContactController));
			tree.AddController("", "cart", typeof(CartController));

			Assert.AreEqual( typeof(HomeController), tree.GetController("", "home") );
			Assert.AreEqual( typeof(ContactController), tree.GetController("", "contact") );
			Assert.AreEqual( typeof(CartController), tree.GetController("", "cart") );
		}

		[Test]
		public void FewAreas()
		{
			DefaultControllerTree tree = new DefaultControllerTree();

			tree.AddController("", "home", typeof(HomeController));
			tree.AddController("", "contact", typeof(ContactController));
			tree.AddController("", "cart", typeof(CartController));
			tree.AddController("clients", "home", typeof(ClientHomeController));
			tree.AddController("clients", "contact", typeof(ClientContactController));
			tree.AddController("clients", "cart", typeof(ClientCartController));
			tree.AddController("lists", "home", typeof(ListController));

			Assert.AreEqual( typeof(HomeController), tree.GetController("", "home") );
			Assert.AreEqual( typeof(ContactController), tree.GetController("", "contact") );
			Assert.AreEqual( typeof(CartController), tree.GetController("", "cart") );

			Assert.AreEqual( typeof(ClientHomeController), tree.GetController("clients", "home") );
			Assert.AreEqual( typeof(ClientContactController), tree.GetController("clients", "contact") );
			Assert.AreEqual( typeof(ClientCartController), tree.GetController("clients", "cart") );

			Assert.AreEqual( typeof(ListController), tree.GetController("lists", "home") );
		}

		[Test]
		public void AddingController_RaisesNotifcationEvent() 
		{
			DefaultControllerTree tree = new DefaultControllerTree();
			bool eventRaised = false;
			tree.ControllerAdded += delegate { eventRaised = true; };
			tree.AddController("clients","home",typeof(ClientHomeController));
			Assert.IsTrue(eventRaised);
		}
	}
}