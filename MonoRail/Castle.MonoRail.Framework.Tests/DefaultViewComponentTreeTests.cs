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

namespace Castle.MonoRail.Framework.Tests
{
	using System;
	using Castle.MonoRail.Framework.Services;
	using Castle.MonoRail.Framework.Tests.Controllers;
	using Castle.MonoRail.Framework.Tests.Controllers.Clients;
	using Castle.MonoRail.Framework.Tests.Controllers.Products;
	using NUnit.Framework;

	[TestFixture]
	public class DefaultViewComponentTreeTests
	{
		#region Member Data
		private DefaultViewComponentTree _tree;
		#endregion

		#region Test Setup and Teardown Methods
		[SetUp]
		public void Setup()
		{
			_tree = new DefaultViewComponentTree();
		}
		#endregion

		#region Test Methods
		[Test]
		public void AddViewComponent_NewComponent_Works()
		{
			_tree.AddViewComponent("MyViewComponent", typeof(ViewComponent));
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void AddViewComponent_DuplicateComponent_ThrowsException()
		{
			_tree.AddViewComponent("MyViewComponent", typeof(ViewComponent));
			_tree.AddViewComponent("MyViewComponent", typeof(ViewComponent));
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void AddViewComponent_NonViewComponent_ThrowsException()
		{
			_tree.AddViewComponent("MyViewComponent", typeof(DefaultViewComponentTree));
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void GetViewComponent_MissingComponent_ThrowsException()
		{
			_tree.GetViewComponent("MissingComponent");
		}

		[Test]
		public void GetViewComponent_ExistingComponent_Works()
		{
			Type type = typeof(ViewComponent);
			_tree.AddViewComponent("MyViewComponent", type);
			Assert.AreEqual(type, _tree.GetViewComponent("MyViewComponent"));
		}

		[Test]
		public void GetViewComponent_NamedViaAttribute_Works()
		{
			Type type = typeof(ATestViewComponent);
			_tree.AddViewComponent("MyViewComponent", type);
			Assert.AreEqual(type, _tree.GetViewComponent("DifferentName"));
		}
		#endregion
	}

	[ViewComponentDetails("DifferentName")]
	public class ATestViewComponent : ViewComponent
	{
	}
}
