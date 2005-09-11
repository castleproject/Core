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
namespace Castle.MonoRail.Views.Brail.Tests

import System
import System.IO
import NUnit.Framework
import Castle.MonoRail.Framework
import Castle.MonoRail.Framework.Internal
import Castle.MonoRail.Engine
import Castle.MonoRail.Views.Brail
import Castle.MonoRail.Views.Brail.Tests.Fakes
import Castle.MonoRail.Framework.Tests

[TestFixture]
class ComponentsTestCase:

	viewEngine as BooViewEngine
	engine as ProcessEngine
	
	[SetUp]
	def Setup():
		viewEngine = BooViewEngine()
		componentFactory = DefaultViewComponentFactory()
		componentFactory.Inspect( Reflection.Assembly.GetExecutingAssembly() )
		viewEngine.ViewComponentFactory = componentFactory
		componentFactory.ViewEngine = viewEngine
		viewEngine.ViewRootDir = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			"../Castle.MonoRail.Views.Brail.Tests/Views")
		viewEngine.Init()
		factory = FakeControllerFactory()
		engine = ProcessEngine(factory,viewEngine)
	
	[Test]
	def SimpleInlineViewComponent():
		expected = "static 1\r\nHello from SimpleInlineViewComponent\r\nstatic 2"
		context = RailsEngineContextImpl("/usingcomponents/index1.rails")
		engine.Process(context)
		Assert.AreEqual( expected, context.Output)
	
	[Test]
	def InlineComponentUsingRender():
		expected = 'static 1\r\nThis is a view used by a component static 2'
		context as RailsEngineContextImpl = RailsEngineContextImpl('/usingcomponents/index2.rails')
		engine.Process(context)
		Assert.AreEqual(expected, context.Output)
	
	[Test()]
	def InlineComponentNotOverridingRender():
		expected = 'static 1\r\ndefault component view picked up automatically static 2'
		context as RailsEngineContextImpl = RailsEngineContextImpl('/usingcomponents/index3.rails')
		engine.Process(context)
		Assert.AreEqual(expected, context.Output)
	
	[Test]
	def InlineComponentWithParam1():
		context as RailsEngineContextImpl = RailsEngineContextImpl('/usingcomponents/index4.rails')
		engine.Process(context)
		Assert.AreEqual('Done', context.Output)

	[Test]
	def BlockComp1():
		context as RailsEngineContextImpl = RailsEngineContextImpl('/usingcomponents/index5.rails')
		engine.Process(context)
		System.Diagnostics.Trace.WriteLine(context.Output)
		Assert.AreEqual('  item 0\r\n  item 1\r\n  item 2\r\n', context.Output)
	
	[Test]
	def BlockWithinForEach():
		context as RailsEngineContextImpl = RailsEngineContextImpl('/usingcomponents/index8.rails')
		engine.Process(context)
		Assert.AreEqual('inner content 1\r\ninner content 2\r\n', context.Output)
	
	[Test]
	def SeveralComponentsInvocation():
		for i in range(10):
			expected = 'static 1\r\nContent 1\r\nstatic 2\r\nContent 2\r\nstatic 3\r\nContent 3\r\nstatic 4\r\nContent 4\r\nstatic 5\r\nContent 5\r\n'
			context as RailsEngineContextImpl = RailsEngineContextImpl('/usingcomponents/index9.rails')
			engine.Process(context)
			Assert.AreEqual(expected, context.Output)
			
