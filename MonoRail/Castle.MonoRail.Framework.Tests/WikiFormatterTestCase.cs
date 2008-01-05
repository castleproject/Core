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

namespace Castle.MonoRail.Framework.Tests
{
	using Castle.MonoRail.Framework.TransformFilters.Formatters;
	using NUnit.Framework;

	[TestFixture]
	public class WikiFormatTesting
	{
		#region Styles

		[Test]
		public void BoldItalic()
		{
			string content = "'''''replace me'''''";
			string formatted = new WikiFormatter().Format(content);
			string result = "<b><i>replace me</i></b>" + "\n";
			Assert.IsTrue(result.Equals(formatted));
		}

		[Test]
		public void Bold()
		{
			string content = "'''replace me'''";
			string formatted = new WikiFormatter().Format(content);
			string result = "<b>replace me</b>" + "\n";
			Assert.IsTrue(result.Equals(formatted));
		}

		[Test]
		public void Italic()
		{
			string content = "''replace me''";
			string formatted = new WikiFormatter().Format(content);
			string result = "<i>replace me</i>" + "\n";
			Assert.IsTrue(result.Equals(formatted));
		}

		[Test]
		public void Underlined()
		{
			string content = "__replace me__";
			string formatted = new WikiFormatter().Format(content);
			string result = "<u>replace me</u>" + "\n";
			Assert.IsTrue(result.Equals(formatted));
		}

		[Test]
		public void Striked()
		{
			string content = "--replace me--";
			string formatted = new WikiFormatter().Format(content);
			string result = "<strike>replace me</strike>" + "\n";
			Assert.IsTrue(result.Equals(formatted));
		}

		[Test]
		public void Code()
		{
			string content = "{{replace me}}";
			string formatted = new WikiFormatter().Format(content);
			string result = "<code>replace me</code>" + "\n";
			Assert.IsTrue(result.Equals(formatted));
		}

		[Test]
		public void Pre()
		{
			string content = "{{{{replace me}}}}";
			string formatted = new WikiFormatter().Format(content);
			string result = "<pre>replace me</pre>" + "\n";
			Assert.IsTrue(result.Equals(formatted));
		}

		[Test]
		public void Box()
		{
			string content = "(((replace me)))";
			string formatted = new WikiFormatter().Format(content);
			string result = @"<table class=""box"" cellpadding=""0"" cellspacing=""0""><tr><td>replace me</td></tr></table>" +
			                "\n";
			Assert.IsTrue(result.Equals(formatted));
		}

		#endregion

		#region Table testing

		[Test]
		public void EmptyTableWithCaption()
		{
			string content = "{|" + "\n" +
			                 "|+ caption" + "\n" +
			                 "|}";

			string table = new WikiFormatter().Format(content);

			string result = "<table>" +
			                "<caption>caption</caption>" +
			                "</table>" + "\n";

			Assert.IsTrue(result.Equals(table));
		}

		[Test]
		public void TableWithCaptionAndTwoRows()
		{
			string content = "{|" + "\n" +
			                 "|+ The table's caption" + "\n" +
			                 "|-" + "\n" +
			                 "|-" + "\n" +
			                 "|}";

			string table = new WikiFormatter().Format(content);

			string result = "<table><caption>The table's caption</caption><tr></tr><tr></tr></table>" + "\n";

			Assert.IsTrue(result.Equals(table));
		}

		[Test]
		public void TableWithCaptionAndTwoRowsAndCells()
		{
			string content = "{|" + "\n" +
			                 "|+ The table's caption" + "\n" +
			                 "|-" + "\n" +
			                 "|Cell 1 || Cell 2 || Cell 3" + "\n" +
			                 "|-" + "\n" +
			                 "|Cell A " + "\n" +
			                 "|Cell B" + "\n" +
			                 "|Cell C" + "\n" +
			                 "|}";

			string table = new WikiFormatter().Format(content);

			string result =
				"<table><caption>The table's caption</caption><tr><td>Cell 1</td><td>Cell 2</td><td>Cell 3</td></tr><tr><td>Cell A</td><td>Cell B</td><td>Cell C</td></tr></table>" +
				"\n";

			Assert.IsTrue(result.Equals(table));
		}

