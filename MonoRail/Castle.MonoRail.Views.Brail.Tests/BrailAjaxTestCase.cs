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
{
	using Castle.MonoRail.Framework.Tests;
	using NUnit.Framework;

	[TestFixture]
	public class BrailAjaxTestCase : AbstractTestCase
	{
		[Test]
		public void JsFunctions()
		{
			DoGet("ajax/JsFunctions.rails");
			string expected = "<script type=\"text/javascript\" src=\"/MonoRail/Files/AjaxScripts.rails\"></script>";
			AssertSuccess();
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void LinkToFunction()
		{
			DoGet("ajax/LinkToFunction.rails");
			string expected =
				"<a href=\"javascript:void(0);\"  onclick=\"alert('Ok'); return false;\" ><img src='myimg.gid'></a>";
			AssertSuccess();
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void LinkToRemote()
		{
			DoGet("ajax/LinkToRemote.rails");
			string expected =
				"<a href=\"javascript:void(0);\"  onclick=\"new Ajax.Request('/controller/action.rails', {asynchronous:true, evalScripts:true}); return false;\" ><img src='myimg.gid'></a>";
			AssertSuccess();
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void BuildFormRemoteTag()
		{
			DoGet("ajax/BuildFormRemoteTag.rails");
			string expected =
				"<form  onsubmit=\"new Ajax.Request('url', {asynchronous:true, evalScripts:true, parameters:Form.serialize(this)}); return false;\" enctype=\"multipart/form-data\">";
			AssertSuccess();
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void ObserveField()
		{
			DoGet("ajax/ObserveField.rails");
			string expected =
				"<script type=\"text/javascript\">new Form.Element.Observer('myfieldid', 2, function(element, value) { new Ajax.Updater('elementToBeUpdated', '/url', {asynchronous:true, evalScripts:true, parameters:newcontent}) })</script>";
			AssertSuccess();
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void ObserveForm()
		{
			DoGet("ajax/ObserveForm.rails");
			string expected =
				"<script type=\"text/javascript\">new Form.Observer('myfieldid', 2, function(element, value) { new Ajax.Updater('elementToBeUpdated', '/url', {asynchronous:true, evalScripts:true, parameters:newcontent}) })</script>";
			AssertSuccess();
			AssertReplyEqualTo(expected);
		}
	}
}