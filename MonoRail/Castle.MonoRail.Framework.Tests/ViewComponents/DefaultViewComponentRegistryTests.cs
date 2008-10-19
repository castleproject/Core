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

namespace Castle.MonoRail.Framework.Tests.ViewComponents
{
	using System;
	using Castle.MonoRail.Framework.Services;
	using NUnit.Framework;

	[TestFixture]
	public class DefaultViewComponentRegistryTests
	{
		private DefaultViewComponentRegistry registry;

		[SetUp]
		public void Setup()
		{
			registry = new DefaultViewComponentRegistry();
		}

		[Test]
		public void AddViewComponent_NewComponent_Works()
		{
			registry.AddViewComponent("MyViewComponent", typeof(ViewComponent));
		}

		[Test]
		[ExpectedException(typeof(MonoRailException))]
		public void AddViewComponent_DuplicateComponent_ThrowsException()
		{
			registry.AddViewComponent("MyViewComponent", typeof(ViewComponent));
			registry.AddViewComponent("MyViewComponent", typeof(ViewComponent));
		}

		[Test]
		[ExpectedException(typeof(MonoRailException))]
		public void AddViewComponent_NonViewComponent_ThrowsException()
		{
			registry.AddViewComponent("MyViewComponent", typeof(DefaultViewComponentRegistry));
		}

		[Test]
		[ExpectedException(typeof(MonoRailException))]
		public void GetViewComponent_MissingComponent_ThrowsException()
		{
			registry.GetViewComponent("MissingComponent");
		}

		[Test]
		public void GetViewComponent_ExistingComponent_Works()
		{
			Type type = typeof(ViewComponent);
			registry.AddViewComponent("MyViewComponent", type);
			Assert.AreEqual(type, registry.GetViewComponent("MyViewComponent"));
		}

		[Test]
		public void GetViewComponent_ExistingWithoutComponentWithoutSuffixLookup_Works()
		{
			Type type = typeof(ViewComponent);
			registry.AddViewComponent("MyView", type);
			Assert.AreEqual(type, registry.GetViewComponent("MyView"));
		}

		[Test]
		public void GetViewComponent_ExistingComponentWithoutSuffix_Works()
		{
			Type type = typeof(ViewComponent);
			registry.AddViewComponent("MyViewComponent", type);
			Assert.AreEqual(type, registry.GetViewComponent("MyView"));
		}

		[Test]
		public void GetViewComponent_NamedViaAttribute_Works()
		{
			Type type = typeof(ATestViewComponent);
			registry.AddViewComponent("MyViewComponent", type);
			Assert.AreEqual(type, registry.GetViewComponent("DifferentName"));
		}

		[Test]
		public void HasViewComponent_ExistingComponent_Works()
		{
			Type type = typeof(ViewComponent);
			registry.AddViewComponent("MyViewComponent", type);
			Assert.IsTrue(registry.HasViewComponent("MyViewComponent"));
		}

		[Test]
		public void HasViewComponent_ExistingComponentWithoutSuffixLookup_Works()
		{
			Type type = typeof(ViewComponent);
			registry.AddViewComponent("MyViewComponent", type);
			Assert.IsTrue(registry.HasViewComponent("My"));
			Assert.IsTrue(registry.HasViewComponent("MyView"));
		}

		[Test]
		public void HasViewComponent_MissingComponent_ReturnsFalse()
		{
			Type type = typeof(ViewComponent);
			registry.AddViewComponent("MyViewComponent", type);
			Assert.IsFalse(registry.HasViewComponent("DifferentName"));
		}
	}

	[ViewComponentDetails("DifferentName")]
	public class ATestViewComponent : ViewComponent
	{
	}
}