		[Test]
		public void TableWithColumnHeadings()
		{
			string content = "{|" + "\n" +
			                 "|+ The table's caption" + "\n" +
			                 "! Column heading 1 !! Column heading 2 !! Column heading 3" + "\n" +
			                 "|-" + "\n" +
			                 "|Cell 1 || Cell 2 || Cell 3" + "\n" +
			                 "|-" + "\n" +
			                 "|Cell A" + "\n" +
			                 "|Cell B" + "\n" +
			                 "|Cell C" + "\n" +
			                 "|}";

			string table = new WikiFormatter().Format(content);

			string result =
				"<table><caption>The table's caption</caption><tr><th>Column heading 1</th><th>Column heading 2</th><th>Column heading 3</th></tr><tr><td>Cell 1</td><td>Cell 2</td><td>Cell 3</td></tr><tr><td>Cell A</td><td>Cell B</td><td>Cell C</td></tr></table>" +
				"\n";

			Assert.IsTrue(result.Equals(table));
		}

		[Test]
		public void TableWithColumnAndRowHeadings()
		{
			string content = "{|" + "\n" +
			                 "|+ The table's caption" + "\n" +
			                 "! Column heading 1 !! Column heading 2 !! Column heading 3" + "\n" +
			                 "|-" + "\n" +
			                 "! Row heading 1 " + "\n" +
			                 "| Cell 2 || Cell 3" + "\n" +
			                 "|-" + "\n" +
			                 "! Row heading A " + "\n" +
			                 "|Cell B" + "\n" +
			                 "|Cell C" + "\n" +
			                 "|}";

			string table = new WikiFormatter().Format(content);

			string result =
				"<table><caption>The table's caption</caption><tr><th>Column heading 1</th><th>Column heading 2</th><th>Column heading 3</th></tr><tr><th>Row heading 1</th><td>Cell 2</td><td>Cell 3</td></tr><tr><th>Row heading A</th><td>Cell B</td><td>Cell C</td></tr></table>" +
				"\n";

			Assert.IsTrue(result.Equals(table));
		}

		[Test]
		public void MultiplicationTableTest()
		{
			string content = "{| class=\"wikitable\" style=\"text-align:center\"" + "\n" +
			                 "|+Multiplication table" + "\n" +
			                 "|-" + "\n" +
			                 "! &times; !! 1 !! 2 !! 3" + "\n" +
			                 "|-" + "\n" +
			                 "! 1" + "\n" +
			                 "| 1 || 2 || 3" + "\n" +
			                 "|-" + "\n" +
			                 "! 2" + "\n" +
			                 "| 2 || 4 || 6" + "\n" +
			                 "|-" + "\n" +
			                 "! 3" + "\n" +
			                 "| 3 || 6 || 9" + "\n" +
			                 "|-" + "\n" +
			                 "! 4" + "\n" +
			                 "| 4 || 8 || 12" + "\n" +
			                 "|-" + "\n" +
			                 "! 5" + "\n" +
			                 "| 5 || 10 || 15" + "\n" +
			                 "|}";

			string table = new WikiFormatter().Format(content);

			string result =
				"<table class=\"wikitable\" style=\"text-align:center\"><caption>Multiplication table</caption><tr><th>&times;</th><th>1</th><th>2</th><th>3</th></tr><tr><th>1</th><td>1</td><td>2</td><td>3</td></tr><tr><th>2</th><td>2</td><td>4</td><td>6</td></tr><tr><th>3</th><td>3</td><td>6</td><td>9</td></tr><tr><th>4</th><td>4</td><td>8</td><td>12</td></tr><tr><th>5</th><td>5</td><td>10</td><td>15</td></tr></table>" +
				"\n";

			Assert.IsTrue(result.Equals(table));
		}

		[Test]
		public void ColorTest()
		{
			string content = "{|" + "\n" +
			                 "| style=\"background:red; color:white\" | abc" + "\n" +
			                 "| def" + "\n" +
			                 "| bgcolor=\"red\" | <font color=\"white\"> ghi </font>" + "\n" +
			                 "| jkl" + "\n" +
			                 "|}";

			string table = new WikiFormatter().Format(content);

			string result =
				"<table><tr><td style=\"background:red; color:white\">abc</td><td>def</td><td bgcolor=\"red\"><font color=\"white\"> ghi </font></td><td>jkl</td></tr></table>" +
				"\n";

			Assert.IsTrue(result.Equals(table));
		}

