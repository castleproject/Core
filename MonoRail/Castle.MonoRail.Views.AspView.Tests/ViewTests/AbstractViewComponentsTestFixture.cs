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

namespace Castle.MonoRail.Views.AspView.Tests.ViewTests
{
	using System;
	using Framework.Helpers;
	using Framework;
	using Framework.Services;

	public abstract class AbstractViewComponentsTestFixture : AbstractViewTestFixture
	{
		protected MyViewComponentFactory componentFactory = new MyViewComponentFactory();

		protected override void Clear()
		{
			base.Clear();
			componentFactory = null;
		}

		protected override void CreateStubsAndMocks()
		{
			base.CreateStubsAndMocks();
			componentFactory = new MyViewComponentFactory();
		}

		protected override void CreateDefaultStubsAndMocks()
		{
			base.CreateDefaultStubsAndMocks();
			context.AddService(typeof(IViewComponentFactory), componentFactory);
		}

		protected void RegisterComponent(string name, Type type)
		{
			componentFactory.RegisteredComponents.Add(name, type);
		}
		protected class MyViewComponentFactory : DefaultViewComponentFactory
		{
			public readonly DictHelper.MonoRailDictionary RegisteredComponents =
				new DictHelper.MonoRailDictionary();

			public override ViewComponent Create(string name)
			{
				return Activator.CreateInstance((Type)RegisteredComponents[name]) as ViewComponent;
			}
		}
	}
}
