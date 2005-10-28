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

import Castle.MonoRail.TestSupport

[TestFixture]
class BrailSmartControllerTestCase(AbstractMRTestCase):
	
	[Test]
	def StringMethod():
		DoGet("/smart/stringmethod.rails?NAME=hammett")
		expected = "incoming hammett"
		AssertReplyEqualsTo(expected)
	
	[Test]
	def Complex():
		DoGet("/smart/complex.rails?strarg=hammett&intarg=1&strarray=a")
		expected = "incoming hammett 1 a"
		AssertReplyEqualsTo(expected)

		DoGet("/smart/complex.rails?strarg=&intarg=&strarray=a,b,c")
		expected = "incoming  0 a,b,c"
		AssertReplyEqualsTo(expected)
		
	[Test]
	def SimpleBind():
		DoGet("/smart/SimpleBind.rails?name=hammett&itemcount=11&price=20")
		expected = "incoming hammett 11 20"
		AssertReplyEqualsTo(expected)

		DoGet("/smart/SimpleBind.rails?name=hammett")
		expected = "incoming hammett 0 0"
		AssertReplyEqualsTo(expected)
	
	[Test]
	def ComplexBind():
		DoGet("/smart/ComplexBind.rails?name=hammett&itemcount=11&price=20&id=1&contact.email=x&contact.phone=y")
		expected = "incoming hammett 11 20 1 x y"
		AssertReplyEqualsTo(expected)
	

	[Test]
	def ComplexBindWithPrefix():
		DoGet("/smart/ComplexBindWithPrefix.rails?name=hammett&itemcount=11&price=20&person.id=1&person.contact.email=x&person.contact.phone=y")
		expected = "incoming hammett 11 20 1 x y"
		AssertReplyEqualsTo(expected)

	[Test]
	def FillingBehavior1():
		System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-us")
		DoGet("/smart/FillingBehavior.rails?name=someone&date1day=11&date1month=10&date1year=2005")
		date1 = DateTime(2005,10,11)
		expected = "incoming someone ${date1.ToShortDateString()} " + DateTime.Now.AddDays(1).ToShortDateString()
		AssertReplyEqualsTo(expected)
	

	[Test]
	def FillingBehavior2():
		System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-us")
		DoGet("/smart/FillingBehavior.rails")
		expected = "incoming hammett " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.AddDays(1).ToShortDateString()
		AssertReplyEqualsTo(expected)