		[Test]
		public void WidthHeightTest()
		{
			string content = "{| style=\"width:75%; height:200px\" border=\"1\"" + "\n" +
			                 "|- " + "\n" +
			                 "| abc || def || ghi" + "\n" +
			                 "|- style=\"height:100px\" " + "\n" +
			                 "| jkl || style=\"width:200px\" |mno || pqr" + "\n" +
			                 "|-" + "\n" +
			                 "| stu || vwx || yz" + "\n" +
			                 "|}";

			string table = new WikiFormatter().Format(content);

			string result =
				"<table style=\"width:75%; height:200px\" border=\"1\"><tr><td>abc</td><td>def</td><td>ghi</td></tr><tr style=\"height:100px\"><td>jkl</td><td style=\"width:200px\">mno</td><td>pqr</td></tr><tr><td>stu</td><td>vwx</td><td>yz</td></tr></table>" +
				"\n";

			Assert.IsTrue(result.Equals(table));
		}

		[Test]
		public void MoreMarkUpTesting()
		{
			string content = "{| style=\"background:yellow; color:green\"" + "\n" +
			                 "|- " + "\n" +
			                 "| abc || def || ghi" + "\n" +
			                 "|- style=\"background:red; color:white\"" + "\n" +
			                 "| jkl || mno || pqr" + "\n" +
			                 "|-" + "\n" +
			                 "| stu || style=\"background:silver\" | vwx || yz" + "\n" +
			                 "|}";

			string table = new WikiFormatter().Format(content);

			string result =
				"<table style=\"background:yellow; color:green\"><tr><td>abc</td><td>def</td><td>ghi</td></tr><tr style=\"background:red; color:white\"><td>jkl</td><td>mno</td><td>pqr</td></tr><tr><td>stu</td><td style=\"background:silver\">vwx</td><td>yz</td></tr></table>" +
				"\n";

			Assert.IsTrue(result.Equals(table));
		}

		[Test]
		public void ColumnWidthTest()
		{
			string content = "{| border=\"1\" cellpadding=\"2\"" + "\n" +
			                 "!width=\"50\"|Name" + "\n" +
			                 "!width=\"225\"|Effect" + "\n" +
			                 "!width=\"225\"|Games Found In" + "\n" +
			                 "|-" + "\n" +
			                 "|Poké Ball || Regular Poké Ball || All Versions" + "\n" +
			                 "|-" + "\n" +
			                 "|Great Ball || Better than a Poké Ball || All Versions" + "\n" +
			                 "|}";

			string table = new WikiFormatter().Format(content);

			string result =
				"<table border=\"1\" cellpadding=\"2\"><tr><th width=\"50\">Name</th><th width=\"225\">Effect</th><th width=\"225\">Games Found In</th></tr><tr><td>Poké Ball</td><td>Regular Poké Ball</td><td>All Versions</td></tr><tr><td>Great Ball</td><td>Better than a Poké Ball</td><td>All Versions</td></tr></table>" +
				"\n";

			Assert.IsTrue(result.Equals(table));
		}

		[Test]
		public void VerticalAlignment()
		{
			string content = "{| border=\"1\" cellpadding=\"2\"" + "\n" +
			                 "|-valign=\"top\"" + "\n" +
			                 "|width=\"10%\"|<b>Row heading</b>" + "\n" +
			                 "|width=\"70%\"|A longer piece of text. Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum." +
			                 "\n" +
			                 "|width=\"20%\"|A shorter piece of text." + "\n" +
			                 "|-valign=\"top\"" + "\n" +
			                 "|<b>Row heading</b>" + "\n" +
			                 "|A longer piece of text.Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum." +
			                 "\n" +
			                 "|A shorter piece of text." + "\n" +
			                 "|}";

			string table = new WikiFormatter().Format(content);

			string result =
				"<table border=\"1\" cellpadding=\"2\"><tr valign=\"top\"><td width=\"10%\"><b>Row heading</b></td><td width=\"70%\">A longer piece of text. Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.</td><td width=\"20%\">A shorter piece of text.</td></tr><tr valign=\"top\"><td><b>Row heading</b></td><td>A longer piece of text.Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.</td><td>A shorter piece of text.</td></tr></table>" +
				"\n";

			Assert.IsTrue(result.Equals(table));
		}

		#endregion
	}
}
