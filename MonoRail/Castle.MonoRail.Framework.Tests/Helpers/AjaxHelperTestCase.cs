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

namespace Castle.MonoRail.Framework.Tests.Helpers
{
	using System;
	using System.Collections;

    using NUnit.Framework;

    using Castle.MonoRail.Framework.Helpers;

	/// <summary>
	/// Summary description for AjaxHelperTestCase.
	/// </summary>
	[TestFixture]
	[Ignore("")]
	public class AjaxHelperTestCase
	{
        Castle.MonoRail.Framework.Helpers.AjaxHelper _hlp;

		[SetUp]
        public void Setup()
		{
            this._hlp = new AjaxHelper();
		}

        [Test]
        public void LinkToFunction2args() 
        {
            String expected = "<a href=\"#\" onclick=\"function; return false;\" >name</a>";
            String actual = this._hlp.LinkToFunction("name", "function");
            Assert.AreEqual(expected, actual);

        }

        [Test]
        public void LinkToFunction3args()
        {
            String expected = "<a href=\"#\" class=\"styleClass\" onclick=\"function; return false;\" >name</a>";
            String actual = this._hlp.LinkToFunction("name", "function", "styleClass");
            Assert.AreEqual(expected, actual);

        }

        [Test]
        public void ButtonToFunction2args()
        {
            String expected = "<input type=\"button\" onclick=\"function; return false;\" value=\"name\" />";
            String actual = this._hlp.ButtonToFunction("name", "function");
            Assert.AreEqual(expected, actual);
        }

		[Test]
        public void ButtonToFunction3args() 
        {
            String expected = "<input type=\"button\" class=\"styleClass\" onclick=\"function; return false;\" value=\"name\" />";
            String actual = this._hlp.ButtonToFunction("name", "function", "styleClass");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LinkToRemote3StringArgs()
        {
            String expected = "<a href=\"#\" onclick=\"new Ajax.Updater('update', 'url', {asynchronous:true}); return false;\" >name</a>";
            String actual = this._hlp.LinkToRemote("name", "url", "update");
            Assert.AreEqual(expected, actual);
        }

		[Test]
        public void LinkToRemote2String1DictArgs()
        {
            Hashtable dict = new Hashtable();
            dict.Add("key", "value");
            String expected = "<a href=\"#\" onclick=\"new Ajax.Request('url', {asynchronous:true}); return false;\" >name</a>";
            String actual = this._hlp.LinkToRemote("name", "url", dict);
            Assert.AreEqual(expected, actual);
        }
        
		[Test]
        public void LinkToRemote4StringArgs()
        {
            String expected = "<a href=\"#\" onclick=\"new Ajax.Updater('update', 'url', {asynchronous:true, parameters:with}); return false;\" >name</a>";
            String actual = this._hlp.LinkToRemote("name", "url", "update", "with");
            Assert.AreEqual(expected, actual);
        }
        
		[Test]
        public void LinkToRemote3String1BoolArgs()
        {
            Hashtable dict = new Hashtable();
            dict.Add("key", "value");
            String expected = "<a href=\"#\" onclick=\"new Ajax.Updater('with', 'url', {asynchronous:true, parameters:Form.serialize(this)}); return false;\" >name</a>";
            String actual = this._hlp.LinkToRemote("name", "url", "with", true);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ButtonToRemote3StringArgs() 
        {
            String expected = "<input type=\"button\" onclick=\"new Ajax.Updater('update', 'url', {asynchronous:true}); return false;\" value=\"name\" />";
            String actual = this._hlp.ButtonToRemote("name", "url", "update");
            Assert.AreEqual(expected, actual);
        }

		[Test]
        public void ButtonToRemote2String1DictArgs() 
        {
            Hashtable dict = new Hashtable();
            dict.Add("key", "value");
            String expected = "<input type=\"button\" onclick=\"new Ajax.Request('url', {asynchronous:true}); return false;\" value=\"name\" />";
            String actual = this._hlp.ButtonToRemote("name", "url", dict);
            Assert.AreEqual(expected, actual);
        }
        
		[Test]
        public void ButtonToRemote4StringArgs() 
        {
            String expected = "<input type=\"button\" onclick=\"new Ajax.Updater('update', 'with', {asynchronous:true, parameters:url}); return false;\" value=\"name\" />";
            String actual = this._hlp.ButtonToRemote("name", "url", "update", "with");
            Assert.AreEqual(expected, actual);
        }
        
		[Test]
        public void ButtonToRemote3String1BoolArgs() 
        {
            String expected = "<input type=\"button\" onclick=\"new Ajax.Updater('update', 'url', {asynchronous:true, parameters:Form.serialize(this)}); return false;\" value=\"name\" />";
            String actual = this._hlp.ButtonToRemote("name", "url", "update", true);
            Assert.AreEqual(expected, actual);
        }
	}
}
