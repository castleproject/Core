// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Views.NVelocity.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.MonoRail.TestSupport;


	[TestFixture]
	public class NVelocityAjaxTestCase : AbstractMRTestCase
	{
		[Test]
		public void JsFunctions()
		{
			DoGet("ajax/JsFunctions.rails");
			
			String expected = "<script type=\"text/javascript\" src=\"/MonoRail/Files/AjaxScripts.rails\"></script>";

			AssertReplyEqualsTo(expected);
		}

		[Test]
		public void BehaviourFunctions()
		{
			DoGet("ajax/BehaviourFunctions.rails");
			
			String expected = "<script type=\"text/javascript\" src=\"/MonoRail/Files/BehaviourScripts.rails\"></script>";

			AssertReplyEqualsTo(expected);
		}
		
		[Test]
		public void LinkToFunction()
		{
			DoGet("ajax/LinkToFunction.rails");

			String expected = "<a href=\"javascript:void(0);\" onclick=\"alert('Ok'); return false;\" ><img src='myimg.gid'></a>";

			AssertReplyEqualsTo(expected);
		}

		[Test]
		public void LinkToRemote()
		{
			DoGet("ajax/LinkToRemote.rails");

			String expected = "<a href=\"javascript:void(0);\" onclick=\"new " + 
				"Ajax.Request('/controller/action.rails', {asynchronous:true}); " + 
				"return false;\" ><img src='myimg.gid'></a>";

			AssertReplyEqualsTo(expected);
		}

		[Test]
		public void BuildFormRemoteTag()
		{
			DoGet("ajax/BuildFormRemoteTag.rails");

			String expected = "<form  onsubmit=\"new Ajax.Request('url', " +
				"{asynchronous:true, parameters:Form.serialize(this)}); " + 
				"return false;\" enctype=\"multipart/form-data\">";

			AssertReplyEqualsTo(expected);
		}

		[Test]
		public void ObserveField()
		{
			DoGet("ajax/ObserveField.rails");
			
			String expected = "<script type=\"text/javascript\">new Form.Element.Observer('myfieldid', 2, " + 
				"function(element, value) { new Ajax.Updater('elementToBeUpdated', '/url', " + 
				"{asynchronous:true, parameters:newcontent}) })</script>";

			AssertReplyEqualsTo(expected);
		}

		[Test]
		public void ObserveForm()
		{
			DoGet("ajax/ObserveForm.rails");
			
			String expected = "<script type=\"text/javascript\">new Form.Observer('myfieldid', 2, " + 
				"function(element, value) { new Ajax.Updater('elementToBeUpdated', '/url', " + 
				"{asynchronous:true, parameters:newcontent}) })</script>";

			AssertReplyEqualsTo(expected);
		}
	}
}
