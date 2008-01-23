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

namespace Castle.MonoRail.Views.Brail.Tests
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Net;
	using System.Threading;
	using DynamicProxy;
	using NUnit.Framework;

    [TestFixture]
    public class BrailBasicFunctionality : BaseViewOnlyTestFixture
	{
		[Test]
		public void AppPath()
		{
			ProcessView_StripRailsExtension("apppath/index.rails");
			AssertReplyEqualTo("Current apppath is /TestBrail");
		}

		[Test]
		public void CanUseUrlHelperWithoutPrefix()
		{
            PropertyBag["url"] = "blah";

            ProcessView_StripRailsExtension("home/CanUseUrlHelperWithoutPrefix.rails");
			AssertReplyEqualTo("Castle.MonoRail.Framework.Helpers.UrlHelper");
		}

		[Test]
		public void WithNullableDynamicProxyObject()
		{
            ProxyGenerator generator = new ProxyGenerator();
            SimpleProxy proxy = (SimpleProxy)generator.CreateClassProxy(typeof(SimpleProxy), new StandardInterceptor());
            PropertyBag["src"] = proxy;
			ProcessView_StripRailsExtension("home/WithNullableDynamicProxyObject.rails");
			string expected = @"BarBaz
foo
what?
there";
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void AppPathChangeOnTheFly()
		{
			string script = Path.Combine(ViewSourcePath, @"Views\AppPath\Index.brail");
			string newContent = "new content";
			string old;
			using (TextReader read = File.OpenText(script))
			{
				old = read.ReadToEnd();
			}
            ProcessView_StripRailsExtension("apppath/index.rails");//init brail engine
			using (TextWriter write = File.CreateText(script))
			{
				write.Write(newContent);
				write.Flush();
			}
			Thread.Sleep(1000);
			try
			{
				ProcessView_StripRailsExtension("apppath/index.rails");
				AssertReplyEqualTo(newContent);
			}
			finally
			{
				using (TextWriter write = File.CreateText(script))
				{
					write.Write(old);
				}
			}
		}

		[Test]
		public void CommonScripts()
		{
		    PropertyBag["doesExists"] = "foo";
			ProcessView_StripRailsExtension("home/nullables.rails");
			string expected = "\r\nfoo";
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void Empty()
		{
			ProcessView_StripRailsExtension("home/Empty.rails");
			string expected = "";
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void CommonScriptsChangeOnTheFly()
		{
			string common = Path.Combine(ViewSourcePath, @"Views\CommonScripts\Hello.brail");
			string old;
			using (TextReader read = File.OpenText(common))
			{
				old = read.ReadToEnd();
			}
			string @new = @"
def SayHello(name as string):
	return 'Hello, '+name+'! Modified!' 
end";
			using (TextWriter write = File.CreateText(common))
			{
				write.Write(@new);
			}
			string expected = "Hello, Ayende! Modified!";
			// Have to wait for the common scripts recompilation otherwise you get random test failure since the request
			// sometimes gets there faster you can recompile and it gets the old version.
			Thread.Sleep(500);
			try
			{
				ProcessView_StripRailsExtension("home/hellofromcommon.rails");
				AssertReplyEqualTo(expected);
			}
			finally
			{
				using (TextWriter write = File.CreateText(common))
				{
					write.Write(old);
				}
			}
			// Have to wait again to make sure that the second recompilation happened, since otherwise a second test
			// might get the modified version.
			Thread.Sleep(500);
		}

		[Test]
		public void LayoutsChangeOnTheFly()
		{
		    Layout = "defaultlayout";
			string layout = Path.Combine(ViewSourcePath, @"Views\layouts\defaultlayout.brail");
			string old;
			using (TextReader read = File.OpenText(layout))
			{
				old = read.ReadToEnd();
			}
			string newLayout = @"start modified
${ChildOutput}
end";
			using (TextWriter write = File.CreateText(layout))
			{
				write.Write(newLayout);
			}
			string expected = @"start modified
content
end";
			Thread.Sleep(500);
			try
			{
				ProcessView_StripRailsExtension("defaultlayout/index.rails");
				AssertReplyEqualTo(expected);
			}
			finally
			{
				using (TextWriter write = File.CreateText(layout))
				{
					write.Write(old);
				}
			}
			Thread.Sleep(500);
		}


		[Test]
		public void PreProcessor()
        {
            this.PropertyBag.Add("Title", "Ayende");
			ProcessView_StripRailsExtension("home/preprocessor.rails");
			string expected =
				@"
<html>
<body>
<title>AYENDE</title>
<body>
<ul>
<li>0</li><li>1</li><li>2</li>
</ul>
</body>
</html>
";
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void CanUseNamespacesFromConfig()
		{
		    BrailOptions.NamespacesToImport.Add(typeof (TransportType).Namespace);
			string expected = "Using Udp without namespace, since it is in the web.config\r\n";
			ProcessView_StripRailsExtension("home/namespacesInConfig.rails");
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void ComplexNestedExpressions()
		{
		    PropertyBag["title"] = "first";
            PropertyBag["pageIndex"] = 5;
			string expected = "<a href=\"/TestBrail/customers/list.tdd\" onclick=\"paginate(5)\" >first</a>";
			ProcessView_StripRailsExtension("home/complexNestedExpressions.rails");
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void ComplexNestedExpressions2()
		{
            PropertyBag["title"] = "first";
            PropertyBag["pageIndex"] = 5;

            string expected = "<a onclick=\"alert(5)\"  href=\"/TestBrail/customers/list.tdd?page=5\">first</a>";
			ProcessView_StripRailsExtension("home/complexNestedExpressions2.rails");
			AssertReplyEqualTo(expected);
		}
		
		[Test]
		public void Javascript()
		{
			string expected = @"<script type='text/javascript'>
	function paginate(index)
	{
		alert(index);
	}
</script>";;
			ProcessView_StripRailsExtension("home/javascript.rails");
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void Javascript2()
		{
			string expected = @"<script type='text/javascript'>
	function paginate(index)
	{
		var url = '/TestBrail/customers/list.tdd';
		var params = 'page='+index+'&isAjax=true';
		new Ajax.Request(url, {
			method: 'get', 
			evalScripts: true, 
			parameters: params
			});
	}
</script>"; ;
			ProcessView_StripRailsExtension("home/javascript2.rails");
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void OutputSubViewInDiv()
		{
			string expected = @"<div>
Contents for heyhello View
</div>";
			ProcessView_StripRailsExtension("home/subview.rails");
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void UsingQuotes()
		{
            string expected = "<script type=\"text/javascript\" language=\"javascript\" src=\"/TestBrail/Content/js/datepicker.js\"></script>";
			ProcessView_StripRailsExtension("home/usingQuotes.rails");
			AssertReplyEqualTo(expected);
		}

        [Test]
        public void DuckOverloadToString()
        {
            PropertyBag["birthday"] = new DateTime(1981, 12, 20);
            ProcessView_StripRailsExtension("home/DuckOverloadToString.rails");
            AssertReplyEqualTo("20-12-1981");   
        }

        [Test]
        public void NullPropagation()
        {
            ProcessView_StripRailsExtension("home/NullPropagation.rails");
            AssertReplyEqualTo("");
        }

        public class SimpleProxy
        {
            private string text = "BarBaz";
            readonly Hashtable items = new Hashtable();

            public virtual object this[string key]
            {
                get { return items[key]; }
                set { items[key] = value; }
            }

            public virtual string Text
            {
                get { return text; }
                set { text = value; }
            }

            public virtual string Say()
            {
                return "what?";
            }
        }
	}
}
