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

namespace Castle.CastleOnRails.Framework.Tests
{
	using System;
	using System.Reflection;

	using NUnit.Framework;

	using Castle.CastleOnRails.Framework.Internal;

	/// <summary>
	/// Inspect this assembly registering 
	/// the controllers.
	/// </summary>
	[TestFixture]
	public class DefaultControllerFactoryTestCase
	{
		[Test]
		public void ControllerHierarchy()
		{
			string extension = "rails";

			DefaultControllerFactory factory = new DefaultControllerFactory();
			factory.Inspect( Assembly.GetExecutingAssembly() );
			
			Controller controller = factory.GetController( new UrlInfo("", "", "home", "", extension) );

			Assert.IsNotNull( controller );
			Assert.AreEqual( 
				"Castle.CastleOnRails.Framework.Tests.Controllers.HomeController", 
				controller.GetType().FullName );

			controller = factory.GetController( new UrlInfo("", "clients", "home", "", extension) );

			Assert.IsNotNull( controller );
			Assert.AreEqual( 
				"Castle.CastleOnRails.Framework.Tests.Controllers.Clients.HomeController", 
				controller.GetType().FullName );

			controller = factory.GetController( new UrlInfo("", "clients", "hire-us", "", extension) );

			Assert.IsNotNull( controller );
			Assert.AreEqual( 
				"Castle.CastleOnRails.Framework.Tests.Controllers.Clients.OtherController", 
				controller.GetType().FullName );

			controller = factory.GetController( new UrlInfo("", "ourproducts", "shoppingcart", "", extension) );

			Assert.IsNotNull( controller );
			Assert.AreEqual( 
				"Castle.CastleOnRails.Framework.Tests.Controllers.Products.CartController", 
				controller.GetType().FullName );

			controller = factory.GetController( new UrlInfo("", "ourproducts", "lista", "", extension) );

			Assert.IsNotNull( controller );
			Assert.AreEqual( 
				"Castle.CastleOnRails.Framework.Tests.Controllers.Products.ListController", 
				controller.GetType().FullName );
		}
	}
}
