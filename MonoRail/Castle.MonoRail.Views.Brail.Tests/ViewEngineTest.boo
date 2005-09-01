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
import Castle.MonoRail.Engine
import Castle.MonoRail.Views.Brail
import Castle.MonoRail.Views.Brail.Tests.Fakes

[TestFixture]
class ViewEngineTest:

	viewEngine as BooViewEngine
	engine as ProcessEngine
	
	[SetUp]
	def Setup():
		viewEngine = BooViewEngine()
		viewEngine.ViewRootDir = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			"../Castle.MonoRail.Views.Brail.Tests/Views")
		viewEngine.Init()
		factory = FakeControllerFactory()
		engine = ProcessEngine(factory,viewEngine)
	
	[Test]
	def SimpleRequest():
		context = FakeEngineContext("/home/index.rails")
		engine.Process(context)
		Assert.AreEqual( "Brail is wonderful", context.Output)
		
	[Test]
	def SimpleRequestWithDifferentView():
		context = FakeEngineContext("/home/other.rails")
		engine.Process(context)
		Assert.AreEqual("Brail is wonderful!", context.Output)
	
	[Test]
	def PropertyBag():
		context = FakeEngineContext("/home/bag.rails")
		engine.Process(context)
		Assert.AreEqual("Ayende is the name\r\n a\r\n b\r\n c\r\n", context.Output)
		
	[Test]
	def Layout():
		context = FakeEngineContext("/layout/index.rails")
		engine.Process(context)
		Assert.AreEqual("My layout\r\n\r\nindex contents\r\n\r\nFooter", context.Output )
		context = FakeEngineContext("/layout/save.rails");
		engine.Process( context );
		Assert.AreEqual( "My printable layout\r\n\r\nsave contents\r\n\r\nFooter", context.Output )
		
	[Test]
	def Resource():
		context = FakeEngineContext("/home/resource.rails")
		engine.Process(context)
		Assert.AreEqual("current language - english", context.Output)
		
		
		
	

