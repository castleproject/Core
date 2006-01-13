using ExtendedProperties = Commons.Collections.ExtendedProperties;

namespace NVelocity.Test
{
	using System;
	using System.Collections;
	using System.IO;
	using NUnit.Framework;

	/// <summary>
	/// Make sure that properties files are loaded correctly
	/// </summary>
	[TestFixture]
	public class CommonsTest
	{
		[TearDown]
		protected void TearDown()
		{
			FileInfo file = new FileInfo("test1.properties");
			try
			{
				file.Delete();
			}
			catch (Exception)
			{
				// ignore problems cleaning up file
			}
		}

		[Test]
		public void Test_ExtendedProperties()
		{
			FileInfo file = new FileInfo("test1.properties");
			StreamWriter sw = file.CreateText();
			sw.WriteLine("# lines starting with # are comments.  Blank lines are ignored");
			sw.WriteLine("");
			sw.WriteLine("# This is the simplest property");
			sw.WriteLine("prefix.key = value");
			sw.WriteLine("");
			sw.WriteLine("# A long property may be separated on multiple lines");
			sw.WriteLine("prefix.longvalue = aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa \\");
			sw.WriteLine("                   aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
			sw.WriteLine("");
			sw.WriteLine("# This is a property with many tokens");
			sw.WriteLine("prefix.tokens_on_a_line = first token, second token");
			sw.WriteLine("");
			sw.WriteLine("# This sequence generates exactly the same result");
			sw.WriteLine("prefix.tokens_on_multiple_lines = first token");
			sw.WriteLine("prefix.tokens_on_multiple_lines = second token");
			sw.WriteLine("");
			sw.WriteLine("# commas may be escaped in tokens");
			sw.WriteLine("prefix.commas.excaped = Hi\\, what'up?");
			sw.Flush();
			sw.Close();

			StreamReader sr = file.OpenText();
			String s = sr.ReadToEnd();
			sr.Close();

			ExtendedProperties props = new ExtendedProperties(file.FullName);
			VerifyProperties(props, "prefix.");

			StringWriter writer = new StringWriter();
			props.Save(writer, "header");

			// make sure that combine does not change types
			ExtendedProperties p = new ExtendedProperties();
			p.Combine(props);
			VerifyProperties(p, "prefix.");

			// make sure that subset does not change property types
			ExtendedProperties p2 = p.Subset("prefix");
			VerifyProperties(p2, "");
		}

		private void VerifyProperties(ExtendedProperties props, String prefix)
		{
			Assert.IsTrue(props.Count == 5, "expected to have 5 properties, had " + props.Count.ToString());

			Assert.IsTrue(props.GetString(prefix + "key").Equals("value"), "key was not correct: " + props.GetString(prefix + "key"));

			// make sure the comma escaping is working correctly
			Assert.IsTrue(props.GetString(prefix + "commas.excaped").Equals("Hi, what'up?"), "commas.excaped was not correct: " + props.GetString(prefix + "commas.excaped"));

			// make sure that multiple tokens on a single line are parsed correctly
			Object o = props.GetProperty(prefix + "tokens_on_a_line");
			Assert.IsTrue((o is ArrayList), prefix + "tokens_on_a_line was expected to be an ArrayList");
			Assert.IsTrue(((ArrayList) o).Count == 2, prefix + "tokens_on_a_line was expected to be an ArrayList with 2 elements");

			// make sure that tokens specified on multiple lines get put together correctly
			o = props.GetProperty(prefix + "tokens_on_multiple_lines");
			Assert.IsTrue((o is ArrayList), prefix + "tokens_on_multiple_lines was expected to be an ArrayList");
			Assert.IsTrue(((ArrayList) o).Count == 2, prefix + "tokens_on_multiple_lines was expected to be an ArrayList with 2 elements");
		}
	}
}