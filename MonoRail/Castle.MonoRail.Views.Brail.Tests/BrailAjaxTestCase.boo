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
import Castle.MonoRail.TestSupport
import Castle.MonoRail.Framework.Tests

[TestFixture]
class BrailAjaxTestCase(AbstractMRTestCase):
	
	[Test]
	def JsFunctions():
		DoGet("ajax/JsFunctions.rails")
		expected = "<script type=\"text/javascript\" src=\"/MonoRail/Files/AjaxScripts.rails\"></script>"
		AssertSuccess()
		AssertReplyEqualTo(expected)
		
	[Test]
	def LinkToFunction():
		DoGet("ajax/LinkToFunction.rails")
		expected = "<a href=\"javascript:void(0);\"  onclick=\"alert('Ok'); return false;\" ><img src='myimg.gid'></a>"
		AssertSuccess()
		AssertReplyEqualTo(expected)
		
	[Test]
	def LinkToRemote():
		DoGet("ajax/LinkToRemote.rails")
		expected = "<a href=\"javascript:void(0);\"  onclick=\"new Ajax.Request('/controller/action.rails', {asynchronous:true, evalScripts:true}); return false;\" ><img src='myimg.gid'></a>"
		AssertSuccess()
		AssertReplyEqualTo(expected)
		
	[Test]
	def BuildFormRemoteTag():
		DoGet("ajax/BuildFormRemoteTag.rails")
		expected = "<form  onsubmit=\"new Ajax.Request('url', {asynchronous:true, evalScripts:true, parameters:Form.serialize(this)}); return false;\" enctype=\"multipart/form-data\">"
		AssertSuccess()
		AssertReplyEqualTo(expected)
		
	[Test]
	def ObserveField():
		DoGet("ajax/ObserveField.rails")
		expected = "<script type=\"text/javascript\">new Form.Element.Observer('myfieldid', 2, function(element, value) { new Ajax.Updater('elementToBeUpdated', '/url', {asynchronous:true, evalScripts:true, parameters:newcontent}) })</script>"
		AssertSuccess()
		AssertReplyEqualTo(expected)
	
	
	[Test]
	def ObserveForm():
		DoGet("ajax/ObserveForm.rails")
		expected = "<script type=\"text/javascript\">new Form.Observer('myfieldid', 2, function(element, value) { new Ajax.Updater('elementToBeUpdated', '/url', {asynchronous:true, evalScripts:true, parameters:newcontent}) })</script>"
		AssertSuccess()
		AssertReplyEqualTo(expected)
