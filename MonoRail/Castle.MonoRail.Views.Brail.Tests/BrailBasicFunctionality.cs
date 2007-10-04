// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.IO;
	using System.Threading;
	using Castle.MonoRail.Framework.Tests;
	using NUnit.Framework;

	[TestFixture]
	public class BrailBasicFunctionality : AbstractTestCase
	{
		[Test]
		public void AppPath()
		{
			DoGet("apppath/index.rails");
			AssertReplyEqualTo("Current apppath is ");
		}

		[Test]
		public void AppPathChangeOnTheFly()
		{
			string script = Path.Combine(GetPhysicalDir(), @"Views\AppPath\Index.brail");
			string newContent = "new content";
			string old;
			using (TextReader read = File.OpenText(script))
			{
				old = read.ReadToEnd();
			}
			using (TextWriter write = File.CreateText(script))
			{
				write.Write(newContent);
			}
			// Wait half a sec so Brail could pick up that a change in the file occured.
			Thread.Sleep(500);
			try
			{
				DoGet("apppath/index.rails");
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
			DoGet("home/nullables.rails");
			string expected = "\r\nfoo";
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void Empty()
		{
			DoGet("home/Empty.rails");
			string expected = "";
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void CommonScriptsChangeOnTheFly()
		{
			string common = Path.Combine(GetPhysicalDir(), @"Views\CommonScripts\Hello.brail");
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
				DoGet("home/hellofromcommon.rails");
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
			string layout = Path.Combine(GetPhysicalDir(), @"Views\layouts\defaultlayout.brail");
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
				DoGet("defaultlayout/index.rails");
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
			DoGet("home/preprocessor.rails");
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
			string expected = "Using Udp without namespace, since it is in the web.config\r\n";
			DoGet("home/namespacesInConfig.rails");
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void ComplexNestedExpressions()
		{
			string expected = "<a href=\"/customers/list.rails\" onclick=\"paginate(5)\" >first</a>";
			DoGet("home/complexNestedExpressions.rails");
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void ComplexNestedExpressions2()
		{
			string expected = "<a onclick=\"alert(5)\"  href=\"/customers/list.rails?page=5\">first</a>";
			DoGet("home/complexNestedExpressions2.rails");
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
			DoGet("home/javascript.rails");
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void Javascript2()
		{
			string expected = @"<script type='text/javascript'>
	function paginate(index)
	{
		var url = '/customers/list.rails';
		var params = 'page='+index+'&isAjax=true';
		new Ajax.Request(url, {
			method: 'get', 
			evalScripts: true, 
			parameters: params
			});
	}
</script>";;
			DoGet("home/javascript2.rails");
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void OutputSubViewInDiv()
		{
			string expected = @"<div>
Contents for heyhello View
</div>";
			DoGet("home/subview.rails");
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void UsingQuotes()
		{
			string expected = "<script type=\"text/javascript\" language=\"javascript\" src=\"/Content/js/datepicker.js\"></script>";
			DoGet("home/usingQuotes.rails");
			AssertReplyEqualTo(expected);
		}

        [Test]
        public void DuckOverloadToString()
        {
            DoGet("home/DuckOverloadToString.rails");
            AssertReplyEqualTo("20-12-1981");   
        }

        [Test]
        public void NullPropagation()
        {
            DoGet("home/NullPropagation.rails");
            AssertReplyIsBlank();
        }
	}
}
