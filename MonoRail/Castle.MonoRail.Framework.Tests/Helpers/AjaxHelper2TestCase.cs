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

namespace Castle.MonoRail.Framework.Tests.Helpers
{
	using System;

	using NUnit.Framework;

	using Castle.MonoRail.Framework.Helpers;

	[TestFixture]
	public class AjaxHelper2TestCase
	{
		private AjaxHelper helper = new AjaxHelper();

		[Test]
		public void ObserverField1()
		{
			String expected = "<script type=\"text/javascript\">" + 
				"new Form.Element.Observer('fieldid', 1, " + 
				"function(element, value) { new Ajax.Updater('elementtoupdate', " + 
				"'update.rails', {asynchronous:true, evalScripts:true, parameters:value}) })</script>";

			String actual = helper.ObserveField("fieldid", 1, "update.rails", new DictHelper().CreateDict("update=elementtoupdate") );
			
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ObserverField2()
		{
			String expected = "<script type=\"text/javascript\">" + 
				"new Form.Element.Observer('fieldid', 1, " + 
				"function(element, value) { new Ajax.Updater('elementtoupdate', " + 
				"'update.rails', {asynchronous:true, evalScripts:true, parameters:obtainvalue()}) })</script>";

			String actual = helper.ObserveField("fieldid", 1, "update.rails", "elementtoupdate", "obtainvalue()" );
			
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void OnSuccessFailureCallbacks()
		{
			String expected = "<form  onsubmit=\"new Ajax.Request('something.rails', " 
				+ "{onSuccess:function(request) { javascriptcode } , onFailure:function("
				+ "request) { javascriptcode } , asynchronous:true, evalScripts:true, "
				+ "parameters:Form.serialize(this)}); return false;\" enctype=\"multipart/form-data\">";

			String actual = helper.BuildFormRemoteTag( new DictHelper().CreateDict("url=something.rails", "onfailure=javascriptcode", "onsuccess=javascriptcode") );
			
			Assert.AreEqual(expected, actual);
		}
	}
}
